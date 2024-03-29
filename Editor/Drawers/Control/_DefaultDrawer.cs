// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DefaultDrawerAttribute))]
	internal class _DefaultDrawer : __ControlDrawer<DefaultDrawerAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			EditorGUI.PropertyField(ctx.position, ctx.property, GUIContent.none);
		}
	}
}

#endif