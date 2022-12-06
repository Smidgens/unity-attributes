// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownTypeAttribute))]
	internal class _Dropdown_Type : __ControlDrawer<DropdownTypeAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
		}
	}
}

#endif