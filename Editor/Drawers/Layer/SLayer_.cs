// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SortLayerAttribute))]
	internal class SLayer_ : AttributeDrawer<SortLayerAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			Popup.SLayer(ctx.position, ctx.property);
		}
	}
}