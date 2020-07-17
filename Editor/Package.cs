
using UnityEditor;

namespace Hananoki.CustomHierarchy {
  public static class Package {
    public const string name = "CustomHierarchy";
    public const string editorPrefName = "Hananoki.CustomHierarchy";
    public const string version = "0.6.1-preview";
  }
  
#if UNITY_EDITOR
  [EditorLocalizeClass]
  public class LocalizeEvent {
    [EditorLocalizeMethod]
    public static void Changed() {
      foreach( var filename in DirectoryUtils.GetFiles( AssetDatabase.GUIDToAssetPath( "2eae63f13c909d54eb5e8f493fcde80b" ), "*.csv" ) ) {
        if( filename.Contains( EditorLocalize.GetLocalizeName() ) ) {
          EditorLocalize.Load( Package.name, AssetDatabase.AssetPathToGUID( filename ), "30e3d46b035db1c42998201d13f2a3c9" );
        }
      }
    }
  }
#endif
}
