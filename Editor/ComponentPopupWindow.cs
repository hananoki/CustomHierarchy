#pragma warning disable 649

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Hananoki.Reflection;
using Hananoki.Extensions;

#if UNITY_EDITOR
#endif


namespace Hananoki.CustomHierarchy {
	public class ComponentPopupWindow : HEditorWindow {

		Action<bool> closeEvent;
		bool bOK;
		Component com;
		Type type => Type.GetType( typeName );
		string typeName;

		[NonSerialized]
		public Editor editor;



		public Vector2 GetWindowSize() {
			//	var v2 = com.GetWindowSize();
			//	if( v2 != Vector2.zero ) return v2;
			//	if( editor == null ) return Vector2.zero;
			//	var count = ( (IEventCommandInspector) editor ).count;
			return new Vector2( 300, ( 20 * 1 ) + ( 28 ) + 28 );
			//return com.GetWindowSize();
		}


		public static void Open( Component com, Action<bool> closeEvent ) {
			var window = GetWindow<ComponentPopupWindow>( true );
			window.SetTitle( com.GetType().Name, EditorIcon.scriptableobject );
			window.com = com;
			window.typeName = com.GetType().AssemblyQualifiedName;

			window.closeEvent = closeEvent;
			window.OnEnable2();
		}

		void OnEnable2() {

		}
		void OnSelectionChange() {
			Component comp = null;
			if( Selection.activeGameObject == null ) {
				if( type == null ) {
					Close();
					return;
				}
			}
			else {
				comp = (Component) Selection.activeGameObject.GetComponent( UnityTypes.TMPro_TMP_Text );
				if( comp == null ) {
					comp = (Component) Selection.activeGameObject.GetComponent( UnityTypes.UnityEngine_UI_Image );
					if( comp == null ) {
						comp = (Component) Selection.activeGameObject.GetComponent( UnityTypes.UnityEngine_SpriteRenderer );
					}
				}
			}

			//var comp = Selection.activeGameObject.GetComponent( type );
			if( comp == null ) return;
			com = comp;
			typeName = com.GetType().AssemblyQualifiedName;
			com = comp;
			Repaint();
		}


		void OnGUI() {
			//if( editor == null ) {
			//	editor = Editor.CreateEditor( com );
			//	editor.OnInspectorGUI();
			//	position = new Rect( position.position, GetWindowSize() );
			//}
			//GUILayout.Label( com.GetTitleName(), "OL Title" );
			//GUILayout.Space( 4 );

			//editor.OnInspectorGUI();
			if( UnityTypes.TMPro_TMP_Text != null && com.GetType().IsSubclassOf( UnityTypes.TMPro_TMP_Text ) ) {
				var s = com.GetProperty<string>( "text" );
				HGUIScope.Horizontal( _ );
				void _() {
					if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
					if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
					if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				}
				EditorGUI.BeginChangeCheck();
				s = EditorGUILayout.TextArea( s, EditorStyles.textField, GUILayout.Height( 20 * 3 ) );
				if( EditorGUI.EndChangeCheck() ) {
					com.SetProperty<string>( "text", s );
					//Debug.Log( typeof( Image ).AssemblyQualifiedName );
					//Debug.Log( typeof( RawImage ).AssemblyQualifiedName );
				}
			}
			if( UnityTypes.UnityEngine_UI_Image != null && com.GetType() == ( UnityTypes.UnityEngine_UI_Image ) ) {
				HGUIScope.Horizontal( _ );
				void _() {
					if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
					if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
					if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				}
				var s = com.GetProperty<Sprite>( "sprite" );
				EditorGUI.BeginChangeCheck();
				s = HEditorGUILayout.ObjectField<Sprite>( s );
				if( EditorGUI.EndChangeCheck() ) {
					com.SetProperty<Sprite>( "sprite", s );
					//Debug.Log( typeof( Image ).AssemblyQualifiedName );
					//Debug.Log( typeof( RawImage ).AssemblyQualifiedName );
				}
			}
			if( UnityTypes.UnityEngine_SpriteRenderer != null && com.GetType() == ( UnityTypes.UnityEngine_SpriteRenderer ) ) {
				HGUIScope.Horizontal( _ );
				void _() {
					if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
					if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
					if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				}
				var s = com.GetProperty<Sprite>( "sprite" );
				EditorGUI.BeginChangeCheck();
				s = HEditorGUILayout.ObjectField<Sprite>( s );
				if( EditorGUI.EndChangeCheck() ) {
					com.SetProperty<Sprite>( "sprite", s );
					//Debug.Log( typeof( Image ).AssemblyQualifiedName );
					//Debug.Log( typeof( RawImage ).AssemblyQualifiedName );
				}
			}

			//GUILayout.Space( 8 );
			//HGUIScope.Horizontal( () => {
			//	GUILayout.FlexibleSpace();
			//	if( GUILayout.Button( "OK" ) ) {
			//		bOK = true;
			//		Close();
			//	}
			//	if( GUILayout.Button( "キャンセル" ) ) {
			//		Close();
			//	}
			//} );
			//GUILayout.Space( 8 );
		}

		void OnDestroy() {
			closeEvent?.Invoke( bOK );
		}


	}
}
