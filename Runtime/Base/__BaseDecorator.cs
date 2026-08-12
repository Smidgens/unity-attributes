// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public abstract class __BaseDecorator : __Base
	{
		protected static Color Parse(in string c, in Color defaultValue)
		{
			if (ColorUtility.TryParseHtmlString(c ?? string.Empty, out var r))
			{
				return r;
			}
			return defaultValue;
		}
	}
}