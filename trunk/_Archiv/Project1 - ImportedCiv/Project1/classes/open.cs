using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for open.
	/// </summary>
	public class open
	{
		public static void cityFrm( CityList city )
		{
			cityFrm( city.player.player, city.city, null );
		}

		public static void cityFrm( byte player, int city, System.Windows.Forms.Control parent )
		{
			wC.show = true;
			FrmCity frm2 = new FrmCity( player, city);

			if ( parent != null )
			{
				string title = parent.Text;
				parent.Text = "";

				wC.show = false;
				frm2.ShowDialog();
			
				parent.Text = title;
			}
			else
			{
				wC.show = false;
				frm2.ShowDialog();
			}

		}
	}
}
