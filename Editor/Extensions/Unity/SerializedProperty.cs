// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using SP = UnityEditor.SerializedProperty;

	/// <summary>
	/// Extensions for UnityEditor.SerializedProperty
	/// </summary>
	internal static partial class SerializedProperty_
	{
		public static bool IsString(this SP p) => p.propertyType == SerializedPropertyType.String;
		public static bool IsFloat(this SP p) => p.propertyType == SerializedPropertyType.Float;
		public static bool IsInt(this SP p) => p.propertyType == SerializedPropertyType.Integer;

		public static bool IsRefType<T>(this SP p)
		{
			return p.IsRefType(typeof(T).Name);
		}

		public static bool IsRefType(this SP p, string typeName)
		{
			if (p.propertyType != SerializedPropertyType.ObjectReference) { return false; }
			var refName = $"PPtr<${typeName}>";
			return p.type == refName;
		}

		public static FieldType GetTypeFlags(this SP prop)
		{
			var pt = prop.propertyType;
			switch (pt)
			{
				case SerializedPropertyType.Enum: return FieldType.Enum;
				case SerializedPropertyType.String: return FieldType.String;
				case SerializedPropertyType.Integer: return FieldType.Int;
				case SerializedPropertyType.Float: return FieldType.Float;
				case SerializedPropertyType.Boolean: return FieldType.Bool;
				case SerializedPropertyType.ObjectReference: return FieldType.Object;
				case SerializedPropertyType.Color: return FieldType.Color;
			}
			return FieldType.Any;
		}

	}
}

#endif