// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(HexColorAttribute))]
	internal class HexColor_ : __ControlDrawer<HexColorAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			ColorGUI.HexColor(ctx.position, ctx.property);
		}
	}
}

#endif