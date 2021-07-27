using HananokiEditor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	public class ComponentEditorWindow : HNEditorWindow<ComponentEditorWindow> {

		Vector2 m_scroll;
		public Editor m_currentEditor;

		static UnityObject s_component;
		static ComponentEditorWindow s_window;



		/////////////////////////////////////////
		public static void Open( UnityObject component ) {
			s_component = component;
			if( s_window == null ) {
				s_window = EditorWindowUtils.ShowWindow<ComponentEditorWindow>();
			}
			else {
				s_window.Init();
				s_window.Repaint();
				s_window.Focus();
			}
		}


		/////////////////////////////////////////
		void OnEnable() {
			Init();
		}


		/////////////////////////////////////////
		void OnDestroy() {
			s_window = null;
		}


		/////////////////////////////////////////
		void Init() {
			SetTitle( s_component.GetType().Name, s_component.GetCachedIcon() );
			m_currentEditor = ComponentEditorCache.Get( s_component );
		}


		/////////////////////////////////////////
		public override void OnDefaultGUI() {

			EditorGUIUtility.hierarchyMode = true;
			EditorGUIUtility.wideMode = true;

			ScopeHorizontal.Begin();
			m_currentEditor?.DrawHeader();
			ScopeHorizontal.End();

			using( var sc = new GUILayout.ScrollViewScope( m_scroll ) )
			using( new GUILayoutScope( 16, 4 ) ) {
				m_scroll = sc.scrollPosition;
				m_currentEditor?.OnInspectorGUI();
			}
		}

	}
}
