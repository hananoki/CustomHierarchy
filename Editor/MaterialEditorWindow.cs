using UnityEditor;
using UnityEngine;

namespace HananokiEditor.CustomHierarchy {
	public class MaterialEditorWindow : HNEditorWindow<MaterialEditorWindow> {
		Material m_material;
		bool m_inited;
		MaterialEditor m_currentEditor;
		Vector2 m_scroll;

		public static void Open( Material material ) {
			var window = GetWindow<MaterialEditorWindow>();
			window.m_material = material;
		}

		void Init() {
			SetTitle( "マテリアル", EditorIcon.icons_processed_unityengine_material_icon_asset );
			m_currentEditor = (MaterialEditor) Editor.CreateEditor( m_material );
		}

		public override void OnDefaultGUI() {
			if( !m_inited ) Init();

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
