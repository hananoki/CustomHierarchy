using HananokiEditor.Extensions;
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
			SetTitle( "Material", EditorIcon.icons_processed_unityengine_material_icon_asset );
			m_currentEditor = (MaterialEditor) Editor.CreateEditor( m_material );
			UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded( m_material, true );
			m_inited = true;
		}

		public override void OnDefaultGUI() {
			if( !m_inited ) Init();

			EditorGUIUtility.hierarchyMode = true;
			EditorGUIUtility.wideMode = true;

			HGUIToolbar.Begin();
			if( HGUIToolbar.DropDown( "URP" ) ) {
				ShaderInfo[] allShaderInfo = ShaderUtil.GetAllShaderInfo();
				var m = new GenericMenu();
				foreach( var p in allShaderInfo ) {
					if( p.name.StartsWith( "Hidden" ) ) continue;
					if( p.name.Contains( "Universal Render Pipeline" ) ) {
						//Debug.Log( p.name );
						m.AddItem( p.name.Replace( "Universal Render Pipeline/", "" ), ( context ) => {
							EditorHelper.Dirty( m_material, () => {

								//Debug.Log( m_material.mainTexture.name );
								m_material.shader = Shader.Find( (string) context ); ;
								if( m_material.HasProperty( "_MainTex" ) ) {
									m_material.SetTexture( "_BaseMap", m_material.GetTexture( "_MainTex" ) );
								}
								if( m_material.HasProperty( "_TintColor" ) ) {
									m_material.SetColor( "_BaseColor", m_material.GetColor( "_TintColor" ) );
								}
								if( m_material.HasProperty( "_Color" ) ) {
									m_material.SetColor( "_BaseColor", m_material.GetColor( "_Color" ) );
								}
								if( m_material.HasProperty( "_Metallic" ) ) {
									m_material.SetFloat( "_Metallic", m_material.GetFloat( "_Metallic" ) );
								}
								if( m_material.HasProperty( "_Glossiness" ) ) {
									m_material.SetFloat( "_Smoothness", m_material.GetFloat( "_Glossiness" ) );
								}
							} );
						}, p.name );
					}
				}
				m.DropDown( HEditorGUI.lastRect );
				//m_material.shader = Shader.Find( "Universal Render Pipeline/Lit" ); ;
			}
			GUILayout.FlexibleSpace();
			HGUIToolbar.End();

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
