using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for civHappiness.
	/// </summary>
	public class nationHappiness
	{
		public nationHappiness()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int recurring, thisTurn;

		public int happiness
		{
			get
			{
				return recurring + thisTurn;
			}
			set
			{
				recurring = value;
			}
		}

		public void endOfTurn()
		{
		}

		public void beginOfTurn()
		{
		}

		public void doEvent()
		{
		}

		public enum events
		{
		}
	}
}
