// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Dropdown of Type values (saved as string)
	/// </summary>
	public sealed class DropdownTypeAttribute : __BaseControl
	{
		public DropdownTypeAttribute(params Type[] types)
		{
			Types = types ?? Array.Empty<Type>();
		}
		internal Type[] Types { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownTypeAttribute))]
	internal sealed class _DropdownTypeAttribute : __ControlDrawer<DropdownTypeAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
		}
	}
}

#endif