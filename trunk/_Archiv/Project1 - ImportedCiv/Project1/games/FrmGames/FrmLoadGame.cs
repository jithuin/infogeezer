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
	public class FrmLoadGame : FrmBaseGame
	{
		public FrmLoadGame( string directory )
		{
			this.directory = directory;
			this.Text = language.getAString( language.order.filesLoadingGame );

			this.scenario = false;
			getFilesInDirectory();
			drawAll();
		}

		public override void getFilesInDirectory() // string directory )
		{
			string atoDir = directory + @"auto\";

			if ( !System.IO.Directory.Exists( directory ) )
				System.IO.Directory.CreateDirectory( directory );

			if ( !System.IO.Directory.Exists( atoDir ) )
				System.IO.Directory.CreateDirectory( atoDir );

			string[] normalPaths = System.IO.Directory.GetFiles( directory ),
				autoPaths = System.IO.Directory.GetFiles( atoDir );

			string[] paths = new string[ normalPaths.Length + autoPaths.Length ];
			normalPaths.CopyTo( paths, 0 );
			autoPaths.CopyTo( paths, normalPaths.Length );

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

			foreach ( FileHeader file in files )
			{
				string dir = System.IO.Path.GetFileName( System.IO.Path.GetDirectoryName( file.path ) );

				if ( dir != "saves" )
					file.name = dir + @" \ "+ file.name;
			}

			posAtTop = 0;

			if ( files.Length > 0 )
				selected = files[ 0 ];
		}

		public static string getNow()
		{
			wC.show = true;
			FrmLoadGame flg = new FrmLoadGame( Form1.options.savesDirectoryFullPath );
			wC.show = false;

			flg.ShowDialog();
			return flg.result;
		}
	}
}
