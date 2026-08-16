// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Wraps field in box
	/// </summary>
	public sealed class BoxAttribute : __BaseControl
	{
		public BoxAttribute(): base(false) {}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	[CustomPropertyDrawer(typeof(BoxAttribute))]
	internal sealed class _BoxAttribute : __ControlDrawer<BoxAttribute>
	{
		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var h = EditorGUI.GetPropertyHeight(prop, label);
			var s = EditorStyles.helpBox;
			h += s.padding.bottom;
			h += s.padding.top;
			return h;
		}

		protected override void OnField(in DrawContext ctx)
		{
			GUI.Box(ctx.position, GUIContent.none, EditorStyles.helpBox);
			var pos = ctx.position.Padded(EditorStyles.helpBox.padding);
			EditorGUI.PropertyField(pos, ctx.property, ctx.label);
		}
		
		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
		
		}

	}
}

#endif