// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	public sealed class DropdownIntAttribute : __BaseControl
	{
		internal int[] Values { get; }

		public DropdownIntAttribute(int start, int n)
		{
			Values = GetValues(start, n);
		}

		public DropdownIntAttribute(params int[] values)
		{
			Values = values ?? Array.Empty<int>();
		}

		private static int[] GetValues(int start, int n)
		{
			if (n <= 0) { return Array.Empty<int>(); }
			int[] values = new int[n];
			for (var i = 0; i < n; i++) { values[i] = start + i; }
			return values;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(DropdownIntAttribute))]
	internal sealed class _DropdownIntAttribute : __ControlDrawer<DropdownIntAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			DrawerGUI.IntegerDropdown(ctx.position, ctx.property, _Attribute.Values);
		}
	}
}

#endif