using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for showProgress.
	/// </summary>
	public class showProgress
	{

		public static int progressBar
		{
			get
			{
				return opening.progressBar1.Value;
			}
			set
			{
				opening.progressBar1.Visible = true;
				opening.progressBar1.Value = value;
				opening.progressBar1.Update();
			}
		}

		public static void incProgressBar()
		{
			opening.progressBar1.Visible = true;
			opening.progressBar1.Value ++;
			opening.progressBar1.Update();
		}

		public static string lblInfo
		{
			get
			{
				return opening.lblNewGameInfo.Text;
			}
			set
			{
				opening.lblNewGameInfo.BringToFront();
				opening.lblNewGameInfo.Visible = true;
				opening.lblNewGameInfo.Text = value;
				opening.lblNewGameInfo.Update();
			}
		}

		public static bool showProgressInd
		{
			set
			{
				/*opening.lblNewGameInfo.Visible = value;
				opening.progressBar1.Visible = value;*/
				opening.panel3.Visible = value;

				if ( value == false )
				{
					opening.lblNewGameInfo.Text = "";
					opening.progressBar1.Value = 0;
				}
			}
			get
			{
				return opening.lblNewGameInfo.Visible;
			}
		}

	}
}
