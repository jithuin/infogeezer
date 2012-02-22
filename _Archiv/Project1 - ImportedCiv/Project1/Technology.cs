using System;
using System.Drawing;

namespace xycv_ppc.Stat
{
	/// <summary>
	/// Summary description for Technology.
	/// </summary>
	public class Technology : General
	{
	//	public string name, description;
		public byte[] needs;
		public int cost;
		public string shortDesc;
		public bool canBeResearched;
		public int militaryValue;

		#region drawing

		public byte line;
		public int posOnLine;
		public System.Drawing.Rectangle rect;

		#endregion

		public Technology( byte type )
		{
			this.type = type;
		}
		
		public Technology( byte type, string name, int cost, byte line, int posOnLine, byte needs0, byte needs1, byte needs2, bool canBeResearched, string shortDesc, string description)
		{
			this.type = type;
			this.needs = new byte[3];

			this.name = name;
			this.description = description;
			this.shortDesc = shortDesc;
			this.canBeResearched = !canBeResearched;

			if ( needs0 != 0 )
			{
				this.needs[ 0 ] = needs0;
				this.needs[ 1 ] = needs1;
				this.needs[ 2 ] = needs2;
			}

			if ( type == 0 )
				this.cost = 1;
			else
				this.cost = line * line * 30; 

			this.line = line;
			this.posOnLine = posOnLine - 1;

			Font sciTreeFont = new Font( "tahoma", 9, FontStyle.Regular );
			SizeF nameSize = Graphics.FromImage( new Bitmap( 1, 1 ) ).MeasureString( "  " + this.name, sciTreeFont );
			
			this.rect = new Rectangle(
				40 * this.posOnLine - (int)nameSize.Width / 2,
				/*296 - */ 30 * this.line,
				(int)nameSize.Width,
				(int)nameSize.Height + 2
				);
		}
	}
}
