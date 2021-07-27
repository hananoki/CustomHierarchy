using HananokiEditor.Extensions;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HananokiEditor.CustomHierarchy {

	using Item = TreeViewHideSelect.Item;


	public sealed class TreeViewHideSelect : HTreeView<Item> {

		public class Item : TreeViewItem {
			public GameObject go;
			public bool enable;

		}


		public GameObject[] m_gameObjects;
		bool m_allFlag;
		Item m_allItem;

		/////////////////////////////////////////
		public TreeViewHideSelect() : base( new TreeViewState() ) {

			rowHeight = EditorGUIUtility.singleLineHeight;
		}



		/////////////////////////////////////////
		public void UpdateRootGameObjects() {
			var scene = SceneManager.GetActiveScene();
			m_gameObjects = scene.GetRootGameObjects();
		}


		/////////////////////////////////////////
		public void RegisterFiles() {
			InitID();
			MakeRoot();

			var hides = m_gameObjects.Where( x => x.HasHideFlags( HideFlags.HideInHierarchy ) );

			var count = hides.Count();
			//Debug.Log( count );
			m_allItem = new Item {
				id = GetID(),
				displayName = "All",
				enable = count == 0,
			};

			m_root.AddChild( m_allItem );


			foreach( var p in m_gameObjects ) {
				var item = new Item {
					id = GetID(),
					displayName = p.name,
					go = p,
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
		protected override void OnRowGUI( Item item, RowGUIArgs args ) {
			var r = args.rowRect;
			r = r.TrimL( 4 );
			if( item.go == null ) {
				ScopeChange.Begin();
				var _b = EditorGUI.Toggle( r.W( 16 ), item.enable );
				if( ScopeChange.End() ) {
					if( _b ) {
						foreach( var go in m_gameObjects ) {
							go.Show();
						}
					}
					else {
					}
					item.enable = _b;
					//EditorUtility.SetDirty( item.go );
					EditorWindowUtils.RepaintHierarchyWindow();
				}

				var rr = args.rowRect;
				rr.y = rr.yMax - 1;
				rr.height = 1;
				EditorGUI.DrawRect( rr, new Color( 0, 0, 0, 0.2f ) );

			}
			else {
				r = r.TrimL( 16 );

				ScopeChange.Begin();
				var _b = EditorGUI.Toggle( r.W( 16 ), m_allItem.enable ? false : !item.go.HasHideFlags( HideFlags.HideInHierarchy ) );
				if( ScopeChange.End() ) {
					if( _b ) {
						SelectShow( item.go );
					}
					else {
						SelectHide( item.go );
					}
					EditorWindowUtils.RepaintHierarchyWindow();
				}
			}
			Label( args, r.TrimL( 16 ), item.displayName );
			//DefaultRowGUI( args );
		}


		void SelectHide( GameObject go ) {
			go.Hide();
			EditorUtility.SetDirty( go );

			UpdateItemRoot();
		}


		void SelectShow( GameObject go ) {
			foreach( var g in m_gameObjects ) {
				if( m_allItem.enable ) {
					g.Hide();
					EditorUtility.SetDirty( g );
				}
				else {
					if( g.HasHideFlags( HideFlags.HideInHierarchy ) ) {
						g.Hide();
						EditorUtility.SetDirty( g );
					}
				}
			}

			go.Show();
			EditorUtility.SetDirty( go );

			UpdateItemRoot();
		}


		void UpdateItemRoot() {
			var count = m_gameObjects.Where( x => x.HasHideFlags( HideFlags.HideInHierarchy ) ).Count();

			m_allItem.enable = count == 0;

			if( m_allItem.enable ) {
				foreach( var go in m_gameObjects ) {
					go.Show();
					EditorUtility.SetDirty( go );
				}
			}
		}
	}



	public static class Extensions {
		public static void Show( this GameObject go ) {
			go.DisableHideFlags( HideFlags.HideInHierarchy );
#if UNITY_2019_2_OR_NEWER
			SceneVisibilityManager.instance.Show( go, true );
#endif
		}
		public static void Hide( this GameObject go ) {
#if UNITY_2019_2_OR_NEWER
			SceneVisibilityManager.instance.Hide( go, true );
#endif
			go.EnableHideFlags( HideFlags.HideInHierarchy );
		}
	}


}


