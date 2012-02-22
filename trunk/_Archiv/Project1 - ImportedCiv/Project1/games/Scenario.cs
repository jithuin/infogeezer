using System;
using System.IO;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for Scenario.
	/// </summary>
	public class Scenario : Game
	{
		public string name, description;
		public GoalType goalType;
		public int goalInd;

		// goal, type and ind

		public Scenario()
		{
		}
		
		public enum GoalType
		{
			onlySurvivor,
			globalPeace,
			surviving // time
		}

		public bool loadScenario()
		{
			return true;
		}

	/*	public ScenarioInfos getInfos( string path )
		{
			bool success;
			BinaryReader reader = null;
			ScenarioInfos infos;

			try
			{
				reader = new BinaryReader( path );

				int version = reader.ReadInt32();
				reader.ReadInt32();

			/*	if ( version == 887 )
				{*

				infos.name = reader.ReadString();
				infos.description = reader.ReadString();
				infos.goalType = reader.ReadByte();
				infos.goalInd = reader.ReadInt32();


				success = true;
			}
			catch ( Exception e )
			{
				success = false;
			}
			finally
			{
				reader.Close();
			}

			if ( success )
				return infos;
			else
				return null;
		}*/

	/*	public struct ScenarioInfos
		{
			public string name, description;
			public GoalType goalType;
			public int goalInd;
		}*/
	}
}
