#pragma warning disable 618

using Hananoki.Extensions;
using Hananoki.Reflection;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

using E = Hananoki.CustomHierarchy.SettingsEditor;
using SS = Hananoki.SharedModule.S;
using UnityObject = UnityEngine.Object;

namespace Hananoki.CustomHierarchy {
	[InitializeOnLoad]
	public static class CustomHierarchy {

		const int WIDTH = 16;

		public class Styles {
			public Texture2D PrefabNormal => Icon.Get( "$PrefabNormal" );
			public Texture2D PrefabModel => Icon.Get( "$PrefabModel" );
			public Texture2D MissingPrefabInstance => Icon.Get( "$MissingPrefabInstance" );
			public Texture2D DisconnectedPrefab => Icon.Get( "$DisconnectedPrefab" );
			public Texture2D DisconnectedModelPrefab => Icon.Get( "$DisconnectedModelPrefab" );
			public Texture2D TreeLine => Icon.Get( "CH_I" );
			public Texture2D TreeLineB => Icon.Get( "CH_T" );
			public GUIStyle ControlLabel;
			public Color lineColor;

			public Styles() {
				ControlLabel = new GUIStyle( "ControlLabel" );
			}
		}

		public static Styles s_styles;


		static CustomHierarchy() {
			E.Load();
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
			//EditorSceneManager.sceneOpened += ( scene, mode ) => { Debug.Log( "sceneOpened" ); };
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="instanceID"></param>
		/// <param name="selectionRect"></param>
		static void HierarchyWindowItemCallback( int instanceID, Rect selectionRect ) {

			if( !E.i.Enable ) return;

			if( s_styles == null ) {
				s_styles = new Styles();
				s_styles.lineColor = E.i.lineColor;

				if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
					object wnd = EditorUtils.SceneHierarchyWindow();
					var _sceneHierarchy = wnd.GetProperty<object>( "sceneHierarchy" );
					var _treeView = _sceneHierarchy.GetProperty<object>( "treeView" );
					var _gui = _treeView.GetProperty<object>( "gui" );
					//var _selectionStyle = _gui.GetProperty<object>( "selectionStyle" );

					_gui.SetProperty( "selectionStyle", new GUIStyle( "FrameBox" ) );
				}
			}

			if( UnitySymbol.Has( "UNITY_2019_1" ) ) {
				selectionRect.x += 24;
				selectionRect.width -= 24;
			}

			var go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;
			if( go == null ) {
				var rr = selectionRect;
				if( E.i.SceneIconClickPing ) {
					selectionRect.width = 16;
					if( EditorHelper.HasMouseClick( selectionRect ) ) {
						var scene = (Scene) R.Method( "GetSceneByHandle", typeof( EditorSceneManager ) ).Invoke( null, new object[] { instanceID } );
						EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath( scene.path, typeof( SceneAsset ) ) );
					}
				}
#if LOCAL_TEST
				rr.x += 100;
				if( GUI.Button( rr, "Show Hide Objects", EditorStyles.miniLabel ) ) {
					//Debug.Log( "Hide" );
					foreach( var g in EditorHelper.GetSceneObjects<GameObject>() ) {
						//Debug.Log( $"{g.name}: {g.hideFlags.ToString()}" );
						g.hideFlags = HideFlags.None;
					}
				}
#endif
				return;
			}

			if( E.i.enableLineColor ) {
				DrawBackColor( selectionRect, 0x01 );
			}
			//if( go.layer != LayerMask.NameToLayer( "Category" ) ) {
			//	EditorGUI.DrawRect( selectionRect , Color.cyan);
			//}


			var rcL = selectionRect;
			var rc = selectionRect;
			var max = rc.xMax;

			rcL.x = 0;


			rc.x = max;
			//#if LOCAL_DEBUG
			rc.x -= E.i.offsetPosX;
			//#endif
			if( E.i.activeToggle ) {
				rc.x = rc.x - WIDTH;
				DrawActiveButtonGUI( go, rc );
			}
			if( E.i.prefabStatus ) {

				if( DrawPrefabIconGUI( rc, go ) ) {
					rc.x = rc.x - WIDTH;
				}
			}

