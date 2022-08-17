// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	public class StaticActionAttribute : __BaseDecorator
	{
		public bool onlyPlayMode = false;

		public StaticActionAttribute
		(
			string methodName,
			Type declaringType,
			params object[] args
		) : this(methodName, methodName, declaringType, args) { }

		public StaticActionAttribute
		(
			string label,
			string methodName,
			Type declaringType,
			params object[] args
		)
		{
			order = 2;

			Label = label;
			_methodName = methodName;
			_declaringType = declaringType;
			Args = args;
			_argTypes = new Type[args.Length];
			for(var i = 0; i < args.Length; i++)
			{
				_argTypes[i] = args[i].GetType();
			}
		}

		internal readonly object[] Args = null;
		internal readonly string Label = null;

		internal MethodInfo GetMethod()
		{
			if (!_method.Item2)
			{
				var m =
				_declaringType.GetMethod(_methodName, _FLAGS, null, _argTypes, null);

				_method = (m, true);
			}
			return _method.Item1;
		}

		private readonly Type _declaringType = null;
		private readonly string _methodName = null;
		private readonly Type[] _argTypes = null;
		private (MethodInfo, bool) _method = default; // lazy cache

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static;

	}
}