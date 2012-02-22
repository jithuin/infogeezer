using System;
using System.Drawing;
using xycv_ppc.Stat;

namespace xycv_ppc//.Stat
{
	/// <summary>
	/// Summary description for Statistics.
	/// </summary>
	public class Statistics
	{
		public static bool variableInitialized;
		public static int normalCivilizationNumber;

		#region constructions

		public static Stat.Unit[] units;
		public static Stat.Building[] buildings;
		public static Stat.Wonder[] wonders;
		public static Stat.SmallWonder[] smallWonders;

		#endregion

		public static Stat.Technology[] technologies;
		public static Stat.Civilization[] civilizations;
		public static Stat.Economy[] economies;
		public static Stat.Governement[] governements;
		public static Stat.Terrain[] terrains;
		public static Stat.Resource[] resources;

	/*	public Statistics()
		{
		}*/

		public static void initCivilizations()
		{
			#region civilizations
			civilizations = new Civilization[]
			{
				new Civilization(	0,		"Egypt",			new string[] { "Cleopatra" },			"Egyptians",	Color.Yellow,		null			),
				new Civilization(	1,		"Greece",			new string[] { "Alexander" },			"Greeks",		Color.LightGreen,	null			),
				new Civilization(	2,		"Carthage",			new string[] { "Hannibal" },			"Greeks",		Color.Gold,			"Maya"			),
				new Civilization(	3,		"Persia",			new string[] { "Bob Moon" },			"Greeks",		Color.Turquoise,	"Maya"			),
				new Civilization(	4,		"France",			new string[] { "Louis" },				"Greeks",		Color.Cyan,			null			),
				new Civilization(	5,		"Great Britain",	new string[] { "Henry" },				"Greeks",		Color.Orange,		"England"		),
				new Civilization(	6,		"Rome",				new string[] { "Julius Ceasar" },		"Greeks",		Color.Red,			null			),
				new Civilization(	7,		"Babylon",			new string[] { "Nabuchadnezzar" },		"Greeks",		Color.LimeGreen,	"Maya"			),
				new Civilization(	8,		"Germany",			new string[] { "Otto von Bismack" },	"Greeks",		Color.Linen,		null			),
				new Civilization(	9,		"China",			new string[] { "Mao Zedong" },			"Greeks",		Color.MintCream,	null			),
				new Civilization(	10,		"Aztec",			new string[] { "Moctezuma" },			"Greeks",		Color.Tomato,		null			),
				new Civilization(	11,		"Spain",			new string[] { "Philip V" },			"Greeks",		Color.OrangeRed,	null			),
				new Civilization(	12,		"Maya",				new string[] { "Kukulcan" },			"Greeks",		Color.Salmon,		null			),
				new Civilization(	13,		"Mongol",			new string[] { "Genghis Khan" },		"Greeks",		Color.BurlyWood,	null			),
				new Civilization(	14,		"Viking",			new string[] { "Eric the Red" },		"Greeks",		Color.BlueViolet,	"Maya"			),
				new Civilization(	15,		"Russia",			new string[] { "Joseph Stalin" },		"Greeks",		Color.Sienna,		null			),
				new Civilization(	16,		"Austria",			new string[] { "Francis" },				"Greeks",		Color.MediumOrchid,	"Maya"			),
				new Civilization(	17,		"Ottoman",			new string[] { "Osman" },				"Greeks",		Color.OldLace,		"Maya"			),
				new Civilization(	18,		"Inca",				new string[] { "Atahualpa" },			"Greeks",		Color.Plum,			"Maya"			),
				new Civilization(	19,		"United States",	new string[] { "Abraham Lincoln" },		"Greeks",		Color.Tomato,		"Usa"			),
				new Civilization(	20,		"Portugal",			new string[] { "Manuel I" },			"Greeks",		Color.IndianRed,	null			)
			};
			#endregion

			normalCivilizationNumber = civilizations.Length;
		}

