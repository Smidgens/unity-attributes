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
		protected override void OnField(in DrawContext ctx)
		{
			EditorGUI.PropertyField(ctx.position, ctx.property, GUIContent.none);
		}
	}
}

#endif