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
		internal const char INNER_TOKEN = '.';
		internal const float MIN_W = 0.1f;

		public FieldButtonAttribute
		(
			string method,
			EFieldUsable flags = EFieldUsable.Always,
			object[] args = null,
			string label = null,
			float width = 0.5f
		)
		{
			this.width = Mathf.Clamp(width, MIN_W, 1f);
			this.method = method;
			this.label = label;
			this.flags = flags;
			this.args = args ?? Array.Empty<object>();
			argTypes = this.args.Length > 0 ? new Type[this.args.Length] : Array.Empty<Type>();
			for (int i = 0; i < this.args.Length; i++)
			{
				argTypes[i] = this.args[i].GetType();
			}
			
			if (method.Length >= 2 && method.StartsWith(INNER_TOKEN))
			{
				useInner = true;
				this.method = method.Substring(1);
			}
			else if (method.Contains(';'))
			{
				staticMethod = ReflectionUtils.ParseStaticMethodString(method, null, argTypes);
			}
		}

		internal string label { get; }
		internal EFieldUsable flags { get; }
		internal bool useInner { get; }
		internal MethodInfo staticMethod { get; }
		internal float width { get; } // button width (0-1)
		internal object[] args { get; }
		internal Type[] argTypes { get; }

		internal MethodInfo GetMethod(FieldInfo field)
		{
			if (staticMethod != null)
			{
				return staticMethod;
			}

			var type = field.DeclaringType;
			if (useInner)
			{
				type = field.FieldType.GetInnermostType();
			}
			var m = (type!).GetMethod(method, _INST_FLAGS, null, argTypes, null);
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