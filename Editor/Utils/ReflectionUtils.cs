// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	internal static class ReflectionUtils
	{
		public static MethodInfo ParseStaticMethodString(string methodPath, Type returnType = null, Type[] pTypes = null)
		{
			if (string.IsNullOrEmpty(methodPath))
			{
				return null;
			}

			returnType ??= typeof(void);
			int sepIndex = methodPath.IndexOf(';');

			if (sepIndex < 0 || sepIndex >= methodPath.Length - 1)
			{
				return null;
			}
			var methodName = methodPath.Substring(0, sepIndex);
			var typeName = methodPath.Substring(sepIndex + 1);
			var type = Type.GetType(typeName);
			if (type == null)
			{
				return null;
			}

			pTypes ??= Array.Empty<Type>();

			var m = type.GetMethod(methodName, _FLAGS, null, pTypes, null);

			if (m == null || m.ReturnType != returnType)
			{
				return null;
			}
			return m;
		}

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static;
	}
}