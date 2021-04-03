using HananokiEditor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	public class Window_ComponentEditor : HNEditorWindow<Window_ComponentEditor> {
		UnityObject m_component;
		bool inited;
		Vector2 m_scroll;
		public Editor m_currentEditor;


		/////////////////////////////////////////
		public static void Open( UnityObject component ) {
			var window = EditorWindowUtils.ShowWindow<Window_ComponentEditor>();
			window.m_component = component;
		}


		/////////////////////////////////////////
		void Init() {
			SetTitle( m_component.GetType().Name, m_component.GetCachedIcon() );
			m_currentEditor = ComponentEditorCache.Get( m_component );
			inited = true;
		}


		/////////////////////////////////////////
		public override void OnDefaultGUI() {
			if( !inited ) Init();

			EditorGUIUtility.hierarchyMode = true;
			EditorGUIUtility.wideMode = true;

			ScopeHorizontal.Begin();
			m_currentEditor?.DrawHeader();
			ScopeHorizontal.End();

			using( var sc = new GUILayout.ScrollViewScope( m_scroll ) )
			using( new GUILayoutScope( 16, 4 ) ) {
				m_scroll = sc.scrollPosition;
				//m_currentEditor?.DrawDefaultInspector();

				m_currentEditor?.OnInspectorGUI();
			}
		}

	}
}
