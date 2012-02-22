using System;
using System.Drawing;
using System.IO;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for FileHeader.
	/// </summary>
	public class FileHeader
	{
		public string path, name, playerName, civName, description, scenarioName;
		public Bitmap bmp;

		public const int BmpWidth = 40;
		public static int BmpHeight = BmpWidth * 2/3;

		public Type type;
		public Scenario.GoalType goalType;
		public int goalInd, turn;
		public DateTime modified;

		public FileHeader()
		{
		}

#region getFromStream
		public static FileHeader getFromStream( FileHeader intro, string path, int version, BinaryReader reader )
		{
			intro.path = path;
			intro.modified = System.IO.File.GetLastWriteTime( path );

			if ( version == 887 )
			{
				intro.type = (Type)reader.ReadByte();

				if ( intro.type == Type.playedGame ) // normal
				{
					intro.name = System.IO.Path.GetFileNameWithoutExtension( path );

					intro.playerName = reader.ReadString();
					intro.civName = reader.ReadString();
					intro.turn = reader.ReadInt32();

					loadMap( intro, reader );
				}
				else if ( intro.type == Type.playedScenario ) // played scenario
				{
					intro.playerName = reader.ReadString();
					intro.civName = reader.ReadString();
					intro.name = System.IO.Path.GetFileNameWithoutExtension( path );
					intro.scenarioName = reader.ReadString();
					intro.turn = reader.ReadInt32();

					loadMap( intro, reader );
					
					intro.goalType = (Scenario.GoalType)reader.ReadByte();
					intro.goalInd = reader.ReadInt32();
				}
				else if ( intro.type == Type.scenario ) // scenario
				{
					intro.name = reader.ReadString();
					intro.description = reader.ReadString();
					intro.turn = reader.ReadInt32();

					loadMap( intro, reader );
					
					intro.goalType = (Scenario.GoalType)reader.ReadByte();
					intro.goalInd = reader.ReadInt32();
				}

			}
			else
			{
				intro.playerName = "-";
				intro.civName = "-";
				intro.turn = 0;
				intro.name = System.IO.Path.GetFileNameWithoutExtension( path );
			}

			return intro;
		}
#endregion
		
#region getFromPath

		public static FileHeader getFromPath( string path )
		{
			FileHeader intro = new FileHeader();
			FileStream file = null;
			BinaryReader reader = null;
			bool success;

		//	intro.path = path;

			try
			{	
				file = new FileStream( path, FileMode.Open, FileAccess.Read );
				reader = new BinaryReader( file );

				int version = reader.ReadInt32();

				intro = getFromStream( intro, path, version, reader );

				success = true;
			}
			catch ( Exception /*e*/ )
			{
				success = false;
			}
			finally
			{
				if ( file != null )
					file.Close();

				if ( reader != null )
					reader.Close();
			}

			if ( success )
				return intro;
			else return null;
		}
#endregion

#region draw

		public virtual void draw( Graphics g, Rectangle dest )
		{
			Font txtFont = new Font(
				"Tahoma",
				9,
				FontStyle.Regular
				);
			Brush blackBrush = new SolidBrush( Color.Black );
			int border = 4, 
				textHeight = (int)g.MeasureString( "a", txtFont ).Height - 2;

			g.FillRectangle( 
				new SolidBrush( Form1.defaultBackColor ), 
				dest 
				);

			if ( bmp != null )
				g.DrawImage(
					bmp,
					new Rectangle( 
						dest.Left + border,
						dest.Top + dest.Height / 2 - BmpWidth / 3,
						BmpWidth, 
						BmpHeight 
						),
					new Rectangle( 
						0, 
						0, 
						bmp.Width, 
						bmp.Height 
						),
					GraphicsUnit.Pixel
					);
			else
				g.FillRectangle( 
					blackBrush, 
					new Rectangle( 
						dest.Left + border, 
						dest.Top + dest.Height / 2 - BmpWidth / 3, 
						BmpWidth, 
						BmpHeight
						) 
					);

			g.DrawString( 
				name,
				txtFont,
				blackBrush,
				dest.Left + BmpWidth + 2*border,
				dest.Top // + border
				);

			if ( type == Type.playedGame || type == Type.playedScenario ) // played
			{
				g.DrawString( 
					playerName,
					txtFont,
					blackBrush,
					dest.Left + BmpWidth + 2*border,
					dest.Top + textHeight // + border
					);
				g.DrawString( 
					civName,
					txtFont,
					blackBrush,
					dest.Left + BmpWidth + 2*border,
					dest.Top + 2*textHeight // + border
					);

				string time; 
				if ( 
					modified.Year == System.DateTime.Now.Year && 
					modified.DayOfYear == System.DateTime.Now.DayOfYear 
					) 
					time = modified.ToShortTimeString(); 
				else 
					time = modified.ToShortDateString(); 

				g.DrawString( 
					time, 
					txtFont, 
					blackBrush, 
					dest.Right - g.MeasureString( time, txtFont ).Width - 2, 
					dest.Top + textHeight 
					); 
			}
			else if ( type == Type.playedScenario )
			{
				g.DrawString( 
					playerName,
					txtFont,
					blackBrush,
					dest.Left + BmpWidth + 2*border,
					dest.Top + textHeight // + border
					);
				g.DrawString( 
					civName,
					txtFont,
					blackBrush,
					dest.Left + BmpWidth + 2*border,
					dest.Top + 2*textHeight // + border
					);
				
				string time; 
				if ( 
					modified.Year == System.DateTime.Now.Year && 
					modified.DayOfYear == System.DateTime.Now.DayOfYear 
					) 
					time = modified.ToShortTimeString(); 
				else 
					time = modified.ToShortDateString(); 

				g.DrawString( 
					time, 
					txtFont, 
					blackBrush, 
					dest.Right - g.MeasureString( time, txtFont ).Width - 2, 
					dest.Top + textHeight 
					); 
			}
			else if ( type == Type.scenario ) // normal
			{
				g.DrawString( 
					description,
					txtFont,
					blackBrush,
					new Rectangle(
						dest.Left + BmpWidth + 2*border,
						dest.Top + textHeight /* + border*/,
						dest.Width - 3 * border - BmpWidth,
						dest.Height - border - textHeight
						)
					);
			}
		}
		#endregion

#region loadMap
	/*	public static void loadMap( int width, int height, BinaryReader reader )
		{
			FileHeader intro = new FileHeader();
	//		intro.
			loadMap
		}	*/

		public static void loadMap( FileHeader intro, BinaryReader reader )
		{
			if ( intro == null )
				intro = new FileHeader();

			intro.bmp = new Bitmap( reader.ReadInt32(), reader.ReadInt32() );
			Color[] colors = new Color[ reader.ReadInt32() ];

			for ( int c = 0; c < colors.Length; c ++ )
				colors[ c ] = Color.FromArgb( reader.ReadInt32() );

			for ( int x = 0; x < intro.bmp.Width; x ++ )
				for ( int y = 0; y < intro.bmp.Height; y ++ )
					intro.bmp.SetPixel( x, y, colors[ reader.ReadByte() ] ); // Color.FromArgb( reader.ReadInt32() ) );
		}
		#endregion

		public enum Type : byte
		{
			playedGame,
			playedScenario,
			scenario
		}
	}
}
