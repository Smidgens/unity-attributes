// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System;
	using Editor;
	using UnityEngine;

	/// <summary>
	/// Displays action above specific field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FieldButtonAttribute : __BaseModifier
	{
		internal const char OUTER_TOKEN = '~';
		internal const float MIN_W = 0.1f;

		public FieldButtonAttribute
		(
			string method,
			EFieldUsable flags = EFieldUsable.Always,
			string label = null,
			float width = 0.5f
		)
		{
			if (method.Length >= 2 && method.StartsWith(OUTER_TOKEN))
			{
				useOuter = true;
				method = method.Substring(1);
			}
			else if (method.Contains(';'))
			{
				staticMethod = ReflectionUtils.ParseStaticMethodString(method);
			}

			this.width = Mathf.Clamp(width, MIN_W, 1f);
			this.method = method;
			this.label = label;
			this.flags = flags;
		}

		internal string label { get; }
		internal EFieldUsable flags { get; }
		internal bool useOuter { get; }
		internal MethodInfo staticMethod { get; }
		internal float width { get; } // button width (0-1)

		internal MethodInfo GetMethod(FieldInfo field)
		{
			if (staticMethod != null)
			{
				return staticMethod;
			}

			var type = field.FieldType.GetInnermostType();
			if (useOuter)
			{
				type = field.DeclaringType;
			}
			var m = (type!).GetMethod(method, _INST_FLAGS, null, Array.Empty<Type>(), null);
			if (m == null || m.ReturnType != typeof(void))
			{
				return null;
			}
			return m;
		}

		private string method { get; }

		private const BindingFlags _INST_FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Instance;
	}
}