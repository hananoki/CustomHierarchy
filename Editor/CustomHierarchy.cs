#pragma warning disable 618

using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityReflection;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using UnityEditorSceneManagementEditorSceneManager = UnityReflection.UnityEditorSceneManagementEditorSceneManager;


namespace HananokiEditor.CustomHierarchy {


	[InitializeOnLoad]
	public static partial class CustomHierarchy {

		const int WIDTH = 16;

		static bool init;
		internal static GameObject go;

		internal static bool initSelection;

		/////////////////////////////////////////
		static CustomHierarchy() {
			E.Load();
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
			EditorApplication.hierarchyChanged += OnHierarchyChanged;
			EditorSceneManager.sceneOpened += ( scene, mode ) => {
				ComponentHandler.Reset();
			};
			ComponentHandler.Reset();
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}



		/////////////////////////////////////////
		static void OnHierarchyChanged() {
			DragAndDropEx.OnHierarchyChanged();
			ComponentHandler.Reset();
		}



		/////////////////////////////////////////
		static void OnPlayModeStateChanged( PlayModeStateChange playModeStateChange ) {
			switch( playModeStateChange ) {
			case PlayModeStateChange.EnteredPlayMode:
			case PlayModeStateChange.EnteredEditMode:
				ComponentHandler.Reset();
				break;
			}
		}


		/////////////////////////////////////////
		static void ヒエラルキークリックでインスペクターFocus() {
			if( !AssetDatabase.GetAssetOrScenePath( Selection.activeObject ).Contains( ".unity" ) ) return;

			EditorWindowUtils.FocusInspector();
		}


		/////////////////////////////////////////
		static void HierarchyWindowItemCallback( int instanceID, Rect selectionRect ) {
			//Debug.Log( selectionRect );
			if( !E.i.Enable ) return;

			Styles.Init();

			if( !initSelection ) {
				Selection.selectionChanged -= ヒエラルキークリックでインスペクターFocus;
				if( E.i.ヒエラルキークリックでインスペクターFocus ) Selection.selectionChanged += ヒエラルキークリックでインスペクターFocus;
				initSelection = true;
			}

			if( E.i.extendedDragAndDrop ) DragAndDropEx.Execute( selectionRect );

			if( E.i.dockPaneBar ) DockPaneBar.Setup();
			if( E.i.commandBar ) CommandBar.Setup();

			if( !init ) {
				init = true;
#if LOCAL_TEST
				if( UnitySymbol.Has( "UNITY_2019_3_OR_NEWER" ) ) {
					object wnd = EditorWindowUtils.Find( UnityTypes.UnityEditor_SceneHierarchyWindow );
					var _sceneHierarchy = wnd.GetProperty<object>( "sceneHierarchy" );
					var _treeView = _sceneHierarchy.GetProperty<object>( "treeView" );
					var _gui = _treeView.GetProperty<object>( "gui" );
					//var _selectionStyle = _gui.GetProperty<object>( "selectionStyle" );

					_gui.SetProperty( "selectionStyle", new GUIStyle( "AvatarMappingBox" ) );
				}
#endif
			}



			if( UnitySymbol.Has( "UNITY_2019_1" ) ) {
				selectionRect.x += 24;
				selectionRect.width -= 24;
			}

			go = EditorUtility.InstanceIDToObject( instanceID ) as GameObject;

			if( go == null ) {
				if( E.i.sceneIconClickPing ) {
					var rr = selectionRect;
					if( E.i.sceneIconClickPing ) {
						selectionRect.width = 16;
						if( EditorHelper.HasMouseClick( selectionRect ) ) {
							var scene = UnityEditorSceneManagementEditorSceneManager.GetSceneByHandle( instanceID );
							EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath( scene.path, typeof( SceneAsset ) ) );
						}
					}
				}
				return;
			}


			if( E.i.enableLineColor ) {
				DrawBackColor( selectionRect, 0x01 );
			}


			var rcL = selectionRect;
			var rc = selectionRect;
			var max = rc.xMax;

			rcL.x = 0;


			rc.x = max;
			rc.x -= E.i.offsetPosX;


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
					rc.x = rc.x - cont3.CalcWidth_miniLabel();
					HEditorGUI.MiniLabel( rc, cont3 );
				}


