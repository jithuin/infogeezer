using System;
using System.Drawing;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for struct.
	/// </summary>
	public class structures
	{
		public struct negoGive
		{
			public long moneyLeft;	//temp
			public long moneyGive;
			public bool worldMap;
			public bool territoryMap;
			public bool[] technoGive;
			public bool[] cityGive;
			public bool[] regionGive;
			public bool[] contactGive;
			public bool allContacts;
		}
		
		public struct nego
		{
			public negoGive[] playerGive;
			public bool[] treaties; 
			public bool[] threats; 
			public bool[] declareWarTo; 
			public bool[] embargo; 
			public bool[] breakAlliance; 
			public bool[] ceaseFire; 
		}

		public struct GainVsLoss
		{
			public Int64 gain;
			public Int64 loss;
		}
		
		public struct cfgFile 
		{
			public string playerName;
			public string savePath;

			public bool showGrid;
			public bool dPad;
			public bool batteryStatus;
			public string languageFile;
		}

		public struct construction
		{
			public byte type;
			public int unit;
			public int cost;
			public int curConstPoints;
		}

		public struct caseImprovement
		{
			public System.Drawing.Point pos;
			/// <summary>
			/// 0 = none, 1 = civic, 2 = military
			/// </summary>
			public byte type;
			public byte owner;
			public byte construction;
			public int constructionPntLeft;
			public UnitList[] units;
		}

		public struct caseImprovementStats
		{
			public string name;
			public byte cost;
			public System.Drawing.Bitmap bmp;
		}

		public struct drawRegion
		{
			public int xMin;
			public int yMin;
			public int xMax;
			public int yMax;
		}

	/*	public struct governements
		{
			public string name;
			public string description;
			public bool oppresive;
			public bool surrenderOnConquer;
			public bool canBeChoosedByPlayer;

			public byte productionPerc;
			public byte foodPerc;
			public byte tradePerc;
			
			public byte popGrowthPerc;
			public byte techno;
		}*/

	/*	public struct economy
		{
			public byte techno;
			public string name;
			public string desc;
			public bool supportSlavery;
			public bool communism;
			public byte prod;
			public byte food;
			public byte trade;
		}*/

		public struct technoList
		{
			public bool researched;
			public int pntDiscovered;
		}

	/*	public struct spyList
		{
		/*	public int gov;
			public int military;
			public int civic;
			public int science;

			public byte govEfficiency;
			public byte militaryEfficiency;
			public byte civicEfficiency;
			public byte scienceEfficiency;
			public sSpies[] spies;
		}*/

		public struct preferences
		{
			public sbyte laborProd;
			public sbyte laborFood;
			public sbyte laborTrade;

			public sbyte reserve;
			public sbyte military;
			public sbyte science;
			public sbyte intelligence;
			public sbyte buildings;
			public sbyte culture;
			public sbyte space;
			public sbyte exchanges;
		}

		public struct sSpies
		{
			public byte efficiency;
			public int nbr;
			public byte state;
		}

		public struct pntWithDist
		{
			public int X;
			public int Y;
			public int dist;
		}

		public struct lastSeen
		{
			public int city;
			public byte cityPop;

			public byte civicImp, militaryImp, road;

			public byte territory;
			public int turn;
		}
		
		public struct intByte
		{
			public byte type;
			public int info;
		}
		
	/*	public struct Statistics.resources
		{
			public byte type; 
			public string name,
				description; 
			public byte terrain; 
			public byte techno; 
			public byte[] terrainTypes; 

			public byte foodBonus,
				prodBonus,
				tradeBonus; 

			public Bitmap bmp, bmpUnseen; 
		}*/
		
		public struct exchange 
		{ 
			public byte type; 
			public int info; 
			public int quantity; 
			public int endTurn; 
		} 
		
	/*	public struct economyType
		{
			public string name;
			public string description;
			public int prodMod;
			public int foodMod;
			public int tradeMod;
		}*/
		
		public struct Pnt
		{
			public int X;
			public int Y;
		}
		
		public struct memory 
		{ 
			public byte type; 
			public int[] ind; 
			public int turn; 
		} 
		
		public struct constructionList
		{
			public byte type;
			public int ind;

			/*	public Bitmap bmp; 
				public Bitmap bmpUnseen; */
		}

		public struct treaties
		{
			bool politics;
			bool declareWarTo;
			bool embargo;
			bool breakAlliance;
			bool ceaseFire;
			bool threat;
		}
		
		public struct singleSattelite
		{
			public int pos;
			public double xStart;
			public byte[] path;
			public bool specialPath;
			public byte[] nextPath;
			public byte lastY;
		}
		
		public struct roadList
		{
			public bool road;
			public bool rail;
		}
		
		public struct sStack
		{
			public byte owner;
			public int unit;
		}

/*		public struct sGrid
		{
			public int X, Y;

			public byte type;
			public int city;
			public byte selectedLevel;

			public byte militaryImprovement;
			public byte civicImprovement;
			public int stackPos;
			public byte laborOwner;
			public int laborCity;
			public byte continent;
			public byte territory;
			public sStack[] stack;
			public byte resources;

			public bool polluted;
			public bool isNextToWater;

			public bool river;
			public bool[] riversDir;

			public byte roadLevel;
			public structures.roadList[] roadDir;

			public bool water
			{
				get
				{
					return type == (byte)enums.terrainType.coast ||
						type == (byte)enums.terrainType.sea;
				}
			}

			public bool canBuildCity
			{
				get
				{
					return true;
				}
			}

			/*	public bool canMoveLandUnit
				{
					get
					{
						return true;
					}
				}	*

			public bool canMoveSeaUnit
			{
				get
				{
					return true;
				}
			}	
		}*/
			
		public struct sSelected
		{
			public int X;
			public int Y;
			public byte state;
			public byte name;
			public byte owner;
			public int unit;
			public byte unitInTransport;
		}
		
		public struct sForeignRelation
		{
			public bool madeContact;
			public byte politic;
			public byte economic;
			public int quality;
			public bool embargo;
			public structures.sSpies[] spies;
		}
	}
}