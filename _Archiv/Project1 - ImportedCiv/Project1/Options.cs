using System;
using System.IO;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options
	{
		public FrontierTypes frontierType;
		public MiniMapTypes miniMapType;

		public bool hideUndiscovered, // not to be saved
			showGrid, 
			showOnScreenDPad, 
			showBatteryStatus, 
			showLabels, 
			showCommonSpyDialogs, 
			autosave; 

		public string lastPlayerName,
	//		lastSavePath,
			languageFile,
			savesDirectory;	
	
		public string savesDirectoryFullPath
		{
			get
			{
				if ( System.IO.Path.IsPathRooted( savesDirectory ) )
					return savesDirectory;
				else
				{
					string path = platformSpec.main.appPath + @"\" + savesDirectory;
					return path;
				}
			}
		}
		
		private string m_lastSavePath;
		public string lastSavePath
		{
			get
			{
				if ( Form1.game == null )
					return m_lastSavePath;
				else
					return Form1.game.savePath;
			}

			set	{	m_lastSavePath = value;	}
		}

		public static Options Default = new Options( 
									FrontierTypes.nationColor, MiniMapTypes.showTerritories,
									true, false, false, false, true, true, true,
									platformSpec.main.ownerName, "", "", @"saves\"
									);

		public Options()
		{
		}

		public Options( 
			FrontierTypes frontierType, MiniMapTypes miniMapType, 
			bool hideUndiscovered, bool showGrid, bool showOnScreenDPad, bool showBatteryStatus, bool showLabels, bool showCommonSpyDialogs, bool autosave,
			string lastPlayerName, string lastSavePath, string languageFile, string savesDirectory
			)
		{
			this.frontierType = frontierType;
			this.miniMapType = miniMapType;

			this.hideUndiscovered = hideUndiscovered;
			this.showGrid = showGrid;
			this.showOnScreenDPad = showOnScreenDPad;
			this.showBatteryStatus = showBatteryStatus;
			this.showLabels = showLabels;
			this.showCommonSpyDialogs = showCommonSpyDialogs;
			this.autosave = autosave;

			this.lastPlayerName = lastPlayerName;
			this.lastSavePath = lastSavePath;
			this.languageFile = languageFile;
			this.savesDirectory = savesDirectory;
		}

		public void setDefaults()
		{
			hideUndiscovered = true;
		}

		public enum MiniMapTypes : byte
		{
			showTerrrains,
			showTerritories
		}

		public enum FrontierTypes : byte
		{
			relations,
			nationColor
		}

#region IO

		private static string configurationFilePath
		{
			get
			{
				return Form1.appPath + "\\user.cfg";
			}
		}

		static string[] prefixes = new string[]{
											 "Pocket Humanity version: ",
											 "Player name: ",
											 "Last saved game: ",

											 "Frontier type: ",
											 "Mini map type: ",

											 "Show grid: ",
											 "Show on screen DPad: ",
											 "Show battery status: ",
											 "Show labels: ",
											 "Show common spy dialogs: ",
											 "Autosave: ",

											 "Language file: ",
											 "Saves directory: ",
										 };

		public static Options load()
		{
	/*		bool b0 = System.IO.Path.IsPathRooted( @"\My Documents\" );
			bool b1 = System.IO.Path.IsPathRooted( @"My Documents\" );
			bool b2 = System.IO.Path.IsPathRooted( @"My Documents" );
			char[] c0 = System.IO.Path.InvalidPathChars;
			char c1 = System.IO.Path.PathSeparator;
			char c2 = System.IO.Path.VolumeSeparatorChar;*/

			bool success = false;
			Options options = new Options();
			StreamReader reader = null;
			FileStream file = null;

			try
			{
				file = new FileStream( configurationFilePath, FileMode.Open, FileAccess.Read );
				reader = new StreamReader( file );

				//	string vStr = reader.ReadLine().Remove( 0, prefixes[ 0 ].Length );
				double version = Convert.ToDouble( reader.ReadLine().Remove( 0, prefixes[ 0 ].Length ) );

				if ( version < 0.885 )
				{
				}
				else if ( version < 0.887 )
				{
					options.lastPlayerName =		reader.ReadLine().Remove( 0, prefixes[ 1 ].Length );
					options.lastSavePath =			reader.ReadLine().Remove( 0, prefixes[ 2 ].Length );
					options.frontierType =			(FrontierTypes)Convert.ToInt32( reader.ReadLine().Remove( 0, prefixes[ 3 ].Length ) );
					options.miniMapType =			(MiniMapTypes)Convert.ToInt32( reader.ReadLine().Remove( 0, prefixes[ 4 ].Length ) );
					options.showGrid =				Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 5 ].Length ) );
					options.showOnScreenDPad =		Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 6 ].Length ) );
					options.showBatteryStatus =		Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 7 ].Length ) );
					options.showLabels =			Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 8 ].Length ) );
					options.showCommonSpyDialogs =	Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 9 ].Length ) );
					options.autosave =				Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 10 ].Length ) );
					options.languageFile =			reader.ReadLine().Remove( 0, prefixes[ 11 ].Length );

					options.savesDirectory =		Default.savesDirectory;
				}
				else
				{
					options.lastPlayerName =		reader.ReadLine().Remove( 0, prefixes[ 1 ].Length );
					options.lastSavePath =			reader.ReadLine().Remove( 0, prefixes[ 2 ].Length );
					options.frontierType =			(FrontierTypes)Convert.ToInt32( reader.ReadLine().Remove( 0, prefixes[ 3 ].Length ) );
					options.miniMapType =			(MiniMapTypes)Convert.ToInt32( reader.ReadLine().Remove( 0, prefixes[ 4 ].Length ) );
					options.showGrid =				Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 5 ].Length ) );
					options.showOnScreenDPad =		Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 6 ].Length ) );
					options.showBatteryStatus =		Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 7 ].Length ) );
					options.showLabels =			Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 8 ].Length ) );
					options.showCommonSpyDialogs =	Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 9 ].Length ) );
					options.autosave =				Convert.ToBoolean( reader.ReadLine().Remove( 0, prefixes[ 10 ].Length ) );
					options.languageFile =			reader.ReadLine().Remove( 0, prefixes[ 11 ].Length );
					options.savesDirectory =		reader.ReadLine().Remove( 0, prefixes[ 12 ].Length );
				}

				success = true;
			}
			catch ( System.IO.FileNotFoundException /*e*/ )
			{
				options = Default;
			}
			catch ( Exception /*e*/ )
			{
				MessageBox.Show( 
					"Reading user preferences failed. The configuration file will now be cleared. This should happen only once every major updates.", 
					"Error" 
					);

			}
			finally
			{
				if ( reader != null )
					reader.Close();

				if ( file != null )
					file.Close();
			}

			if ( !success )
				options.save();

			return options;
		}

		public bool save()
		{
			bool success = false;
			StreamWriter writer = null;
			FileStream file = null;

		/*	try
			{*/
				file = new FileStream( configurationFilePath, FileMode.Create, FileAccess.Write );
				writer = new StreamWriter( file );

				writer.WriteLine( prefixes[ 0 ] + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() );
				writer.WriteLine( prefixes[ 1 ] + lastPlayerName );
				writer.WriteLine( prefixes[ 2 ] + lastSavePath ); // lastSavePath );
				writer.WriteLine( prefixes[ 3 ] + (int)frontierType );
				writer.WriteLine( prefixes[ 4 ] + (int)miniMapType );
				writer.WriteLine( prefixes[ 5 ] + showGrid );
				writer.WriteLine( prefixes[ 6 ] + showOnScreenDPad );
				writer.WriteLine( prefixes[ 7 ] + showBatteryStatus );
				writer.WriteLine( prefixes[ 8 ] + showLabels );
				writer.WriteLine( prefixes[ 9 ] + showCommonSpyDialogs );
				writer.WriteLine( prefixes[ 10 ] + autosave );
				writer.WriteLine( prefixes[ 11 ] + languageFile );
				writer.WriteLine( prefixes[ 12 ] + savesDirectory );

				success = true;
		/*	}
			catch ( Exception e )
			{
				MessageBox.Show( "Saving user preference failed. Sorry", e.Message );
			}
			finally
			{*/

				if ( writer != null )
				{
			//		writer.Flush();
					writer.Close();
				}

				if ( file != null )
					file.Close();
		//	}

			return success;
		}
/**/
#endregion

	}
}
