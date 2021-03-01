using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityReflection;

namespace HananokiEditor.CustomHierarchy {
	public static class DragAndDropEx {
		static HashSet<Rect> m_rects = new HashSet<Rect>();

		public static void OnHierarchyChanged() {
			m_rects.Clear();
		}

		public static void Execute( Rect rect ) {
			// 通常のスクリプトアタッチを邪魔しないようにする.
			if( m_rects.Add( rect ) ) return;
			if( m_rects.Any( x => x.Contains( Event.current.mousePosition ) ) ) return;

			if( DragAndDrop.objectReferences == null ) return;
			if( DragAndDrop.objectReferences.Length == 0 ) return;

			if( _MonoScript() ) goto perform;
			//if( _Sprite() ) goto perform;
			return;

			perform:
			if( Event.current.type == EventType.DragPerform ) {
				DragAndDrop.AcceptDrag();
				Event.current.Use();
			}
		}


		static bool _MonoScript() {
			var tt = DragAndDrop.objectReferences
				.OfType<MonoScript>()
				.Select( x => x.GetClass() )
				//.Where( x => x.IsSubclassOf( typeof( MonoBehaviour ) ) )
				.Where( x => typeof( MonoBehaviour ).IsAssignableFrom( x ) )
				.ToArray();

			if( tt.Length == 0 ) return false;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if( Event.current.type != EventType.DragPerform ) return true;

			var gobj = new GameObject( tt[ 0 ].Name );
			foreach( var t in tt ) {
				gobj.AddComponent( t );
			}
			Undo.RegisterCreatedObjectUndo( gobj, $"new GameObject( \"{tt[ 0 ].Name}\" )" );
			return true;
		}

		static bool _Sprite() {
			var tt = DragAndDrop.objectReferences
				.OfType<Sprite>()
				//.Select( x => x.GetClass() )
				////.Where( x => x.IsSubclassOf( typeof( MonoBehaviour ) ) )
				//.Where( x => typeof( MonoBehaviour ).IsAssignableFrom( x ) )
				.ToArray();

			if( tt.Length == 0 ) return false;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			if( Event.current.type != EventType.DragPerform ) return true;

			foreach( var p in tt ) {
				Debug.Log( p.name );
			}
			//var gobj = new GameObject( tt[ 0 ].Name );
			//foreach( var t in tt ) {
			//	gobj.AddComponent( t );
			//}
			//Undo.RegisterCreatedObjectUndo( gobj, $"new GameObject( \"{tt[ 0 ].Name}\" )" );
			return true;
		}

	}
}
