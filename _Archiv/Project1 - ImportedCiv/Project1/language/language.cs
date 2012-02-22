using System;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for languageReader.
	/// </summary>
	public class language
	{
		public static bool moreThanOneLanguageFileDetected;
		private static string curLanguage;

#region populate
		public static void populate( bool forceChooseLanguage ) 
		{ 
			moreThanOneLanguageFileDetected = true;
			string languageFilePath = "";

			if ( 
				!forceChooseLanguage && 
				Form1.options.languageFile.Length > 0 
				)
			{
				languageFilePath = Form1.options.languageFile;

				try
				{
					System.IO.FileStream file1 = new System.IO.FileStream( languageFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read ); 
				}
				catch //( System.Exception e )
				{
					forceChooseLanguage = true;
				}
			}
			
			if ( 
				forceChooseLanguage || 
				languageFilePath == "" 
				)
			{
				string[] strList1 = System.IO.Directory.GetFiles( Form1.appPath + "\\languages\\" );

				int pos = 0;
				int[] refer = new int[ strList1.Length ];
				int english = 0;

				for ( int i = 0; i < strList1.Length; i ++ )
					if ( System.IO.Path.GetExtension( strList1[ i ] ) == ".lng" )
					{
						if ( System.IO.Path.GetFileNameWithoutExtension( strList1[ i ] ).ToLower() == "english" )
							english = pos;
						
						refer[ pos ] = i;
						pos ++;
					}

				if ( pos == 0 )
				{
					System.Windows.Forms.MessageBox.Show( "Sorry, no language files detected. Make sure you have the .lng files in the same directory than pocketHumanity.exe!", "Error" );
					//	Application.End();
				}
				else if ( pos == 1 )
				{
					languageFilePath = strList1[ refer[ 0 ] ];
					moreThanOneLanguageFileDetected = false;
				}
				else
				{
					moreThanOneLanguageFileDetected = true;
					string[] strList3 = new string[ pos ];
					for ( int i = 0; i < pos; i++ )
					{
						strList3[ i ] = System.IO.Path.GetFileNameWithoutExtension( strList1[ refer[ i ] ] );
					}

					userChoice ui = new userChoice( 
						"Language file", 
						"Please choose a language",
						strList3,
						english,
						"Ok",
						"Default"
						);
					platformSpec.cursor.showWaitCursor = false;
					ui.ShowDialog();
					int res = ui.result;

					if ( res == -1 )
						res = english;
				
					languageFilePath = strList1[ refer[ res ] ];
				}
			}
		
			System.IO.FileStream file = new System.IO.FileStream( languageFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read ); 
			System.IO.StreamReader sr = new System.IO.StreamReader( file, true ); 


	//		structures.cfgFile cfgFile = cfgOut.getValues; // = 
			//		cfgFile.languageFile = languageFilePath;
			Form1.options.languageFile = languageFilePath; 

	//		cfgOut.getValues = cfgFile;
	//		cfgOut.writeCfgFile();
			Form1.options.save();

			string fullList = sr.ReadToEnd(); 
			string[] temp = fullList.Split( "\n".ToCharArray() ); 

			int tot = 0; 
			//System.Collections.BitArray isOk = new System.Collections.BitArray( temp.Length, false );
			bool[] isOk = new bool[ temp.Length ];
			for ( int i = 0; i < temp.Length; i ++ ) 
			{ 
				temp[ i ] = temp[ i ].TrimEnd( "\r".ToCharArray() ); 

				if ( 
					temp[ i ] != "" && 
					!temp[ i ].StartsWith( "//" ) 
					) 
				{ 
					isOk[ i ] = true; 
					tot ++; 
				} 
			}

			list = new string[ tot ];
			int pos1 = 0;
			for ( int i = 0; i < temp.Length; i ++ )
				if ( isOk[ i ] )
				{
					list[ pos1 ] = temp[ i ];
					pos1 ++;
				}

			curLanguage = languageFilePath;

			sr.Close();
			file.Close();
		} 
#endregion

#region getAString

		public static string getAString(order ind)
		{
			if ( list == null )
				populate( false );

			string s = null;

			try 
			{
				s = list[ (int)ind ];
			}
			catch
			{
				s = "Error getting string: " + ind.ToString();
			}

			return s;
		}

#endregion

		public static string current
		{
			get
			{
				if ( list == null )
					populate( false );

				return curLanguage;
			}
		}

		private static string[] list;
		
		public enum order : int
		{

#region opening
			openingContinue,
			openingNewGame,
			openingLoadGame,
			openingLoadMap,
			openingExit,
			
#endregion

#region new game
			nGYourName,
			nGYourNation,
			nGDifficulty,
			nGAis,
			nGBack,
			nGNext,
			nGMapSize,
			nGContinentSize,
			nGPercOfWater,
			nGWorldAge,
			nGHaveANiceBeginningSite,
			nGTutorialMode,
			nGGenerateMap,
			nGCustomNation,
#endregion

#region file
			file,
			fileSave,
			fileSaveAs,
			fileLoad,
			fileOption,
			fileMainMenu,
			
		#endregion

			unit,
			build,
			action,

			civ,
			science,
			budjet,
			relation,
			info,
			satellites,

			endTurn,

			//?
			questionMark,
			about,
			encyclopedia,
			labels,
			labelCreate,
			labelRename,
			labelDelete,

			infosOn,
			infosOnUnit,

			next,
			previous,

			ok,
			cancel,
			yes,
			no,
			BC,
			AD,
			gold,
			playerDied,
			youDied,
			unitsLeft,
			spy,

			// new
			peace,
			ceaseFire,
			war,
			alliance,
			Protected,
			Protector,

			yourAlly,
			yourEnnemy,

			TheirAlly,
			TheirEnnemy,

			turnAgo,
			turnsAgo,

		/*	Research,
			Researched,
			Researching,*/

			Passive,
			Agressive,

			goldPerTurn,
			turnLeft,
			turnsLeft,

			budgetReserve,
			budgetMilitary,
			budgetScience,
			budgetCulture,
			budgetIntelligence,
			budgetBuildingUpkeep,

			Politic,
			People,
			Science,
			Military,

			Negotiate,
			DeclareWar,

			declareWarConfirmation,

			//nego 1
			Add,
			Remove,
			Refuse,
			Discuss,
			Propose,

			SpecifyTheAmount,

			noAiWantToContinue,

			Tiny,
			Small,
			MediumSize,
			Large,
			Huge,

			Young,
			MediumAge,
			Old,
			
			ssBombard,
			ssPlaneRebase,
			ssPlaneRecon,
			ssGoTo,
			ssLaunchMissile,

			turnBeforeGrowth,
			turnsBeforeGrowth,

			// relations form
			counterIntelligence,
			Details,
			contactNation,
			rfGeneral,
			rfSpies,
			rfCouncil,

		
#region user input
			uiNewCity,
			uiRenameCity,
			uiPleaseEntrerCityName,
			uiJustBuiltA,
			uiBuildSettlerKillCity,
			uiAccept,
			uiGoToCity,
			uiOpenTechnoTree,
			uiNewConstrution,
			uiNewTechnology,
			uiYouJustDiscovered,
			uiYouJustDiscoveredEverything,
			uiNameLabel,
			uiPleaseNameLabel,
			uiLabelHidden,
#endregion

			// transport
			uiUnloadingUnit,
			uiPleaseChooseUnitToUnload,
			uiUnload,
			uiLoadingUnitOnShip,
			uiPleaseChooseAShip,
			uiEmbark,

			// negociation
			negoTreaty,
			negoGive,
			negoDemand,
			negoThreat,

			negoTreatyPolitic,
			negoTreatyEconomic,
			negoTreatyEmbargoOn,
			negoTreatyWarOn,
			negoTreatyBreakAlliance,
			negoTreatyVote,
			
			negoGDCity,
			negoGDTechno,
			negoGDContact,
			negoGDMoney,
			negoGDMoneyPerTurn,
			negoGDMap,
			negoGDRegion,
			negoGDResource,
		/*	negoSubPolitic,
			negoSubEconomic,*/

			ancientRegionName,
		
			negoGivePoliticTreaty,
			negoGiveEconomicTreaty,

		//	negoGiveThreat,
			negoGiveThreatWar,
			negoGiveThreatEmbargo,
			negoGiveThreatBreakAlliance,

			negoGiveBreakAllianceWith,
			negoGiveEmbargoOn,
			negoGiveWarOn,
			negoGiveCity,
			negoGiveRegion,
			negoGiveTechno,
			negoGiveContactWith,
			negoGiveMoney,
			negoGivePercBrut,
			negoGiveMoneyPerTurn,
			negoGiveResource,

		//	negoGiveMap,
			negoGiveWorldMap,
			negoGiveTerritoryMap,

			negoGiveVotes,

			uiSpecifyTheAmountOnce,
			uiSpecifyTheAmountPerTurn,

			panNegociatingWith,
			proposingRefusesPropose,
			proposingRefusesAll,
			proposingAccept,
			tradeRefusesTo,
			tradeDoYouWantTo,

		/*	negoGiveBreakAlliance,
			negoGiveCity,
			negoGiveContact,
			negoGiveDeclareWarTo,
			negoGiveEmbargo,
			negoGiveMaps,
			negoGiveMoney,
			negoGiveTechno,
			negoGiveTerritoryMap,
			negoGiveThreats,
			negoGiveTreaty,
			negoGiveWorldMap,*/

			miJoinCity,
			miSellFor,
			miUpgradeUnit,

			// science tree
			stResearching,
			stUnavailable,
			stFound,
			stUnknown,
			stResearched,
			stResearch,

			// custom nation
			cnNationName,
			cnImportCityFrom,
			cnColor,
			cnRed,
			cnGreen,
			cnBlue,
			cnDescription,

			saveYourGameTitle,
			saveYourGameQ,
			errorTitle,
			errorUnrecongnisedSaveVersion,

			cityBuild,
			cityBuy,
			cityMore,
			cityRename,
			cityEvacuate,
			cityCancelEvacuation,
			citySetAsCapital,
			cityStateEvacuating,

			slaveryTitle,
			slaveryAquiredFromCity,

			// encyclopedia
			encyclopediaUnits,
			encyclopediaCivilizations,
			encyclopediaTerrains,
			encyclopediaTechnologies,
			encyclopediaGovernements,
			encyclopediaEconomies,
			encyclopediaResources,
			encyclopediaTutorial,
			encyclopediaBuildings,
			encyclopediaSmallWonders,
			encyclopediaWonders,
			encyclopediaMisc,

			encyclopediaNeededTechno,
			encyclopediaFoodAdv,
			encyclopediaProdAdv,
			encyclopediaTradeAdv,

			encyclopediaGovOppresive,
			encyclopediaGovNonOppresive,
			encyclopediaGovSurrenderOnConquer,
			encyclopediaGovResistInvaders,
			encyclopediaGovCanBeChosenByPlayers,
			encyclopediaGovUnavailableToPlayers,

			//	encyclopediaGovPopGrowthAdv,
			//	encyclopediaGovSupport,
			
			encyclopediaEcoCommunism,
			encyclopediaEcoCapitalism,
			encyclopediaEcoSupportSlavery,
			encyclopediaEcoDoesNotSupportSlavery,
			
			encyclopediaTechnoCanBeResearched,
			encyclopediaTechnoCanNotBeResearched,
			encyclopediaTechnoNeededTechnos,
			encyclopediaTechnoAllowTechnos,
			encyclopediaTechnoDoesNotAllowAnyTechnos,

			encyclopediaTerrainMoveDifficulty,
			encyclopediaTerrainDefenseBonus,
			encyclopediaTerrainFood,
			encyclopediaTerrainProduction,
			encyclopediaTerrainTrade,
			encyclopediaTerrainIrrigationFoodBonus,
			encyclopediaTerrainMineProductionBonus,
			encyclopediaTerrainRoadTradeBonus,
			encyclopediaTerrainRailroadTradeBonus,
			
			encyclopediaResourceNeededTechnos,
			encyclopediaResourceFoodBonus,
			encyclopediaResourceProductionBonus,
			encyclopediaResourceTradeBonus,

			encyclopediaUnitCost,
			encyclopediaUnitAttack,
			encyclopediaUnitDefense,
			encyclopediaUnitMove,
			encyclopediaUnitEntrenchable,
			encyclopediaUnitHighSeaSync,
			encyclopediaUnitHighSeaSyncResistant,
			encyclopediaUnitSpeciality,
			encyclopediaUnitTransportCapacity,
			encyclopediaUnitStealth,
			encyclopediaUnitSight,
			encyclopediaUnitRange,
			encyclopediaUnitRanderedObseleteBy,

			encyclopediaCivilizationLeaderNames,

			// tutorial info
			tutorialJustStartedTitle, 
			tutorialJustStartedText, 
			tutorialFirstCityTitle, 
			tutorialFirstCityText, 
			tutorialFirstTechnologiesTitle, 
			tutorialFirstTechnologiesText, 
			tutorialSecondSettlerTitle, 
			tutorialSecondSettlerText, 

			filesLoadingGame,
			filesSavingGame,
			filesLoadingScenario,
			filesSavingScenario,
			filesNewFile,
			filesNewFileDialogTitle,
			filesNewFileDialogText,
			empty,
			delete,
			filesDialogDeleteTitle,
			filesDialogDeleteText,

			mainYearBC,
			mainYearAD,

			editorActivateMi,
			editorActivateDialogTitle,
			editorActivateDialogText,
			editorWorld, 
			editorCase,
			editorUnits,
			editorCity,
			editorPlayers,
			editorRelations,
			editorDeals,
			editorSaveAs,
			editorLoad,
			editorSaveScenarioAs,

			killYouKilled,
			killHasBeenKilledBy,
			killYouHaveBeenKilled,

			endGameYouLost
			

		/*	
			tutorialFirstResourceTitle, 
			tutorialFirstResourceText, 
			tutorialFirstContactTitle, 
			tutorialFirstContactText, 
			tutorialFirstWarTitle, 
			tutorialFirstWarText, 
			tutorialFirstTransportTitle,
			tutorialFirstTransportText	
		*/

		}
	}
}
