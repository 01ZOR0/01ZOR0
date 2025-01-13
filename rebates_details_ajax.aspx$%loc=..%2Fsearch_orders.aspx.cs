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
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Schema;


public partial class search_orders : System.Web.UI.Page
{
    private ArrayList _alFileList;

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtCreditCard.Text == "" && txtName.Text == "")
        {
            lblErrorMessage.Text = "Please enter search Criteria";
            return;
        }
        _alFileList = new ArrayList();
        search_files(Server.MapPath("orders"), "*.*");
        XmlDocument xdocMain = new XmlDocument();
        XmlElement rootNode = xdocMain.CreateElement("orders");
        




        for (int i = 0; i < _alFileList.Count; i++)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(_alFileList[i].ToString());
            XmlNode newNode = xdoc.SelectSingleNode("/order");
            XmlNode importNewNode = xdocMain.ImportNode(newNode, true);
            rootNode.AppendChild(importNewNode);
        }
        xdocMain.AppendChild(rootNode);
        XmlNodeList xResultName = xdocMain.SelectNodes("/orders/order/customer_details[contains(creditcard,'" + txtCreditCard.Text + "') and contains(name,'" + txtName.Text + "')]");
        if (xResultName.Count > 0)
        {
            XmlDocument xdoc_new = new XmlDocument();
            XmlElement xdoc_new_main = xdoc_new.CreateElement("orders");
            for (int i = 0; i < xResultName.Count; i++)
            {
                string a = xResultName[i].ParentNode.SelectSingleNode("order_number").InnerText;
                XmlDocument x_result_file = new XmlDocument();
                x_result_file.Load(Server.MapPath("orders/" + a));
                XmlNode newNode = x_result_file.SelectSingleNode("/order");
                XmlNode importNewNode = xdoc_new.ImportNode(newNode, true);
                xdoc_new_main.AppendChild(importNewNode);
            }
            xdoc_new.AppendChild(xdoc_new_main);
            XslTransform xslTransform = new XslTransform();
            xslTransform.Load(Request.ServerVariables["APPL_PHYSICAL_PATH"].ToString() + "order_search.xsl");
            Stream stream_xml = new MemoryStream();
            xslTransform.Transform((IXPathNavigable)xdoc_new, null, stream_xml);
            stream_xml.Position = 0;
            StreamReader sr = new StreamReader(stream_xml);
            //Session["Result"] = sr.ReadToEnd();
            lblErrorMessage.Text = "Total Order(s) Found: " + xResultName.Count.ToString();
            divResults.InnerHtml = sr.ReadToEnd();
            
            //Response.Write(sr.ReadToEnd());
            //Response.Redirect("checkout.aspx");
            //xdoc_new.Save(Server.MapPath("test"));
        }
        else
        {
            divResults.InnerHtml = "";
            lblErrorMessage.Text="No Results Found...";
            
        }


    }
    private void search_files(string directory, string filetosearch)
    {
        try
        {
            string[] fileEntries = Directory.GetFiles(directory, filetosearch);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                _alFileList.Add(fileEntries[i]);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        catch (PathTooLongException ex)
        {
        }
        catch (DirectoryNotFoundException ex)
        {
        }
    }
}
(c) dvds4less 2000-2010