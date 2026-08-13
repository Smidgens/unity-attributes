// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draw structs expanded out
	/// </summary>
	public sealed class ExpandAttribute : __BaseControl
	{
		public ExpandAttribute(bool innerOnly = false) : base(true)
		{
			this.innerOnly = innerOnly;
		}

		internal bool innerOnly { get; }
	}
}

/*
 * TODOS
 *	- support arrays
 */

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(ExpandAttribute))]
	internal sealed class _ExpandAttribute : __ControlDrawer<ExpandAttribute>
	{
		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var total = Mathf.Max(0f, EditorGUIUtility.standardVerticalSpacing * (_fields.Count - 1));

			var showLabel = !_Attribute.innerOnly && GetCustomLabel() != null;
			
			if (showLabel)
			{
				total += EditorGUIUtility.standardVerticalSpacing;
				total += EditorGUIUtility.singleLineHeight;
			}

			foreach (var f in _fields)
			{
				total += EditorGUI.GetPropertyHeight(prop.FindPropertyRelative(f.Name));
			}

			return total;
		}

		protected override void OnLabel(ref Rect pos, GUIContent l) { }

		protected override void OnField(in DrawContext ctx)
		{
			var tIndent = EditorGUI.indentLevel;

			var pos = ctx.position;

			var customLabel = GetCustomLabel();

			var showLabel = !_Attribute.innerOnly && customLabel != null;
			
			if (showLabel)
			{
				var labelRect = pos.SliceTop(EditorGUIUtility.singleLineHeight);
				var label = ctx.label;

				if (customLabel != string.Empty)
				{
					label.text = customLabel;
				}
				EditorGUI.LabelField(labelRect, label);
				pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}
			var indent = showLabel ? 1 : 0;
			EditorGUI.indentLevel += indent;
			foreach (var f in _fields)
			{
				var p = ctx.property.FindPropertyRelative(f.Name);
				var h = EditorGUI.GetPropertyHeight(p);
				var fRect = pos.SliceTop(h);
				fRect =	EditorGUI.PrefixLabel(fRect, new GUIContent(p.displayName));
				EditorGUI.PropertyField(fRect, p, GUIContent.none);
				pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
			}
			EditorGUI.indentLevel -= indent;
			EditorGUI.indentLevel = tIndent;
		}

		protected override void OnInit()
		{
			_fields = fieldInfo.FieldType.GetInnermostType().FindInspectorFields<object>();
		}

		private IReadOnlyList<FieldInfo> _fields;



	}
}

#endif