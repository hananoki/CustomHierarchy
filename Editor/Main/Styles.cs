
using UnityEditor;
using UnityEngine;

namespace HananokiEditor.CustomHierarchy {
	public class Styles {
		public static Texture2D prefabNormal => s_styles.PrefabNormal;
		public static Texture2D prefabModel => s_styles.PrefabModel;
		public static Texture2D missingPrefabInstance => s_styles.MissingPrefabInstance;
		public static Texture2D disconnectedPrefab => s_styles.DisconnectedPrefab;
		public static Texture2D disconnectedModelPrefab => s_styles.DisconnectedModelPrefab;

		public static Texture2D treeLine => s_styles.TreeLine;
		public static Texture2D treeLineB => s_styles.TreeLineB;

		public static GUIStyle controlLabel => s_styles.ControlLabel;

		public static GUIStyle objectField => s_styles.ObjectField;


		public Texture2D PrefabNormal => Icon._PrefabNormal;
		public Texture2D PrefabModel => Icon._PrefabModel;
		public Texture2D MissingPrefabInstance => Icon._MissingPrefabInstance;
		public Texture2D DisconnectedPrefab => Icon._DisconnectedPrefab;
		public Texture2D DisconnectedModelPrefab => Icon._DisconnectedModelPrefab;
		public Texture2D TreeLine => EditorIcon.CH_I;
		public Texture2D TreeLineB => EditorIcon.CH_T;


		public GUIStyle ControlLabel;

		public GUIStyle ObjectField;

		public Styles() {

			ControlLabel = new GUIStyle( "ControlLabel" );

			ObjectField = new GUIStyle( EditorStyles.objectField );
			ObjectField.padding.top = 1;
			ObjectField.padding.left = 2;
		}

		static Styles s_styles;

		public static void Init() {
			if( s_styles == null ) {
				s_styles = new Styles();
			}
		}
	}
}
