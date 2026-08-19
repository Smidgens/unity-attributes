// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Default control
	/// </summary>
	public sealed class DefaultDrawerAttribute : __BaseControl
	{
		public DefaultDrawerAttribute() : base(true)
		{
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DefaultDrawerAttribute))]
	internal sealed class _DefaultDrawerAttribute : __ControlDrawer<DefaultDrawerAttribute>
	{
		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
			// handling label elsewhere
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(prop, label, prop.isExpanded);
		}

		protected override void OnField(in DrawContext ctx)
		{
			EditorGUI.PropertyField(ctx.position, ctx.property, ctx.label, ctx.property.isExpanded);
		}
	}
}

#endif