				if( !notag ) {
					var cont = EditorHelper.TempContent( go.tag );
					rc.x = rc.x - cont.CalcWidth_miniLabel();
					HEditorGUI.MiniLabel( rc, cont );
				}
			}

			if( E.i.componentHandler ) ComponentHandler.Execute( selectionRect );

			if( E.i.iconClickContext ) IconClickContext.Execute( selectionRect );

			if( E.i.enableTreeImg && SceneHierarchyUtils.searchFilter.IsEmpty() ) {
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
					GUI.DrawTexture( ra, Styles.treeLineB );
				}
				//for( int i = 0; i < go.transform.childCount; i++ ) {
				//	go.transform.GetChild( i ).hideFlags = HideFlags.None;
				//	Debug.Log( go.transform.GetChild( i ).name );
				//}
				var tras = go.transform;
				while( tras != null ) {
					ra.x -= 14;
					if( tras.parent != null ) {
						GUI.DrawTexture( ra, Styles.treeLine );
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
			if( E.i.numpadCtrl && !EditorGUIUtility.editingTextField ) {
				if( Event.current.type == EventType.KeyUp ) {
					if( Event.current.keyCode == KeyCode.Keypad8 ) {
						if( 1 <= Selection.activeGameObject.transform.GetSiblingIndex() ) {
							Selection.activeGameObject.transform.SetSiblingIndex( Selection.activeGameObject.transform.GetSiblingIndex() - 1 );
							Event.current.Use();
						}
					}
					if( Event.current.keyCode == KeyCode.Keypad2 ) {
						Selection.activeGameObject.transform.SetSiblingIndex( Selection.activeGameObject.transform.GetSiblingIndex() + 1 );
						Event.current.Use();
					}
					if( Event.current.keyCode == KeyCode.Keypad4 ) {
						Undo.SetTransformParent( Selection.activeGameObject.transform, Selection.activeGameObject.transform.parent.parent, "" );
						EditorUtility.SetDirty( Selection.activeGameObject );
						Event.current.Use();
					}
					if( Event.current.keyCode == KeyCode.Keypad5 ) {
						//Undo.SetTransformParent( Selection.activeGameObject.transform, Selection.activeGameObject.transform.parent.parent, "" );
						//EditorUtility.SetDirty( Selection.activeGameObject );
						Selection.activeGameObject.SetActive( !Selection.activeGameObject.activeSelf );
						Event.current.Use();
					}
#if LOCAL_TEST
					if( Event.current.keyCode == KeyCode.KeypadDivide ) {
						//Undo.SetTransformParent( Selection.activeGameObject.transform, Selection.activeGameObject.transform.parent.parent, "" );
						//EditorUtility.SetDirty( Selection.activeGameObject );
						Selection.activeGameObject.hideFlags |= HideFlags.HideInHierarchy;
						EditorUtility.SetDirty( Selection.activeGameObject );
						if( Selection.activeGameObject.transform.parent != null ) {
							Selection.activeGameObject = Selection.activeGameObject.transform.parent.gameObject;
						}
						EditorApplication.RepaintHierarchyWindow();
						Event.current.Use();
					}
#endif
				}
			}
			//P4_DeletedLocal
			if( E.i.removeGameObject && Selection.activeGameObject == go ) {
				var stage = PrefabStageUtility.GetCurrentPrefabStage();
				if( stage == null ) {

					var re = selectionRect;
					re.x = 16 + 16;
					re.width = 16;
					if( HEditorGUI.IconButton( re, EditorIcon.p4_deletedlocal ) ) {
						Undo.DestroyObjectImmediate( go );
					}
				}
			}
		}



		/////////////////////////////////////////
		static void DrawBackColor( Rect selectionRect, int mask ) {
			//if( _SimaSima == false ) return;

			var index = ( (int) selectionRect.y ) >> 4;

			if( ( index & 0x01 ) == mask ) {
				return;
			}

			var pos = selectionRect;
			pos.x = 0;
			pos.xMax = selectionRect.xMax;

			EditorGUI.DrawRect( pos, E.i.lineColor );
		}



		/////////////////////////////////////////
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



		/////////////////////////////////////////
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
				var status = UnityEditorPrefabUtility.GetPrefabInstanceStatus( go );
				var type = UnityEditorPrefabUtility.GetPrefabAssetType( go );
				//var status =  t.MethodInvoke<int>( "GetPrefabInstanceStatus", new object[] { go } );
				//var type = t.MethodInvoke<int>( "GetPrefabAssetType", new object[] { go } );

				switch( status ) {
				case PrefabInstanceStatus.NotAPrefab:// PrefabInstanceStatus.NotAPrefab:
					break;
				case PrefabInstanceStatus.Connected:
					var ico = type == PrefabAssetType.Model ? Styles.prefabModel : Styles.prefabNormal;
					if( HEditorGUI.IconButton( rc, ico ) ) {
						//var aa = PrefabUtility.GetCorrespondingObjectFromSource( go );
						//t.GetMethod( "GetCorrespondingObjectFromSource" );
						//method.MakeGenericMethod( typeof( string ) );

						var aa = (GameObject) UnityEditorPrefabUtility.GetCorrespondingObjectFromSource_internal( go );// t.MethodInvoke<GameObject>( typeof( GameObject ), "GetCorrespondingObjectFromSource", new object[] { go } );
						if( aa != go ) {
							EditorHelper.PingObject( aa );
						}
					}
					break;
				case PrefabInstanceStatus.Disconnected:
					if( GUI.Button( rc, Styles.disconnectedPrefab, Styles.controlLabel ) ) {
					}
					break;
				case PrefabInstanceStatus.MissingAsset:
					if( GUI.Button( rc, Styles.missingPrefabInstance, Styles.controlLabel ) ) {
					}
					break;
				}
			}
			else {
				var type = PrefabUtility.GetPrefabType( go );
				if( type == PrefabType.PrefabInstance ) {
					if( GUI.Button( rc, Styles.prefabNormal, Styles.controlLabel ) ) {
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
					if( GUI.Button( rc, Styles.prefabModel, Styles.controlLabel ) ) {
						//Debug.Log( "PrefabType.ModelPrefabInstance" );
					}
				}
				else if( type == PrefabType.DisconnectedPrefabInstance ) {
					if( GUI.Button( rc, Styles.disconnectedPrefab, Styles.controlLabel ) ) {
						//Debug.Log( "PrefabType.DisconnectedPrefabInstance" );
					}
				}
				else if( type == PrefabType.DisconnectedModelPrefabInstance ) {
					if( GUI.Button( rc, Styles.disconnectedModelPrefab, Styles.controlLabel ) ) {
						//Debug.Log( "PrefabType.DisconnectedModelPrefabInstance" );
					}
				}
				else if( type == PrefabType.MissingPrefabInstance ) {
					if( GUI.Button( rc, Styles.missingPrefabInstance, Styles.controlLabel ) ) {
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
}
