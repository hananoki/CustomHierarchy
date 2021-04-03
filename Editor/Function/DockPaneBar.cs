using HananokiEditor.Extensions;
using System;
using UnityEditor;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#endif

using E = HananokiEditor.CustomHierarchy.SettingsEditor;

namespace HananokiEditor.CustomHierarchy {
	public static class DockPaneBar {

		static EditorWindow s_sceneHierarchy;
		static object s_IMGUIContainer;

		internal static int s_initButton = -1;




		internal static void Setup() {
			if( !UnitySymbol.UNITY_2019_1_OR_NEWER ) return;

			if( s_initButton < 0 ) {
				if( E.i.dockPaneBar ) {
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
			s_sceneHierarchy?.RemoveIMGUIContainer( s_IMGUIContainer, true );
		}


		internal static void RegisterDockPane() {
			if( s_sceneHierarchy == null ) {
				s_sceneHierarchy = EditorWindowUtils.Find( UnityTypes.UnityEditor_SceneHierarchyWindow );
			}
			s_IMGUIContainer = Activator.CreateInstance( UnityTypes.UnityEngine_UIElements_IMGUIContainer, new object[] { (Action) OnDrawDockPane } );
			s_sceneHierarchy?.AddIMGUIContainer( s_IMGUIContainer, true );

#if UNITY_2019_1_OR_NEWER
			IMGUIContainer con = (IMGUIContainer) s_IMGUIContainer;

			con.style.height = 20;
			con.style.marginRight = 42;
			con.style.width = 144 + 8;
			con.style.alignSelf = Align.FlexEnd;

			//if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
			//	con.style.height = 20;
			//	con.style.width = 26;
			//	con.style.marginLeft = 36;
			//}
			//else if( UnitySymbol.UNITY_2019_2_OR_NEWER ) {
			//	con.style.height = 18;
			//	con.style.width = 28;
			//	con.style.marginLeft = 50;
			//	con.style.marginTop = -2;
			//}
			//else {
			//	con.style.top = -1;
			//	con.style.height = 16;
			//	con.style.width = 28;
			//	con.style.marginLeft = 50;
			//}
#endif
			//if( !UnitySymbol.UNITY_2019_3_OR_NEWER ) {
			//	( (IMGUIContainer) _IMGUIContainer ).style.height = 16;
			//}
		}

		static void OnDrawDockPane() {

			ScopeHorizontal.Begin();
			GUILayout.FlexibleSpace();

			if( HEditorGUILayout.IconButton( EditorIcon.icons_processed_unityengine_gameobject_icon_asset, "GameObject" ) ) {
				EditorApplication.ExecuteMenuItem( "GameObject/Create Empty Child" );
			}
			if( HEditorGUILayout.IconButton( EditorIcon.icons_processed_unityengine_particlesystem_icon_asset, "ParticleSystem" ) ) {
				EditorApplication.ExecuteMenuItem( "GameObject/Effects/Particle System" );
			}
			if( HEditorGUILayout.IconButton( EditorIcon.icons_processed_unityengine_ui_image_icon_asset, "Image" ) ) {
				EditorApplication.ExecuteMenuItem( "GameObject/UI/Image" );
			}
			if( HEditorGUILayout.IconButton( EditorIcon.icons_processed_unityengine_ui_rawimage_icon_asset, "RawImage" ) ) {
				EditorApplication.ExecuteMenuItem( "GameObject/UI/Raw Image" );
			}
			if( HEditorGUILayout.IconButton( EditorIcon.icons_processed_collabdeleted_icon_asset, "Selection Clear" ) ) {
				Selection.activeObject = null;
			}
			//if( HEditorGUILayout.IconButton( EditorIcon.unityeditor_timeline_timelinewindow, SS._Timeline ) ) {
			//	HEditorWindow.ShowWindow( UnityTypes.UnityEditor_Timeline_TimelineWindow );
			//}
			//GUILayout.Space( 8 );
			//if( HEditorGUILayout.IconButton( EditorIcon.unityeditor_consolewindow, SS._Console ) ) {
			//	HEditorWindow.ShowWindow( UnityTypes.UnityEditor_ConsoleWindow );
			//}
			//if( HEditorGUILayout.IconButton( EditorIcon.unityeditor_profilerwindow, SS._Profiler ) ) {
			//	HEditorWindow.ShowWindow( UnityTypes.UnityEditor_ProfilerWindow );
			//}

			GUILayout.Space( 12 );
			ScopeHorizontal.End();
		}

	}
}
