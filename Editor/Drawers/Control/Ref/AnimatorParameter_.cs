// smidgens @ github

#if UNITY_EDITOR && ATTRIBUTES_ANIMATION_1

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	internal class AnimatorParameter_ : __ControlDrawer<AnimatorParameterAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			DrawerGUI.AnimatorParameter(ctx.position, ctx.property, _Attribute.AnimatorField);
		}
	}
}

#endif