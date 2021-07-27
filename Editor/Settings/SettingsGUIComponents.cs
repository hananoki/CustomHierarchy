using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using HananokiRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using E = HananokiEditor.CustomHierarchy.EditorPref;
using SS = HananokiEditor.SharedModule.S;


namespace HananokiEditor.CustomHierarchy {

	public class SettingsGUIComponents {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = $"{Package.nameNicify}/Components",
				version = "",
				gui = DrawGUI,
				customLayoutMode = true,
			};
		}


		static TreeViewComponents m_treeview;


		/////////////////////////////////////////
		static void OnDropDown() {
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

			var componentTypes = AssemblieUtils.SubclassesOf( typeof( Component ) )
				.OrderBy( x => x.Assembly.FullName )
				.ThenBy( x => x.Name );

			foreach( var p in componentTypes ) {
				if( 0 <= E.i.m_componentHandlerData.FindIndex( x => x.type == p ) ) continue;

				var asmName = p.Assembly.FullName.Split( ',' )[ 0 ];
				m.AddItem( $"{asmName[ 0 ]}/{asmName}/{p.Name}", ( context ) => {
					E.i.m_componentHandlerData.Add( new ComponentHandlerData( (Type) context ) );
					E.Save();
					m_treeview.RegisterFiles();
				}, p );
			}
			m.DropDown( HEditorGUI.lastRect );
		}


		/////////////////////////////////////////
		static void 現在のシーンのコンポーネント型を取得() {
			var cc = new HashSet<Type>();
			var scene = SceneManager.GetActiveScene();
			foreach( var p in scene.GetRootGameObjects() ) {
				foreach( var pp in p.GetGameObjects() ) {
					foreach( var ccc in pp.GetComponents( typeof( Component ) ).Where( x => x != null ).OrEmptyIfNull() ) {
						cc.Add( ccc.GetType() );
					}
				}
			}
			foreach( var c in cc ) {
				var idx = E.i.m_componentHandlerData.FindIndex( x => x.type == c );
				if( 0 <= idx ) continue;
				E.i.m_componentHandlerData.Add( new ComponentHandlerData( c ) );
			}
			E.Save();
			m_treeview.RegisterFiles();
		}


		/////////////////////////////////////////
		public static void DrawGUI() {
			E.Load();

			HGUIToolbar.Begin();
			HGUIToolbar.DropDown( EditorIcon.toolbar_plus, OnDropDown );
			if( HGUIToolbar.Button( EditorIcon.toolbar_minus ) ) {
				foreach( var p in m_treeview.GetSelectionItems() ) {
					E.i.m_componentHandlerData.Remove( p.data );
				}
				E.Save();
				m_treeview.RegisterFiles();
			}
			if( HGUIToolbar.Button( EditorIcon.search_icon ) ) {
				HEditorDialog.Info( S._Registerallthecomponentsusedinthecurrentscene, 現在のシーンのコンポーネント型を取得 );
			}
			GUILayout.FlexibleSpace();
			if( HGUIToolbar.Button( EditorIcon.alphabeticalsorting ) ) {
				E.i.m_componentHandlerData = E.i.m_componentHandlerData.OrderBy( x => x.type.Name ).ToList();
				E.Save();
				m_treeview.RegisterFiles();
			}
			HGUIToolbar.End();


			Helper.New( ref m_treeview );

			//////////////////
			using( new GUILayoutScope( 1, 0 ) ) {
				m_treeview.DrawLayoutGUI();
			}
		}


	}
}