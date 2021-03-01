using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using HananokiRuntime.Extensions;
using HananokiRuntime;
using HananokiEditor.SharedModule;
using UnityReflection;
using UnityEngine.UI;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.CustomHierarchy {
	public class SettingsDrawer_GameObject {
		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = $"{Package.nameNicify}/GameObject",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}

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
