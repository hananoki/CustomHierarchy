using HananokiEditor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityReflection;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {

	[Hananoki_Hierarchy_ComponentTool( typeof( Light ) )]
	class LightTool : HierarchyComponentTool {
		Light self => (Light) obj;

		static GUIStyle s_label;

		public override void OnGUI( ref Rect rect ) {
			rect.width = 16;
			_ColorField( ref rect, self, self.color, ( color ) => {
				self.color = color;
			} );

			if( s_label == null ) {
				s_label = new GUIStyle( EditorStyles.miniLabel );
				s_label.padding = new RectOffset( 0, 0, 0, 0 );
				s_label.margin = new RectOffset( 0, 0, 0, 0 );
				s_label.alignment = TextAnchor.UpperLeft;
				s_label.fontSize = 7;
			}

			var type = L10n.Tr( self.type.ToString() );
			var lightmapBakeType = L10n.Tr( self.lightmapBakeType.ToString() );
			rect.width = Mathf.Max( type.CalcSize( s_label ).x, lightmapBakeType.CalcSize( s_label ).x );
			rect.width = type.CalcSize( EditorStyles.miniLabel ).x;
			EditorGUI.LabelField( rect, type, EditorStyles.miniLabel );

			//rect.x += rect.width;
			//rect.width = "|".CalcSize( EditorStyles.miniLabel ).x;
			//EditorGUI.LabelField( rect, "|", EditorStyles.miniLabel );

			rect.x += rect.width;
			rect.width = lightmapBakeType.CalcSize( EditorStyles.miniLabel ).x;
			EditorGUI.LabelField( rect, lightmapBakeType, EditorStyles.miniLabel );

			rect.x += rect.width;
		}
	}


	class RendererTool : HierarchyComponentTool {
		Renderer self => (Renderer) obj;
		public override void OnGUI( ref Rect rect ) {
			if( self.sharedMaterial == null ) return;

			for( int i = 0; i < self.sharedMaterials.Length; i++ ) {
				var m = self.sharedMaterials[ i ];
				if( m == null ) continue;
				var icon = EditorIcon.icons_processed_unityengine_material_icon_asset;
				if( m.HasProperty( "_MainTex" ) ) {
					icon = AssetPreview.GetAssetPreview( m.GetTexture( "_MainTex" ) );
				}
				else {
					icon = AssetPreview.GetAssetPreview( m );
				}
				rect.width = 16;
				if( HEditorGUI.IconButton( rect, icon ) ) {
					//s_currentComponents.particleSystem.main
					MaterialEditorWindow.Open( m );

				}
				//EditorGUI.DrawRect( rect, new Color(1,0,0,0.1f) );

				//var so = new SerializedObject( self );
				//var it = so.FindProperty( "sharedMaterial" );
				//Debug.Log( it.isExpanded );
				//;
				//while( it.Next( true ) ) {
				//	if( it.isExpanded )
				//	Debug.Log( $"display: {it.displayName}: {it.isExpanded}" );
				//}
				rect.x += 16;

				_ObjectField( ref rect, m, ( material ) => {
					self.sharedMaterials[ i ] = material;
				} );
			}
		}
	}

	[Hananoki_Hierarchy_ComponentTool( typeof( MeshRenderer ) )]
	class MeshRendererTool : RendererTool {
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( SkinnedMeshRenderer ) )]
	class SkinnedMeshRendererTool : RendererTool {
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( ParticleSystemRenderer ) )]
	class ParticleSystemRendererTool : RendererTool {
		ParticleSystemRenderer self => (ParticleSystemRenderer) obj;
		public override void OnGUI( ref Rect rect ) {
			base.OnGUI( ref rect );

			if( self.trailMaterial != null ) {
				if( HEditorGUI.IconButton( rect, EditorIcon.icons_processed_unityengine_material_icon_asset ) ) {
					//s_currentComponents.particleSystem.main
					MaterialEditorWindow.Open( self.trailMaterial );
				}
				rect.x += 20;
			}
		}
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( SpriteRenderer ) )]
	class SpriteRendererTool : HierarchyComponentTool {
		SpriteRenderer self => (SpriteRenderer) obj;
		public override void OnGUI( ref Rect rect ) {
			_ObjectField( ref rect, self.sprite, ( spr ) => {
				self.sprite = spr;
			} );
			_ColorField( ref rect, self, self.color, ( color ) => {
				self.color = color;
			} );
			_AlphaSlider( ref rect, self, self.color, ( color ) => {
				self.color = color;
			} );
		}
	}


	class GraphicTool : HierarchyComponentTool {
		Graphic self => (Graphic) obj;
		public override void OnGUI( ref Rect rect ) {
			_Graphic( ref rect, self );
		}
	}


	[Hananoki_Hierarchy_ComponentTool( typeof( Image ) )]
	class ImageTool : GraphicTool {
		Image self => (Image) obj;
		public override void OnGUI( ref Rect rect ) {
			_ObjectField( ref rect, self.sprite, ( spr ) => {
				self.sprite = spr;
			} );

			base.OnGUI( ref rect );
		}
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( RawImage ) )]
	class RawImageTool : GraphicTool {
		RawImage self => (RawImage) obj;
		public override UnityObject GetReferenceObject() => self.texture;
		public override void OnGUI( ref Rect rect ) {
			_ObjectField( ref rect, self.texture, ( spr ) => {
				self.texture = spr;
			} );

			base.OnGUI( ref rect );
		}
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( Text ) )]
	class TextTool : GraphicTool {
		Text self => (Text) obj;
		public override void OnGUI( ref Rect rect ) {
			_ObjectField( ref rect, self.font, ( spr ) => {
				self.font = spr;
			} );
			base.OnGUI( ref rect );
		}
	}



	[Hananoki_Hierarchy_ComponentTool( "TMPro.TMP_Text" )]
	class TMProTMP_TextTool : GraphicTool {

		TMProTMP_Text self;

		public override void OnGUI( ref Rect rect ) {
			if( self == null ) {
				self = new TMProTMP_Text( obj );
			}
			_ObjectField( ref rect, self.font, ( spr ) => {
				//self.font = spr;
				self.m_instance.SetProperty<object>( "font", spr );
			} );

			rect.width = 16;
			if( HEditorGUI.IconButton( rect, EditorIcon.search_icon ) ) {
				ProjectBrowserUtils.SetSearch( "t:TMP_FontAsset" );
			}
			rect.x += rect.width + 4;

			base.OnGUI( ref rect );
		}
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( CanvasGroup ) )]
	class CanvasGroupTool : HierarchyComponentTool {
		CanvasGroup self => (CanvasGroup) obj;
		public override void OnGUI( ref Rect rect ) {
			_AlphaSlider( ref rect, self, self.alpha, ( alpha ) => {
				self.alpha = alpha;
			} );
		}
	}
}

