
using UnityEditor;

namespace HananokiEditor.CustomHierarchy {
  public static class Package {
    public const string reverseDomainName = "com.hananoki.custom-hierarchy";
    public const string name = "CustomHierarchy";
    public const string nameNicify = "Custom Hierarchy";
    public const string editorPrefName = "Hananoki.CustomHierarchy";
    public const string version = "0.7.3";
		[HananokiEditorMDViewerRegister]
		public static string MDViewerRegister() {
			return "d16f935205a1fd148bcaac46a4932372";
		}
  }
}
