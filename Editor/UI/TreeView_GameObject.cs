using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HananokiEditor.Extensions;
using HananokiRuntime;
using HananokiRuntime.Extensions;
using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UI;

using E = HananokiEditor.CustomHierarchy.SettingsEditor;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	using Item = TreeView_GameObject.Item;

	public sealed class TreeView_GameObject : HTreeView<Item> {

		const int kMove = 0;
		//const int kEnable = 1;
		const int kComponents = 1;
		const int kSearch = 2;
		const int kInspector = 3;

		public class Item : TreeViewItem {
			//public Type type;
			//public ComponentHandlerData data;
		}


		public TreeView_GameObject() : base( new TreeViewState() ) {
			E.Load();

			showAlternatingRowBackgrounds = true;
			rowHeight = EditorGUIUtility.singleLineHeight;

			var lst = new List<MultiColumnHeaderState.Column>();

			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = GUIContent.none,
				width = 18,
				maxWidth = 18,
				minWidth = 18,
			} );
			//lst.Add( new MultiColumnHeaderState.Column() {
			//	headerContent = GUIContent.none,
			//	width = 18,
			//	maxWidth = 18,
			//	minWidth = 18,
			//} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( "Menu Command" ),
			} );
			//lst.Add( new MultiColumnHeaderState.Column() {
			//	headerContent = new GUIContent( EditorIcon.search_icon ),
			//	width = 24,
			//	maxWidth = 24,
			//	minWidth = 24,
			//} );
			//lst.Add( new MultiColumnHeaderState.Column() {
			//	headerContent = new GUIContent( EditorIcon.unityeditor_inspectorwindow ),
			//	width = 24,
			//	maxWidth = 24,
			//	minWidth = 24,
			//} );

			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( lst.ToArray() ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}


		public void RegisterFiles() {

			var lst = new List<Item>();
			InitID();

			foreach( var p in E.i.m_menuCommandData ) {
				var item = new Item {
					id = GetID(),
					displayName = p.menuItem,
				};
				lst.Add( item );
			}

			m_registerItems = lst;
			ReloadAndSorting();
		}


		public void ReloadAndSorting() {
			Reload();
		}



		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;
			bool changed = false;



			//if( item.currentEnabled && !args.selected ) {
			//	var col1 = ColorUtils.RGB( 169, 201, 255 );
			//	col1.a = 0.5f;
			//	EditorGUI.DrawRect( args.rowRect, col1 );
			//}

			for( var i = 0; i < args.GetNumVisibleColumns(); i++ ) {
				var rect = args.GetCellRect( i );
				var columnIndex = args.GetColumn( i );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kMove: {
					EditorGUI.LabelField( rect.AlignCenterH( 16 ), EditorHelper.TempContent( EditorIcon.toolbar_minus ) );
					break;
				}
				//case kEnable: {
				//	ScopeChange.Begin();
				//	var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.enable );
				//	if( ScopeChange.End() ) {
				//		item.data.enable = _b;
				//		E.Save();
				//	}
				//	break;
				//}
				case kComponents: {
					Label( args, rect, item.displayName, item.icon );
					break;
				}
				//case kSearch: {
				//	ScopeChange.Begin();
				//	var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.search );
				//	if( ScopeChange.End() ) {
				//		item.data.search = _b;
				//		changed = true;
				//	}
				//	break;
				//}
				//case kInspector: {
				//	ScopeChange.Begin();
				//	var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.inspector );
				//	if( ScopeChange.End() ) {
				//		item.data.inspector = _b;
				//		changed = true;
				//	}
				//	break;
				//}
				default:
					break;
				}
			}

			if( changed ) {
				E.Save();
				ComponentHandler.Reset();
				EditorWindowUtils.RepaintHierarchyWindow();
			}
		}
	}
}
