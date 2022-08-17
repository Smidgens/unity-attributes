// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	internal class Slider_ : __ControlDrawer<SliderAttribute>
	{
		protected override void OnField(in FieldContext ctx)
		{
			var a = ctx.attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, a.Min, a.Max, a.Step, a.Precision);
		}
	}
}