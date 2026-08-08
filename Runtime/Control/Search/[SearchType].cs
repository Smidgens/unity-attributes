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

		private TypeSearch.Constraints GetConstraints()
		{
			if (!_popupInit) { InitConstraints(); }
			return _constraints;
		}

		private TypeSearch.Constraints _constraints = default;
		
		private static void AssemblyTypePopup(in Rect pos, SerializedProperty prop, Func<TypeSearch.Constraints> optsFn)
		{
			var label = "...";

			Type t = null;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null
					? t.FullName
					: "<type missing>";
			}

			var brect = pos;
			brect.width -= pos.height + 2f;

			var clearRect = pos;
			clearRect.width = pos.height;
			clearRect.position += new Vector2(brect.width + 2f, 0f);

			GUI.Box(brect, "", EditorStyles.helpBox);

			EditorGUIUtility.AddCursorRect(brect, MouseCursor.Link);

			var c = GUI.color;
			GUI.color *= 0.5f;
			if (GUI.Button(brect, t == null ? label : ""))
			{
				var opts = optsFn.Invoke();
				TypeSearch.Open(brect, t, opts, v =>
				{
					prop.stringValue = v?.AssemblyQualifiedName ?? "";
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			GUI.color = c;

			if(t != null)
			{
				var lpos = brect;
				lpos.width -= 10f;
				lpos.position += new Vector2(5f, 0f);
				EditorGUI.LabelField(lpos, label, EditorStyles.miniLabel);
			}

			using (new EditorGUI.DisabledGroupScope(t == null))
			{
				if (GUI.Button(clearRect, "", EditorStyles.miniButton))
				{
					prop.stringValue = "";
					prop.serializedObject.ApplyModifiedProperties();
				}
				
			}
			EditorGUI.LabelField(clearRect, "x", EditorStyles.centeredGreyMiniLabel);
		}

	}
}

#endif