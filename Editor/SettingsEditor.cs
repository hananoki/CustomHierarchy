//#define ENABLE_LEGACY_PREFERENCE

using UnityEditor;
using UnityEngine;
using Hananoki.Extensions;
using Hananoki.SharedModule;

using E = Hananoki.CustomHierarchy.SettingsEditor;
using SS = Hananoki.SharedModule.S;

namespace Hananoki.CustomHierarchy {
	[System.Serializable]
	public class SettingsEditor {
		public bool Enable = true;

		public bool activeToggle = true;
		public bool prefabStatus = true;
		public bool enableTreeImg = true;

		public bool enableLineColor = true;
		public Color lineColorPersonal = new Color( 0, 0, 0, 0.05f );
		public Color lineColorProfessional = new Color( 1, 1, 1, 0.05f );

		public bool IconClickContext;
		public bool SceneIconClickPing;
		public bool showLayerAndTag;
		//#if LOCAL_DEBUG
		public float offsetPosX;
		//#endif
		public bool toolbarOverride;

		public static E i;

		public Color lineColor {
			get {
				return EditorGUIUtility.isProSkin ? lineColorProfessional : lineColorPersonal;
			}
			set {
				if( EditorGUIUtility.isProSkin ) lineColorProfessional = value;
				else lineColorPersonal = value;
			}
		}

		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}



		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}



	public class SettingsEditorWindow : HSettingsEditorWindow {
			
		public static void Open() {
			var w = GetWindow<SettingsEditorWindow>();
			w.SetTitle( new GUIContent( "Project Settings", EditorIcon.settings ) );
			w.headerMame = Package.name;
			w.headerVersion = Package.version;
			w.gui = DrawGUI;
		}


		/// <summary>
		/// 
		/// </summary>
		public static void DrawGUI() {
			E.Load();
			EditorGUI.BeginChangeCheck();

			/* アイコン消しテスト、コンテキストメニューと競合する
			if( GUILayout.Button( "aaa" ) ) {
				object a = R.Field( "s_LastInteractedHierarchy", "UnityEditor.SceneHierarchyWindow" ).GetValue( null );
				var sceneHierarchy = a.Property<object>( "sceneHierarchy" );
				var treeView = sceneHierarchy.Property<object>( "treeView" );
				var gui = treeView.Property<object>( "gui" );

				gui.Field( "k_IconWidth", 0 );
				
				Debug.Log( a.GetType().Name );
			}
			*/


			E.i.Enable = HEditorGUILayout.ToggleLeft( SS._Enable, E.i.Enable );
			EditorGUI.indentLevel++;
			GUILayout.Space( 8f );

			bool _toolbarOverride;
			using( new EditorGUI.DisabledGroupScope( !E.i.Enable ) ) {

				E.i.enableTreeImg = HEditorGUILayout.ToggleLeft( S._Displaythetree, E.i.enableTreeImg );
				E.i.activeToggle = HEditorGUILayout.ToggleLeft( S._Showtoggletotogglegameobjectactive, E.i.activeToggle );
				E.i.prefabStatus = HEditorGUILayout.ToggleLeft( S._Displayprefabstatuswithicons, E.i.prefabStatus );
				//Settings.i.HierarchyAnim = EditorGUILayout.ToggleLeft( "Monitor for Animation Curve", Settings.i.HierarchyAnim, GUILayout.ExpandWidth( false ) );

				E.i.enableLineColor = HEditorGUILayout.ToggleLeft( SS._Changecolorforeachrow, E.i.enableLineColor );

				using( new EditorGUI.DisabledGroupScope( !E.i.enableLineColor ) ) {
					EditorGUI.indentLevel++;
					E.i.lineColor = EditorGUILayout.ColorField( SS._Rowcolor, E.i.lineColor );
					EditorGUI.indentLevel--;
				}
				//#if LOCAL_DEBUG
				E.i.offsetPosX = EditorGUILayout.FloatField( S._ItemdisplayoffsetX, E.i.offsetPosX );
				//#endif

				GUILayout.Space( 8f );
				EditorGUILayout.LabelField( $"* {SS._Experimental}", EditorStyles.boldLabel );
				E.i.IconClickContext = HEditorGUILayout.ToggleLeft( SS._ContextMenuWithIconClick, E.i.IconClickContext );
				E.i.SceneIconClickPing = HEditorGUILayout.ToggleLeft( S._Pingascenefilebyclickingthesceneicon, E.i.SceneIconClickPing );
				E.i.showLayerAndTag = HEditorGUILayout.ToggleLeft( S._Displaytagnameandlayername, E.i.showLayerAndTag );

				_toolbarOverride = HEditorGUILayout.ToggleLeft( "Toolbar Override (UNITY_2019_3_OR_NEWER)", E.i.toolbarOverride );
			}
			EditorGUI.indentLevel--;


			if( EditorGUI.EndChangeCheck() ) {
				
				if( CustomHierarchy.s_styles != null ) {
					CustomHierarchy.s_styles.lineColor = E.i.lineColor;
				}
				
				if( E.i.toolbarOverride != _toolbarOverride ) {
					CustomHierarchy._window = HEditorWindow.Find( UnityTypes.SceneHierarchyWindow );
					E.i.toolbarOverride = _toolbarOverride;
					if( E.i.toolbarOverride ) {
						CustomHierarchy._window?.AddIMGUIContainer( CustomHierarchy._IMGUIContainer, true );
					}
					else {
						CustomHierarchy._window?.RemoveIMGUIContainer( CustomHierarchy._IMGUIContainer, true );
					}
				}
				E.Save();
				EditorApplication.RepaintHierarchyWindow();
			}

			GUILayout.Space( 8f );

		}




#if !ENABLE_HANANOKI_SETTINGS
#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Package.name}", SettingsScope.User ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = () => GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel ),
			};
			return provider;
		}
		public static void PreferencesGUI( string searchText ) {
#else
		[PreferenceItem( Package.name )]
		public static void PreferencesGUI() {
#endif
			using( new LayoutScope() ) DrawGUI();
		}
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	[SettingsClass]
	public class SettingsEvent {
		[SettingsMethod]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				//mode = 1,
				displayName = Package.name,
				version = Package.version,
				gui = SettingsEditorWindow.DrawGUI,
			};
		}
	}
#endif
}
