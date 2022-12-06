// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(LayerAttribute))]
	internal class Layer_ : __ControlDrawer<LayerAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			DrawerGUI.Layer(ctx.position, ctx.property);
		}
	}
}

#endif