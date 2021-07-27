using UnityEngine;
using HananokiEditor.Extensions;
using System;
using UnityEditor;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#endif



using E = HananokiEditor.CustomHierarchy.EditorPref;


namespace HananokiEditor.CustomHierarchy {
	public static class CommandBar {

		static EditorWindow s_sceneHierarchy;
		static object s_IMGUIContainer;

		internal static int s_initButton = -1;

		internal static void Setup() {
			if( !UnitySymbol.UNITY_2019_1_OR_NEWER ) return;

			if( s_initButton < 0 ) {
				if( E.i.commandBar ) {
					RegisterDockPane();
					s_initButton = 1;
				}
				else {
					UnRegisterDockPane();
					s_initButton = 0;
				}
			}
		}


		internal static void UnRegisterDockPane() {
			if( s_sceneHierarchy == null ) {
				s_sceneHierarchy = EditorWindowUtils.Find( UnityTypes.UnityEditor_SceneHierarchyWindow );
			}
			s_sceneHierarchy?.RemoveIMGUIContainer( s_IMGUIContainer );
		}


		internal static void RegisterDockPane() {
			if( s_sceneHierarchy == null ) {
				s_sceneHierarchy = EditorWindowUtils.Find( UnityTypes.UnityEditor_SceneHierarchyWindow );
			}
			s_IMGUIContainer = Activator.CreateInstance( UnityTypes.UnityEngine_UIElements_IMGUIContainer, new object[] { (Action) OnDrawDockPane } );
			s_sceneHierarchy?.AddIMGUIContainer( s_IMGUIContainer );

#if UNITY_2019_1_OR_NEWER
			IMGUIContainer imgui = (IMGUIContainer) s_IMGUIContainer;

			imgui.style.position = Position.Absolute;

			if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
				imgui.style.height = 20;
				if( ExternalPackages.SelectionHistory.enabled ) {
					imgui.style.width = 26 * 2 + 36 * 1;
				}
				else {
					imgui.style.width = 36 * 2;
				}
				imgui.style.left = 36;
			}
			else if( UnitySymbol.UNITY_2019_2_OR_NEWER ) {
				imgui.style.height = 18;
				if( ExternalPackages.SelectionHistory.enabled ) {
					imgui.style.width = 26 * 2 + 40 * 1;
				}
				else {
					imgui.style.width = 40 * 1;
				}
				imgui.style.top = -2;
				imgui.style.left = 49;
			}
			else {
				imgui.style.height = 16;
				if( ExternalPackages.SelectionHistory.enabled ) {
					imgui.style.width = 26 * 2 + 40 * 1;
				}
				else {
					imgui.style.width = 40 * 1;
				}
				imgui.style.top = -1;
				imgui.style.left = 49;
			}
#endif
			//if( !UnitySymbol.UNITY_2019_3_OR_NEWER ) {
			//	( (IMGUIContainer) _IMGUIContainer ).style.height = 16;
			//}
		}


		static void OnDrawDockPane() {
			E.Load();
			GUILayout.BeginArea( new Rect( 0, 0, s_sceneHierarchy.position.width, 20 ) );
			ScopeHorizontal.Begin();
			GUILayout.Space( 1 );

			if( ExternalPackages.SelectionHistory.enabled ) {
				ScopeDisable.Begin( !ExternalPackages.SelectionHistory.hasPrev );
				if( HGUIToolbar.Button( EditorIcon.tab_prev ) ) ExternalPackages.SelectionHistory.Prev();
				ScopeDisable.End();
				ScopeDisable.Begin( !ExternalPackages.SelectionHistory.hasNext );
				if( HGUIToolbar.Button( EditorIcon.tab_next ) ) ExternalPackages.SelectionHistory.Next();
				ScopeDisable.End();
			}
			HGUIToolbar.DropDown( EditorIcon.AnimationFilterBySelection, () => {
				//var m = new GenericMenu();
				//m.AddItem( S._UnhideGameObject, _UnhideGameObject );
				//m.DropDown( HEditorGUI.lastRect );
				PopupContentHideSelect.Show( HEditorGUI.lastRect );
				//SceneVisibilityManager.instance.HideAll();
			}, GUILayout.Width( 38 ) );
			//HGUIToolbar.DropDown( EditorIcon.search_icon, () => {
			//	var m = new GenericMenu();
			//	m.AddItem( "None", _Filter, null );
			//	var ll = E.i.m_componentHandlerData.Where( x => x.type != null ).OrderBy( x => x.type.Name ).Where( x => x.search );
			//	if( 0 < ll.Count() ) {
			//		m.AddSeparator();
			//		foreach( var p in ll ) {
			//			m.AddItem( p.type.Name, _Filter, p );
			//		}
			//	}

			//	m.DropDown( HEditorGUI.lastRect );
			//} );

			ScopeHorizontal.End();
			GUILayout.EndArea();
		}


		static void _UnhideGameObject() {
			foreach( var g in EditorHelper.GetSceneObjects<GameObject>() ) {
				g.hideFlags = HideFlags.None;
			}
		}

		//static void _Filter( object context ) {
		//	var p = (ComponentHandlerData) context;
		//	if( p != null ) {
		//		//	filter = p.type.Name;
		//		//	mode = SearchableEditorWindow.SearchMode.Type;
		//		//	//filter = s.filter.Split('/').Last();
		//		//	//mode = s.searchMode;
		//		//SceneHierarchyUtils.SetSearchFilter( filter, mode );
		//		SceneModeUtility.SearchForType( p.type );
		//	}
		//	else {
		//		SceneModeUtility.SearchForType( null );
		//	}
		//}

	}
}
