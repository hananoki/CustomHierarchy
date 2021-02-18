﻿using System;
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
	public class SettingsDrawer_Components {
		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = $"{Package.nameNicify}/Components",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}


		static TreeView_Components m_treeview;


		public static void DrawGUI() {
			E.Load();

			HGUIToolbar.Begin();
			HGUIToolbar.DropDown( EditorIcon.toolbar_plus, () => {
				var ppp = AssemblieUtils.SubclassesOf( typeof( Component ) ).OrderBy( x => x.Assembly.FullName );
				var m = new GenericMenu();
				m.AddItem( SS._ReturnToDefault, () => {
					var tt = new List<ComponentHandlerData>();
					tt.Add( new ComponentHandlerData( typeof( Camera ) ) );
					tt.Add( new ComponentHandlerData( typeof( Light ) ) );
					tt.Add( new ComponentHandlerData( typeof( Image ) ) );
					tt.Add( new ComponentHandlerData( typeof( RawImage ) ) );
					tt.Add( new ComponentHandlerData( typeof( SpriteRenderer ) ) );
					tt.Add( new ComponentHandlerData( UnityTypes.TMPro_TextMeshProUGUI ) );
					E.i.m_componentHandlerData = tt;
					E.Save();
					m_treeview.RegisterFiles();
				} );
				m.AddSeparator();
				foreach( var p in ppp ) {
					var asmName = p.Assembly.FullName.Split( ',' )[ 0 ];
					m.AddItem( $"{asmName[ 0 ]}/{asmName}/{p.Name}", ( context ) => {
						E.i.m_componentHandlerData.Add( new ComponentHandlerData( (Type) context ) );
						E.Save();
						m_treeview.RegisterFiles();
					}, p );
				}
				m.DropDown( HEditorGUI.lastRect );
			} );
			if( HGUIToolbar.Button( EditorIcon.toolbar_minus ) ) {
				foreach( var p in m_treeview.GetSelectionItems() ) {
					E.i.m_componentHandlerData.Remove( p.data );
				}
				E.Save();
				m_treeview.RegisterFiles();
			}
			GUILayout.FlexibleSpace();
			HGUIToolbar.End();

			Helper.New( ref m_treeview );
			
			//////////////////
			using( new GUILayoutScope( 1, 0 ) ) {
				m_treeview.DrawLayoutGUI();
			}
		}
	}
}