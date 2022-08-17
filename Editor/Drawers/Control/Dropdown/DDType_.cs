// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownTypeAttribute))]
	internal class DDType_ : __ControlDrawer<DropdownTypeAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
		}
	}
}