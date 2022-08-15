// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Extensions for types in System.Reflection
	/// </summary>
	internal static class Reflection_
	{
		/// <summary>
		/// Get type of field or type of element if field is array
		/// </summary>
		public static Type GetInnerType(this FieldInfo fi)
		{
			if (fi.FieldType.IsArray) { return fi.FieldType.GetElementType(); }
			return fi.FieldType;
		}

		/// <summary>
		/// Is field array type
		/// </summary>
		public static bool IsArray(this FieldInfo fo)
		{
			return fo.FieldType.IsArray;
		}
	}
}
