// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System;
	using Editor;

	/// <summary>
	/// Displays action above specific field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FieldButtonAttribute : __BaseModifier
	{
		internal const char OUTER_TOKEN = '~';

		public FieldButtonAttribute
		(
			string methodName,
			EFieldUsable flags = EFieldUsable.Always,
			string label = null
		)
		{
			if (methodName.Length >= 2 && methodName.StartsWith(OUTER_TOKEN))
			{
				useOuter = true;
				methodName = methodName.Substring(1);
			}

			_methodName = methodName;
			this.label = label;
			this.flags = flags;
		}

		internal string label { get; }
		internal EFieldUsable flags { get; }
		internal bool useOuter { get; }

		internal MethodInfo GetMethod(FieldInfo field)
		{
			var type = field.FieldType.GetInnermostType();
			if (useOuter)
			{
				type = field.DeclaringType;
			}
			var m = (type!).GetMethod(_methodName, _FLAGS, null, Array.Empty<Type>(), null);
			if (m == null || m.ReturnType != typeof(void))
			{
				return null;
			}
			return m;
		}

		private string _methodName { get; }

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Instance;
	}
}