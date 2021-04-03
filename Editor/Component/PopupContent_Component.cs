#pragma warning disable 649

using HananokiRuntime;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {

	public class PopupContent_Component : PopupWindowContent {

		static PopupContent_Component s_content;
		static UnityObject s_component;

		public Editor m_currentEditor;
		Vector2 m_scroll;

		/////////////////////////////////////////
		public static void Show( Rect rect, UnityObject component ) {
			s_component = component;
			Helper.New( ref s_content );
			PopupWindow.Show( rect, s_content );
		}


		/////////////////////////////////////////
		public override Vector2 GetWindowSize() {
			return new Vector2( 400, 300 );
		}


		/////////////////////////////////////////
		public override void OnGUI( Rect rect ) {
			using( new GUILayout.AreaScope( rect ) )
			using( var sc = new GUILayout.ScrollViewScope( m_scroll ) )
			using( new GUILayoutScope( 16, 4 ) ) {
				EditorGUIUtility.hierarchyMode = true;
				EditorGUIUtility.wideMode = true;

				m_scroll = sc.scrollPosition;
				//m_currentEditor?.DrawDefaultInspector();
				GUILayout.Space( 4 );
				m_currentEditor?.OnInspectorGUI();
			}
		}


		/////////////////////////////////////////
		public override void OnOpen() {
			m_currentEditor = ComponentEditorCache.Get( s_component );
		}


		/////////////////////////////////////////
		public override void OnClose() {
		}
	}
}
