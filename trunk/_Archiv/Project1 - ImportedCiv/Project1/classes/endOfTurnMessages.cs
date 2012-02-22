using System;

namespace xycv_ppc.classes
{
	/// <summary>
	/// Summary description for endOfTurnMessages.
	/// </summary>
	public class endOfTurnMessages
	{
		enum types : byte
		{
			aiMeetAi,
			aiDeclaredWarToAi,
			aiMadePeaceWithAi,
			youMeetAi,
		}

		struct structure
		{
			types type;
		}
	}
}
