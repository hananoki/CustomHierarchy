using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using HananokiRuntime;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityReflection;
using HananokiEditor.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityReflection;
using System.Collections;

using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	abstract class HierarchyComponentTool {
		public Type type;
		public GameObject go;
		public UnityObject obj;
		public abstract void OnGUI( ref Rect rect );

		public virtual UnityObject GetReferenceObject() => null;

		protected void _ColorField( ref Rect rect, UnityObject obj, Color color, Action<Color> changed ) {
			rect.width = 32;
			ScopeChange.Begin();
			var _color = UnityEditorEditorGUI.DoColorField( rect, obj.GetInstanceID(), color, true, true, false );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					changed( _color );
				} );
			}
			rect.x += rect.width + 4;
		}

		protected void _AlphaSlider( ref Rect rect, UnityObject obj, float alpha, Action<float> changed ) {
			rect.width = 40;

			ScopeChange.Begin();
			var _a = HEditorGUI.Slider( rect, alpha, 0, 1 );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					changed( _a );
				} );
			}
			rect.x += rect.width + 4;
		}

		protected void _AlphaSlider( ref Rect rect, UnityObject obj, Color color, Action<Color> changed ) {
			rect.width = 40;

			ScopeChange.Begin();
			var _a = HEditorGUI.Slider( rect, color.a, 0, 1 );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					changed( ColorUtils.RGBA( color, _a ) );
				} );
			}
			rect.x += rect.width + 4;
		}

		protected void _ObjectField<T>( ref Rect rect, T obj, Action<T> changed ) where T : UnityObject {
			rect.width = 33;
			ScopeChange.Begin();
			var id = go.GetInstanceID();
			var t = obj != null ? obj.GetType() : typeof( T );
			var _obj = UnityEditorEditorGUI.DoObjectField( rect, rect, id, obj, t, null, null, false, Styles.objectField );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( go, () => {
					changed( (T) _obj );
				} );
			}
			rect.x += rect.width + 4;
		}

		protected void _Graphic( ref Rect rect, Graphic obj ) {
			_ColorField( ref rect, obj, obj.color, ( color ) => {
				obj.color = color;
			} );
			_AlphaSlider( ref rect, obj, obj.color, ( color ) => {
				obj.color = color;
			} );

			rect.width = 16;
			ScopeChange.Begin();
			var _b = EditorGUI.Toggle( rect, obj.raycastTarget );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					obj.raycastTarget = _b;
				} );
			}
		}
	}



	[Hananoki_Hierarchy_ComponentTool( typeof( Light ) )]
	class LightTool : HierarchyComponentTool {
		Light self => (Light) obj;
		public override void OnGUI( ref Rect rect ) {
			rect.width = 16;
			_ColorField( ref rect, self, self.color, ( color ) => {
				self.color = color;
			} );
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
				if( HEditorGUI.IconButton( rect, icon ) ) {
					//s_currentComponents.particleSystem.main
					MaterialEditorWindow.Open( m );
				}
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

