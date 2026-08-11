// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using UnityEngine;

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
		public static bool IsStaticClass(this Type t) => t.IsClass && t.IsAbstract && t.IsSealed;
		public static bool IsStruct(this Type t) => t.IsValueType && !t.IsPrimitive && !t.IsEnum;
		public static bool IsDelegate(this Type t) => t.IsClass && typeof(Delegate).IsAssignableFrom(t);
		public static bool IsAttribute(this Type t) => t.IsClass && typeof(Attribute).IsAssignableFrom(t);
		public static bool IsException(this Type t) => t.IsClass && typeof(Exception).IsAssignableFrom(t);

		// most common editor types
		private static readonly Type[] _KNOWN_EDITOR_TYPES =
		{
			typeof(UnityEditor.Editor),
			typeof(UnityEditor.PropertyDrawer),
			typeof(UnityEditor.DecoratorDrawer),
			typeof(UnityEditor.EditorWindow),
			typeof(UnityEditor.AssetImporter),
		};
		
		private static readonly string[] _KNOWN_IRRELEVANT_ASSEMBLIES =
		{
			"Unity.InputSystem.DocCodeSamples"
		};

		private static readonly string[] _KNOWN_EDITOR_ASSEMBLIES =
		{
			"UnityEditor",
			"JetBrains",
			"PlayerBuild",
			"Bee."
		};

		private static readonly string[] _KNOWN_EDITOR_NAMESPACES =
		{
			"JetBrains",
			"UnityEditor"
		};

		public static bool IsEditorType(this Type t)
		{
			// absolutely not robust
			if (!string.IsNullOrEmpty(t.Namespace))
			{
				foreach (var ns in _KNOWN_EDITOR_NAMESPACES)
				{
					if (t.Namespace.StartsWith(ns))
					{
						return true;
					}
				}
				
				// this is iffy - could conceivably refer to a runtime editor/gameplay related
				if (t.Namespace.EndsWith(".Editor"))
				{
					return true;
				}
			}

			var aName = t.Assembly.GetName().Name;
			if (aName.StartsWith("Unity.") && t.Name.EndsWith("Editor"))
			{
				return true;
			}

			// this is unnecessarily costly and not 100% exhaustive
			if (t.DerivesFromAny(_KNOWN_EDITOR_TYPES))
			{
				return true;
			}
			return false;
		}

		public static bool IsEditorAssembly(this Assembly assembly)
		{
			// Note: is there a simple way to check if assembly is compiled only with UNITY_EDITOR?

			if(assembly.IsDefined(typeof(AssemblyIsEditorAssembly)))
			{
				return true;
			}
			
			var aName = assembly.GetName().Name;

			foreach (var prefix in _KNOWN_EDITOR_ASSEMBLIES)
			{
				if (aName.StartsWith(prefix))
				{
					return true;
				}
			}

			// not too great...
			if (aName.StartsWith("Unity") && aName.Contains(".Editor"))
			{
				return true;
			}
			return false;
		}

		public static bool IsUserRelevant(this Assembly assembly)
		{
			// TODO: identify absolutely irrelevant assemblies more smartly

			var aName = assembly.GetName().Name;
			foreach (var prefix in _KNOWN_IRRELEVANT_ASSEMBLIES)
			{
				if (aName.StartsWith(prefix))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNewable(this Type t)
		{
			return t.GetConstructor(Type.EmptyTypes) != null;
		}

		public static bool IsUserRelevant(this Type t)
		{
			if (t.IsSecurityCritical || t.IsSecuritySafeCritical)
			{
				return false;
			}
			
			if (t.FullName != null)
			{
				if (t.FullName.StartsWith("UnitySource"))
				{
					return false;
				}
			}
			
			return true;
		}

		public static bool IsCompilerGenerated(this Type t)
		{
			if (t.Name.StartsWith("<"))
			{
				return true;
			}

			if (t.IsDefined(typeof(CompilerGeneratedAttribute)))
			{
				return true;
			}
			return t.IsNested && t.FullName != null && t.FullName.StartsWith("<");
		}

		public static bool IsObsolete(this Type t)
		{
			return t.IsDefined(typeof(ObsoleteAttribute), true);
		}

		public static bool IsSerializable(this Type t, bool inherit = true)
		{
			return t.IsDefined(typeof(SerializableAttribute), inherit);
		}

		public static bool IsClassOrStruct(this Type t)
		{
			return (t.IsStruct() || t.IsClass);
		}

		public static bool DerivesFrom(this Type t, Type bt)
		{
			return bt.IsAssignableFrom(t);
		}

		public static bool DerivesFromAny(this Type t, Type[] baseTypes)
		{
			for(var i = 0; i < baseTypes.Length; i++)
			{
				if (t.DerivesFrom(baseTypes[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}

#endif