// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;

	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class EventFoldoutAttribute : __BaseControl
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(EventFoldoutAttribute))]
	internal sealed class _EventFoldoutAttribute : __ControlDrawer<EventFoldoutAttribute>
	{
		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var h = _FOLDOUT_STYLE.Value.CalcHeight(GUIContent.none, 100);
			if (prop.isExpanded)
			{
				h += EditorGUIUtility.standardVerticalSpacing;
				h += EditorGUI.GetPropertyHeight(prop, GUIContent.none);
				h += _PAD;
			}
			h += _PAD * 2f;
			return h;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var property = ctx.property;
			var position = ctx.position;

			var customLabel = GetCustomLabel();

			if (!string.IsNullOrEmpty(customLabel))
			{
				ctx.label.text = customLabel;
			}

			var count = property.FindPropertyRelative("m_PersistentCalls.m_Calls").arraySize;
			
			var foldoutHeight = _FOLDOUT_STYLE.Value.CalcHeight(GUIContent.none, 100);
			
			GUI.Box(position, GUIContent.none);
			GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

			position = position.Resized(-_PAD);
			var foldoutRect = position.SliceTop(foldoutHeight);

			EditorGUI.indentLevel++;
			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, ctx.label, true);
			EditorGUI.indentLevel--;
			
			GUI.Label(foldoutRect, count.ToString(), _RIGHT_LABEL_STYLE.Value);

			if (property.isExpanded)
			{
				position.SliceTop(_PAD);
				DrawerGUI.IndentRect(ref position, 1);
				EditorGUI.PropertyField(position, property, GUIContent.none);
			}
		}

		protected override void OnLabel(ref Rect pos, GUIContent l)
		{
			
		}

		private const float _PAD = 5f;

		private static readonly Lazy<GUIStyle> _FOLDOUT_STYLE = new(() => new GUIStyle(EditorStyles.foldout)
		{

		});

		private static readonly Lazy<GUIStyle> _RIGHT_LABEL_STYLE = new(() => new GUIStyle(EditorStyles.miniLabel)
		{
			alignment = TextAnchor.MiddleRight
		});
	}
}

#endif