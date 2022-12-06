// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SortLayerAttribute))]
	internal class _SortLayer : __ControlDrawer<SortLayerAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			Popup.SLayer(ctx.position, ctx.property);
		}
	}
}

#endif