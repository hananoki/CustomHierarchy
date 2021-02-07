#pragma warning disable 649

using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using HananokiEditor.Extensions;


using UnityObject = UnityEngine.Object;
namespace HananokiEditor.CustomHierarchy {
	public class ComponentPopupWindow : HEditorWindow {

		Action<bool> closeEvent;
		bool bOK;
		Component com;
		Type type => Type.GetType( typeName );
		string typeName;

		[NonSerialized]
		public Editor editor;

		static List<Type> _types;
		List<Type> m_types {
			get {
				if( _types == null ) {
					_types = new List<Type>();
					_types.Add( UnityTypes.TMPro_TMP_Text );
					_types.Add( UnityTypes.UnityEngine_UI_Image );
					_types.Add( UnityTypes.UnityEngine_UI_RawImage );
					_types.Add( UnityTypes.UnityEngine_SpriteRenderer );
				}
				return _types;
			}
		}

		SerializedObject m_serializedObject;
		SerializedProperty m_RaycastTarget;

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
			window.OnInitSerializedObject( com );
		}

		void OnInitSerializedObject( UnityObject obj ) {
			m_serializedObject = new SerializedObject( obj );
			m_RaycastTarget = m_serializedObject.FindProperty( "m_RaycastTarget" );
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
				foreach( var t in m_types ) {
					comp = (Component) Selection.activeGameObject.GetComponent( t );
					if( comp != null ) break;
				}
			}


			//var comp = Selection.activeGameObject.GetComponent( type );
			if( comp == null ) return;
			com = comp;
			typeName = com.GetType().AssemblyQualifiedName;
			com = comp;
			SetTitle( com.GetType().Name );
			OnInitSerializedObject( comp );
			Repaint();
		}


		public override void OnDefaultGUI() {
			//if( editor == null ) {
			//	editor = Editor.CreateEditor( com );
			//	editor.OnInspectorGUI();
			//	position = new Rect( position.position, GetWindowSize() );
			//}
			//GUILayout.Label( com.GetTitleName(), "OL Title" );
			//GUILayout.Space( 4 );

			//editor.OnInspectorGUI();
			if( UnityTypes.TMPro_TMP_Text != null && com.GetType().IsSubclassOf( UnityTypes.TMPro_TMP_Text ) ) {
				//var _TMP_Text = (TMP_Text) com;
				var s = com.GetProperty<string>( "text" );
				ScopeHorizontal.Begin();
				if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
				if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
				if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				ScopeHorizontal.End();

				ScopeChange.Begin();
				s = EditorGUILayout.TextArea( s, EditorStyles.textField, GUILayout.Height( 20 * 3 ) );
				if( ScopeChange.End() ) {
					com.SetProperty<string>( "text", s );
					EditorWindowUtils.RepaintSceneView();
				}

			}
			if( UnityTypes.UnityEngine_UI_Image != null && com.GetType() == ( UnityTypes.UnityEngine_UI_Image ) ) {
				ScopeHorizontal.Begin();
				if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
				if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
				if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				ScopeHorizontal.End();

				var s = com.GetProperty<Sprite>( "sprite" );
				ScopeChange.Begin();
				s = HEditorGUILayout.ObjectField<Sprite>( s );
				if( ScopeChange.End() ) {
					com.SetProperty<Sprite>( "sprite", s );
					SceneView.RepaintAll();
					//Debug.Log( typeof( Image ).AssemblyQualifiedName );
					//Debug.Log( typeof( RawImage ).AssemblyQualifiedName );
				}

			}
			if( m_RaycastTarget != null ) {
				ScopeChange.Begin();
				var b = HEditorGUILayout.ToggleLeft( "raycastTarget".nicify(), m_RaycastTarget.boolValue );
				if( ScopeChange.End() ) {
					using( new SerializedObjectScope( m_serializedObject ) ) {
						m_RaycastTarget.boolValue = b;
					}
					EditorWindowUtils.RepaintSceneView();
				}
			}

			if( UnityTypes.UnityEngine_SpriteRenderer != null && com.GetType() == ( UnityTypes.UnityEngine_SpriteRenderer ) ) {
				ScopeHorizontal.Begin();
				if( GUILayout.Button( "#", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "#" );
				if( GUILayout.Button( "##", GUILayout.ExpandWidth( false ) ) ) EditorHelper.SetGameObjectName( com, "##" );
				if( GUILayout.Button( "Set GameObject Name" ) ) EditorHelper.SetGameObjectName( com );
				ScopeHorizontal.End();

				var s = com.GetProperty<Sprite>( "sprite" );
				ScopeChange.Begin();
				s = HEditorGUILayout.ObjectField<Sprite>( s );
				if( ScopeChange.End() ) {
					com.SetProperty<Sprite>( "sprite", s );
					SceneView.RepaintAll();
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
