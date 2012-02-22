using System;
using System.IO;
using System.Drawing;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Civilization.
	/// </summary>
	public class Civilization : General
	{
	//	public string name, description;
		public System.Drawing.Color color;
		public byte cityType;
		public string[] leaderNames;
		public string[] cityNames;

		public Civilization( byte type )
		{
			this.type = type;
		}

		public Civilization( byte type, string name, string[] leaderNames, string description, Color color, string fileName )
		{
			this.type = type;

			this.name = name; 
			this.leaderNames = leaderNames; 
			this.description = description; 
			this.color = color; 
			
			if ( fileName == null ) 
				fileName = Form1.appPath + @"\cities\" + name + "Cities.txt"; 
			else 
				fileName = Form1.appPath + @"\cities\" + fileName + "Cities.txt"; 

			try
			{
				FileStream fs = new FileStream( fileName, FileMode.Open );
				StreamReader sr = new StreamReader( fs, true );
				string[] list = new string[ 100 ];
				string fullList = sr.ReadToEnd();
				sr.Close(); 
				fs.Close(); 

				this.cityNames = fullList.Split( "\n".ToCharArray() );

				for ( int i = 0; i < this.cityNames.Length; i ++ )
					this.cityNames[ i ] = this.cityNames[ i ].TrimEnd( "\r".ToCharArray() );
			}
			catch ( Exception e )
			{
				throw ( new Exception( "Error loading: " + fileName + "\n\n" + e.Message ) );
			}
		}	
		
	}
}
