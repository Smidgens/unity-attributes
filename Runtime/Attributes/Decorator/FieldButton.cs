// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System;
	using System.Linq;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class FieldButtonAttribute : __BaseModifier
	{
		public FieldButtonAttribute(string label, string methodName, params object[] args)
		{
			Label = label;
			_args = args;
			_methodName = methodName;
			_types = args.Select(x => x.GetType()).ToArray();
		}

		internal void Invoke(object target)
		{
			if(target == null) { return; }
			var m = GetMethod(target.GetType());
			if(m == null) { return; }
			m.Invoke(m.IsStatic ? null : target, _args);
		}

		internal MethodInfo GetMethod(Type owner)
		{
			if (!_cache.Item2)
			{
				if (owner.IsArray) { owner = owner.GetElementType(); }
				var m = owner.GetMethod(_methodName, _FLAGS, null, _types, null);
				_cache = (m, true);
			}
			return _cache.Item1;
		}

		internal string Label { get; } = "";

		private string _methodName = "";
		private object[] _args = { };
		private Type[] _types = { };
		private (MethodInfo, bool) _cache = default;

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static
		| BindingFlags.Instance;
	}
}