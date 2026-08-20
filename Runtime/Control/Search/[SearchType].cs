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
		Static = 4,
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
		/// Include editor-only assembly types
		/// </summary>
		EditorAssembly = 8192,
		/// <summary>
		/// Include delegate types
		/// </summary>
		Delegate = 16384,
		/// <summary>
		/// System.Attribute
		/// </summary>
		Attribute = 32768,
		/// <summary>
		/// Value types only
		/// </summary>
		ValueType = Primitive|Enum|Struct,
		/// <summary>
		/// Skips irrelevant types such as ones marked Obsolete
		/// </summary>
		RuntimeRelevant = ~(EditorAssembly|Obsolete),
		/// <summary>
		/// Concrete type that can be instantiated
		/// </summary>
		ConcreteType = (Class|Struct) & ~(Interface|Abstract|Generic),
		/// <summary>
		/// Can be instantiated for [SerializeReference] fields
		/// </summary>
		ReferenceSerializable = ~Attribute & (Class|Serializable|Newable),
		/// <summary>
		/// Include non-public and nested types
		/// </summary>
		AnyAccessor = NonPublic|Nested,
		/// <summary>
		/// Sensible defaults
		/// </summary>
		Default = All & RuntimeRelevant & ~Serializable & ~Newable,
		/// <summary>
		/// Include every type
		/// </summary>
		All = ~0
	}
}

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using Editor;
	using UnityEngine;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public sealed class SearchTypeAttribute : __BaseControl
	{
		public SearchTypeAttribute
		(
			ESearchType flags = ESearchType.Default,
			Type baseType = null,
			string typeFilter = null,
			string assemblyFilter = null,
			string labelFn = null,
			bool hideIcon = false
		)
		{
			this.flags = flags;
			this.typeFilter = FindFunc<Type,bool>(typeFilter);
			this.assemblyFilter = FindFunc<Assembly,bool>(assemblyFilter);
			this.labelFn = FindFunc<Type, string>(labelFn) ?? GetTypeDisplayName;
			if (baseType != null)
			{
				baseTypes = new []{ baseType };
			}
			this.hideIcon = hideIcon;
		}

		internal bool hideIcon { get; }
		internal Func<Type,bool> typeFilter { get; }
		internal Func<Assembly,bool> assemblyFilter { get; }
		internal Func<Type,string> labelFn { get; }
		internal Type[] baseTypes { get; }
		internal ESearchType flags { get; }
		
		private static string GetTypeDisplayName(Type type)
		{
			var dn = type.GetCustomAttribute<DisplayNameAttribute>();
			if (dn != null)
			{
				return dn.DisplayName;
			}
			if (type.IsNested && type.DeclaringType != null)
			{
				return $"{type.DeclaringType.Name}.{type.Name}";
			}
			return type.Name;
		}

		private static Func<T, RT> FindFunc<T, RT>(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			int i = path.IndexOf(';');

			if (i < 0)
			{
				return null;
			}

			var typeName = path.Substring(i + 1);
			var methodName = path.Substring(0,i);
			return Type.GetType(typeName)?.GetStaticMethodDelegate<T, RT>(methodName);
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
			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			AssemblyTypePopup(ctx.position, ctx.property, GetConstraints);
			EditorGUI.indentLevel = tIndent;
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
		
		private static bool DrawClearButton(Rect pos)
		{
			var pressed = GUI.Button(pos, GUIContent.none, GUIStyle.none);
			var color = EditorGUIUtility.isProSkin
			? Color.white * 0.7f
			: Color.black * 0.5f;
			var icoRect = pos.Resized(-pos.height * 0.2f);
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Close, color);
			return pressed;
		}

		private static GUIStyle _defaultLabelStyle;
		private static readonly GUIContent _dummyLabel = new ();
		
		private string GetTypeLabel(Type t, float width)
		{
			return _Attribute.labelFn.Invoke(t);
			// var name = string.IsNullOrEmpty(t.Namespace)
			// ? $"{t.Assembly.GetName().Name}.{t.Name}"
			// : t.FullName;
			//
			// _dummyLabel.text = name;
			// var w = _BTN_LABEL_STYLE.Value.CalcSize(_dummyLabel).x;
			// if (width - 15 < w)
			// {
			// 	name = t.Name;
			// }
			// return name;
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

			var label = PluginConstants.Label.POPUP_UNSET;

			Type t = null;

			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			// var icoRect = pos.SliceLeft(pos.height).Resized(-pos.height * 0.1f);
			// PluginAtlas.DrawIcon(icoRect, EAtlasIcon.Code, _ICON_COLOR);

			if (!_Attribute.hideIcon)
			{
				DrawerGUI.DrawControlPrefixIcon(ref pos, EAtlasIcon.CurlyBrackets);
			}

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null ? GetTypeLabel(t, pos.width) :  PluginConstants.Label.MISSING;
			}

			var missingType = !string.IsNullOrEmpty(prop.stringValue) && t == null;

			var clearRect = !string.IsNullOrEmpty(prop.stringValue)
			? pos.SliceRight(pos.height)
			: default;

			var brect = pos;
			
			// _dummyLabel.text = t == null ? label : string.Empty;
			_dummyLabel.text = label;
			_dummyLabel.tooltip = prop.stringValue;
			var shouldOpenPopup = EditorGUI.DropdownButton(brect, _dummyLabel, FocusType.Keyboard);

			if (missingType)
			{
				EditorGUI.DrawRect(brect, Color.red * 0.2f);
			}

			if(t != null)
			{
				// EditorGUI.LabelField(brect, label, _BTN_LABEL_STYLE.Value);
			}

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				if (DrawClearButton(clearRect))
				{
					prop.stringValue = string.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				}
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