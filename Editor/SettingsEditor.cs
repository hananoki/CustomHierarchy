using HananokiRuntime.Extensions;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;

namespace HananokiEditor.CustomHierarchy {
	[System.Serializable]
	public class SettingsEditor {

		public int flag;

		const int DOCKPANE_BAR = ( 1 << 0 );
		const int COMMAND_BAR = ( 1 << 1 );
		const int ICON_CLICK_CONTEXT = ( 1 << 2 );
		const int COMPONENT_HANDLER = ( 1 << 3 );
		const int ACTIVE_TOGGLE = ( 1 << 4 );
		const int PREFAB_STATUS = ( 1 << 5 );
		const int TREE_LINE = ( 1 << 6 );
		const int SHOW_LAYER_AND_TAG = ( 1 << 7 );
		const int NUMPAD_CTRL = ( 1 << 8 );
		const int SCENE_ICON_CLICK_PING = ( 1 << 9 );
		const int REMOVE_GAME_OBJECT = ( 1 << 10 );
		const int ENABLE_LINE_COLOR = ( 1 << 11 );
		const int EXTEND_DD = ( 1 << 12 );

		public bool dockPaneBar {
			get => flag.Has( DOCKPANE_BAR );
			set => flag.Toggle( DOCKPANE_BAR, value );
		}
		public bool commandBar {
			get => flag.Has( COMMAND_BAR );
			set => flag.Toggle( COMMAND_BAR, value );
		}
		public bool iconClickContext {
			get => flag.Has( ICON_CLICK_CONTEXT );
			set => flag.Toggle( ICON_CLICK_CONTEXT, value );
		}
		public bool componentHandler {
			get => flag.Has( COMPONENT_HANDLER );
			set => flag.Toggle( COMPONENT_HANDLER, value );
		}
		public bool activeToggle {
			get => flag.Has( ACTIVE_TOGGLE );
			set => flag.Toggle( ACTIVE_TOGGLE, value );
		}
		public bool prefabStatus {
			get => flag.Has( PREFAB_STATUS );
			set => flag.Toggle( PREFAB_STATUS, value );
		}
		public bool enableTreeImg {
			get => flag.Has( TREE_LINE );
			set => flag.Toggle( TREE_LINE, value );
		}
		public bool showLayerAndTag {
			get => flag.Has( SHOW_LAYER_AND_TAG );
			set => flag.Toggle( SHOW_LAYER_AND_TAG, value );
		}
		public bool numpadCtrl {
			get => flag.Has( NUMPAD_CTRL );
			set => flag.Toggle( NUMPAD_CTRL, value );
		}
		public bool sceneIconClickPing {
			get => flag.Has( SCENE_ICON_CLICK_PING );
			set => flag.Toggle( SCENE_ICON_CLICK_PING, value );
		}
		public bool removeGameObject {
			get => flag.Has( REMOVE_GAME_OBJECT );
			set => flag.Toggle( REMOVE_GAME_OBJECT, value );
		}
		public bool enableLineColor {
			get => flag.Has( ENABLE_LINE_COLOR );
			set => flag.Toggle( ENABLE_LINE_COLOR, value );
		}
		public bool extendedDragAndDrop {
			get => flag.Has( EXTEND_DD );
			set => flag.Toggle( EXTEND_DD, value );
		}


		public bool Enable = true;


		public Color lineColorPersonal = new Color( 0, 0, 0, 0.05f );
		public Color lineColorProfessional = new Color( 1, 1, 1, 0.05f );

		public float offsetPosX;

		public bool toolbarOverride;

		public List<ComponentHandlerData> m_componentHandlerData;
		public List<MenuCommandData> m_menuCommandData;

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

		public static bool HasInspecClass( Type t ) {
			foreach( var p in i.m_componentHandlerData ) {
				if( p.type == t ) {
					if( p.inspector ) return true;
				}
			}
			return false;
		}
	}



	//public class SettingsEditorWindow : HSettingsEditorWindow {
	//	public static void Open() {
	//		var w = GetWindow<SettingsEditorWindow>();
	//		w.SetTitle( new GUIContent( "Project Settings", EditorIcon.settings ) );
	//		w.headerMame = Package.name;
	//		w.headerVersion = Package.version;
	//		w.gui = SettingsDrawer.DrawGUI;
	//	}
	//}
}
