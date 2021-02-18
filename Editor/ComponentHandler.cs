using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using HananokiRuntime;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityReflection;
using HananokiEditor.Extensions;


using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	internal static class ComponentHandler {

		static Dictionary<int, ComponentObjects> s_componets;

		static GUIStyle s_objectField;
		static GUIStyle objectField {
			get {
				if( s_objectField != null ) return s_objectField;

				s_objectField = new GUIStyle( EditorStyles.objectField );
				s_objectField.padding.top = 1;
				s_objectField.padding.left = 2;
				return s_objectField;
			}
		}
		public class ComponentObjects {
			public Component[] components;
			public ParticleSystemRenderer particleSystemRenderer;
			public ReflectionProbe reflectionProbe;
			public MeshRenderer meshRenderer;
			public SkinnedMeshRenderer skinnedMeshRenderer;
			public SpriteRenderer spriteRenderer;
			public Image image;
			public RawImage rawImage;
			public Light light;
			public TMProTMP_Text textmeshpro;
			//public RawImage rawImage;
			public UnityEditorPrefabOverridesWindow prefabOverride;

			public Component inspec;
		}

		static ComponentObjects s_current;

		static GameObject go => CustomHierarchy.go;


		public static void Reset() {
			s_componets = new Dictionary<int, ComponentObjects>();
		}

		public static bool hasTextMeshPro => s_current.textmeshpro.m_instance != null;
		public static bool hasRawImage => s_current.rawImage != null;
		public static RawImage rawImage => s_current.rawImage;



		public static void Execute( Rect selectionRect ) {
			if( s_componets == null ) {
				s_componets = new Dictionary<int, ComponentObjects>();
			}
			s_componets.TryGetValue( go.GetInstanceID(), out s_current );

			if( s_current == null ) {
				s_current = new ComponentObjects {
					components = go.GetComponents( typeof( Component ) ),
					particleSystemRenderer = go.GetComponent<ParticleSystemRenderer>(),
					reflectionProbe = go.GetComponent<ReflectionProbe>(),
					meshRenderer = go.GetComponent<MeshRenderer>(),
					skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>(),
					spriteRenderer = go.GetComponent<SpriteRenderer>(),
					light = go.GetComponent<Light>(),
					image = go.GetComponent<Image>(),
					rawImage = go.GetComponent<RawImage>(),
					textmeshpro = new TMProTMP_Text( go.GetComponent( UnityTypes.TMPro_TMP_Text ) ),
					prefabOverride = UnityEditorPrefabUtility.GetPrefabInstanceStatus( go ) == PrefabInstanceStatus.Connected ? new UnityEditorPrefabOverridesWindow( go ) : null,

				};
				foreach( var c in s_current.components ) {
					if( c == null ) continue;
					if( E.HasInspecClass( c.GetType() ) ) {
						s_current.inspec = c;
						break;
					}
				}
				s_componets.Add( go.GetInstanceID(), s_current );
			}

			var rc = selectionRect;

			var pos = HEditorStyles.treeViewLine.CalcSize( EditorHelper.TempContent( go.name ) );
			rc.x += pos.x + 20;
			rc.x += 4;

			rc.width = 16;

			if( s_current.inspec != null ) _ComponentButton( ref rc , s_current.inspec );
			OnLight( ref rc );
			OnReflectionProbe( ref rc );
			OnParticleSystem( ref rc );
			OnMeshRenderer( ref rc );
			OnSkinnedMeshRenderer( ref rc );
			OnTextMeshPro( ref rc );
			OnImage( ref rc );
			OnRawImage( ref rc );
			OnSpriteRenderer( ref rc );
		}




		static void _ComponentButton( ref Rect rect, UnityObject obj, UnityObject icont = null ) {
			var icon = (Texture2D) obj.ObjectContent().image;
			if( obj.GetType() == typeof( SpriteRenderer ) ) {
				icon = ((SpriteRenderer) obj ).sprite.GetCachedIcon();
			}
			else if( obj.GetType() == typeof( Image ) ) {
				icon = ( (Image) obj ).sprite.GetCachedIcon();
			}
			else if( obj.GetType() == typeof( RawImage ) ) {
				icon = AssetPreview.GetAssetPreview( ( (RawImage) obj ).texture );
			}
			else if( icon == null ) {
				icon = obj.GetType().GetIcon();
			}
			//if( icont != null ) {
			//	if( icont.GetType() == typeof( Sprite ) ) {
			//		icon = icont.GetCachedIcon();
			//	}
			//	else {
			//		icon = AssetPreview.GetAssetPreview( icont );
			//	}
			//}


			if( HEditorGUI.IconButton( rect, icon ) ) {
				//ComponentPopupWindow.Open( s_current.image, ( b ) => { } );
				ComponentEditor.Open( obj );
				Event.current.Use();
			}
			rect.x += 20;
		}


		static void _Graphic( ref Rect rect, Graphic obj ) {
			if( Helper.IsNull( obj ) ) return;

			rect.width = 32;
			//if( EditorHelper.HasMouseClick( rect ) ) {
			//	//EditorWindow.GetWindow( UnityTypes.UnityEditor_ColorPicker );
			//	//var win=(EditorWindow) UnityEditorColorPicker.instance;
			//	//win.Show();

			//	UnityEditorColorPicker.Show( UnityEditorGUIView.current, obj.color, true, true );
			//}
			ScopeChange.Begin();
			var _col = UnityEditorEditorGUI.DoColorField( rect, obj.GetInstanceID(), obj.color, true, true, false );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					obj.color = ColorUtils.RGBA( _col, obj.color );
				} );
			}

			rect.x += rect.width + 4;
			rect.width = 40;
			int controlID = GUIUtility.GetControlID( "HEditorSliderKnob".GetHashCode(), FocusType.Passive, rect );
			ScopeChange.Begin();
			var _f = GUI.Slider( rect, obj.color.a, 0, 0, 1, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, true, controlID );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					obj.color = ColorUtils.RGBA( obj.color, _f );
				} );
			}

			rect.x += 40 + 4;
			rect.width = 16;
			ScopeChange.Begin();
			var _b = EditorGUI.Toggle( rect, obj.raycastTarget );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( obj, () => {
					obj.raycastTarget = _b;
				} );
			}
		}


		static void _ObjectLinkText( ref Rect rect, UnityObject obj ) {
			if( Helper.IsNull( obj ) ) return;
			var nam = obj.name;
			rect.width = HEditorStyles.treeViewLine.CalcSize( EditorHelper.TempContent( nam ) ).x;
			if( HEditorGUI.FlatButton( rect, nam, HEditorStyles.treeViewLine ) ) {
				EditorHelper.PingObject( obj );
			}
			rect.x += ( rect.width + 4 );
		}


		static void _ObjectField<T>( ref Rect rect, T obj, Action<T> changed ) where T : UnityObject {
			rect.width = 33;
			ScopeChange.Begin();
			var id = go.GetInstanceID();
			var t = obj != null ? obj.GetType() : typeof( T );
			var _obj = UnityEditorEditorGUI.DoObjectField( rect, rect, id, obj, t, null, null, false, objectField );
			if( ScopeChange.End() ) {
				EditorHelper.Dirty( go, () => {
					changed( (T) _obj );
				} );
			}
			rect.x += rect.width + 4;
		}


		static void _ColorField( ref Rect rect, UnityObject obj, Color color, Action<Color> changed ) {
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


		static void _AlphaSlider( ref Rect rect, UnityObject obj, Color color, Action<Color> changed ) {
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


		static void OnLight( ref Rect rect ) {
			if( s_current.light == null ) return;
			rect.width = 16;

			var target = s_current.light;

			//_ComponentButton( ref rect, target );

			_ColorField( ref rect, target, target.color, ( color ) => {
				target.color = color;
			} );
			//_ObjectField( ref rect, target.sprite, ( spr ) => {
			//	s_current.spriteRenderer.sprite = spr;
			//} );
			//_ColorField( ref rect, target, target.color, ( color ) => {
			//	s_current.spriteRenderer.color = color;
			//} );
			//_AlphaSlider( ref rect, target, target.color, ( color ) => {
			//	s_current.spriteRenderer.color = color;
			//} );
		}


		static void OnSpriteRenderer( ref Rect rect ) {
			if( s_current.spriteRenderer == null ) return;
			rect.width = 16;

			var target = s_current.spriteRenderer;

			//_ComponentButton( ref rect, target, target.sprite );

			_ObjectField( ref rect, target.sprite, ( spr ) => {
				target.sprite = spr;
			} );
			_ColorField( ref rect, target, target.color, ( color ) => {
				target.color = color;
			} );
			_AlphaSlider( ref rect, target, target.color, ( color ) => {
				target.color = color;
			} );
		}



		static void OnTextMeshPro( ref Rect rect ) {
			if( s_current.textmeshpro.m_instance == null ) return;
			rect.width = 16;

			//_ComponentButton( ref rect, (Component) s_current.textmeshpro.m_instance );

			_ObjectField( ref rect, (UnityObject) s_current.textmeshpro.font, ( font ) => {
				s_current.textmeshpro.m_instance.SetProperty<UnityObject>( "font", font );
				Debug.Log( "set" );
			} );
			//_ObjectLinkText( ref rect, s_current.textmeshpro.fon );
			_Graphic( ref rect, (Graphic) s_current.textmeshpro.m_instance );

			//var a=new TMProTMP_Text( s_current.textmeshpro );
			//Debug.Log( a.font.name );
		}



		static void OnImage( ref Rect rect ) {
			if( s_current.image == null ) return;
			rect.width = 16;

			//_ComponentButton( ref rect, s_current.image, s_current.image.sprite );

			_ObjectField( ref rect, s_current.image.sprite, ( spr ) => {
				s_current.image.sprite = spr;
			} );

			_Graphic( ref rect, s_current.image );
		}



		static void OnRawImage( ref Rect rect ) {
			if( s_current.rawImage == null ) return;
			rect.width = 16;

			var target = s_current.rawImage;

			//_ComponentButton( ref rect, target, target.texture );

			_ObjectField( ref rect, target.texture, ( texture ) => {
				target.texture = texture;
			} );

			_Graphic( ref rect, target );
		}



		static void OnReflectionProbe( ref Rect rc ) {
			if( s_current.reflectionProbe == null ) return;

			rc.width = 16;

			GUI.DrawTexture( rc, EditorIcon.icons_processed_unityengine_reflectionprobe_icon_asset );
			if( EditorHelper.HasMouseClick( rc ) ) {
				//ComponentPopupWindow.Open( comp, ( b ) => { } );
				//Event.current.Use();
			}
			rc.x += rc.width + 8;
			rc.width = 40;
			if( GUI.Button( rc, EditorHelper.TempContent( "Bake", EditorIcon.icons_processed_unityengine_reflectionprobe_icon_asset ) ) ) {
				UnityEditorLightmapping.BakeReflectionProbeSnapshot( s_current.reflectionProbe );
			}

		}


		static void OnRenderer( ref Rect rect, Renderer renderer ) {
			if( renderer.sharedMaterial == null ) return;

			var target = renderer;

			var icon = EditorIcon.icons_processed_unityengine_material_icon_asset;
			if( target.sharedMaterial.HasProperty( "_MainTex" ) ) {
				icon = AssetPreview.GetAssetPreview( target.sharedMaterial.mainTexture );
			}
			else {
				icon = AssetPreview.GetAssetPreview( target.sharedMaterial );
			}
			if( HEditorGUI.IconButton( rect, icon ) ) {
				//s_currentComponents.particleSystem.main
				MaterialEditorWindow.Open( target.sharedMaterial );
			}
			rect.x += 16;

			_ObjectField( ref rect, target.sharedMaterial, ( material ) => {
				target.sharedMaterial = material;
			} );
		}


		static void OnParticleSystem( ref Rect rc ) {
			if( s_current.particleSystemRenderer == null ) return;
			rc.width = 16;

			OnRenderer( ref rc, s_current.particleSystemRenderer );

			if( s_current.particleSystemRenderer.trailMaterial != null ) {
				if( HEditorGUI.IconButton( rc, EditorIcon.icons_processed_unityengine_material_icon_asset ) ) {
					//s_currentComponents.particleSystem.main
					MaterialEditorWindow.Open( s_current.particleSystemRenderer.trailMaterial );
				}
				rc.x += 20;
			}
		}


		static void OnMeshRenderer( ref Rect rc ) {
			if( s_current.meshRenderer == null ) return;
			rc.width = 16;

			OnRenderer( ref rc, s_current.meshRenderer );
		}



		static void OnSkinnedMeshRenderer( ref Rect rc ) {
			if( s_current.skinnedMeshRenderer == null ) return;
			rc.width = 16;

			OnRenderer( ref rc, s_current.skinnedMeshRenderer );
		}

	}
}
