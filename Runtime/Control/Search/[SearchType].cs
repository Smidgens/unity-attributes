// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Filtering flags for [SearchType]
	/// </summary>
	[System.Flags]
	public enum ESearchType
	{
		/// <summary>
		/// You probably won't need this one...
		/// </summary>
		None = 0,
		/// <summary>
		/// Include private/internal
		/// </summary>
		NonPublic = 1,
		/// <summary>
		/// Include interfaces
		/// </summary>
		Interface = 2,
		/// <summary>
		/// Include static classes
		/// </summary>
		StaticClass = 4,
		/// <summary>
		/// Include abstract classes
		/// </summary>
		Abstract = 8,
		/// <summary>
		/// Include nested types
		/// </summary>
		Nested = 16,
		/// <summary>
		/// Include enums
		/// </summary>
		Enum = 32,
		/// <summary>
		/// Include structs
		/// </summary>
		Struct = 64,
		/// <summary>
		/// Include classes
		/// </summary>
		Class = 128,
		/// <summary>
		/// Include int, float, etc
		/// </summary>
		Primitive = 256,
		/// <summary>
		/// Has parameterless constructor
		/// </summary>
		Newable = 512,
		/// <summary>
		/// Generic types like List
		/// </summary>
		Generic = 1024,
		/// <summary>
		/// Marked [Obsolete]
		/// </summary>
		Obsolete = 2048,
		/// <summary>
		/// Marked [Serializable]
		/// </summary>
		Serializable = 4096,
		/// <summary>
		/// Include editor only assemblies
		/// </summary>
		EditorAssembly = 8192,
		/// <summary>
		/// Include delegate types
		/// </summary>
		Delegate = 16384,
		/// <summary>
		/// All & ~Interface & ~Abstract
		/// </summary>
		ConcreteClass = All & ~Interface & ~Abstract,
		/// <summary>
		/// Include every type
		/// </summary>
		All = ~0
	}
}

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public sealed class SearchTypeAttribute : __BaseControl
	{
		private const ESearchType DEFAULT_FLAGS =
		ESearchType.All
		& ~ESearchType.Obsolete
		& ~ESearchType.Serializable
		& ~ESearchType.EditorAssembly
		& ~ESearchType.Newable
		& ~ESearchType.Generic;

		public SearchTypeAttribute
		(
			ESearchType flags = DEFAULT_FLAGS,
			Type baseType = null,
			string typeFilter = null,
			string assemblyFilter = null,
			string labelFn = null
		)
		{
			this.flags = flags;
			this.typeFilter = FindFunc<Type,bool>(typeFilter);
			this.assemblyFilter = FindFunc<Assembly,bool>(assemblyFilter);
			this.labelFn = FindFunc<Type, string>(labelFn);
			if (baseType != null)
			{
				baseTypes = new []{ baseType };
			}
		}

		internal Func<Type,bool> typeFilter { get; }
		internal Func<Assembly,bool> assemblyFilter { get; }
		internal Func<Type,string> labelFn { get; }
		internal Type[] baseTypes { get; }
		internal ESearchType flags { get; }

		private static Func<T, RT> FindFunc<T, RT>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			var segments = path.Trim().Split(";");

			if (segments.Length != 2)
			{
				return null;
			}

			var type = Type.GetType(segments[1]);

			if (type == null)
			{
				return null;
			}

			var methodFlags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;

			var method = type.GetMethod(segments[0], methodFlags);

			if (method == null)
			{
				return null;
			}

			if (method.ReturnType != typeof(RT))
			{
				return null;
			}
			var pms = method.GetParameters();

			if (pms.Length != 1 || pms[0].ParameterType != typeof(T))
			{
				return null;
			}
			return (Func<T, RT>)method.CreateDelegate(typeof(Func<T, RT>));
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using System;

	[CustomPropertyDrawer(typeof(SearchTypeAttribute))]
	internal sealed class _SearchTypeAttribute : __ControlDrawer<SearchTypeAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			AssemblyTypePopup(ctx.position, ctx.property, GetConstraints);
		}

		private static readonly Lazy<GUIStyle> _BTN_LABEL_STYLE = new(() =>
		{
			var s = new GUIStyle(EditorStyles.miniLabel);
			s.fontSize = (int)(s.fontSize * 0.85f);
			s.padding = new RectOffset(4,20,0,0);
			return s;
		});

		private TypeSearch.Options GetConstraints()
		{
			return new TypeSearch.Options
			{
				flags = _Attribute.flags,
				baseTypes = _Attribute.baseTypes,
				typeFilter = _Attribute.typeFilter,
				assemblyFilter = _Attribute.assemblyFilter,
				labelFn = _Attribute.labelFn,
			};
		}

		private const string _SWITCH_GUID = "e769e4d9f339626498a12b64168231ee";

		private static readonly Rect _CLOSE_COORDS = new Rect(0.25f, 0, 0.25f, 0.25f);

		// icon atlas
		private static readonly Lazy<Texture2D> _TEX_ATLAS = new (() =>
		{
			var path = AssetDatabase.GUIDToAssetPath(_SWITCH_GUID);
			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		});

		private static bool DrawClearButton(Rect pos)
		{
			var pressed = GUI.Button(pos, GUIContent.none, GUIStyle.none);
			var color = EditorGUIUtility.isProSkin
			? Color.white * 0.7f
			: Color.black * 0.5f;
			var icoRect = pos.Resized(-pos.height * 0.2f);
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, icoRect, _CLOSE_COORDS, color);
			return pressed;
		}

		private const string _EMPTY_LABEL = "<none>";
		private const string _MISSING_LABEL = "<type missing>";

		private static GUIStyle _defaultLabelStyle;

		private static readonly GUIContent _dummyLabel = new GUIContent();
		
		private static string GetTypeLabel(Type t, float width)
		{
			var name = string.IsNullOrEmpty(t.Namespace)
			? $"{t.Assembly.GetName().Name}.{t.Name}"
			: t.FullName;

			_dummyLabel.text = name;
			var w = _BTN_LABEL_STYLE.Value.CalcSize(_dummyLabel).x;
			if (width - 15 < w)
			{
				name = t.Name;
			}
			return name;
		}

		private TypeSearch.MenuNode _cachedMenu;

		private void AssemblyTypePopup(Rect pos, SerializedProperty prop, Func<TypeSearch.Options> optsFn)
		{
			if (_defaultLabelStyle == null)
			{
				_defaultLabelStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					padding = new RectOffset(5,0,0,0),
					fontStyle = FontStyle.Bold,
				};
			}

			var label = _EMPTY_LABEL;

			Type t = null;

			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null ? GetTypeLabel(t, pos.width) : _MISSING_LABEL;
			}

			var missingType = !string.IsNullOrEmpty(prop.stringValue) && t == null;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				if (DrawClearButton(pos.SliceRight(pos.height)))
				{
					prop.stringValue = string.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				}
			}

			var brect = pos;
			
			GUI.Box(brect, GUIContent.none, EditorStyles.helpBox);

			EditorGUIUtility.AddCursorRect(brect, MouseCursor.Link);

			_dummyLabel.text = t == null ? label : string.Empty;
			_dummyLabel.tooltip = prop.stringValue;

			var shouldOpenPopup = GUI.Button(brect, _dummyLabel, EditorStyles.popup);

			if (missingType)
			{
				EditorGUI.DrawRect(brect, Color.red * 0.2f);
			}

			if(t != null)
			{
				EditorGUI.LabelField(brect, label, _BTN_LABEL_STYLE.Value);
			}

			EditorGUI.indentLevel = tIndent;

			if (shouldOpenPopup)
			{
				if (_cachedMenu == null)
				{
					_cachedMenu = TypeSearch.CreateTypeMenuTree(optsFn.Invoke());
				}

				_cachedMenu.Filter(TypeSearch.SearchFilter.Empty);

				TypeSearch.Open(brect, t, _cachedMenu, v =>
				{
					prop.stringValue = v?.AssemblyQualifiedName ?? String.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			
			
		}

	}
}

#endif