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
using System.Text;

public partial class Controls_RichTextEditor : System.Web.UI.UserControl
{
    public string Text
    {
        set
        {
            textArea.Value = value;
        }
        get
        {
            string data = textArea.Value.Replace("'", "''").Replace("\"", "''");
            //byte[] dataBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(data);
            //data = System.Text.ASCIIEncoding.ASCII.GetString(dataBytes);
            ViewState["__EditorText"] = data;
            if (ViewState["__EditorText"] != null)
            {
                return ViewState["__EditorText"].ToString();
            }
            else
                return "";
        }
    }

    private string height;
    private string width;
    public string EditorHeight
    {
        set
        {
            height = value;
        }
        get
        {
            if (height == null || height.Equals(string.Empty))
            {
                return "200";
            }
            return height;
        }
    }
    public string EditorWidth
    {
        set
        {
            width = value;
        }
        get
        {
            if (width == null || width.Equals(string.Empty))
            {
                return "530";
            }
            return width;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string script = "document.forms[0].onsubmit = onsubmithandler;\n";
        script += "function onsubmithandler() { document.getElementById('" + textArea.ClientID + "').value = frames['textEditor'].document.body.innerHTML; }";
        Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "__GET_TEXT", script, true);
    }
}
