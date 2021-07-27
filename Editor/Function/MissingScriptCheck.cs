using HananokiEditor.Extensions;
using HananokiRuntime;
using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using E = HananokiEditor.CustomHierarchy.EditorPref;


namespace HananokiEditor.CustomHierarchy {

	public sealed class MissingScriptCheck {

		public struct Data {
			public GameObject target;
			public GameObject root;
		}

		internal static List<Data> m_missingScript;
		internal static List<GameObject> m_sceneObjects;

		static GameObject go => Main.go;


		internal static void Setup() {
#if UNITY_2019_2_OR_NEWER
			Setup( SceneManager.GetActiveScene() );
#endif
		}


		internal static void Setup( Scene scene ) {
#if UNITY_2019_2_OR_NEWER
			if( !E.i.MissingScriptチェック ) return;

			Helper.New( ref m_missingScript );
			Helper.New( ref m_sceneObjects );
			m_missingScript.Clear();
			m_sceneObjects.Clear();
			foreach( var p in scene.GetRootGameObjects() ) {
				m_sceneObjects.AddRange( p.GetGameObjects() );
			}
			EditorApplication.update -= MissingScriptCheck_Update;
			EditorApplication.update += MissingScriptCheck_Update;
#endif
		}


		static void MissingScriptCheck_Update() {
#if UNITY_2019_2_OR_NEWER
			if( 0 < m_sceneObjects.Count ) {
				var go = m_sceneObjects.Last();
				if( 0 < GameObjectUtility.GetMonoBehavioursWithMissingScriptCount( go ) ) {
					m_missingScript.Add( new Data { target = go, root = go.GetParentRoot(), } );
				}
				m_sceneObjects.Remove( go );
				return;
			}
			EditorApplication.update -= MissingScriptCheck_Update;
#endif
		}


		public static void HierarchyChanged() {
#if UNITY_2019_2_OR_NEWER
			if( !E.i.MissingScriptチェック ) return;

			m_missingScript = m_missingScript.Where( x => x.target != null ).ToList();

			var rm = m_missingScript.Where( x => 0 == GameObjectUtility.GetMonoBehavioursWithMissingScriptCount( x.target ) ).ToArray();
			for( int i = 0; i < rm.Length; i++ ) {
				m_missingScript.Remove( rm[ i ] );
			}
#endif
		}


		public static bool Is( GameObject gobj ) {
			if( 0 <= m_missingScript.FindIndex( x => x.target == gobj ) ) {
				return true;
			}
			return false;
		}

		public static void Execute( Rect rect ) {
#if UNITY_2019_2_OR_NEWER
			if( !E.i.MissingScriptチェック ) return;
			if( m_missingScript.Count <= 0 ) return;

			if( 0 <= m_missingScript.FindIndex( x => x.target == go ) ) {
				var r = rect;
				var maxx = r.xMax;
				r.x = 0;
				r.xMax = maxx;
				EditorGUI.DrawRect( r, ColorUtils.RGBA( Color.red, 0.2f ) );
			}
			var idx = m_missingScript.FindIndex( x => x.root == go );
			if( 0 <= idx ) {
				var re = rect;
				re.x = 16 + 16;
				re.width = 16;
				if( HEditorGUI.IconButton( re, EditorIcon.warning ) ) {
					Selection.activeGameObject = m_missingScript[ idx ].target;
				}
				GUI.DrawTexture( re, EditorIcon.warning );
				//EditorGUI.DrawRect( rect.W(16), ColorUtils.RGBA( Color.red, 0.2f ) );
			}
#endif
		}


	}
}

