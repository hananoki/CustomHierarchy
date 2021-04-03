using HananokiEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using E = HananokiEditor.CustomHierarchy.SettingsEditor;


namespace HananokiEditor.CustomHierarchy {
	using Item = TreeView_Components.Item;

	public sealed class TreeView_Components : HTreeView<Item> {

		const int kMove = 0;
		const int kComponents = 1;
		const int kSearch = 2;
		const int kInspector = 3;

		public class Item : TreeViewItem {
			public Type type;
			public ComponentHandlerData data;
		}



		/////////////////////////////////////////
		public TreeView_Components() : base( new TreeViewState() ) {
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

			multiColumnHeader = new MultiColumnHeader( new MultiColumnHeaderState( lst.ToArray() ) );
			multiColumnHeader.ResizeToFit();
			multiColumnHeader.height = 22;
			//multiColumnHeader.sortingChanged += OnSortingChanged;

			RegisterFiles();
		}



		/////////////////////////////////////////
		public void RegisterFiles() {

			m_root = new Item { depth = -1 };

			InitID();

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
			Reload();
			//RollbackLastSelect();
		}



		/////////////////////////////////////////
		protected override void OnRowGUI( RowGUIArgs args ) {
			var item = (Item) args.item;
			bool changed = false;

			for( var i = 0; i < args.GetNumVisibleColumns(); i++ ) {
				var rect = args.GetCellRect( i );
				var columnIndex = args.GetColumn( i );

				var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;

				switch( columnIndex ) {
				case kMove: {
					EditorGUI.LabelField( rect.AlignCenterH( 16 ), EditorHelper.TempContent( EditorIcon.toolbar_minus ) );
					break;
				}
				case kComponents: {
					Label( args, rect, item.displayName, item.icon );
					break;
				}
				case kSearch: {
					ScopeChange.Begin();
					var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.search );
					if( ScopeChange.End() ) {
						item.data.search = _b;
						changed = true;
					}
					break;
				}
				case kInspector: {
					ScopeChange.Begin();
					var _b = EditorGUI.Toggle( rect.AlignCenter( 16, 16 ), item.data.inspector );
					if( ScopeChange.End() ) {
						item.data.inspector = _b;
						changed = true;
					}
					break;
				}
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
		protected override void SingleSelectionChanged( Item item ) {
			BackupLastSelect( item );
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


		protected override void SetupDragAndDrop( SetupDragAndDropArgs args ) {
			if( args.draggedItemIDs == null ) return;

			DragAndDrop.PrepareStartDrag();

			DragAndDrop.paths = null;
			DragAndDrop.SetGenericData( dragID, ToItems( args.draggedItemIDs ).ToList() );
			DragAndDrop.visualMode = DragAndDropVisualMode.None;
			DragAndDrop.StartDrag( $"{dragID}.StartDrag" );
		}

		protected override bool CanStartDrag( CanStartDragArgs args ) {
			return true;
		}

		#endregion
	}
}
