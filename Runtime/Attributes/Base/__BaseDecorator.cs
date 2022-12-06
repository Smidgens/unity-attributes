// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	// l,r,t,b
	using BoxOffset = System.ValueTuple<byte, byte, byte, byte>;
	using BoxMargin = System.ValueTuple<byte, byte>;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public abstract class __BaseDecorator : __Base
	{
		internal BoxMargin Margin = (2, 2);
		internal BoxOffset Padding = (5, 5, 5, 5);
		internal string Text = string.Empty;
		internal Color TextColor = Color.white;
		internal float BoxOpacity = 0.4f;
		internal Color BackgroundColor = Color.white;

		protected void SetDefaults()
		{
			order = 1;
		}

		protected static Color Parse(in string c, in Color defaultValue)
		{
			if (string.IsNullOrEmpty(c)) { return defaultValue; }
			if (ColorUtility.TryParseHtmlString(c, out var r))
			{
				return r;
			}
			return defaultValue;
		}
	}
}