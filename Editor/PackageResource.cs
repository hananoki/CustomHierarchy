﻿using UnityEngine;

namespace Hananoki.CustomHierarchy {
	internal static class PackageResource {
		public static string[] i = { "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" };
	}

	internal static class Icon {
		static PackageResourceIcon packageResourceIcons;
		public static Texture2D Get( int n ) {
			if( packageResourceIcons == null ) {
				packageResourceIcons = new PackageResourceIcon( PackageResource.i );
			}
			return packageResourceIcons.Get( n );
		}
		public static Texture2D _PrefabNormal => Get( 0 );
		public static Texture2D _PrefabModel => Get( 1 );
		public static Texture2D _MissingPrefabInstance => Get( 2 );
		public static Texture2D _DisconnectedPrefab => Get( 3 );
		public static Texture2D _DisconnectedModelPrefab => Get( 4 );
		public static Texture2D CH_T => Get( 5 );
		public static Texture2D CH_I => Get( 6 );
	}
}