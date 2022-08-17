// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System;
	using System.Linq;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class FieldActionAttribute : __BaseModifier
	{
		public bool onlyPlayMode = false;
		public bool callRoot = false;

		public FieldActionAttribute(string label, string methodName, params object[] args)
		{
			Label = label;
			Args = args;
			_methodName = methodName;
			_argTypes = args.Select(x => x.GetType()).ToArray();
		}

		public FieldActionAttribute(string methodName, params object[] args)
			: this(methodName, methodName, args)
		{ }

		internal readonly string Label = null;
		internal readonly object[] Args = null;

		internal MethodInfo GetMethod(FieldInfo field)
		{
			var type = field.FieldType;
			if (callRoot)
			{
				type = field.DeclaringType;
			}
			return type.GetMethod(_methodName, _FLAGS, null, _argTypes, null);
		}

		private readonly string _methodName = string.Empty;
		private readonly Type[] _argTypes = null;

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Instance;
	}
}