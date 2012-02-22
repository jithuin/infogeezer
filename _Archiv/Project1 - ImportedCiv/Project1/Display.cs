using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Display.
	/// </summary>
	public class Display
	{
		public Display( Form1 form )
		{
			this.form = form;
			backBuffer = new Bitmap( form.ClientSize.Width, form.ClientSize.Height );
		}

		public Point center;
		Form1 form;
		Image backBuffer,
			miniMapBuffer,
			unitBuffer,
			terrainBuffer;

		public void draw()
		{
		}

		public void drawBack( Graphics g )
		{
			#region map
		//	for
		//		for
			#endregion
			
			#region unit list
			#endregion
			
			#region mini map
			#endregion
		}
	}
}
