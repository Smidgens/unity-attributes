// smidgens @ github

// resharper disable all

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public sealed class SearchTypeAttribute : __BaseControl
	{
		public bool hideNested = true;
		public bool hideAbstract = false;
		public bool showHidden = false;

		public bool onlyStatic = false;
		public bool onlyInterfaces = false;

		public string[] namespaces = null;
		public string[] assemblies = null;
		public Type[] baseTypes = null;
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

		private bool _popupInit = false;

		private void InitConstraints()
		{
			_constraints = new TypeSearch.Constraints();
			_constraints.showAbstract = !_Attribute.hideAbstract;
			_constraints.staticOnly = _Attribute.onlyStatic;
			_constraints.derivedTypes = _Attribute.baseTypes;
			_constraints.namespaces = _Attribute.namespaces;
			_constraints.assemblies = _Attribute.assemblies;
			_constraints.includeHidden = _Attribute.showHidden;
			_popupInit = true;
		}

		private static Lazy<GUIStyle> _BTN_LABEL_STYLE = new(() =>
		{
			var s = new GUIStyle(EditorStyles.miniLabel);
			s.fontSize = (int)(s.fontSize * 0.85f);
			return s;

		});
		
		private TypeSearch.Constraints GetConstraints()
		{
			if (!_popupInit) { InitConstraints(); }
			return _constraints;
		}

		private TypeSearch.Constraints _constraints = default;
		
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

			var icoRect = pos.Resize(-pos.height * 0.4f);
			
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);

			DrawerGUI.DrawTex(_TEX_ATLAS.Value, icoRect, _CLOSE_COORDS, color);
			
			return pressed;
		}
		
		private static void AssemblyTypePopup(Rect pos, SerializedProperty prop, Func<TypeSearch.Constraints> optsFn)
		{
			var label = "(none)";

			Type t = null;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null
				? t.FullName
				: "<type missing>";
			}

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

			var c = GUI.color;
			// GUI.color *= 0.5f;
			if (GUI.Button(brect, GUIContent.none))
			{
				var opts = optsFn.Invoke();
				TypeSearch.Open(brect, t, opts, v =>
				{
					prop.stringValue = v?.AssemblyQualifiedName ?? String.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			GUI.color = c;
			EditorGUI.LabelField(brect, t == null ? label : "", EditorStyles.centeredGreyMiniLabel);

			if(t != null)
			{
				var lpos = brect;
				var lCenter = lpos.center;
				lpos.size -= new Vector2(10f, 0f);
				lpos.center = lCenter;
				var tIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;
				EditorGUI.LabelField(lpos, label, _BTN_LABEL_STYLE.Value);
				EditorGUI.indentLevel = tIndent;
			}

			
			
		}

	}
}

#endif