			if( E.i.showLayerAndTag ) {
				var nolayer = false;

				if( go.layer == 0 ) {
					nolayer = true;
				}
				if( !nolayer ) {
					var lay = InternalEditorUtility.GetLayerName( go.layer );
					var cont2 = lay.content();
					var sz2 = EditorStyles.miniBoldLabel.CalcSize( cont2 );
					rc.x = rc.x - sz2.x;

					GUI.Label( rc, cont2, EditorStyles.miniBoldLabel );
				}

				var notag = false;
				if( go.tag.Contains( "Untagged" ) ) {
					notag = true;
				}
				if( !nolayer && !notag ) {
					var cont3 = "|".content();
					var sz3 = EditorStyles.miniLabel.CalcSize( cont3 );
					rc.x = rc.x - sz3.x;
					GUI.Label( rc, cont3, EditorStyles.miniLabel );
				}
				var cont = go.tag.content();
				var sz = EditorStyles.miniLabel.CalcSize( cont );
				rc.x = rc.x - sz.x;
				if( !notag ) {
					GUI.Label( rc, go.tag, EditorStyles.miniLabel );
				}
			}


			if( E.i.IconClickContext ) {
				var r = selectionRect;
				//r.x += 3;
				r.width = 16;
				//EditorGUI.DrawRect( r, new Color( 0, 0, 1, 0.5f ) );
				if( EditorHelper.HasMouseClick( r ) ) {
					var m = new GenericMenu();
					m.AddItem( SS._OpenInNewInspector, false, _uobj => EditorHelper.ShowNewInspector( _uobj.ToCast<UnityObject>() ), go );
					m.AddItem( S._Moveuponelevel, false, _uobj => {
						var gobj = _uobj as GameObject;
						Undo.SetTransformParent( gobj.transform, gobj.transform.parent.parent, "" );
						EditorUtility.SetDirty( gobj );
					}, go );
#if LOCAL_TEST
					m.AddItem( "Hide", false, _uobj => {
						var gobj = _uobj as GameObject;
						gobj.hideFlags |= HideFlags.HideInHierarchy;
						EditorUtility.SetDirty( gobj );
						EditorApplication.RepaintHierarchyWindow();
					}, go );
#endif
					m.DropDown( r );
					Event.current.Use();
				}
			}

			if( E.i.enableTreeImg ) {
				var ra = selectionRect;
				ra.width = 16;
				ra.x -= 16;
				bool check( Transform trs ) {
					if( trs.childCount == 0 ) return true;

					int ii = go.transform.childCount;
					for( int i = 0; i < go.transform.childCount; i++ ) {
						if( 0 != ( trs.GetChild( i ).hideFlags & HideFlags.HideInHierarchy ) ) {
							ii--;
						}
					}
					return ii == 0 ? true : false;
				}
				if( check( go.transform ) ) {
					GUI.DrawTexture( ra, s_styles.TreeLineB );
				}
				//for( int i = 0; i < go.transform.childCount; i++ ) {
				//	go.transform.GetChild( i ).hideFlags = HideFlags.None;
				//	Debug.Log( go.transform.GetChild( i ).name );
				//}
				var tras = go.transform;
				while( tras != null ) {
					ra.x -= 14;
					if( tras.parent != null ) {
						GUI.DrawTexture( ra, s_styles.TreeLine );
					}
					tras = tras.parent;
				}
				//EditorGUI.DrawRect( selectionRect ,new Color(0,0,1,0.25f));
			}

#if false
			if( PreferenceSettings.i.EnableHierarchyItem ) {
				rc.x = rc.x - WIDTH;
				DrawAnimationMonitor( rc, go );
			}
#endif

			//if( _RinkakuChange ) {
			//	pos.x = pos.x - WIDTH;
			//	pos.width = WIDTH;
			//	if( GUI.Button( pos, EditorGUIUtility.FindTexture( "UnityEditor.SceneHierarchyWindow" ), (GUIStyle) "ControlLabel" ) ) {
			//		Event evt = Event.current;
			//		Vector2 mousePos = evt.mousePosition;
			//		Rect contextRect = new Rect( 0, 0, Screen.width, Screen.height );
			//		if( contextRect.Contains( mousePos ) ) {
			//			// Now create the menu, add items and show it
			//			GenericMenu menu = new GenericMenu();

			//			menu.AddItem( new GUIContent( "Hidden" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Hidden ); }, go );
			//			menu.AddItem( new GUIContent( "Wireframe" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Wireframe ); }, go );
			//			menu.AddItem( new GUIContent( "Highlight" ), false, ( obj ) => { setSelectedRenderState( (GameObject) obj, EditorSelectedRenderState.Highlight ); }, go );
			//			menu.ShowAsContext();
			//			evt.Use();
			//		}
			//	}
			//}



		}



