// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;

	// reflection / type helpers
	internal static class Reflection_
	{
		public static Type GetItemType(this FieldInfo fi)
		{
			return fi.IsArray()
			? fi.FieldType.GetElementType()
			: fi.FieldType;
		}
		public static bool IsArray(this FieldInfo fo) => fo.FieldType.IsArray;
		public static bool IsStatic(this Type t) => t.IsAbstract && t.IsSealed;

		public static bool IsStruct(this Type t) => t.IsValueType && !t.IsPrimitive;

		public static bool IsClassOrStruct(this Type t)
		{
			return (t.IsStruct() || t.IsClass);
		}

		public static bool DerivesFrom(this Type t, Type bt)
		{
			return bt.IsAssignableFrom(t);
		}

		public static bool DerivesFrom(this Type t, Type[] baseTypes)
		{
			for(var i = 0; i < baseTypes.Length; i++)
			{
				if (!t.DerivesFrom(baseTypes[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}

#endif