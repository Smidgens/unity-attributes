// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(Slider01Attribute))]
	internal class _Slider01 : __ControlDrawer<Slider01Attribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			var a = _Attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, 0f, 1f, a.Step, a.Precision);
		}
	}
}

#endif