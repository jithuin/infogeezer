using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc.forms
{
	/// <summary>
	/// Summary description for FrmOpenScenario.
	/// </summary>
	public class FrmOpenScenario : System.Windows.Forms.Form
	{
		Result result;
		Label lblMapName, lblPlayer, lblDifficulty;
		ComboBox cbPlayer;
		TrackBar tbDifficulty;

		public FrmOpenScenario()
		{
			#region controls

			const int spacing = 8;

			lblMapName = new Label();
			lblMapName.Location = new Point( spacing, spacing );
			lblMapName.Width = this.ClientSize.Width; 
			this.Controls.Add( lblMapName );
			

			lblPlayer = new Label();
			lblPlayer.Location = new Point( spacing, lblMapName.Bottom + spacing );
			lblPlayer.Width = (this.ClientSize.Width - 3* spacing) / 2;
			lblPlayer.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add( lblPlayer );

			cbPlayer = new ComboBox();
			cbPlayer.Location = new Point( lblPlayer.Right + spacing, lblPlayer.Top );
			cbPlayer.Width = (this.ClientSize.Width - 3* spacing) / 2;
			this.Controls.Add( cbPlayer );
			

			lblDifficulty = new Label();
			lblDifficulty.Location = new Point( spacing, cbPlayer.Bottom + spacing );
			lblDifficulty.Width = (this.ClientSize.Width - 3* spacing) / 2;
			lblDifficulty.TextAlign = ContentAlignment.TopRight;
			this.Controls.Add( lblDifficulty );

			tbDifficulty = new TrackBar();
			tbDifficulty.Location = new Point( lblDifficulty.Right + spacing, lblDifficulty.Top );
			tbDifficulty.Width = (this.ClientSize.Width - 3* spacing) / 2;
			this.Controls.Add( tbDifficulty );

			#endregion

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "pH map (*.phm)|*.map|All files (*.*)|*.*";

			if ( ofd.ShowDialog() == DialogResult.OK )
			{
				lblMapName.Text = "";
			}
			else
			{
				result = new Result( 0 );
				result.valid = false;
			}
		}

		public static Result show()
		{
			FrmOpenScenario fos = new FrmOpenScenario();
			fos.ShowDialog();
			return fos.result;
		}

		public struct Result
		{
			public Result( int playerInd )
			{
				this.playerInd = playerInd;
				difficulty = 0;
				valid = true;
				reader = null;
			}

			public int playerInd, difficulty;
			public bool valid;
	//		public string playerName;
			public System.IO.BinaryReader reader;
		}
	}
}
