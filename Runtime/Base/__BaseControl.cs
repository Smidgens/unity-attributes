// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public abstract class __BaseControl : __Base
	{
		public bool buttons = true;
	}

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