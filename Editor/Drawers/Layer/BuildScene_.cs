// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BuildSceneAttribute))]
	internal class BuildScene_ : AttributeDrawer<BuildSceneAttribute>
	{
		protected override PType GetSupportedTypes() => PType.String | PType.Int;

		protected override void DrawField(in FieldContext ctx)
		{
			Popup.Scene(ctx.position, ctx.property);
		}
	}
}