using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for FrmBaseGame.
	/// </summary>
	public class FrmBaseGame : System.Windows.Forms.Form
	{
		public FileHeader[] files;
		public bool scenario;
		public int/* selected,*/ posAtTop;

		Graphics bufferGraphics, screenGraphics;
		Bitmap bufferBitmap;
		Pen selectedPen = new Pen( Color.White ),
			normalPen = new Pen( Color.Black );
		Brush arrowBrush = new SolidBrush( Color.BurlyWood );
		Point[] topArrow, bottomArrow;

		public string result = null;
		public string directory;

		const int itemHeight = 40,
			borders = 8;
		public int topBorder;
		public int maxOnScreen;

		public FileHeader m_selected;
		public FileHeader selected
		{
			set
			{
				butDelete.Enabled = value != null && !(value is NewFileHeader);
				m_selected = value;
			}
			get
			{
				return m_selected;
			}
		}

		int indSelected;

	//	Button butOk, butDelete, butCancel
		MenuItem butOk, butDelete, butCancel, separator;

		bool cancel = false;

		public FrmBaseGame() //string directory, bool scenario )
		{
			this.ControlBox = false;
			MainMenu menu = new MainMenu();

			butOk = new MenuItem();
			butOk.Text = language.getAString( language.order.ok );
			butOk.Click +=new EventHandler(butOk_Click);
			menu.MenuItems.Add( butOk );

			butCancel = new MenuItem();
			butCancel.Text = language.getAString( language.order.cancel );
			butCancel.Click += new EventHandler(butCancel_Click);
			menu.MenuItems.Add( butCancel );

			separator = new MenuItem();
			separator.Text = "|";
			separator.Enabled = false; //.Click += new EventHandler(butCancel_Click);
			menu.MenuItems.Add( separator );

			butDelete = new MenuItem();
			butDelete.Text = language.getAString( language.order.delete ); // todo delete
			butDelete.Click += new EventHandler(butDelete_Click);
			menu.MenuItems.Add( butDelete );

			this.Menu = menu;

			maxOnScreen = ( this.ClientSize.Height - borders) / ( itemHeight + borders );
			topBorder = ( this.ClientSize.Height - maxOnScreen * (itemHeight + borders ) + borders ) / 2;    //( this.ClientSize.Height - maxOnScreen * (itemHeight + borders ) ) / 2;

			topArrow = new Point[]{
										  new Point( this.ClientSize.Width / 2,		topBorder * 1 / 4 ),
										  new Point( this.ClientSize.Width * 3 / 4,	topBorder * 3 / 4 ),
										  new Point( this.ClientSize.Width * 1 / 4,	topBorder * 3 / 4 )
									  };
			bottomArrow = new Point[]{
										  new Point( this.ClientSize.Width / 2,		this.ClientSize.Height - topBorder * 1 / 4 ),
										  new Point( this.ClientSize.Width * 3 / 4,	this.ClientSize.Height - topBorder * 3 / 4 ),
										  new Point( this.ClientSize.Width * 1 / 4,	this.ClientSize.Height - topBorder * 3 / 4 )
									  };

			bufferBitmap = new Bitmap( this.ClientSize.Width, this.ClientSize.Height );
			bufferGraphics = Graphics.FromImage( bufferBitmap );

			screenGraphics = this.CreateGraphics();
		}

		public virtual void getFilesInDirectory()// string directory )
		{
			if ( !System.IO.Directory.Exists( directory ) )
				System.IO.Directory.CreateDirectory( directory );

			string[] paths = System.IO.Directory.GetFiles( directory )/*,
				dirs = System.IO.Directory.GetDirectories( directory )*/;

			int pos = 0;

			for ( int i = 0; i < paths.Length; i ++ )
				if ( System.IO.Path.GetExtension( paths[ i ] ) == ( scenario? ".phm" : ".phs") )
					pos ++;
				else paths[ i ] = null;

			FileHeader[] tempFiles = new FileHeader[ pos ];

			pos = 0;
			for ( int i = 0; i < paths.Length; i ++ )
				if ( paths[ i ] != null )
				{
					tempFiles[ pos ] = FileHeader.getFromPath( paths[ i ] );

					if ( tempFiles[ pos ] != null )
						pos ++;
				}

			files = new FileHeader[ pos ];

			pos = 0;
			for ( int i = 0; i < tempFiles.Length; i ++ )
				if ( tempFiles[ i ] != null )
				{
					files[ pos ] = tempFiles[ i ]; // FileHeader.getFrom( paths[ i ] );
					pos ++;
				}

			posAtTop = 0;

			if ( files.Length > 0 )
				selected = files[ 0 ];
		}

		public void drawFrames()
		{
			for ( int i = posAtTop, pos = 0; i < files.Length && pos < maxOnScreen; i ++, pos ++ )
				if ( selected == files[ i ] )
					screenGraphics.DrawRectangle(
						selectedPen,
						getRectAtPos( pos ) 
						);
				else
					screenGraphics.DrawRectangle( 
						normalPen, 
						getRectAtPos( pos ) 
						);
		}

		public void drawAll()
		{
			drawAll( false );
		}

		public void drawAll( bool invalidate )
		{
			bufferGraphics.Clear( Form1.defaultBackColor );

			if ( files.Length > 0 )
			{
				for ( int i = posAtTop, pos = 0; i < files.Length && pos < maxOnScreen; i ++, pos ++ )
					files[ i ].draw( bufferGraphics, getRectAtPos( pos ) );

				if ( canGoUp )
				{
					bufferGraphics.FillPolygon(
						arrowBrush,
						topArrow
						);
					bufferGraphics.DrawPolygon(
						normalPen,
						topArrow
						);
				}

				if ( canGoDown )
				{
					bufferGraphics.FillPolygon(
						arrowBrush,
						bottomArrow
						);
					bufferGraphics.DrawPolygon(
						normalPen,
						bottomArrow
						);
				}


			}
			else
			{
				Font txtFont = new Font(
					"Tahoma",
					9,
					FontStyle.Regular
					);
				SizeF size = bufferGraphics.MeasureString( 
					language.getAString( language.order.empty ),
					txtFont
					);

				bufferGraphics.DrawString(
					language.getAString( language.order.empty ),
					txtFont,
					new SolidBrush( Color.Black ),
					(int)(this.ClientSize.Width - size.Width) / 2,
					(int)(this.ClientSize.Height - size.Height) / 2
					);

			}

			if ( invalidate )
				this.Invalidate();

			drawFrames();
		}

		private Rectangle getRectAtPos( int pos )
		{
			return new Rectangle( 
				borders,
				topBorder + borders * pos + pos*itemHeight,
				this.ClientSize.Width - 2*borders,
				itemHeight
				);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(
				bufferBitmap,
				e.ClipRectangle,
				e.ClipRectangle,
				GraphicsUnit.Pixel
				);

			drawFrames();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if ( e.Y < topBorder )
				goUp();
			else if ( e.Y > topBorder + maxOnScreen * (itemHeight + borders) )
			{
		//		int bob = topBorder + maxOnScreen * (itemHeight + borders);
				goDown();
			}
			else for ( int i = posAtTop, pos = 0; i < files.Length && pos < maxOnScreen; i ++, pos ++ )
					 if ( getRectAtPos( pos ).Contains( e.X, e.Y ) )
					 {
						 if ( selected != files[ i ] )
						 {
							 selected = files[ i ];
							 indSelected = i;
							 drawFrames();
						 }

						 break;
					 }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch ( e.KeyCode )
			{
				case System.Windows.Forms.Keys.Up:
					goUp();
					break;
					
				case System.Windows.Forms.Keys.Down:
					goDown();
					break;
			}
		}

		private bool canGoUp
		{
			get{ return posAtTop > 0; }
		}
		private void goUp()
		{
			if ( canGoUp ) //posAtTop > 0 )
			{
				posAtTop --;

				if ( indSelected > posAtTop + maxOnScreen - 1 )
				{
					selected = files[ posAtTop + maxOnScreen - 1 ];
					indSelected = posAtTop + maxOnScreen;
				}

				drawAll( true );
			}
		}

		private bool canGoDown
		{
			get{ return files.Length > maxOnScreen + posAtTop; }
		}
		private void goDown()
		{
			if ( canGoDown )//files.Length > maxOnScreen + posAtTop )
			{
				posAtTop ++;

				if ( indSelected < posAtTop )
				{
					selected = files[ posAtTop ];
					indSelected = posAtTop;
				}

				drawAll( true );
			}
		}

		private void butOk_Click(object sender, EventArgs e)
		{
			result = selected.path;
			this.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if ( !cancel )
				result = selected.path;

			base.OnClosing (e);
		}

		private void butCancel_Click(object sender, EventArgs e)
		{
			result = null;
			cancel = true;
			this.Close();
		}

	/*	public static string getNow( string directory, bool scenario )
		{
			FrmLoadGame flg = new FrmLoadGame( directory, scenario );
			flg.ShowDialog();
			return flg.result;
		}*/

		private void butDelete_Click(object sender, EventArgs e)
		{
			if ( 
				MessageBox.Show( 
					String.Format( language.getAString( language.order.filesDialogDeleteText ), selected.name ),
					language.getAString( language.order.filesDialogDeleteTitle ),
					MessageBoxButtons.YesNo,
					MessageBoxIcon.None,
					MessageBoxDefaultButton.Button2
					) == DialogResult.Yes
				)
			{
				System.IO.File.Delete( selected.path );
				getFilesInDirectory();
				drawAll( true );
			}
		}
	}
}
