// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(Slider01Attribute))]
	internal class Slider01_ : __ControlDrawer<Slider01Attribute>
	{
		protected override void OnField(in FieldContext ctx)
		{
			var a = ctx.attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, 0f, 1f, a.Step, a.Precision);
		}
	}
}