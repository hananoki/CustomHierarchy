//#define ENABLE_LEGACY_PREFERENCE

using UnityEditor;
using UnityEngine;

using E = Hananoki.CustomHierarchy.SettingsEditor;
using SS = Hananoki.SharedModule.S;

namespace Hananoki.CustomHierarchy {
	[System.Serializable]
	public class SettingsEditor {
		public bool Enable = true;

		public bool activeToggle = true;
		public bool prefabStatus = true;
		
		public bool enableLineColor = true;
		public Color lineColorPersonal= new Color( 0, 0, 0, 0.05f );
		public Color lineColorProfessional = new Color( 1, 1, 1, 0.05f );

		public bool IconClickContext;
		public bool SceneIconClickPing;
		public bool showLayerAndTag;

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
			var window = GetWindow<SettingsEditorWindow>();
			window.SetTitle( new GUIContent( Package.name, Icon.Get( "SettingsIcon" ) ) );
		}

		void OnEnable() {
			drawGUI = DrawGUI;
			E.Load();
		}



		/// <summary>
		/// 
		/// </summary>
		static void DrawGUI() {

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

			using( new PreferenceLayoutScope() ) {
				E.i.Enable = HEditorGUILayout.ToggleLeft( SS._Enable, E.i.Enable );

				GUILayout.Space( 8f );

				using( new EditorGUI.DisabledGroupScope( !E.i.Enable ) ) {

					E.i.activeToggle = HEditorGUILayout.ToggleLeft( S._Showtoggletotogglegameobjectactive, E.i.activeToggle );
					E.i.prefabStatus = HEditorGUILayout.ToggleLeft( S._Displayprefabstatuswithicons, E.i.prefabStatus );
					//Settings.i.HierarchyAnim = EditorGUILayout.ToggleLeft( "Monitor for Animation Curve", Settings.i.HierarchyAnim, GUILayout.ExpandWidth( false ) );

					E.i.enableLineColor = HEditorGUILayout.ToggleLeft( SS._Changecolorforeachrow, E.i.enableLineColor );

					using( new EditorGUI.DisabledGroupScope( !E.i.enableLineColor ) ) {
						EditorGUI.indentLevel++;
						E.i.lineColor = EditorGUILayout.ColorField( SS._Rowcolor, E.i.lineColor );
						EditorGUI.indentLevel--;
					}

					GUILayout.Space( 8f );
					EditorGUILayout.LabelField( $"* {SS._Experimental}", EditorStyles.boldLabel );
					E.i.IconClickContext = HEditorGUILayout.ToggleLeft( SS._ContextMenuWithIconClick, E.i.IconClickContext );
					E.i.SceneIconClickPing = HEditorGUILayout.ToggleLeft( S._Pingascenefilebyclickingthesceneicon, E.i.SceneIconClickPing );
					E.i.showLayerAndTag = HEditorGUILayout.ToggleLeft( S._Displaytagnameandlayername, E.i.showLayerAndTag );
					
				}
			}

			if( EditorGUI.EndChangeCheck() ) {
				E.Save();
				if( CustomHierarchy.s_styles != null ) {
					CustomHierarchy.s_styles.lineColor = E.i.lineColor;
				}
				EditorApplication.RepaintHierarchyWindow();
			}

			GUILayout.Space( 8f );

		}





#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		static void titleBarGuiHandler() {
			GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel );
		}
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Package.name}", SettingsScope.User ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = titleBarGuiHandler,
			};
			return provider;
		}
		public static void PreferencesGUI( string searchText ) {
#else
		[PreferenceItem( Package.name )]
		public static void PreferencesGUI() {
#endif
			E.Load();
			DrawGUI();
		}
	}
}
