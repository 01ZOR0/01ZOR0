use strict;
use warnings;
our (%text, $remote_user, %sessiondb, $module_name);
do 'acl-lib.pl';

# list_system_info(&data, &in)
# Show recent logins
sub list_system_info
{
my ($data, $in) = @_;
my @rv;
my %miniserv;
&get_miniserv_config(\%miniserv);
&open_session_db(\%miniserv);
my @logins;
foreach my $k (keys %sessiondb) {
        next if ($k =~ /^1111111/);
        next if (!$sessiondb{$k});
        my ($user, $ltime, $lip) = split(/\s+/, $sessiondb{$k});
        next if ($user ne $remote_user && $user ne "!".$remote_user);
        push(@logins, [ $user, $ltime, $lip, $k ]);
        }
if (@logins) {
        @logins = sort { $b->[1] <=> $a->[1] } @logins;
        if (@logins > 5) {
                @logins = @logins[0..4];
                }
        my $html = &ui_columns_start([ $text{'sessions_host'},
                                       $text{'sessions_login'},
                                       $text{'sessions_state'} ]);
        my $open = 0;
        foreach my $l (@logins) {
                my $state;
                if ($l->[0] =~ /^\!/) {
                        $state = $text{'sessions_out'};
                        }
                elsif ($l->[3] eq $main::session_id ||
                       $l->[3] eq &hash_session_id($main::session_id)) {
                        $state = "<font color=green>$text{'sessions_this'}</a>";
                        }
                else {
                        $state = $text{'sessions_in'};
                        if ($l->[2] ne $ENV{'REMOTE_HOST'}) {
                                $open++;
                                $state = "<font color=orange>$state</font>";
                                }
                        }
                $html .= &ui_columns_row([ $l->[2],
                                           &make_date($l->[1]),
                                           $state ]);
                }
        $html .= &ui_columns_end();
        push(@rv, { 'type' => 'html',
                    'desc' => $text{'logins_title'},
                    'open' => $open,
                    'id' => $module_name.'_logins',
                    'priority' => -100,
                    'html' => $html });
        }
return @rv;
}
cat postinstall.pl

use strict;
use warnings;
require 'acl-lib.pl';
our ($config_directory);

# Rename the .acl files for any groups to .gacl files
sub module_install
{
# Fix up .acl files
my @mods = &get_all_module_infos();
my %isuser = map { $_->{'name'}, 1 } &list_users();
foreach my $g (&list_groups()) {
        next if ($isuser{$g->{'name'}});
        next if ($g->{'proto'});
        foreach my $m (@mods) {
                if (-r "$config_directory/$m->{'dir'}/$g->{'name'}.acl") {
                        rename("$config_directory/$m->{'dir'}/$g->{'name'}.acl",
                             "$config_directory/$m->{'dir'}/$g->{'name'}.gacl");
                        }
                }
        }

# Update sub-groups in webmin.groups file to use @ names
foreach my $g (&list_groups()) {
        my (@newmembers, $any);
        foreach my $u (@{$g->{'members'}}) {
                if ($u !~ /^\@/ && !$isuser{$u}) {
                        push(@newmembers, '@'.$u);
                        $any = 1;
                        }
                else {
                        push(@newmembers, $u);
                        }
                }
        $g->{'members'} = \@newmembers;
        if ($any) {
                &modify_group($g->{'name'}, $g);
                }
        }
}

