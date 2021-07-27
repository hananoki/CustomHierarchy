using HananokiEditor.Extensions;
using HananokiRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityReflection;
using E = HananokiEditor.CustomHierarchy.EditorPref;
using UnityObject = UnityEngine.Object;


namespace HananokiEditor.CustomHierarchy {

	internal static class ComponentHandler {

		static Dictionary<int, ComponentObjects> s_componets;

		internal static Hashtable m_componetTool;
		internal static List<Type> m_componetToolTypes;

		public class ComponentObjects {
			public Component[] components;
			public Component[] inspec;
			public HierarchyComponentTool[] tools;

			public HierarchyComponentTool GetTool<T>() {
				foreach( var p in tools ) {
					if( p.type == typeof( T ) ) return p;
				}
				return null;
			}
		}

		static ComponentObjects s_current;

		static GameObject go => Main.go;


		public static void Reset() {
			s_componets = new Dictionary<int, ComponentObjects>();
			Helper.New( ref m_componetToolTypes );

			if( E.i.componentHandler ) {
				if( m_componetTool == null ) {
					m_componetTool = new Hashtable();
					foreach( var type in AssemblieUtils.GetAllTypesWithAttribute<Hananoki_Hierarchy_ComponentTool>() ) {
						foreach( var attr in type.GetCustomAttributes( true ) ) {
							if( typeof( Hananoki_Hierarchy_ComponentTool ) != attr.GetType() ) continue;
							var atb = (Hananoki_Hierarchy_ComponentTool) attr;
							//var obj = (HierarchyComponentTool) Activator.CreateInstance( type );
							//obj.type = atb.type;
							m_componetToolTypes.Add( atb.type );
							m_componetTool.Add( atb.type, type );
							break;
						}
					}
				}
			}
			else {
				m_componetTool = null;
			}
		}

		public static HierarchyComponentTool GetTool<T>() {
			return s_current.GetTool<T>();
		}


		public static void Execute( Rect selectionRect ) {
			if( E.i.プレイモード時はコンポーネントツールを消す ) {
				if( Application.isPlaying ) return;
			}

			Helper.New( ref s_componets );
			s_componets.TryGetValue( go.GetInstanceID(), out s_current );

			if( s_current == null ) {
				s_current = new ComponentObjects {
					components = go.GetComponents( typeof( Component ) ).Where( x => x != null ).ToArray(),
				};

				var lst = new List<HierarchyComponentTool>();
				var inspectLst = new List<Component>();
				foreach( var c in s_current.components ) {

					var ctype = c.GetType();

					foreach( var t in m_componetToolTypes ) {
						if( !E.HasShowTool( ctype ) ) continue;

						if( t.IsAssignableFrom( ctype ) ) {
							var tool = (HierarchyComponentTool) Activator.CreateInstance( (Type) m_componetTool[ t ] );
							tool.type = t;
							tool.obj = c;
							tool.go = c.gameObject;
							lst.Add( tool );
						}
					}

					if( E.HasInspecClass( ctype ) ) {
						inspectLst.Add( c );
					}
				}
				s_current.tools = lst.ToArray();
				s_current.inspec = inspectLst.ToArray();
				s_componets.Add( go.GetInstanceID(), s_current );
			}

			var rc = selectionRect;

			var pos = HEditorStyles.treeViewLine.CalcSize( EditorHelper.TempContent( go.name ) );
			rc.x += pos.x + 20;
			rc.x += 4;



			foreach( var c in s_current.inspec ) {
				_InspectorButton( ref rc, c );
			}

			if( 0 < E.i.componentToolPos ) {
				rc.x = E.i.componentToolPos;
			}
			foreach( var p in s_current.tools ) {
				p?.OnGUI( ref rc );
			}
		}




		static void _InspectorButton( ref Rect rect, UnityObject obj, UnityObject icont = null ) {
			rect.width = 16;
			var icon = (Texture2D) obj.ObjectContent().image;
			//if( obj.GetType() == typeof( SpriteRenderer ) ) {
			//	icon = ( (SpriteRenderer) obj ).sprite.GetCachedIcon();
			//}
			//else if( obj.GetType() == typeof( Image ) ) {
			//	icon = ( (Image) obj ).sprite.GetCachedIcon();
			//}
			//else if( obj.GetType() == typeof( RawImage ) ) {
			//	icon = AssetPreview.GetAssetPreview( ( (RawImage) obj ).texture );
			//}
			//else if( icon == null ) {
			//	icon = obj.GetType().GetIcon();
			//}

			//if( icont != null ) {
			//	if( icont.GetType() == typeof( Sprite ) ) {
			//		icon = icont.GetCachedIcon();
			//	}
			//	else {
			//		icon = AssetPreview.GetAssetPreview( icont );
			//	}
			//}

			if( EditorHelper.HasMouseClick( rect, EventMouseButton.R ) ) {
				UnityEditorEditorUtility.DisplayObjectContextMenu( rect, obj, 0 );
			}
			if( HEditorGUI.IconButton( rect, icon ) ) {
				if( !Event.current.control ) {
					ComponentEditorWindow.Open( obj );
				}
				else {
					PopupContentComponentEditor.Show( rect, obj );
				}
				//ComponentPopupWindow.Open( s_current.image, ( b ) => { } );
				Event.current.Use();
			}
			rect.x += 20;
		}

		//static void _ObjectLinkText( ref Rect rect, UnityObject obj ) {
		//	if( Helper.IsNull( obj ) ) return;
		//	var nam = obj.name;
		//	rect.width = HEditorStyles.treeViewLine.CalcSize( EditorHelper.TempContent( nam ) ).x;
		//	if( HEditorGUI.FlatButton( rect, nam, HEditorStyles.treeViewLine ) ) {
		//		EditorHelper.PingObject( obj );
		//	}
		//	rect.x += ( rect.width + 4 );
		//}
	}
}
