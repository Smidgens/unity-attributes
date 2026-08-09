// smidgens @ github

// resharper disable all

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Filtering flags for [SearchType]
	/// </summary>
	[System.Flags]
	public enum ESearchTypeFlags
	{
		None = 0,
		/// <summary>
		/// Private or internal
		/// </summary>
		Hidden = 1,
		/// <summary>
		/// Include interfaces
		/// </summary>
		Interface = 2,
		/// <summary>
		/// Include static
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
		/// int, float, etc
		/// </summary>
		Primitive = 256,
		/// <summary>
		/// Has parameterless constructor
		/// </summary>
		Newable = 512,
		/// <summary>
		/// Generic template types
		/// </summary>
		Generic = 1024,
		/// <summary>
		/// Class + Newable
		/// </summary>
		NewableClass = Newable|Class,
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

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public sealed class SearchTypeAttribute : __BaseControl
	{
		public SearchTypeAttribute()
		{
		}

		private const ESearchTypeFlags DEFAULT_FLAGS =
		ESearchTypeFlags.All
		& ~ESearchTypeFlags.Generic;

		public SearchTypeAttribute(params Type[] baseTypes)
		{
			this.baseTypes = baseTypes.Length == 0 ? null : baseTypes;
			this.flags = this.flags & ~ESearchTypeFlags.Abstract;
		}

		public SearchTypeAttribute
		(
			ESearchTypeFlags flags = DEFAULT_FLAGS,
			string[] namespaces = null,
			string[] assemblies = null,
			Type baseType = null,
			bool searchbar = true,
			bool useDisplayNameAttr = false
		)
		{
			this.flags = flags;
			this.namespaces = namespaces;
			this.assemblies = assemblies;
			this.searchbar = searchbar;
			this.useDisplayNameAttr = useDisplayNameAttr;
			if (baseType != null)
			{
				this.baseTypes = new []{ baseType };
			}
		}

		internal bool searchbar { get; } = true;
		internal bool useDisplayNameAttr { get; } = true;
		internal string[] namespaces { get; }
		internal string[] assemblies  { get; }
		internal Type[] baseTypes { get; }
		internal ESearchTypeFlags flags { get; } = DEFAULT_FLAGS;
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

		private static Lazy<GUIStyle> _BTN_LABEL_STYLE = new(() =>
		{
			var s = new GUIStyle(EditorStyles.miniLabel);
			s.fontSize = (int)(s.fontSize * 0.85f);
			return s;

		});

		private TypeSearch.Options GetConstraints()
		{
			return new TypeSearch.Options
			{
				flags = _Attribute.flags,
				baseTypes = _Attribute.baseTypes,
				namespaces = _Attribute.namespaces,
				assemblies = _Attribute.assemblies,
				searchbar = _Attribute.searchbar,
				useDisplayName = _Attribute.useDisplayNameAttr
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

			var icoRect = pos.Resized(-pos.height * 0.4f);
			
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);

			DrawerGUI.DrawTex(_TEX_ATLAS.Value, icoRect, _CLOSE_COORDS, color);
			
			return pressed;
		}

		private const string _EMPTY_LABEL = "(none)";
		private const string _MISSING_LABEL = "<type missing>";

		private static GUIStyle _defaultLabelStyle;

		private static readonly GUIContent _dummyLabel = new GUIContent();
		
		private static void AssemblyTypePopup(Rect pos, SerializedProperty prop, Func<TypeSearch.Options> optsFn)
		{
			if (_defaultLabelStyle == null)
			{
				_defaultLabelStyle = new GUIStyle(EditorStyles.miniLabel)
				{
					alignment = TextAnchor.MiddleCenter
				};
			}

			var label = _EMPTY_LABEL;

			Type t = null;

			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null ? t.FullName : _MISSING_LABEL;
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

			_dummyLabel.text = string.Empty;
			_dummyLabel.tooltip = prop.stringValue;

			var shouldOpenPopup = GUI.Button(brect, _dummyLabel, EditorStyles.popup);

			if (missingType)
			{
				EditorGUI.DrawRect(brect, Color.red * 0.2f);
			}
			EditorGUI.LabelField(brect, t == null ? label : string.Empty, _defaultLabelStyle);
			

			if(t != null)
			{
				var lpos = brect;
				var lCenter = lpos.center;
				lpos.size -= new Vector2(10f, 0f);
				lpos.center = lCenter;
				EditorGUI.LabelField(lpos, label, _BTN_LABEL_STYLE.Value);
			}

			EditorGUI.indentLevel = tIndent;

			if (shouldOpenPopup)
			{
				var opts = optsFn.Invoke();
				TypeSearch.Open(brect, t, opts, v =>
				{
					prop.stringValue = v?.AssemblyQualifiedName ?? String.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			
			
		}

	}
}

#endif