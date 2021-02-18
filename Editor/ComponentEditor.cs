using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityReflection;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	public class ComponentEditor : HNEditorWindow<ComponentEditor> {
		UnityObject m_component;
		bool inited;
		Vector2 m_scroll;
		public Editor m_currentEditor;

		public static void Open( UnityObject component ) {
			var window = GetWindow<ComponentEditor>();
			window.m_component = component;
		}

		void Init() {
			SetTitle( m_component.GetType().Name, m_component.GetCachedIcon() );
			m_currentEditor = Editor.CreateEditor( m_component );
		}

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
