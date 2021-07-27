using HananokiEditor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityReflection;
using E = HananokiEditor.CustomHierarchy.EditorPref;
using SS = HananokiEditor.SharedModule.S;


namespace HananokiEditor.CustomHierarchy {
	public static class IconClickContext {
		public static void Execute( Rect rect ) {
			rect.width = 16;

			if( !EditorHelper.HasMouseClick( rect ) ) return;

			var m = new GenericMenu();
			m.AddItem( SS._OpenInNewInspector, EditorContextHandler.ShowNewInspectorWindow, Main.go );
			m.AddItem( S._HideGameObject, _Hide, Main.go );
			if( E.i.MissingScriptチェック ) {
				if( MissingScriptCheck.Is( Main.go ) ) {
					m.AddItem( "Remove MonoBehaviours With MissingScript", _RemoveComponent, Main.go );
				}
				else {
					m.AddDisabledItem( "Remove MonoBehaviours With MissingScript" );
				}
			}
			//m.AddItem( "DontDestroyOnLoad", _DontDestroyOnLoad, Main.go );

			var status = UnityEditorPrefabUtility.GetPrefabInstanceStatus( Main.go );
			if( status == PrefabInstanceStatus.Connected ) {
				m.AddSeparator();

				var wnd = new UnityEditorPrefabOverridesWindow( Main.go );
				if( !wnd.IsShowingActionButton() ) {
					m.AddDisabledItem( S._ApplyAll );
					m.AddDisabledItem( S._RevertAll );
				}
				else {
					m.AddItem( S._ApplyAll, _ApplyAll, wnd );
					m.AddItem( S._RevertAll, _RevertAlll, wnd );
					m.DropDownAtMousePosition();
				}
			}

			m.DropDownPopupRect( rect );
		}

		//static void _DontDestroyOnLoad( object context ) {
		//	var gobj = context as GameObject;
		//	Object.DontDestroyOnLoad( gobj );
		//}

		static void _RemoveComponent( object context ) {
			var gobj = context as GameObject;
#if UNITY_2019_2_OR_NEWER
			GameObjectUtility.RemoveMonoBehavioursWithMissingScript( gobj );
			EditorUtility.SetDirty( gobj );
#endif
		}


		static void _Hide( object context ) {
			var gobj = context as GameObject;
			gobj.hideFlags |= HideFlags.HideInHierarchy;
			EditorUtility.SetDirty( gobj );
			EditorWindowUtils.RepaintHierarchyWindow();
		}

		static void _ApplyAll( object context ) {
			var w = (UnityEditorPrefabOverridesWindow) context;
			w.ApplyAll();
		}

		static void _RevertAlll( object context ) {
			var w = (UnityEditorPrefabOverridesWindow) context;
			w.RevertAll();
		}
	}
}
