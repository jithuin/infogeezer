using System;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for tutorial.
	/// </summary>
	public class Tutorial
	{
		public static bool mode;
		public static bool[] alreadySeen;

		public static void show( Form parent, order ind)
		{
			if ( 
				mode &&
				!alreadySeen[ (int)ind ] 
				)
			{
				string title = parent.Text; // S:\pH\Pocket Humanity\forms\
				parent.Text = "";

				frmInfo fi = new frmInfo( enums.infoType.tutorial, (int)ind );
				fi.ShowDialog();
				alreadySeen[ (int)ind ] = true;

				parent.Text = title;
			}
		}

		public static structure getTextFromInd( int ind )
		{
			return new structure(
				language.getAString( (language.order)( (int)language.order.tutorialJustStartedTitle + ind * 2 ) ), 
				language.getAString( (language.order)( (int)language.order.tutorialJustStartedTitle + ind * 2 + 1 ) )
				);
		}

		public static void init( bool tm )
		{
			mode = tm;

			if ( tm )
			{
				alreadySeen = new bool[ (byte)order.tot ];
			}
		}

		public enum order 
		{ 
			justStarted, 
			firstCity, 
			discoveringTechnologies,
			secondSettler, 
		/*	firstResources,
			firstContact, // to do
			firstWar, // to do
			firstTransport, // to do
			/*
			resources, // specialResources
			buildWindow,
			cityPopulation,
			preferenceWindow,
			
			*/
			tot 
		} 

		public struct structure 
		{ 
			public string title, text; 

			public structure( string title, string text )
			{
				this.title = title;
				this.text = text;
			}
		} 
	}
}
