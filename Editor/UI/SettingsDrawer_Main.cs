using HananokiEditor.Extensions;
using HananokiEditor.SharedModule;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using SS = HananokiEditor.SharedModule.S;

namespace HananokiEditor.CustomHierarchy {
	public class SettingsDrawer_Main {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				//mode = 1,
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
			};
		}



		public static void DrawGUI() {
			E.Load();
			ScopeChange.Begin();

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

			ScopeDisable.Begin( !E.i.Enable );

			E.i.enableTreeImg = HEditorGUILayout.ToggleLeft( S._Displaythetree, E.i.enableTreeImg );
			E.i.activeToggle = HEditorGUILayout.ToggleLeft( S._Showtoggletotogglegameobjectactive, E.i.activeToggle );
			E.i.prefabStatus = HEditorGUILayout.ToggleLeft( S._Displayprefabstatuswithicons, E.i.prefabStatus );

			E.i.enableLineColor = HEditorGUILayout.ToggleLeft( SS._Changecolorforeachrow, E.i.enableLineColor );

			ScopeDisable.Begin( !E.i.enableLineColor );
			EditorGUI.indentLevel++;
			E.i.lineColor = EditorGUILayout.ColorField( SS._Rowcolor, E.i.lineColor );
			EditorGUI.indentLevel--;
			ScopeDisable.End();

			//#if LOCAL_DEBUG
			//E.i.offsetPosX = EditorGUILayout.FloatField( S._ItemdisplayoffsetX, E.i.offsetPosX );
			//#endif
			E.i.iconClickContext = HEditorGUILayout.ToggleLeft( SS._ContextMenuWithIconClick, E.i.iconClickContext );
			E.i.sceneIconClickPing = HEditorGUILayout.ToggleLeft( S._Pingascenefilebyclickingthesceneicon, E.i.sceneIconClickPing );
			E.i.showLayerAndTag = HEditorGUILayout.ToggleLeft( S._Displaytagnameandlayername, E.i.showLayerAndTag );
			E.i.numpadCtrl = HEditorGUILayout.ToggleLeft( S._NumpadControl, E.i.numpadCtrl );

			/////////////////////////
			///
			GUILayout.Space( 8f );
			HEditorGUILayout.HeaderTitle( $"* {SS._Experimental}"/*, EditorStyles.boldLabel*/ );

			E.i.extendedDragAndDrop = HEditorGUILayout.ToggleLeft( "Extended Drag And Drop", E.i.extendedDragAndDrop );
			E.i.componentHandler = HEditorGUILayout.ToggleLeft( S._ComponentHandler, E.i.componentHandler );
			ScopeDisable.Begin( !E.i.componentHandler );
			EditorGUI.indentLevel++;
			E.i.componentToolPos = EditorGUILayout.FloatField( "componentToolPos".nicify(), E.i.componentToolPos );
			EditorGUI.indentLevel--;
			ScopeDisable.End();

			E.i.removeGameObject = HEditorGUILayout.ToggleLeft( S._GameObjectDeleteButton, E.i.removeGameObject );

			ScopeChange.Begin();
			E.i.dockPaneBar = HEditorGUILayout.ToggleLeft( "ShortCutBar (UNITY_2019_1_OR_NEWER)", E.i.dockPaneBar );
			if( ScopeChange.End() ) {
				DockPaneBar.s_initButton = -1;
				DockPaneBar.Setup();
				E.Save();
				EditorWindowUtils.RepaintHierarchyWindow();
			}

			ScopeChange.Begin();
			E.i.commandBar = HEditorGUILayout.ToggleLeft( "CommandBar (UNITY_2019_1_OR_NEWER)", E.i.commandBar );
			if( ScopeChange.End() ) {
				CommandBar.s_initButton = -1;
				CommandBar.Setup();
				E.Save();
				EditorWindowUtils.RepaintHierarchyWindow();
			}
			GUILayout.Space( 8f );

			/////////////////////////
			HEditorGUILayout.HeaderTitle( $"* Obsolete" );
			E.i.ヒエラルキークリックでインスペクターFocus = HEditorGUILayout.ToggleLeft( "ヒエラルキークリックでインスペクターFocus", E.i.ヒエラルキークリックでインスペクターFocus );

			ScopeDisable.End();

			EditorGUI.indentLevel--;

			if( ScopeChange.End() ) {
				CustomHierarchy.initSelection = false;
				EditorWindowUtils.RepaintHierarchyWindow();
				E.Save();
			}
		}
	}
}
