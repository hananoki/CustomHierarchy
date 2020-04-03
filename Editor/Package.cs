
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
namespace Hananoki.CustomHierarchy {
  public static class Package {
    public const string name = "CustomHierarchy";
    public const string editorPrefName = "Hananoki.CustomHierarchy";
    public const string version = "0.5.2-preview";
  }
  
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
	public static class Icon {
		static Dictionary<string, Texture2D> icons;
		public static Texture2D Get( string s ) {
			if( icons == null ) {
				icons = new Dictionary<string, Texture2D>();
			}
			bool load = false;
			if( !icons.ContainsKey( s ) ) load = true;
			else if( icons[ s ] == null ) load = true;
			if( load ) {
				for( int i = 0; i < SharedEmbed.num; i++ ) {
					if( SharedEmbed.n[ i ] != s ) continue;
					var bb = B64.Decode( "iVBORw0KGgoAAAAN" + SharedEmbed.i[ i ] );
					var t = new Texture2D( SharedEmbed.x[ i ], SharedEmbed.y[ i ] );
					t.LoadImage( bb );
					t.hideFlags |= HideFlags.DontUnloadUnusedAsset;
					if( icons.ContainsKey( s ) ) {
						icons[ s ] = t;
					}
					else {
						icons.Add( SharedEmbed.n[ i ], t );
					}
				}
			}
			return icons[ s ];
		}
	}
}
