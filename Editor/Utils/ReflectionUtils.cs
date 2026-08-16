// smidgens @ github

#pragma warning disable 0414

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Editor;

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
		
		//
		private const BindingFlags _BF_INSTANCE
		= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		private const BindingFlags _BF_STATIC =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static;
		
		public static (Assembly, Type[])[] GetAllAssemblyTypes()
		{
			if(_cachedTypes == null)
			{
				List<(Assembly, Type[])> filteredAssemblies = new();

				foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!a.IsUserRelevant())
					{
						continue;
					}
					var types = a.GetTypes();
					if (types.Length == 0)
					{
						continue;
					}
					filteredAssemblies.Add((a, types));
				}
				_cachedTypes = new (Assembly, Type[])[filteredAssemblies.Count];

				for (var i = 0; i < filteredAssemblies.Count; i++)
				{
					_cachedTypes[i] = (filteredAssemblies[i].Item1, filteredAssemblies[i].Item2);
				}
			}
			return _cachedTypes;
		}
		
	}
}