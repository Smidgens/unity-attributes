// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public sealed class EventFoldoutAttribute : __BaseControl
	{
		public EventFoldoutAttribute() : base(false)
		{
			
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	[CustomPropertyDrawer(typeof(EventFoldoutAttribute))]
	internal sealed class _EventFoldoutAttribute : __ControlDrawer<EventFoldoutAttribute>
	{
		protected override void OnInit()
		{
			_isEvent = typeof(UnityEventBase).IsAssignableFrom(fieldInfo.FieldType.GetInnermostType());
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			if (!_isEvent)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			var h = DrawerStyles.FoldoutHeight;
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
			if (!_isEvent)
			{
				DrawerGUI.MutedInfo(ctx.position, "Field should be event");
				return;
			}
			
			var property = ctx.property;
			var position = ctx.position;

			var customLabel = GetCustomLabel();

			if (!string.IsNullOrEmpty(customLabel))
			{
				ctx.label.text = customLabel;
			}

			var count = property.GetEventListenerCount();

			var foldoutHeight = DrawerStyles.FoldoutHeight;
			
			GUI.Box(position, GUIContent.none);
			GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

			position = position.Resized(-_PAD);
			var foldoutRect = position.SliceTop(foldoutHeight);

			EditorGUI.indentLevel++;
			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, ctx.label, true, DrawerStyles.Foldout);
			EditorGUI.indentLevel--;

			var tAlignment = DrawerStyles.LabelSM.alignment;
			DrawerStyles.LabelSM.alignment = TextAnchor.MiddleRight;
			GUI.Label(foldoutRect, count.ToString(), DrawerStyles.LabelSM);
			DrawerStyles.LabelSM.alignment = tAlignment;

			if (property.isExpanded)
			{
				position.SliceTop(_PAD);
				position.SliceLeft(DrawerGUI.INDENT_W * 0.5f);
				EditorGUI.PropertyField(position, property, GUIContent.none);
			}
		}
		
		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
			if (!_isEvent)
			{
				base.OnLabel(ref pos, prop, l);
			}
		}

		private const float _PAD = 5f;
		private bool _isEvent;
	}
}

#endif