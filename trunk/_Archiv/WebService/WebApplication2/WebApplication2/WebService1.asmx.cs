using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;
using System.Security;

namespace WebApplication2
{
	/// <summary>
	/// Summary description for WebService1
	/// </summary>
	[WebService(Namespace = "http://leonzrt.dyndns.org:3405/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class WebService1 : System.Web.Services.WebService
	{

		[WebMethod]
		public string HelloWorld()
		{
			try
			{
				FileInfo fi = new FileInfo("C:\\sys.log");
				StreamWriter sw;
				if (fi.Exists)
					sw = fi.AppendText();
				else
					sw = fi.CreateText();
				sw.WriteLine("Hello World!\r\n");
				sw.Close();
			}
			catch (Exception)
			{

				throw;
			}
			return "Hello World";
		}
		[WebMethod]
		public string ReadLog()
		{
			FileInfo fi = new FileInfo("J:\\_net\\sys.log");
			StreamReader sr;
			if (fi.Exists)
			{
				sr = fi.OpenText();
				string s= sr.ReadToEnd();
				sr.Close();
				return s;
			}
			else
				return "üres";
		}
		[WebMethod]
		public string WriteLog(string s)
		{
			try
			{
				FileInfo fi = new FileInfo("J:\\_net\\sys.log");
				StreamWriter sw;
				if (fi.Exists)
					sw = fi.AppendText();
				else
					sw = fi.CreateText();
				sw.WriteLine(s + "\r\n");
				sw.Close();
			}
			catch (IOException ex)
			{
				return "IO Error";
			}
			catch (UnauthorizedAccessException ex)
			{
				return "Unauthorized Access";
			}
			catch (SecurityException ex)
			{
				return "Security Error";
			}
			catch (NotSupportedException ex)
			{
				return "not supported";
			}
			return "Hello World";
		}
	}
}
