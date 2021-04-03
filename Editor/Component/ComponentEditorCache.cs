using HananokiRuntime;
using System.Collections;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {
	public class ComponentEditorCache {

		static Hashtable s_editors;

		/////////////////////////////////////////
		public static Editor Get( UnityObject component ) {
			Helper.New( ref s_editors );
			var editor = (Editor) s_editors[ component ];
			if( editor == null ) {
				editor = Editor.CreateEditor( component );
			}
			return editor;
		}
	}
}