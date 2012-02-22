using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for FrmSaveGame.
	/// </summary>
	public class FrmSaveGame : FrmBaseGame
	{
		string lastPath;
		public FrmSaveGame( string directory, string lastPath )
		{
			this.directory = directory;
			this.Text = language.getAString( language.order.filesSavingGame );

			this.lastPath = lastPath;
			this.scenario = false;

			getFilesInDirectory(  );
			drawAll();
		}

		public override void getFilesInDirectory()// string directory )
		{
			base.getFilesInDirectory(  );

			selected = null;

			FileHeader[] fileBuffer = files;
			files = new FileHeader[ files.Length + 1 ];

			fileBuffer.CopyTo( files, 1 );

			files[ 0 ] = new NewFileHeader();

			for ( int i = 1; i < files.Length; i++ )
				if ( files[ i ].path == lastPath )
				{
					selected = files[ i ];

					if ( i > maxOnScreen )
						posAtTop = i - maxOnScreen;

					break;
				}

			if ( selected == null )
				selected = files[ 0 ];
		}

		public static string getNow( string directory, string lastPath )
		{
			wC.show = true;
			FrmSaveGame fsg = new FrmSaveGame( Form1.options.savesDirectoryFullPath, lastPath );
			wC.show = false;

			fsg.ShowDialog();
			return fsg.result;
		}

	/*	public static string getNow( string directory, bool scenario )
		{
			FrmSaveGame fsg = new FrmSaveGame( directory, scenario );
			fsg.ShowDialog();
			return fsg.result;
		}	*/
	}
}
