using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmSatellites.
	/// </summary>
	public class frmSatellites : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox picBox;
		Bitmap worldMap, back;
		Graphics g;
		int space = 8;
		Size mapBmp;
	//	sSatInfo[] satInfo;
		byte curBound;
		int Top, Bottom;
		double xDiff, yDiff;
 
	/*	struct sSatInfo 
		{ 
			public byte nbr; 
			public byte pos; 
			public int Top; 
			public int Bottom; 
			public byte[] path; 
		} */
	
		public frmSatellites()
		{
			this.Text = language.getAString( language.order.satellites ); //"Satellites";

			this.picBox = new System.Windows.Forms.PictureBox(); 
			picBox.Size = this.Size; 
			this.Controls.Add( picBox );

			picBox.MouseDown += new MouseEventHandler( picBox_MouseDown );
			picBox.MouseUp += new MouseEventHandler(picBox_MouseUp);
			picBox.MouseMove +=new MouseEventHandler(picBox_MouseMove);

			back = new Bitmap( this.Width, this.Height );
			g = Graphics.FromImage( back );

			worldMap = drawMap.getBmp( 0, this.Width - space * 2, ( this.Width - space * 2 ) * 2 / 3 );
			mapBmp = worldMap.Size;
		//	g.Clear( Form1.defaultBackColor );

			curBound = 0;
			Top = Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Top;
			Bottom = Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Bottom;

			xDiff = (double)( mapBmp.Width - 2 ) / (double)Form1.game.width;
			yDiff = (double)( mapBmp.Height - 2 ) / (double)Form1.game.height;
		
			this.Closing += new CancelEventHandler(frmSatellites_Closing);
			justMouseDown = false;

			draw();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.picBox = new System.Windows.Forms.PictureBox();
			// 
			// pictureBox1
			// 
		//	this.pictureBox1.Size = new System.Drawing.Size(240, 272);
			// 
			// frmSatellites
			// 
			this.Controls.Add(this.picBox);
			this.Text = "frmSatellites";

		}
		#endregion

		private void draw()
		{
			g.Clear( Form1.defaultBackColor );
			g.DrawImage(
				worldMap,
				new Rectangle( space, space, worldMap.Width, worldMap.Height ),
				new Rectangle( 0, 0, worldMap.Width, worldMap.Height ),
				GraphicsUnit.Pixel
				);

			Pen pSat = new Pen( Color.Red );

			g.DrawLine( 
				pSat, 
				space + 1, 
				(int)( space + Top * yDiff + yDiff / 2 ), 
				space + mapBmp.Width - 1, 
				(int)( space + Top * yDiff + yDiff / 2 ) 
				);
			g.DrawLine( 
				pSat, 
				space + 1, 
				(int)( space + Bottom * yDiff + yDiff / 2 ), 
				space + mapBmp.Width - 1, 
				(int)( space + Bottom * yDiff + yDiff / 2 ) 
				);
			
			for ( int s = 0; s < Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list.Length; s ++ )
			{
				g.DrawEllipse(
					pSat,
					(int)(Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos * xDiff + xDiff / 2 + space + 1 - 3),
					(int)( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos ] * yDiff + yDiff / 2 + space + 1 - 3 ),
					6,
					6
					);

				double xPos = (int)(space + 1 + xDiff / 2);
				for ( 
					int x = ( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos - 10 > 0 ? Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].pos - 10 : 0 ); //0; 
					x < Form1.game.width - 1; 
					x++, xPos += xDiff 
					) 
				{
					g.DrawLine( pSat, 
						(int)xPos, 
						(int)( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ x ] * yDiff + yDiff / 2 + space + 1 ), 
						(int)(xPos + xDiff), // + diff / 2
						(int)( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ x + 1 ] * yDiff + yDiff / 2 + space + 1 ) 
						);
					g.DrawLine( pSat, 
						(int)xPos, 
						(int)( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ x ] * yDiff + yDiff / 2 + 1 + space + 1 ), 
						(int)(xPos + xDiff), // + diff / 2
						(int)( Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.list[ s ].path[ x + 1 ] * yDiff + yDiff / 2 + 1 + space + 1 ) 
						);	
				}
			}

			picBox.Image = back;
		}

		/// <summary>
		/// Sin( ( x * 6.283 ) / etendu + startX ) * amp + oriY
		/// </summary>
		/// <param name="x">Sin( ( ? * 6.283 ) / etendu + startX ) * amp + oriY</param>
		/// <param name="etendu">Sin( ( x * 6.283 ) / ? + startX ) * amp + oriY</param>
		/// <param name="startX">Sin( ( x * 6.283 ) / etendu + ? ) * amp + oriY</param>
		/// <param name="amp">Sin( ( x * 6.283 ) / etendu + startX ) * ? + oriY</param>
		/// <param name="oriY">Sin( ( x * 6.283 ) / etendu + startX ) * amp + ?</param>
		/// <returns></returns>
		private double sinEquation( int x, int etendu, double startX, int amp, int oriY )
		{ 
			return (double)( Math.Sin( ( x * Math.PI * 2 ) / etendu + startX ) * amp + oriY );
		}

		bool justMouseDown;
		private void picBox_MouseDown(object sender, MouseEventArgs e)
		{
			justMouseDown = true;
		/*	System.Threading.Thread.Sleep( 0 );
			Point click;
			click = picBox.PointToClient( new Point( frmSatellites.MousePosition.X - space, frmSatellites.MousePosition.Y - space ) );

			if ( click.X > 0 && click.X < mapBmp.Width && click.Y > 0 && click.Y < mapBmp.Height )
			{
				if ( click.Y < Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Top + 10 && click.Y > Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Top - 10 )
					curBound = 1;
				else if ( click.Y < Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Bottom + 10 && click.Y > Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Bottom - 10 )
					curBound = 2;
				else
					curBound = 0;
			}
			else
				curBound = 0;*/
		}
		private void picBox_MouseMove(object sender, MouseEventArgs e)
		{
			if ( justMouseDown )
			{
				justMouseDown = false;
				System.Threading.Thread.Sleep( 0 );

				Point click;

				click = picBox.PointToClient( new Point( frmSatellites.MousePosition.X - space, frmSatellites.MousePosition.Y - space ) );
				click = picBox.PointToClient( new Point( frmSatellites.MousePosition.X - space, frmSatellites.MousePosition.Y - space ) );
				if ( click.X > 0 && click.X < mapBmp.Width && click.Y > 0 && click.Y < mapBmp.Height )
				{
					if ( click.Y < Top * yDiff + 7 && click.Y > Top * yDiff - 7 )
						curBound = 1;
					else if ( click.Y < Bottom * yDiff + 7 && click.Y > Bottom * yDiff - 7 )
						curBound = 2;
					else
						curBound = 0;
				}
				else
					curBound = 0;
			}
			else
			{
				System.Threading.Thread.Sleep( 0 );
				if ( curBound > 0 )
				{
					Point click = picBox.PointToClient( new Point( frmSatellites.MousePosition.X - space - 1, frmSatellites.MousePosition.Y - space - 1 ) );
					click = picBox.PointToClient( new Point( frmSatellites.MousePosition.X - space - 1, frmSatellites.MousePosition.Y - space - 1 ) );

					if ( curBound == 1 )
						if ( click.Y > Bottom * yDiff - 10 )
							Top = Bottom - 10;
						else if ( click.Y < 0 )
							Top = 0;
						else
							Top = (int)( click.Y / yDiff );
					else if ( curBound == 2 )
						if ( click.Y < Top + 10 )
							Bottom = Top + 10;
						else if ( click.Y > mapBmp.Height )
							Bottom = Form1.game.height;
						else
							Bottom = (int)( click.Y / yDiff );

					draw();
				}
			}
		}

		private void picBox_MouseUp(object sender, MouseEventArgs e)
		{
			if ( curBound > 0 )
				Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.changePath( Top, Bottom );

			curBound = 0;
			draw();
		}

		private void frmSatellites_Closing(object sender, CancelEventArgs e)
		{
			Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Top = Top;
			Form1.game.playerList[ Form1.game.curPlayerInd ].Sats.spy.Bottom = Bottom;
		}
	}
}
