using HananokiEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.EditorPref;


namespace HananokiEditor.CustomHierarchy {

	using Item = TreeViewComponents.Item;

	public sealed class TreeViewComponents : HTreeView<Item> {

		const int kMove = 0;
		const int kComponents = 1;
		const int kSearch = 2;
		const int kInspector = 3;
		const int kShowTool = 4;

		public class Item : TreeViewItem {
			public Type type;
			public ComponentHandlerData data;
		}



		/////////////////////////////////////////
		public TreeViewComponents() : base( new TreeViewState() ) {
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
				headerContent = new GUIContent( "Components" ),
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( EditorIcon.search_icon ),
				width = 24,
				maxWidth = 24,
				minWidth = 24,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( EditorIcon.unityeditor_inspectorwindow ),
				width = 24,
				maxWidth = 24,
				minWidth = 24,
			} );
			lst.Add( new MultiColumnHeaderState.Column() {
				headerContent = new GUIContent( EditorIcon.CustomTool ),
				width = 24,
				maxWidth = 24,
				minWidth = 24,
			} );


			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( lst.ToArray() ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}



		/////////////////////////////////////////
		public void RegisterFiles() {

			InitID();
			MakeRoot();

			foreach( var p in E.i.m_componentHandlerData ) {
				if( p.type == null ) continue;

				var item = new Item {
					id = GetID(),
					displayName = p.type.Name.nicify(),
					icon = p.type.GetIcon(),
					data = p,
				};
				m_root.AddChild( item );
			}

			ReloadAndSorting();
		}



		/////////////////////////////////////////
		public void ReloadAndSorting() {
			ReloadRoot();
			//RollbackLastSelect();
		}



		/////////////////////////////////////////
		protected override void OnRowGUI( Item item, RowGUIArgs args ) {
			bool changed = false;

			for( var i = 0; i < args.GetNumVisibleColumns(); i++ ) {
				var rect = args.GetCellRect( i );
				var columnIndex = args.GetColumn( i );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kMove:
					EditorGUI.LabelField( rect.AlignCenterH( 16 ), EditorHelper.TempContent( EditorIcon.toolbar_minus ) );
					break;

				case kComponents:
					Label( args, rect, item.displayName, item.icon );
					break;

				case kSearch:
					ScopeChange.Begin();
					item.data.search = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.search );
					if( ScopeChange.End() ) changed = true;
					break;

				case kInspector:
					ScopeChange.Begin();
					item.data.inspector = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.inspector );
					if( ScopeChange.End() ) changed = true;
					break;

				case kShowTool:
					ScopeChange.Begin();
					item.data.showTool = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.showTool );
					if( ScopeChange.End() ) changed = true;
					break;

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



		/////////////////////////////////////////
		protected override void OnSelectionChanged( Item[] items ) {
			BackupLastSelect( items[ 0 ] );
		}



		#region DragAndDrop

		class DragAndDropData {
			public List<Item> dragItems;

			public static DragAndDropVisualMode visualMode = DragAndDropVisualMode.None;
			public DragAndDropArgs args;

			//public Item dropItem => (Item) args.parentItem;

			public DragAndDropData( string dragID, DragAndDropArgs args ) {
				this.args = args;
				dragItems = DragAndDrop.GetGenericData( dragID ) as List<Item>;
			}

			public List<Item> HandleBetweenItems( List<Item> items ) {
				if( dragItems == null ) {
					visualMode = DragAndDropVisualMode.None;
					return null;
				}

				visualMode = DragAndDropVisualMode.Move;

				// ドロップを実行します
				if( !args.performDrop ) return null;

				var lst = items.ToList();
				var insertIndex = args.insertAtIndex;

				foreach( var p in dragItems ) {
					var idx = lst.FindIndex( x => x.id == p.id );
					if( idx < insertIndex ) {
						insertIndex--;
					}
					lst.Remove( p );
				}

				lst.InsertRange( insertIndex, dragItems );

				return lst;
			}
		}


		protected override DragAndDropVisualMode HandleDragAndDrop( DragAndDropArgs args ) {
			var data = new DragAndDropData( dragID, args );

			switch( args.dragAndDropPosition ) {
			case DragAndDropPosition.BetweenItems:
				var lst = data.HandleBetweenItems( m_root.children.OfType<Item>().ToList() );
				if( lst != null ) {
					E.i.m_componentHandlerData = lst.Select( x => x.data ).ToList();
					E.Save();
					RegisterFiles();
					SelectItem( m_root.children.Find( x => x.displayName == data.dragItems[ 0 ].displayName ) );
				}
				break;
			case DragAndDropPosition.UponItem:
			case DragAndDropPosition.OutsideItems:
				DragAndDropData.visualMode = DragAndDropVisualMode.None;
				break;
			}
			return DragAndDropData.visualMode;
		}


		protected override void OnSetupDragAndDrop( Item[] items ) {

			DragAndDrop.PrepareStartDrag();

			DragAndDrop.paths = null;
			DragAndDrop.SetGenericData( dragID, items.ToList() );
			DragAndDrop.visualMode = DragAndDropVisualMode.None;
			DragAndDrop.StartDrag( $"{dragID}.StartDrag" );
		}

		protected override bool OnCanStartDrag( Item item, CanStartDragArgs args ) {
			return true;
		}

		#endregion
	}
}
