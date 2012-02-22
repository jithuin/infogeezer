using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using mshtml;

namespace WebApplication2
{
	public partial class WebForm1 : System.Web.UI.Page
	{
		WebApplication2.WebServiceRefName.WebService1 service = new WebApplication2.WebServiceRefName.WebService1();
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void Button1_Click(object sender, EventArgs e)
		{
			this.TextBox1.Text = service.ReadLog();
			
		}

		protected void Button2_Click(object sender, EventArgs e)
		{
			this.TextBox1.Text = service.WriteLog(this.TextBox1.Text);
		}

		protected void Button4_Click(object sender, EventArgs e)
		{
			IHTMLDocument2 htmlDocument = (IHTMLDocument2)this.form1.InnerHtml;

			IHTMLSelectionObject currentSelection = htmlDocument.selection;

			if (currentSelection != null)
			{
				IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;

				if (range != null)
				{
					MessageBox.Show(range.text);
				}
			}

		}
	}
}
