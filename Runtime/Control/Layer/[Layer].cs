// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of layers
	/// </summary>
	public sealed class LayerAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(LayerAttribute))]
	internal sealed class _LayerAttribute : __ControlDrawer<LayerAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			DrawerGUI.Layer(ctx.position, ctx.property);
		}
	}
}

#endif