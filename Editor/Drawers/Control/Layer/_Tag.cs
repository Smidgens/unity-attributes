// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TagAttribute))]
	internal class _Tag : __ControlDrawer<TagAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			Popup.Tag(ctx.position, ctx.property);
		}
	}
}

#endif