		static void DrawBackColor( Rect selectionRect, int mask ) {
			//if( _SimaSima == false ) return;

			var index = ( (int) selectionRect.y ) >> 4;

			if( ( index & 0x01 ) == mask ) {
				return;
			}

			var pos = selectionRect;
			pos.x = 0;
			pos.xMax = selectionRect.xMax;

			EditorGUI.DrawRect( pos, s_styles.lineColor );
		}



		/// <summary>
		/// アクティブボタン
		/// </summary>
		/// <param name="go"></param>
		/// <param name="rc"></param>
		static void DrawActiveButtonGUI( GameObject go, Rect rc ) {
			rc.width = WIDTH;
			var _b1 = go.activeSelf;
			var _b2 = GUI.Toggle( rc, _b1, string.Empty );
			if( _b1 != _b2 ) {
				if( 1 < Selection.gameObjects.Length ) {
					Undo.RecordObjects( Selection.gameObjects, "Selection.gameObjects" );
					foreach( var goo in Selection.gameObjects ) {
						goo.SetActive( _b2 );
					}
				}
				else {
					Undo.RecordObject( go, "Selection.gameObjects" );
					go.SetActive( _b2 );
				}
				//Debug.LogFormat( "Selection.gameObjects {0}", Selection.gameObjects.Length );
			}
		}


