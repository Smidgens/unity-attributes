﻿// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	internal class _Slider : __ControlDrawer<SliderAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			var a = _Attribute;
			DrawerGUI.Slider(ctx.position, ctx.property, a.Min, a.Max, a.Step, a.Precision);
		}
	}
}

#endif