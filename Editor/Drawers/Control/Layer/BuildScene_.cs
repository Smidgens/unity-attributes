// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(BuildSceneAttribute))]
	internal class BuildScene_ : __ControlDrawer<BuildSceneAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String | FieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			Popup.Scene(ctx.position, ctx.property);
		}
	}
}

#endif