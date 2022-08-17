// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;
	using System.Diagnostics;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public abstract class __BaseControl : PropertyAttribute
	{
		public bool buttons = false;
		internal readonly ModCache<PrefixLabelAttribute> Prefix;
		internal readonly ModCache<IndentAttribute> Indent;
	}

	public abstract class __BaseDropdown : __BaseControl
	{
		internal string[] Labels { get; } = { };
	}
}


namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System.Diagnostics;
	using System.Reflection;
	using System.Linq;

	[Conditional("UNITY_EDITOR")]
	public abstract class __BaseModifier : PropertyAttribute { }

	internal struct ModCache<T> where T : __BaseModifier
	{
		public T FromField(FieldInfo field)
		{
			if (!_cache.Item2)
			{
				var a = field.GetCustomAttributes<T>().FirstOrDefault();
				_cache = (a, true);
			}
			return _cache.Item1;
		}
		private (T, bool) _cache;
	}
}


namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System.Diagnostics;

	// l,r,t,b
	using BoxOffset = System.ValueTuple<byte, byte, byte, byte>;
	using BoxMargin = System.ValueTuple<byte, byte>;

	[Conditional("UNITY_EDITOR")]
	public abstract class __BaseDecorator : PropertyAttribute
	{
		internal BoxMargin Margin { get; set; } = (2, 2);
		internal BoxOffset Padding { get; set; } = (5, 5, 5, 5);
		internal string Text { get; set; } = string.Empty;
		internal Color TextColor { get; set; } = Color.white;
		internal float BoxOpacity { get; set; } = 0.4f;
		internal Color BackgroundColor { get; set; } = Color.white;

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