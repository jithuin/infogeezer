using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for FrmLoadGame.
	/// </summary>
	public class FrmLoadScenario : FrmBaseGame
	{
		public FrmLoadScenario( string directory )
		{
			this.directory = directory;
			this.Text = language.getAString( language.order.filesLoadingScenario );

			this.scenario = true;
			getFilesInDirectory( );
			drawAll();
		}

		public static string getNow( )
		{
			wC.show = true;
			FrmLoadGame flg = new FrmLoadGame( platformSpec.main.appPath + @"\Scenarios\" );
			wC.show = false;

			flg.ShowDialog();
			return flg.result;
		}
	}
}
