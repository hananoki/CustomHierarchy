
using HananokiEditor.Extensions;
using HananokiRuntime;
using System.Linq;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;

namespace HananokiEditor.CustomHierarchy {
	public class SettingsDrawer_GameObject {
#if false
		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = $"{Package.nameNicify}/GameObject",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}
#endif
		static TreeView_GameObject m_treeView;

		public static void DrawGUI() {
			E.Load();
			Helper.New( ref m_treeView );

			HGUIToolbar.Begin();
			HGUIToolbar.DropDown( EditorIcon.toolbar_plus, () => {
				var ss = Unsupported.GetSubmenus( "GameObject" ).Select( x => x.Replace( "GameObject/", "" ) ).ToList();
				ss.Remove( "Center On Children" );
				ss.Remove( "Make Parent" );
				ss.Remove( "Clear Parent" );
				ss.Remove( "Set as first sibling" );
				ss.Remove( "Set as last sibling" );
				ss.Remove( "Move To View" );
				ss.Remove( "Align With View" );
				ss.Remove( "Align View to Selected" );
				ss.Remove( "Toggle Active State" );
				var m = new GenericMenu();
				foreach( var s in ss ) {
					m.AddItem( s, ( context ) => {
						E.i.m_menuCommandData.Add( new MenuCommandData { menuItem = s } );
						E.Save();
						m_treeView.RegisterFiles();
					}, s );
				}
				m.DropDown( HEditorGUI.lastRect );
			} );
			GUILayout.FlexibleSpace();
			HGUIToolbar.End();

			//////////////////
			using( new GUILayoutScope( 1, 0 ) ) {
				m_treeView.DrawLayoutGUI();
			}
		}

	}
}


