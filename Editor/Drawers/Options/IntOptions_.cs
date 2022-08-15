// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(IntOptionsAttribute))]
	internal class IntOptions_ : AttributeDrawer<IntOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			DrawerGUI.IntegerDropdown(ctx.position, ctx.property, ctx.attribute.Values);
		}
	}
}