		static bool DrawPrefabIconGUI( Rect rc, GameObject go ) {

			bool draw = true;
			rc.x = rc.x - WIDTH;
			rc.width = WIDTH;

			//	NotAPrefab = 0,
			//Connected = 1,
			//Disconnected = 2,
			//MissingAsset = 3

			//public enum PrefabAssetType {
			//NotAPrefab = 0,
			//Regular = 1,
			//Model = 2,
			//Variant = 3,
			//MissingAsset = 4
			if( UnitySymbol.Has( "UNITY_2018_3_OR_NEWER" ) ) {
				var t = typeof( PrefabUtility );
				var status = t.MethodInvoke<int>( "GetPrefabInstanceStatus", new object[] { go } );
				var type = t.MethodInvoke<int>( "GetPrefabAssetType", new object[] { go } );
				//var status = PrefabUtility.GetPrefabInstanceStatus( go );
				//var type = PrefabUtility.GetPrefabAssetType( go );
				switch( status ) {
					case 0:// PrefabInstanceStatus.NotAPrefab:
						break;
					case 1:// PrefabInstanceStatus.Connected:
						if( EditorHelper.HasMouseClick( rc, EventMouseButton.R ) ) {
							var wnd = new UnityPrefabOverridesWindow( go );

							var m = new GenericMenu();
							if( !wnd.IsShowingActionButton() ) {
								m.AddDisabledItem( "Apply All" );
								m.AddDisabledItem( "Revert All" );
							}
							else {
								//m.AddItem( "Apply All", ( context ) => {
								//	(var a, var b) = (System.ValueTuple<UnityPrefabOverridesWindow, GameObject>) context;
								//	a.ApplyAll();
								//}, (wnd, go) );
								//m.AddItem( "Revert All", ( context ) => {
								//	(var a, var b) = (System.ValueTuple<UnityPrefabOverridesWindow, GameObject>) context;
								//	a.RevertAll();
								//}, (wnd, go) );
							}
							m.DropDown();
							Event.current.Use();
						}
						var ico = type == 2 ? s_styles.PrefabModel : s_styles.PrefabNormal;
						if( HEditorGUI.IconButton( rc, ico ) ) {
							//var aa = PrefabUtility.GetCorrespondingObjectFromSource( go );
							var aa = t.MethodInvoke<GameObject>( "GetCorrespondingObjectFromSource", new object[] { go } );
							if( aa != go ) {
								EditorHelper.PingObject( aa );
							}
						}
						break;
					case 2:// PrefabInstanceStatus.Disconnected:
						if( GUI.Button( rc, s_styles.DisconnectedPrefab, s_styles.ControlLabel ) ) {
						}
						break;
					case 3:// PrefabInstanceStatus.MissingAsset:
						if( GUI.Button( rc, s_styles.MissingPrefabInstance, s_styles.ControlLabel ) ) {
						}
						break;
				}
			}
			else {
				var type = PrefabUtility.GetPrefabType( go );
				if( type == PrefabType.PrefabInstance ) {
					if( GUI.Button( rc, s_styles.PrefabNormal, s_styles.ControlLabel ) ) {
						//Debug.Log( "PrefabType.PrefabInstance" );
						//var aa = PrefabUtility.GetPrefabParent( go );
						//var bb = AssetDatabase.GetAssetPath( aa );
						//Debug.Log( bb );
						//PrefabHelper.SavePrefab2( go, bb );
						EditorHelper.PingObject( go );
					}
					//GUI.DrawTexture( pos, EditorGUIUtility.FindTexture( "PrefabNormal Icon" ), ScaleMode.ScaleToFit, true );
				}
				else if( type == PrefabType.ModelPrefabInstance ) {
					if( GUI.Button( rc, s_styles.PrefabModel, s_styles.ControlLabel ) ) {
						//Debug.Log( "PrefabType.ModelPrefabInstance" );
					}
				}
				else if( type == PrefabType.DisconnectedPrefabInstance ) {
					if( GUI.Button( rc, s_styles.DisconnectedPrefab, s_styles.ControlLabel ) ) {
						//Debug.Log( "PrefabType.DisconnectedPrefabInstance" );
					}
				}
				else if( type == PrefabType.DisconnectedModelPrefabInstance ) {
					if( GUI.Button( rc, s_styles.DisconnectedModelPrefab, s_styles.ControlLabel ) ) {
						//Debug.Log( "PrefabType.DisconnectedModelPrefabInstance" );
					}
				}
				else if( type == PrefabType.MissingPrefabInstance ) {
					if( GUI.Button( rc, s_styles.MissingPrefabInstance, s_styles.ControlLabel ) ) {
						//Debug.Log( "PrefabType.MissingPrefabInstance" );
					}
				}
				else {
					draw = false;
				}
			}
			return draw;
		}



#if false
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rc"></param>
		/// <param name="go"></param>
		static void DrawAnimationMonitor( Rect rc, GameObject go ) {
			if( PreferenceSettings.i.HierarchyAnim == false ) return;

			var anim = go.GetComponent<Animator>();

			if( anim == null ) return;

			if( Monitor4AnimationCurve._monitorTransform == go.transform ) {
				GUI.Label( rc, EditorGUIUtility.IconContent( "lightMeter/redLight" ) );
				//GUI.Toggle( rcL, true, "", (GUIStyle) "Radio" );
			}
			else {
				GUI.Label( rc, EditorGUIUtility.IconContent( "lightMeter/lightRim" ) );
				//GUI.Toggle( rcL, false, "", (GUIStyle) "Radio" );
			}
			if( Event.current.type == EventType.MouseDown ) {
				if( rc.Contains( Event.current.mousePosition ) ) {
					Event.current.Use();
					if( Monitor4AnimationCurve._monitorTransform == go.transform ) {
						//var menu = new GenericMenu();
						//menu.AddItem( new GUIContent( "Set Monitor" ), true, () => {
						//Monitor4AnimationCurve.SetEnable( go.transform );
						Monitor4AnimationCurve.Disable();
						//Debug.Log( "Set Monitor" );

						//} );
						//menu.ShowAsContext();
					}
					else {
						//var menu = new GenericMenu();
						//menu.AddItem( new GUIContent( "Set Monitor" ), false, () => {
						//Monitor4AnimationCurve.SetEnable( go.transform );
						Monitor4AnimationCurve.SetEnable( go.transform );
						Debug.Log( "Set Monitor" );

						//} );
						//menu.ShowAsContext();
					}

				}
			}

		} // DrawAnimationMonitor
#endif
	}



	public static class Icon {
		static Dictionary<string, Texture2D> icons;
		public static Texture2D Get( string s ) {
			if( icons == null ) {
				icons = new Dictionary<string, Texture2D>();
			}
			bool load = false;
			if( !icons.ContainsKey( s ) ) load = true;
			else if( icons[ s ] == null ) load = true;
			if( load ) {
				for( int i = 0; i < SharedEmbed.num; i++ ) {
					if( SharedEmbed.n[ i ] != s ) continue;
					var bb = B64.Decode( "iVBORw0KGgoAAAAN" + SharedEmbed.i[ i ] );
					var t = new Texture2D( SharedEmbed.x[ i ], SharedEmbed.y[ i ] );
					t.LoadImage( bb );
					t.hideFlags |= HideFlags.DontUnloadUnusedAsset;
					if( icons.ContainsKey( s ) ) {
						icons[ s ] = t;
					}
					else {
						icons.Add( SharedEmbed.n[ i ], t );
					}
				}
			}
			return icons[ s ];
		}
	}
}
