// smidgens @ github

#if ANIMATION_ATTRIBUTES

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	internal class AnimatorParameter_ : AttributeDrawer<AnimatorParameterAttribute>
	{
		protected override void DrawField(in FieldContext ctx)
		{
			DrawerGUI.AnimatorParameter(ctx.position, ctx.property, ctx.attribute.AnimatorField);
		}
	}
}

#endif