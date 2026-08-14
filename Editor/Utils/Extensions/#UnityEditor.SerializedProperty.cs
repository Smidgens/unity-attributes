// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using SP = UnityEditor.SerializedProperty;

	/// <summary>
	/// Extensions for UnityEditor.SerializedProperty
	/// </summary>
	internal static class SerializedProperty_
	{
		public static bool IsString(this SP p) => p.propertyType == SerializedPropertyType.String;
		public static bool IsFloat(this SP p) => p.propertyType == SerializedPropertyType.Float;
		public static bool IsInt(this SP p) => p.propertyType == SerializedPropertyType.Integer;
		public static bool IsEnum(this SP p) => p.propertyType == SerializedPropertyType.Enum;

		public static bool IsNumeric(this SP p)
		{
			return p.IsInt() || p.IsFloat();
		}
		public static bool IsBool(this SP p) => p.propertyType == SerializedPropertyType.Boolean;
		public static bool IsColor(this SP p) => p.propertyType == SerializedPropertyType.Color;

		public static bool IsRefType<T>(this SP p)
		{
			return p.IsRefType(typeof(T).Name);
		}
		
		private static PropertyInfo _refStringProp;
		
		public static bool HasMissingReference(this SerializedProperty sp)
		{
			
			if (_refStringProp == null)
			{
				_refStringProp = typeof(SerializedProperty)
				.GetProperty("objectReferenceStringValue", BindingFlags.NonPublic | BindingFlags.Instance);
			}

			if (_refStringProp == null)
			{
#if SM_DEV
				Debug.Log("SP: Missing ref check broken");
#endif
				return false;
			}
			
			var result = (string)_refStringProp.GetValue(sp, null);
			return result != null && result.StartsWith("Miss");
		}

		// get sibling of given prop
		public static SerializedProperty GetSibling(this SerializedProperty prop, string name)
		{
			// array item
			if (prop.propertyPath.EndsWith(']'))
			{
				return null;
			}
			var fieldName = prop.name;
			var basePath = prop.propertyPath.Substring(0, prop.propertyPath.Length - fieldName.Length);
			var togglePath = $"{basePath}{name}";
			return prop.serializedObject.FindProperty(togglePath);
		}

		// get listener count for UnityEvent property
		public static int GetEventListenerCount(this SP p)
		{
			var l = p.FindPropertyRelative("m_PersistentCalls.m_Calls");
			return l?.arraySize ?? 0;
		}

		public static bool IsRefType(this SP p, string typeName)
		{
			if (p.propertyType != SerializedPropertyType.ObjectReference) { return false; }
			var refName = $"PPtr<${typeName}>";
			return p.type == refName;
		}

		public static bool IsArrayElement(this SerializedProperty p)
		{
			return p.propertyPath.EndsWith(']');
		}

		public static EFieldType GetTypeFlags(this SP prop)
		{
			var pt = prop.propertyType;
			switch (pt)
			{
				case SerializedPropertyType.Enum: return EFieldType.Enum;
				case SerializedPropertyType.String: return EFieldType.String;
				case SerializedPropertyType.Integer: return EFieldType.Int;
				case SerializedPropertyType.Float: return EFieldType.Float;
				case SerializedPropertyType.Boolean: return EFieldType.Bool;
				case SerializedPropertyType.ObjectReference: return EFieldType.Object;
				case SerializedPropertyType.Color: return EFieldType.Color;
			}
			return EFieldType.Any;
		}

	}
}

#endif