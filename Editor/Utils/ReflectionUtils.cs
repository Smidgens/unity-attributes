// smidgens @ github

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

			var m = type.GetMethod(methodName, _BF_STATIC, null, pTypes, null);

			if (m == null || !returnType.IsAssignableFrom(m.ReturnType))
			{
				return null;
			}
			return m;
		}

		private static (Assembly, Type[])[] _cachedTypes;
		
		private const BindingFlags _BF_STATIC =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static;
		
	}
}

