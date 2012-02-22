using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace xycv_ppc
{
	/// <summary>
	/// Summary description for frmInfoTree.
	/// </summary>
	public class frmInfoTree : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView tvInfo;
		string lastSelected;
	
		public frmInfoTree()
		{
			InitializeComponent();

			platformSpec.resolution.set( this.Controls );
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tvInfo = new System.Windows.Forms.TreeView();
			// 
			// tvInfo
			// 
			this.tvInfo.ImageIndex = -1;
			this.tvInfo.SelectedImageIndex = -1;
			this.tvInfo.Size = new System.Drawing.Size(240, 296);
			// 
			// frmInfoTree
			// 
			this.ClientSize = new System.Drawing.Size(240, 296);
			this.Controls.Add(this.tvInfo);
			this.Text = "frmInfoTree";
			this.Load += new System.EventHandler(this.frmInfoTree_Load);

		}
		#endregion

		private void frmInfoTree_Load(object sender, System.EventArgs e)
		{
	/*		TreeNode tnUnits = new TreeNode( "Units" ),
				tnCivilization = new TreeNode( "Civilizations" ),
				tnTerrain = new TreeNode( "Terrain" ),
				tnTechno = new TreeNode( "Technologies" ),
				tnGov = new TreeNode( "Governements" ),
				tnEco = new TreeNode( "Economies" ),
				tnMisc = new TreeNode( "Misc" ),
				tnCaseImp = new TreeNode( "Case improvements" ),
				tnResources = new TreeNode( "Resources" ),
				tnSpecialResources = new TreeNode( "Special resources" ),
				tnTutorial = new TreeNode( "Tutorial" );		*/
			TreeNode tnUnits = new TreeNode( language.getAString( language.order.encyclopediaUnits ) ),
				tnCivilization = new TreeNode( language.getAString( language.order.encyclopediaCivilizations ) ),
				tnTerrain = new TreeNode( language.getAString( language.order.encyclopediaTerrains ) ),
				tnTechno = new TreeNode( language.getAString( language.order.encyclopediaTechnologies ) ),
				tnGov = new TreeNode( language.getAString( language.order.encyclopediaGovernements ) ),
				tnEco = new TreeNode( language.getAString( language.order.encyclopediaEconomies ) ),
			//	tnCaseImp = new TreeNode( "Case improvements" ),
				tnResources = new TreeNode( language.getAString( language.order.encyclopediaResources ) ),
			//	tnSpecialResources = new TreeNode( "Special resources" ),
				tnTutorial = new TreeNode( language.getAString( language.order.encyclopediaTutorial ) ),
				tnBuildings = new TreeNode( language.getAString( language.order.encyclopediaBuildings ) ),
				tnSmallWonders = new TreeNode( language.getAString( language.order.encyclopediaSmallWonders ) ),
				tnWonders = new TreeNode( language.getAString( language.order.encyclopediaWonders ) ),
				tnMisc = new TreeNode( language.getAString( language.order.encyclopediaMisc ) ); 

			tnUnits.Tag = enums.infoType.units; 
			tnCivilization.Tag = enums.infoType.civ; 
			tnTerrain.Tag = enums.infoType.terrain; 
			tnTechno.Tag = enums.infoType.techno; 
			tnGov.Tag = enums.infoType.gov;
			tnEco.Tag = enums.infoType.eco; 
		//	tnMisc.Tag = enums.infoType.misc; 
		//	tnCaseImp.Tag = enums.infoType.caseImp; 
			tnResources.Tag = enums.infoType.resources;
		//	tnSpecialResources.Tag = enums.infoType.specialResources;
			tnTutorial.Tag = enums.infoType.tutorial;
			tnBuildings.Tag = enums.infoType.buildings;
			tnSmallWonders.Tag = enums.infoType.smallWonders;
			tnWonders.Tag = enums.infoType.wonders;
			tnMisc.Tag = enums.infoType.misc;
			
			System.Collections.ArrayList al;

			#region units

			al = new ArrayList( Statistics.units.Length );

			for ( int i = 0; i < Statistics.units.Length; i ++ )
				al.Add( Statistics.units[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnUnits.Nodes.Add( tnUnit );
			}
			/*	for ( int i = 0; i < Statistics.units.Length; i ++ )
				if ( Statistics.units[ i ].bmp != null )
				{
					TreeNode tnUnit = new TreeNode( Statistics.units[ i ].name );

					tnUnit.Tag = i;
					tnUnits.Nodes.Add( tnUnit );
				}*/

			#endregion

			#region civilization

			al = new ArrayList( Statistics.civilizations.Length );

			for ( int i = 0; i < Statistics.civilizations.Length; i ++ )
				al.Add( Statistics.civilizations[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnCivilization.Nodes.Add( tnUnit );
			}
		/*	for ( int i = 0; i < Statistics.civilizations.Length; i ++ )
			{
				TreeNode tnUnit = new TreeNode( Statistics.civilizations[ i ].name );
				tnUnit.Tag = i;
				tnCivilization.Nodes.Add( tnUnit );
			}*/

			#endregion

		#region terrain

			al = new ArrayList( Statistics.terrains.Length );

			for ( int i = 0; i < Statistics.terrains.Length; i ++ )
				al.Add( Statistics.terrains[ i ] );

			al.Sort();

			foreach ( Stat.Terrain t in al )
			{
				TreeNode tnUnit = new TreeNode( t.name );
				tnUnit.Tag = (int)t.type;
				tnTerrain.Nodes.Add( tnUnit );
			}

		#endregion

			#region Statistics.technologies

			al = new ArrayList( Statistics.technologies.Length );

			for ( int i = 0; i < Statistics.technologies.Length; i ++ )
				al.Add( Statistics.technologies[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnTechno.Nodes.Add( tnUnit );
			}

		/*	for ( int i = 0; i < Statistics.technologies.Length; i ++ )
			{
				TreeNode tnUnit = new TreeNode( Statistics.technologies[ i ].name );
				tnUnit.Tag = i;
				tnTechno.Nodes.Add( tnUnit );
			}*/
			#endregion

			#region Statistics.governements

			al = new ArrayList( Statistics.governements.Length );

			for ( int i = 0; i < Statistics.governements.Length; i ++ )
				al.Add( Statistics.governements[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnGov.Nodes.Add( tnUnit );
			}

			/*	
			for ( int i = 0; i < Statistics.governements.Length; i ++ )
			{
				TreeNode tnUnit = new TreeNode( Statistics.governements[ i ].name );
				tnUnit.Tag = i;
				tnGov.Nodes.Add( tnUnit );
			}*/
			#endregion

			#region Statistics.economies

			al = new ArrayList( Statistics.economies.Length );

			for ( int i = 0; i < Statistics.economies.Length; i ++ )
				al.Add( Statistics.economies[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnEco.Nodes.Add( tnUnit );
			}

			/*	
			for ( int i = 0; i < Statistics.economies.Length; i++ )
			{
				TreeNode tnUnit = new TreeNode( Statistics.economies[ i ].name );

				tnUnit.Tag = i;
				tnEco.Nodes.Add( tnUnit );
			}*/
			#endregion

		#region resource

			al = new ArrayList( Statistics.resources.Length );
			al.Clear();

			for ( int i = 0; i < Statistics.resources.Length; i ++ )
				al.Add( Statistics.resources[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnResource = new TreeNode( s.name );
				tnResource.Tag = s.type;
				tnResources.Nodes.Add( tnResource );
			}

			#endregion

			#region tutorial

			for ( int i = 0; i < (int)Tutorial.order.tot; i ++ )
			{
				TreeNode tnI = new TreeNode( Tutorial.getTextFromInd( i ).title );
				tnI.Tag = i;
				tnTutorial.Nodes.Add( tnI );
			}

			#endregion

			#region buildings

			al = new ArrayList( Statistics.buildings.Length );

			for ( int i = 0; i < Statistics.buildings.Length; i ++ )
				al.Add( Statistics.buildings[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnBuildings.Nodes.Add( tnUnit );
			}

			#endregion

			#region small wonders

			al = new ArrayList( Statistics.smallWonders.Length );

			for ( int i = 0; i < Statistics.smallWonders.Length; i ++ )
				al.Add( Statistics.smallWonders[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnSmallWonders.Nodes.Add( tnUnit );
			}

			#endregion

			#region wonders

			al = new ArrayList( Statistics.wonders.Length );

			for ( int i = 0; i < Statistics.wonders.Length; i ++ )
				al.Add( Statistics.wonders[ i ] );

			al.Sort();

			foreach ( Stat.General s in al )
			{
				TreeNode tnUnit = new TreeNode( s.name );
				tnUnit.Tag = (int)s.type;
				tnWonders.Nodes.Add( tnUnit );
			}

			#endregion

	/*		for ( int i = 0; i < Statistics.resources.Length; i++ )
			{
				TreeNode tnResource = new TreeNode( Statistics.resources[ i ].name );

				tnResource.Tag = i;
				tnResources.Nodes.Add( tnResource );
			}*/

		/*	for ( int i = 0; i < Form1.case.Length; i++ )
			{
				TreeNode tnUnit = new TreeNode( Statistics.economies[ i ].name );

				tnUnit.Tag = i;
				tnEco.Nodes.Add( tnUnit );
			}*/

			tvInfo.Nodes.Add( tnUnits );
			tvInfo.Nodes.Add( tnCivilization );
			tvInfo.Nodes.Add( tnTerrain );
			tvInfo.Nodes.Add( tnTechno );
			tvInfo.Nodes.Add( tnGov );
			tvInfo.Nodes.Add( tnEco );
	//		tvInfo.Nodes.Add( tnCaseImp );
	//		tvInfo.Nodes.Add( tnMisc );
			tvInfo.Nodes.Add( tnResources );
			tvInfo.Nodes.Add( tnBuildings );
			tvInfo.Nodes.Add( tnSmallWonders );
			tvInfo.Nodes.Add( tnWonders );
			tvInfo.Nodes.Add( tnTutorial );
			tvInfo.Nodes.Add( tnMisc );
	//		tvInfo.Nodes.Add( tnSpecialResources );

			tvInfo.AfterSelect += new TreeViewEventHandler(tvInfo_AfterSelect);

		}

		private void tvInfo_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if ( tvInfo.SelectedNode.Nodes.Count == 0 )
			{
				string title = this.Text;
				platformSpec.manageWindows.prepareForDialog( this ); //this.Text = "";

				try
				{
					frmInfo fi = new frmInfo( (enums.infoType)Convert.ToInt32( tvInfo.SelectedNode.Parent.Tag ), Convert.ToInt32( tvInfo.SelectedNode.Tag ) );
					fi.ShowDialog();
				}
				catch
				{
					MessageBox.Show( "Error within encyclopedia, some items aren't completed.", "Known bug" );
				}

				this.Text = title;
			}
		}
	}
}
