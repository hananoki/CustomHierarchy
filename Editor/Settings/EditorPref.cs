﻿using HananokiRuntime.Extensions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.EditorPref;


namespace HananokiEditor.CustomHierarchy {

	[Serializable]
	public class EditorPref {

		public int flag;

		#region Flags

		const int _検索フィルターボタン = ( 1 << 0 );
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
		const int _ヒエラルキークリックでインスペクターFocus = ( 1 << 13 );
		const int PREFAB_NOTIFY = ( 1 << 14 );
		const int _プレイモード時はコンポーネントツールを消す = ( 1 << 15 );
		const int _MissingScriptチェック = ( 1 << 16 );


		public bool 検索フィルターボタン {
			get => flag.Has( _検索フィルターボタン );
			set => flag.Toggle( _検索フィルターボタン, value );
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
		public bool ヒエラルキークリックでインスペクターFocus {
			get => flag.Has( _ヒエラルキークリックでインスペクターFocus );
			set => flag.Toggle( _ヒエラルキークリックでインスペクターFocus, value );
		}
		public bool Selectionのプレハブがヒエラルキー上にあると通知 {
			get => flag.Has( PREFAB_NOTIFY );
			set => flag.Toggle( PREFAB_NOTIFY, value );
		}
		public bool プレイモード時はコンポーネントツールを消す {
			get => flag.Has( _プレイモード時はコンポーネントツールを消す );
			set => flag.Toggle( _プレイモード時はコンポーネントツールを消す, value );
		}
		public bool MissingScriptチェック {
			get => flag.Has( _MissingScriptチェック );
			set => flag.Toggle( _MissingScriptチェック, value );
		}
		#endregion

		public bool Enable = true;


		public Color lineColorPersonal = new Color( 0, 0, 0, 0.05f );
		public Color lineColorProfessional = new Color( 1, 1, 1, 0.05f );

		public float offsetPosX;
		public float componentToolPos;


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


		/////////////////////////////////////////
		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}


		/////////////////////////////////////////
		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}


		/////////////////////////////////////////
		public static bool HasInspecClass( Type t ) {
			foreach( var p in i.m_componentHandlerData ) {
				if( p.type == t ) {
					if( p.inspector ) return true;
				}
			}
			return false;
		}


		/////////////////////////////////////////
		public static bool HasShowTool( Type t ) {
			foreach( var p in i.m_componentHandlerData ) {
				if( p.type == t ) {
					if( p.showTool ) return true;
				}
			}
			return false;
		}


	}
}
