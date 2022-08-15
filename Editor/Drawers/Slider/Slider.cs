// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	internal class Slider_ : AttributeDrawer<SliderAttribute>
	{
		protected override void DrawField(in FieldContext ctx)
		{
			var a = ctx.attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, a.Min, a.Max, a.Step, a.Precision);
		}
	}
}