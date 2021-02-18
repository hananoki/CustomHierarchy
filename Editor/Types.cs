using HananokiRuntime.Extensions;
using System;

namespace HananokiEditor.CustomHierarchy {
	[System.Serializable]
	public class ComponentHandlerData {
		const int ENABLE = ( 1 << 0 );
		const int SEARCH = ( 1 << 1 );
		const int INSPECTOR = ( 1 << 2 );
		const int CREATE_GOBJ = ( 1 << 3 );

		public string assemblyQualifiedName;
		public Type type => Type.GetType( assemblyQualifiedName );


		public int flag;

		public bool enable {
			get => flag.Has( ENABLE );
			set => flag.Toggle( ENABLE, value );
		}
		public bool search {
			get => flag.Has( SEARCH );
			set => flag.Toggle( SEARCH, value );
		}
		public bool inspector {
			get => flag.Has( INSPECTOR );
			set => flag.Toggle( INSPECTOR, value );
		}
		public bool createGameObject {
			get => flag.Has( CREATE_GOBJ );
			set => flag.Toggle( CREATE_GOBJ, value );
		}

		public ComponentHandlerData( Type t ) {
			assemblyQualifiedName = t.AssemblyQualifiedName;
		}
	}
}