		public static void initAllBut()
		{
			#region units
			showProgress.lblInfo = "Loading units...";
			units = new Unit[ (byte)Form1.unitType.totp1 ];

			//															type								Name				description									a	d	m	bui								ter		transport	c$		dis	( techno )								obsolete ( unit )				sight	range	entrench	sync		stealth		res
			units[ (byte)Form1.unitType.colon ] =			new Unit( (byte)Form1.unitType.colon,			"Settler",			"colonize",									0,	1,	1,	enums.speciality.builder,		1,		0,			50,		0,											0,								1,		0,		true,		false,		false,		0		);
			units[ (byte)Form1.unitType.warrior ] = 		new Unit( (byte)Form1.unitType.warrior,			"Warrior",			"Tribal warrior",							2,	1,	1,	0,								1,		0,			40,		0,											(byte)Form1.unitType.legion,	1,		0,		true,		false,		false,		0		);
			units[ (byte)Form1.unitType.knight ] =			new Unit( (byte)Form1.unitType.knight,			"Knight",			"Make war",									3,	2,	3,	0,								1,		0,			120,	(byte)Form1.technoList.chivalery,			0,								1,		0,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.phalax ] = 			new Unit( (byte)Form1.unitType.phalax,			"Phalanx",			"Make war",									1,	2,	1,	0,								1,		0,			60,		(byte)Form1.technoList.bronze,				0,								1,		0,		true,		false,		false,		0		);
			units[ (byte)Form1.unitType.camelWarrior ] = 	new Unit( (byte)Form1.unitType.camelWarrior,	"Camel Warrior",	"Fight nicely",								2,	2,	2,	0,								1,		0,			120,	(byte)Form1.technoList.camelBreed,			0,								1,		0,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.cavalier ] = 		new Unit( (byte)Form1.unitType.cavalier,		"Cavalier",			"Fight",									2,	1,	3,	0,								1,		0,			120,	(byte)Form1.technoList.horseBreed,			(byte)Form1.unitType.knight,	1,		0,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.legion ] =			new Unit( (byte)Form1.unitType.legion,			"Legion",			"Fight",									3,	2,	1,	0,								1,		0,			100,	(byte)Form1.technoList.iron,				0,								1,		0,		true,		false,		false,		0		);
			units[ (byte)Form1.unitType.elephant ] = 		new Unit( (byte)Form1.unitType.elephant,		"Elephant",			"Fight",									3,	2,	2,	0,								1,		0,			150,	(byte)Form1.technoList.elephantBreed,		0,								1,		0,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.frigate ] = 		new Unit( (byte)Form1.unitType.frigate,			"Frigate",			"Transport and combat ship",				3,	2,	4,	0,								0,		4,			140,	(byte)Form1.technoList.coastalNavigation,	0,								1,		0,		false,		false,		true,		0		);
			units[ (byte)Form1.unitType.catapult ] = 		new Unit( (byte)Form1.unitType.catapult,		"Catapult",			"Distance attack",							5,	0,	1,	enums.speciality.bombard,		1,		0,			100,	(byte)Form1.technoList.math,				0,								1,		1,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.fighter ] = 		new Unit( (byte)Form1.unitType.fighter,			"Fighter",			"Short range fighter",						4,	4,	1,	enums.speciality.fighter,		2,		0,			180,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		(byte)enums.ressource.aluminum	);
			units[ (byte)Form1.unitType.nukeTactical ] = 	new Unit( (byte)Form1.unitType.nukeTactical,	"Tactical nuke",	"Short range nuclear missile",				4,	4,	3,	enums.speciality.nukeMissile,	1,		0,			300,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
				
			units[ (byte)Form1.unitType.nukeICBM ] = 		new Unit( (byte)Form1.unitType.nukeICBM,		"ICBM",				"Long range nuclear missile",				4,	4,	1,	enums.speciality.nukeIC,		3,		0,			500,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.nukeBomb ] = 		new Unit( (byte)Form1.unitType.nukeBomb,		"Nuclear bomb",		"Bomber carring a nuclear bomb",			4,	4,	1,	enums.speciality.nukeBomb,		2,		0,			250,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.bomber ] = 			new Unit( (byte)Form1.unitType.bomber,			"Bomber",			"First bomber",								4,	4,	2,	enums.speciality.bomber,		2,		0,			200,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.cruiseMissile ] = 	new Unit( (byte)Form1.unitType.cruiseMissile,	"Cruise missile",	"Short range conventional missile",			10,	4,	3,	enums.speciality.cruiseMissile,	1,		0,			100,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.cargoPlane ] = 		new Unit( (byte)Form1.unitType.cargoPlane,		"Cargo plane",		"Aerial transport",							0,	4,	2,	0,								2,		2,			250,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);
			units[ (byte)Form1.unitType.carrier ] = 		new Unit( (byte)Form1.unitType.carrier,			"Carrier",			"Plane transport",							0,	6,	8,	enums.speciality.carrier,		0,		5,			250,	(byte)Form1.technoList.flight,				0,								2,		6,		false,		false,		false,		0		);

			showProgress.progressBar = 3;
			#endregion
			
			#region terrains
			showProgress.lblInfo = "Loading terrains...";
			terrains = new Terrain[ (byte)enums.terrainType.totp1 ];

				//														type									Name        ew  P   F   T   mv  mB  iB  ro  rTB	rFB	color				build	def
			terrains[ (byte)enums.terrainType.plain ] =		new Terrain( (byte)enums.terrainType.plain,			"Plain",	1,  1,  2,  1,  1,  1,  2,  2,	1,  1,	Color.Green,		1,	100	);
			terrains[ (byte)enums.terrainType.prairie ] =	new Terrain( (byte)enums.terrainType.prairie,		"Prairie",	1,  1,  1,  1,  1,  1,  2,  2,	1,  2,	Color.GreenYellow,	1,	100	);
			terrains[ (byte)enums.terrainType.coast ] =		new Terrain( (byte)enums.terrainType.coast,			"Coast",	0,  0,  1,  1,  1,  0,  0,  0,	0,  0,	Color.LightBlue,	2,	75	);
			terrains[ (byte)enums.terrainType.sea ] =		new Terrain( (byte)enums.terrainType.sea,			"Sea",		0,  0,  1,  1,  1,  0,  0,  0,	0,  0,	Color.Blue,			3,	100	);
			terrains[ (byte)enums.terrainType.forest ] =	new Terrain( (byte)enums.terrainType.forest,		"Forest",	1,  2,  0,  1,  2,  0,  0,  2,	1,  1,	Color.DarkGreen,	2,	150	);
			terrains[ (byte)enums.terrainType.hill ] =		new Terrain( (byte)enums.terrainType.hill,			"Hill",		1,  1,  0,  2,  2,  2,  0,  2,	2,  0,	Color.Beige,		2,	200	);
			terrains[ (byte)enums.terrainType.mountain ] =	new Terrain( (byte)enums.terrainType.mountain,		"Mountain",	1,  1,  0,  3,  2,  2,  0,  2,	1,  0,	Color.Brown,		3,	200	);
			terrains[ (byte)enums.terrainType.desert ] =	new Terrain( (byte)enums.terrainType.desert,		"Desert",	1,  1,  0,  1,  1,  1,  2,  2,	2,  2,	Color.Yellow,		1,	100	);
			terrains[ (byte)enums.terrainType.swamp ] =		new Terrain( (byte)enums.terrainType.swamp,			"Swamp",	1,  0,  0,  0,  2,  0,  0,  2,	1,  0,	Color.Turquoise,	2,	100	);
			terrains[ (byte)enums.terrainType.jungle ] =	new Terrain( (byte)enums.terrainType.jungle,		"Jungle",	1,  0,  1,  1,  2,  2,  0,  2,	1,  0,	Color.Olive,		2,	150	);
			terrains[ (byte)enums.terrainType.glacier ] =	new Terrain( (byte)enums.terrainType.glacier,		"Glacier",	1,  0,  1,  1,  2,  2,  0,  2,	0,  0,	Color.White,		2,	100	);
			terrains[ (byte)enums.terrainType.tundra ] =	new Terrain( (byte)enums.terrainType.tundra,		"Tundra",	1,  0,  1,  1,  2,  2,  0,  2,	0,  1,	Color.WhiteSmoke,	1,	100	);

			showProgress.progressBar = 4;
			#endregion

			#region technos
			showProgress.lblInfo = "Loading techno...";
			technologies = new Technology[ (byte)Form1.technoList.totp1 ];
			//{//                  type								Name					cost	line	posOnLine	n0								n1							n2							special		short desc		description
			technologies[ (byte)Form1.technoList.initial ] =		new Technology(	(byte)Form1.technoList.initial,			"Sedentary life",		1,		0,		-2,			0,								0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.ancientDemocraty ] =	new Technology(	(byte)Form1.technoList.ancientDemocraty,	"Ancient democraty",	1,		10,		2,			(byte)Form1.technoList.philosophy,	0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.attelage ] =		new Technology(	(byte)Form1.technoList.attelage,			"Attelage",				1,		6,		0,			(byte)Form1.technoList.horseBreed,	(byte)Form1.technoList.wheel,		(byte)Form1.technoList.iron,		false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.bronze ] =			new Technology(	(byte)Form1.technoList.bronze,			"Bronze",				1,		1,		1,			0,								0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.chivalery ] =		new Technology(	(byte)Form1.technoList.chivalery,			"Chivalery",			1,		10,		-1,			(byte)Form1.technoList.monarchy,		0/*(byte)Form1.technoList.attelage*/,	0,						false,		"",				"description"	);
				
			technologies[ (byte)Form1.technoList.coastalNavigation ] =	new Technology(	(byte)Form1.technoList.coastalNavigation,	"Coastal navigation",	1,		6,		3,			(byte)Form1.technoList.math,			0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.elephantBreed ] =	new Technology(	(byte)Form1.technoList.elephantBreed,		"Elephant breed",		1,		2,		-1,			0,								0,							0,							true,		"",				"description"	);
			technologies[ (byte)Form1.technoList.horseBreed ] =		new Technology(	(byte)Form1.technoList.horseBreed,		"Horse breed",			1,		3,		0,			0,								0,							0,							true,		"",				"description"	);
			technologies[ (byte)Form1.technoList.camelBreed ] =		new Technology(	(byte)Form1.technoList.camelBreed,		"Camel breed",			1,		1,		-1,			0,								0,							0,							true,		"",				"description"	);
			technologies[ (byte)Form1.technoList.iron ] =			new Technology(	(byte)Form1.technoList.iron,				"Iron",					1,		4,		1,			(byte)Form1.technoList.bronze,		0,							0,							false,		"",				"description"	);
				
			technologies[ (byte)Form1.technoList.mapMaking ] =		new Technology(	(byte)Form1.technoList.mapMaking,			"Map making",			1,		7,		3,			(byte)Form1.technoList.coastalNavigation,	0,						0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.masonery ] =		new Technology(	(byte)Form1.technoList.masonery,			"Masonery",				1,		5,		2,			(byte)Form1.technoList.pottery,		0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.math ] =			new Technology(	(byte)Form1.technoList.math,				"Math",					1,		4,		3,			(byte)Form1.technoList.wheel,			0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.monarchy ] =		new Technology(	(byte)Form1.technoList.monarchy,			"Monarchy",				1,		9,		1,			(byte)Form1.technoList.philosophy,	0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.oligarchy ] =		new Technology(	(byte)Form1.technoList.oligarchy,			"Oligarchy",			1,		9,		3,			(byte)Form1.technoList.philosophy,	0,							0,							false,		"",				"description"	);
				
			technologies[ (byte)Form1.technoList.philosophy ] =		new Technology(	(byte)Form1.technoList.philosophy,		"Philosophy",			1,		8,		2,			(byte)Form1.technoList.masonery,		(byte)Form1.technoList.iron,		0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.pottery ] =		new Technology(	(byte)Form1.technoList.pottery,			"Pottery",				1,		2,		2,			0,								0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.warElephant ] =	new Technology(	(byte)Form1.technoList.warElephant,		"War elephant",			1,		5,		-1,			(byte)Form1.technoList.elephantBreed,	(byte)Form1.technoList.iron,		0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.wheel ] =			new Technology(	(byte)Form1.technoList.wheel,				"Wheel",				1,		3,		3,			(byte)Form1.technoList.writing,		0,							0,							false,		"",				"descriptions"	);
			technologies[ (byte)Form1.technoList.writing ] =		new Technology(	(byte)Form1.technoList.writing,			"Writing",				1,		1,		3,			0,								0,							0,							false,		"",				"description"	);

			technologies[ (byte)Form1.technoList.steamPower ] =		new Technology(	(byte)Form1.technoList.steamPower,		"Steam power",			1,		11,		0,			(byte)Form1.technoList.chivalery,		0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.flight ] =			new Technology(	(byte)Form1.technoList.flight,			"Flight",				1,		12,		1,			(byte)Form1.technoList.steamPower,	0,							0,							false,		"",				"description"	);
			technologies[ (byte)Form1.technoList.explosives ] =		new Technology(	(byte)Form1.technoList.explosives,		"Explosives",			1,		12,		-1,			(byte)Form1.technoList.steamPower,	0,							0,							false,		"",				"description"	);

		//	};

			foreach ( Stat.Unit unit in units )
			{
				Statistics.technologies[ unit.disponibility ].militaryValue ++;

				if ( Statistics.technologies[ unit.disponibility ].militaryValue > 20 )
					Statistics.technologies[ unit.disponibility ].militaryValue = 20;
			}
			showProgress.progressBar = 6;
			#endregion

			#region buildings

			showProgress.lblInfo = "Loading buildings...";
			buildings = new Building[ (byte)Form1.buildingList.totp1 ];
		//	{	//																type									Name			cost		disp	upkeep		desc 
			buildings[ (byte)Form1.buildingList.barrack ] =		new Building(	(byte)Form1.buildingList.barrack,		"Barrack",		200,		0,		1,			"Units"		);
			buildings[ (byte)Form1.buildingList.library ] =		new Building(	(byte)Form1.buildingList.library,		"Library",		280,		0,		2,			"Books"		);
			buildings[ (byte)Form1.buildingList.temple ] =		new Building(	(byte)Form1.buildingList.temple,		"Temple",		100,		0,		1,			"Gods"		);
			buildings[ (byte)Form1.buildingList.walls ] =		new Building(	(byte)Form1.buildingList.walls,			"Wall",			500,		0,		3,			"tsé"		);
		//	};
			showProgress.progressBar = 7;

			#endregion

			#region gov
			showProgress.lblInfo = "Loading governements and economies...";
			governements = new Governement[ (byte)enums.governements.tot ];
		//	{//						type																							name						desc		oppressive	sOC				prod	food	trade	ci		sci		techno
			governements[ (byte)enums.governements.anarchy ] =			new Governement(	enums.governements.anarchy,				"Anarchism",				"desc",		false,		true,	false,	100,	100,	100,	100,	100,	0,											Stat.Governement.supportType.military	);
			governements[ (byte)enums.governements.democratyDirect ] =	new Governement(	enums.governements.democratyDirect,		"Democraty Direct",			"desc",		false,		false,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.ancientDemocraty,	Stat.Governement.supportType.military	);
			governements[ (byte)enums.governements.democratyRep ] =		new Governement(	enums.governements.democratyRep,		"Democraty Representative",	"desc",		false,		true,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.ancientDemocraty,	Stat.Governement.supportType.military	);
				//	enterGovStats(	enums.governements.democratyTot,		"Democraty Tot",			"desc",		false,		false,	(byte)Form1.technoList	);
			governements[ (byte)enums.governements.despotism ] =		new Governement(	enums.governements.despotism,			"Despotism",				"desc",		true,		true,	true,	100,	100,	100,	100,	100,	0,											Stat.Governement.supportType.military	);
			
			governements[ (byte)enums.governements.dictatorship ] =		new Governement(	enums.governements.dictatorship,		"Dictatorship",				"desc",		true,		true,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.explosives,			Stat.Governement.supportType.military	);
			governements[ (byte)enums.governements.feodality ] =		new Governement(	enums.governements.feodality,			"Feodality",				"desc",		true,		true,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.chivalery,			Stat.Governement.supportType.military	);
			governements[ (byte)enums.governements.integrism ] =		new Governement(	enums.governements.integrism,			"Integrism",				"desc",		false,		false,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.flight,				Stat.Governement.supportType.military	);
			governements[ (byte)enums.governements.monarchy ] =			new Governement(	enums.governements.monarchy,			"Monarchy",					"desc",		true,		true,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.monarchy,			Stat.Governement.supportType.military	);
				//	enterGovStats(	enums.governements.oligarchy,			"Oligarchy",				"desc",		false,		true,	(byte)Form1.technoList	);
			
			governements[ (byte)enums.governements.totalitarism ] =		new Governement(	enums.governements.totalitarism,		"Totalitarism",				"desc",		true,		false,	true,	100,	100,	100,	100,	100,	(byte)Form1.technoList.explosives,			Stat.Governement.supportType.military	);
		//	};
			#endregion

			#region eco
			economies = new Economy[ (byte)enums.economies.tot ];
		//	{//																	type									name				communism	slavery		prod	food	trade	ci		sci		techno							desc		
			economies[ (byte)enums.economies.natural ] =		new Economy(	(byte)enums.economies.natural,			"Natural",			false,		false,		100,	100,	100,	100,	100,	0,								"Basic, small-sized economic system."								);
			economies[ (byte)enums.economies.slaverySocial ] =	new Economy(	(byte)enums.economies.slaverySocial,	"Social slavery",	false,		true,		80,		60,		120,	100,	100,	(byte)Form1.technoList.bronze,	"Citizen can be transformed to slave by using slavers in cities."	);
			economies[ (byte)enums.economies.slaveryRacial ] =	new Economy(	(byte)enums.economies.slaveryRacial,	"Racial slavery",	false,		true,		80,		60,		120,	100,	100,	(byte)Form1.technoList.iron,	"You can aquire slaves by invading ennemy cities."					);
			economies[ (byte)enums.economies.capitalism ] =		new Economy(	(byte)enums.economies.capitalism,		"Capitalism",		false,		false,		85,		65,		150,	100,	100,	(byte)Form1.technoList.math,	"No descrition yet"													);
			economies[ (byte)enums.economies.communism ] =		new Economy(	(byte)enums.economies.communism,		"Communism",		true,		false,		120,	130,	50,		100,	100,		(byte)Form1.technoList.math,	"No descrition yet"													);
		//	};
			showProgress.progressBar = 8;
			#endregion

			#region resources
			showProgress.lblInfo = "Loading resources...";
			resources = new Resource[ (byte)enums.ressource.tot ];
	//		{	//					type								name			desc		f	p	t	ew	techno							desc			//	T types
			resources[ (byte)enums.ressource.iron ] =		new Resource(	(byte)enums.ressource.iron,			"Iron",			"Iron",		0,	2,	2,	1,	(byte)Form1.technoList.iron,			"Description"	);	//				, new byte[] { (byte)enums.terrainType.plain, (byte)enums.terrainType.hill, (byte)enums.terrainType.mountain } );
			resources[ (byte)enums.ressource.saltpeter ] =	new Resource(	(byte)enums.ressource.saltpeter,	"Saltpeter",	"Iron",		0,	1,	2,	1,	(byte)Form1.technoList.monarchy,		"Description"	);	// gunpowder	, new byte[] { (byte)enums.terrainType.plain, (byte)enums.terrainType.hill, (byte)enums.terrainType.mountain } );
			resources[ (byte)enums.ressource.oil ] =		new Resource(	(byte)enums.ressource.oil,			"Oil",			"Iron",		0,	3,	4,	1,	(byte)Form1.technoList.steamPower,	"Description"	);	// combustion
			resources[ (byte)enums.ressource.aluminum ] =	new Resource(	(byte)enums.ressource.aluminum,		"Aluminum",		"Iron",		0,	3,	3,	1,	(byte)Form1.technoList.iron,			"Description"	);	// lightweigth materials
			resources[ (byte)enums.ressource.uranium ] =	new Resource(	(byte)enums.ressource.uranium,		"Uranium",		"Iron",		0,	3,	3,	1,	(byte)Form1.technoList.steamPower,	"Description"	);	// radiation
			resources[ (byte)enums.ressource.coal ] =		new Resource(	(byte)enums.ressource.coal,			"Coal",			"Iron",		0,	4,	2,	1,	(byte)Form1.technoList.steamPower,	"Description"	);
	//		};		
			showProgress.progressBar = 9;
			#endregion

			#region wonder s

			wonders = new Wonder[] {
									   new Wonder( (byte)Wonder.list.greatPyramid, "Great pyramid", "Useless for now, testing purpose", (byte)Form1.technoList.masonery ),
								   };

			smallWonders = new SmallWonder[] {
									   new SmallWonder( (byte)SmallWonder.list.satteliteControl, "Sattelite control", "Useless for now, testing purpose", (byte)Form1.technoList.flight ),
								   };

			#endregion

			Color transparentColor = terrains[ 0 ].bmp.GetPixel(0,0);
			Form1.ia.SetColorKey(transparentColor, transparentColor);

			variableInitialized = true;
		}
	}
}
