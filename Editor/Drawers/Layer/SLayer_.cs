// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SLayerAttribute))]
	internal class SLayer_ : AttributeDrawer<SLayerAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			Popup.SLayer(ctx.position, ctx.property);
		}
	}
}