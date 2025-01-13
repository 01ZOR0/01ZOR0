using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;
using System.Xml.Schema;
using System.IO;

public partial class checkout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Request.QueryString.Count > 0)
        {
            XmlDocument xdoc=new XmlDocument();
            xdoc.Load(Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + "orders\\" + Request.QueryString["order"].ToString());
            XslTransform xslTransform = new XslTransform();
            xslTransform.Load(Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + "order.xsl");
            Stream stream_xml=new MemoryStream();
            
            xslTransform.Transform((IXPathNavigable)xdoc, null, stream_xml);
            stream_xml.Position = 0;
            StreamReader sr = new StreamReader(stream_xml);
            Response.Write(sr.ReadToEnd());

            //StreamWriter sw = new StreamWriter("C:\\test");
            //this.stream_XML.Position = 0;
            //StreamReader sr = new StreamReader(this.stream_XML);
            //sw.Write(sr.ReadToEnd());
            //sw.Flush();
            //sw.Close();
            //return true;

        }
        else
        {
            if (Session["mydata"] == null)
            {
                Response.Write("Nothing in your Cart");
            }
            else
            {
                Response.Write(Session["mydata"]);
            }
        }
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Default.aspx");
    }
}
(c) dvds4less 2000-2010