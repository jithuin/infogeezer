using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for enums.
	/// </summary>
	public class enums
	{
		
		public enum endTurnState : byte
		{
			wait,
			iaIni, // tehnos, cities
			iaUnit,
			playerIni, // units
			playerCities,
			final
		}
		
		public enum cfgFile : byte
		{
			playerName,
			lastGame
		}
		
		public enum ecoRelation : byte
		{
			none,
			basic,
			freeTrade,
			joinedEconomy,
			Exploited,
			Exploiter
		}
		
		public enum relationQuality : byte
		{
			cold,
			respectful,
			fear,
			angry,
		}
		
	/*	public enum Stat.GovernementType : byte
		{
			despotism,
			anarchy,
			monarchy,
			feodalism,
			communism,
			democraty
		}*/

		public enum playerState : byte
		{
			ok,
			dead
		}
		
		public enum cityState : byte
		{
			ok,
			dead,
			evacuating
		}
		
		public enum speciality : byte
		{
			none,
			builder,
			bombard, 
			bomber,
			fighter,
			nukeBomb,
			nukeMissile,
			nukeIC,
			cruiseMissile,
			carrier,
			aaGun
		}

		
		public enum speciaResources : byte
		{
			none,
			horses,
			elephant,
			camel
		}

		
		public enum cityBuildType : byte
		{
			none,
			unit,
			building,
			wonder,
			wealth
		}

		
		public enum militaryImprovement : byte
		{
			none,
			fort,
			airport,
			mineField
		}

		public enum selectionState
		{
			normal,
			bombard,
			planeRebase,
			planeRecon,
			goTo,
			launchMissile,
			nukeExplosion
			/*launchICBM,
			launchNukeBomb,
			launchNukeMissile,
			launchCruseMissile,*/
		}

	/*	public enum Stat.Governements
		{
			despotism,
			anarchy,
			democratyDirect,
			feodality,
			monarchy,
			totalitarism,
			democratyRep,
		//	democratyTot,
		//	oligarchy,
			dictatorship,
			integrism,
			tot
		}*/

		public enum infoType
		{
			units,
			civ,
			terrain,
			techno,
			gov,
			eco,
			misc,
			caseImp,
			resources,
			specialResources,
			tutorial,
			buildings,
			smallWonders,
			wonders
		}

		public enum spyType
		{
			people,
			gov,
			military,
			science
		}

		public enum playerStrategy
		{
			science,
			military,
			culture
		}

		public enum warStrategy 
		{ 
			superiorScience, 
			alliance 
		} 

		public enum ressource 
		{ 
			iron, 
			saltpeter,
			oil,
			aluminum,
			uranium,
			coal,
			tot
		} 

		public enum luxuries 
		{ 
			gold, 
			incense,
			fur,
			ivory
		} 

	/*	public enum constructionType 
		{ 
			nothing, 
			unit,
			building,
			wealth,
			wonder
		} */
		
		/*public enum economyType
		{
			capitalism,
			communism,
			socialSlavery,
			racialSlavery,
			naturalOrder
		}*/
		
		public enum playerMemory
		{
			empty,
			changeOfCapital, 
			revolution,
			exchangeWith,
			ennemyUnitOnTerritory,
			war,
			peace,

			rebaseParachutist,
			rebaseBomber,
			noWhereToRebaseBomber,
			noWhereToRebaseFighter,
			rebaseFighter,
			destDropMilitary,
			destDropSettler,

			buildingWonder,

			loseCity,
			death,
			beenBombed,
			beenNuked,
			launchedICBM,

			attemptedNego, // ind[ 0 ] == other

			cannotFindCitySiteOnCont,
			cannotFindCitySiteWW,
			foundCitySiteWW,
			sentSettlerToBuildCityAt, // X, Y, ind
			sentSettlersToImproveAt, // X, Y, nbr

			settlerSentToJoinCityOnConts, // cont1, cont 2, cont 3
		}

		public enum exchangeTypes
		{
			goldAmount,
			revenuBrutPerc,
			revenuRealPerc,
			ressources,
			warOn,
			embargoOn,
			ceaseFireWith,
			allianceWith,
			unitMovementInfo,
			shareTechnoDiscoveries
		}
		
		public enum mainMemory
		{
			playerDeath, // killer
		}

		public enum tutorial
		{
			justStarted,
			firstCity,
			secondSettler,
			firstContact,
			firstWar,
			firstTransport,
		}

	/*	public enum terrainType 
		{ 
			water = 0, 
			land = 1, 
			flight = 2, 
			motionless = 3 
		} */

		public enum terrainType : byte
		{
			sea,
			plain,
			forest,
			hill, 
			mountain, 
			coast, 
			prairie, 
			swamp,
			jungle, 
			tundra, 
			glacier, 
			desert, 
			totp1
		}

		public enum economies 
		{ 
			natural, 
			capitalism, 
			communism, 
			slaverySocial,
			slaveryRacial,
		//	globalDeveloppment,
			tot
		} 
		
		public enum governements : byte
		{
			despotism,
			anarchy,
			democratyDirect,
			feodality,
			monarchy,
			totalitarism,
			democratyRep,
			/*		democratyTot,
					oligarchy,		*/
			dictatorship,
			integrism,
			tot
		}
	} 
} 