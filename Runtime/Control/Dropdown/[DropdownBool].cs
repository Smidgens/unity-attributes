// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public sealed class DropdownBoolAttribute : __BaseControl
	{
		public static readonly string[] DEFAULT_LABELS = { "False", "True" };
		public string[] Labels { get; } = DEFAULT_LABELS;
		public DropdownBoolAttribute() { }

		public DropdownBoolAttribute(string l0, string l1)
		{
			Labels = new string[] { l0 ?? DEFAULT_LABELS[0], l1 ?? DEFAULT_LABELS[1] };
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownBoolAttribute))]
	internal sealed class _DropdownBoolAttribute : __ControlDrawer<DropdownBoolAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Bool;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
			var prop = ctx.property;
			prop.boolValue = EditorGUI.Popup(ctx.position, ctx.property.boolValue.ToInt(), _Attribute.Labels).ToBool();
		}
	}
}

#endif