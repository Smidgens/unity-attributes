// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(AssemblyTypeAttribute))]
	internal class AssemblyType_ : AttributeDrawer<AssemblyTypeAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String;

		protected override void DrawField(in FieldContext ctx)
		{
			Popup.AssemblyType(ctx.position, ctx.property);
		}
	}
}