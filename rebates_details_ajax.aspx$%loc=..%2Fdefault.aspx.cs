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
using System.Data.Sql;

public partial class Default : System.Web.UI.Page
{
    
    private nsDB d_db = new nsDB();
    string search = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Response.Clear();
        
        if (Request.QueryString.Count > 0)
        {
            search=Convert.ToString(Request.QueryString["search"]);
        }
        else
        {
            if (IsPostBack)
            {
                search = txtSearch.Text;
            }
                
        }
            ItemsGet();

    }
    private void ItemsGet()
    {
        d_db.openConnection();
        d_db.setCommandConnection();
        if (search != "")
        {
            d_db.setCommandQuery("select * from items where product_name like '%" + search + "%' or product_desc_summary like '%" + search + "%'");
        }
        else
        {
            d_db.setCommandQuery("select * from items");
        }
        DataSet ds = d_db.getDataSet();
        d_db.closeConnection();

        PagedDataSource objPds = new PagedDataSource();
        objPds.DataSource = ds.Tables[0].DefaultView;
        objPds.AllowPaging = true;
        objPds.PageSize = 3;
        objPds.CurrentPageIndex = CurrentPage;

        int start_record = (3 * CurrentPage) + 1;
        int end_record = start_record + 2;
        if (end_record > ds.Tables[0].Rows.Count)
        {
            end_record = ds.Tables[0].Rows.Count;
        }

	if (ds.Tables[0].Rows.Count <= 0) {
		lblCurrentPage.Text = "Your search for '" + search + "' produced no results";
	}
	else {
	        lblCurrentPage.Text = "Items: " + start_record.ToString() + " to " + end_record.ToString() + " of " + ds.Tables[0].Rows.Count.ToString();
	}

        cmdPrev.Visible = !objPds.IsFirstPage;
        cmdNext.Visible = !objPds.IsLastPage;
        Accordion1.DataSource = objPds;
        Accordion1.DataBind();
    }
    private void ItemsGet(string search_criteria)
    {
        d_db.openConnection();
        d_db.setCommandConnection();
        
        DataSet ds = d_db.getDataSet();
        d_db.closeConnection();

        PagedDataSource objPds = new PagedDataSource();
        objPds.DataSource = ds.Tables[0].DefaultView;
        objPds.AllowPaging = true;
        objPds.PageSize = 3;
        objPds.CurrentPageIndex = CurrentPage;

        int start_record = (3 * CurrentPage) + 1;
        int end_record = start_record + 2;
        if (end_record > ds.Tables[0].Rows.Count)
        {
            end_record = ds.Tables[0].Rows.Count;
        }
        lblCurrentPage.Text = "Items: " + start_record.ToString() + " to " + end_record.ToString() + " of " + ds.Tables[0].Rows.Count.ToString();

        cmdPrev.Visible = !objPds.IsFirstPage;
        cmdNext.Visible = !objPds.IsLastPage;
        Accordion1.DataSource = objPds;
        Accordion1.DataBind();
    }
    public int CurrentPage
    {
        get
        {
            object o = this.ViewState["_CurrentPage"];

            if (o == null)
                return 0; // default page index of 0
            else
            {
                return (int)o;
            }
        }
        set
        {
            this.ViewState["_CurrentPage"] = value;
        }
    }

    protected void cmdPrev_Click1(object sender, EventArgs e)
    {
        // Set viewstate variable to the previous page
        CurrentPage = CurrentPage - 1;

        // Reload control
        ItemsGet();
    }
    protected void cmdNext_Click1(object sender, EventArgs e)
    {
        // Set viewstate variable to the next page
        CurrentPage = CurrentPage + 1;

        // Reload control
        ItemsGet();
    }
    protected string IsRebates(Object oName,Object p_id)
    {
        if (!(oName == null || oName == "" || oName == String.Empty))
        {
            //return "<a href=\"javascript: Open_New_Window('rebates.asp?loc=" + oName + "')', 'Rebates', 'menubar=yes,width=450,height=280,top=50,left=50,scrollbars=yes')\">Rebate coupon</a>\"";
            return "<a id=rebates"+ p_id + " onclick=\"FillDiv(this,'" + oName + "');return false;\" style='color:red;' \">Rebate coupon</a>";
        }
        return "";
    }
    protected void AddtoCart_OnServerClick(object source, EventArgs e)
    {
        //int index = ((GridViewRow)((HtmlInputButton)source).Parent.NamingContainer).RowIndex;
        Response.Write(Request.Form["id"].ToString());
        //Response.Redirect("default.aspx");
    }
    //protected void rptcdcatalog_ItemCommand(object source, RepeaterCommandEventArgs e)
    //{
    //    if (e.CommandName == "ID")
    //    {
    //        //Response.Write(((TextBox)(e.Item.FindControl("textbox"))).Text);
    //        HiddenField t = ((HiddenField)(e.Item.FindControl("hiddenID")));
    //        TextBox t1 = ((TextBox)(e.Item.FindControl("textbox")));
    //        addtobasket(Convert.ToInt32(t.Value), Convert.ToInt32(t1.Text));
    //        Response.Redirect("cart.aspx");
    //    }
       
    //}

    private DataTable getBasketDt()
    {
        DataTable dtBasket;

        if (Session["dtBasket"] == null)
        {
            // If the DataTable doesnt exist in the Session then we can create it now
            dtBasket = createProductDT();
        }
        else
        {
            // Else it does exist in Session so just pull our current one out
            dtBasket = (DataTable)Session["dtBasket"];
        }

        return dtBasket;
    }
    public void addtobasket(int productID, int qty)
    {
        DataTable dtBasket = getBasketDt();

        bool found = false;
        for (int i = 0; i < dtBasket.Rows.Count; i++)
        {
            if (Convert.ToInt32(dtBasket.Rows[i]["prod_id"]) == productID)
            {
                dtBasket.Rows[i]["quantity"] = Convert.ToInt32(dtBasket.Rows[i]["quantity"]) + qty;
                found = true;
                break;
            }
        }
        if (!found)
        {
            DataTable dtProducts = populateProducts(productID);
            DataRow drProduct = dtProducts.Rows.Find(productID);

            DataRow newRow = dtBasket.NewRow();
            newRow["prod_id"] = drProduct["prod_id"];
            newRow["id"] = dtBasket.Rows.Count + 1;

            newRow["name"] = drProduct["name"];
            newRow["price"] = drProduct["price"];
            newRow["description"] = drProduct["description"];
            newRow["quantity"] = qty;

            dtBasket.Rows.Add(newRow);
        }

        // Store the newly updated basket back in the Session
        Session["dtBasket"] = dtBasket;

        // Update the basket i.e. re-bind it
    }
    public DataTable populateProducts(int productid)
    {
        // Create the basic structure
        DataTable dtProducts = createProductDT();

        // Add the products to it

        // Create the initial row
        DataRow aProduct = dtProducts.NewRow();

        nsDB d_db = new nsDB();
        d_db.openConnection();
        d_db.setCommandConnection();
        d_db.setCommandQuery("select * from items where product_id=" + productid);
        DataSet ds = d_db.getDataSet();
        d_db.closeConnection();

        aProduct["id"] = 1;
        aProduct["prod_id"] = ds.Tables[0].Rows[0]["product_id"];
        aProduct["name"] = ds.Tables[0].Rows[0]["product_name"];
        aProduct["price"] = ds.Tables[0].Rows[0]["product_price"];
        aProduct["description"] = ds.Tables[0].Rows[0]["product_desc_summary"];

        dtProducts.Rows.Add(aProduct);
        return dtProducts;
    }
    public DataTable createProductDT()
    {

        DataTable dtProducts = new DataTable();
        DataColumn productColumn = new DataColumn();

        productColumn.DataType = System.Type.GetType("System.Int32");
        productColumn.ColumnName = "prod_id";
        productColumn.Unique = true;
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.Int32");
        productColumn.ColumnName = "id";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.String");
        productColumn.ColumnName = "name";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.String");
        productColumn.ColumnName = "description";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.Double");
        productColumn.ColumnName = "price";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.Int32");
        productColumn.ColumnName = "quantity";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.Double");
        productColumn.ColumnName = "subtotal";
        dtProducts.Columns.Add(productColumn);

        productColumn = new DataColumn();
        productColumn.DataType = System.Type.GetType("System.Double");
        productColumn.ColumnName = "total";
        dtProducts.Columns.Add(productColumn);

        //Make "id" the primary key
        DataColumn[] pkColumns = new DataColumn[1];
        pkColumns[0] = dtProducts.Columns["prod_id"];
        dtProducts.PrimaryKey = pkColumns;

        return dtProducts;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        search=txtSearch.Text;
    }
}
(c) dvds4less 2000-2010