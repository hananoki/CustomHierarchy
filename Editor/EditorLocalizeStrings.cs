﻿// Generated by Assets/Hananoki/EditorExtension/CustomHierarchy/Localize/en-US.csv

namespace HananokiEditor.CustomHierarchy {
	public static class L {
		public static string Tr( int n ) {
			try {
				return EditorLocalize.GetPakage( Package.name ).m_Strings[ n ];
			}
			catch( System.Exception ) {
			}
			return string.Empty;
		}
	}
	public static class S {
		public static string _Showtoggletotogglegameobjectactive => L.Tr( 0 );
		public static string _Displayprefabstatuswithicons => L.Tr( 1 );
		public static string _Moveuponelevel => L.Tr( 2 );
		public static string _Pingascenefilebyclickingthesceneicon => L.Tr( 3 );
		public static string _Displaytagnameandlayername => L.Tr( 4 );
		public static string _Displaythetree => L.Tr( 5 );
		public static string _ItemdisplayoffsetX => L.Tr( 6 );
		public static string _NumpadControl => L.Tr( 7 );
		public static string _ApplyAll => L.Tr( 8 );
		public static string _RevertAll => L.Tr( 9 );
		public static string _HideGameObject => L.Tr( 10 );
		public static string _UnhideGameObject => L.Tr( 11 );
		public static string _ComponentHandler => L.Tr( 12 );
		public static string _GameObjectDeleteButton => L.Tr( 13 );
		public static string _Registerallthecomponentsusedinthecurrentscene => L.Tr( 14 );
	}

#if UNITY_EDITOR
  public class LocalizeEvent {
    [HananokiEditorLocalizeRegister]
    public static void Changed() {
			EditorLocalize.Load( Package.name, "2eae63f13c909d54eb5e8f493fcde80b" );
		}
	}
#endif
}
