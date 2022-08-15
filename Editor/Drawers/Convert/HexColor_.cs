// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(HexColorAttribute))]
	internal class HexColor_ : AttributeDrawer<HexColorAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String;

		protected override void DrawField(in FieldContext ctx)
		{
			ColorGUI.HexColor(ctx.position, ctx.property);
		}
	}
}