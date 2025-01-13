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

public partial class cart : System.Web.UI.Page
{
    double total = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable basket = getBasketDt();
            if (basket != null && basket.Rows.Count > 0 )
            {
                lblCartMessage.Text = "";
                btnCheckout.Visible = true;
                update_grid(basket);
            }
            else
            {
                lblCartMessage.Text = "Your cart is empty";
                btnCheckout.Visible = false;
            }
            
        }
    }
    private string calculate_totals()
    {
        DataTable dtBasket = getBasketDt();
        double price = 0;
        int qty = 0;
        for (int i = 0; i < dtBasket.Rows.Count; i++)
        {
            qty = qty + Convert.ToInt32(dtBasket.Rows[i]["quantity"]);
            price = price + (Convert.ToDouble(dtBasket.Rows[i]["price"]) * Convert.ToInt32(dtBasket.Rows[i]["quantity"]));

        }
        return (qty + "|" + price);
    }
    private void update_grid(DataTable dt)
    {
        if (dt.Rows.Count >= 0)
        {
            grdvCart.DataSource = dt;
            grdvCart.DataBind();
            //string data = calculate_totals();
            //data.Split("|");
            
        }
    }
    protected string countsubtotal(string price,string quantity)
    {
        double subtotal=(Convert.ToDouble(price) * Convert.ToInt32(quantity));
        total += subtotal;
        return Convert.ToString(subtotal);
    }
    protected double FinalTotal()
    {
        return total;
    }
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
    protected void grdvCart_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Update")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
            TextBox txtqty = (TextBox)row.FindControl("txtitemqty");
            Label lblProdId = (Label)row.FindControl("lblID");
            DataTable basket = getBasketDt();
            basket.Rows[Convert.ToInt32(lblProdId.Text)-1]["quantity"] = Convert.ToInt32(txtqty.Text);
            //basket.Rows[Convert.ToInt32(lblProdId.Text)-1]["subtotal"] = (Convert.ToDouble(basket.Rows[Convert.ToInt32(lblProdId.Text)-1]["price"])) * Convert.ToInt32(txtqty.Text);
            update_grid(basket);
        }
    }
    protected void grdvCart_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {

    }
    protected void btnCheckout_Click(object sender, EventArgs e)
    {
        Server.Transfer("checkout_form.aspx");
    }
}
(c) dvds4less 2000-2010