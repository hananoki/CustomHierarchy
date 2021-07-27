using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#pragma warning disable 649

using HananokiRuntime;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace HananokiEditor.CustomHierarchy {

	public class PopupContentHideSelect : PopupWindowContent {

		static PopupContentHideSelect s_content;

		Vector2 m_scroll;

		/////////////////////////////////////////
		public static void Show( Rect rect ) {
			Helper.New( ref s_content );
			PopupWindow.Show( rect, s_content );
		}


		/////////////////////////////////////////
		public override Vector2 GetWindowSize() {
			return new Vector2( 400, EditorGUIUtility.singleLineHeight + ( EditorGUIUtility.singleLineHeight * Main.m_treeViewHideSelect.m_gameObjects.Length ) );
		}


		/////////////////////////////////////////
		public override void OnGUI( Rect rect ) {
			using( new GUILayout.AreaScope( rect ) )
			using( var sc = new GUILayout.ScrollViewScope( m_scroll ) ) {
			//using( new GUILayoutScope( 4, 4 ) ) {
				//GUILayout.Space(4);
				Main.m_treeViewHideSelect.DrawLayoutGUI();
				//GUILayout.Space( 4 );
			}
		}


		/////////////////////////////////////////
		public override void OnOpen() {
			Main.m_treeViewHideSelect.RegisterFiles();
		}


		/////////////////////////////////////////
		public override void OnClose() {
		}
	}
}
