// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	// 
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class InstancedReferenceAttribute : __BaseControl
	{
		/// <summary>
		/// Assembly string reference to static function: System.Type->String
		/// </summary>
		public bool labelFn { get; set; }

		/// <summary>
		/// Display label for null values
		/// </summary>
		public string emptyValueLabel { get; set; } = "(none)";
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Reflection;
	using System.Collections.Generic;
	using System.ComponentModel;

	[CustomPropertyDrawer(typeof(InstancedReferenceAttribute))]
	internal sealed class _InstancedReferenceAttribute : __ControlDrawer<InstancedReferenceAttribute>
	{
		protected override void OnLabel(ref Rect pos, GUIContent l)
		{
			// base.OnLabel(ref pos, l);
		}

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;
			var pos = ctx.position;
			var l = ctx.label;

			var isArray = fieldInfo.FieldType.IsArray || prop.propertyPath.EndsWith($"].{prop.name}");

			if (isArray && prop.propertyType != SerializedPropertyType.ManagedReference)
			{
				EditorGUI.LabelField(pos, "Invalid type", EditorStyles.miniLabel);
				return;
			}

			if (!isArray && prop.propertyType != SerializedPropertyType.ManagedReference)
			{
				pos = EditorGUI.PrefixLabel(pos, l);
				EditorGUI.LabelField(pos, "Invalid type", EditorStyles.miniLabel);
				return;
			}

			if (isArray)
			{
				// horrible...
				var indentSize = 5f;
				pos.position -= new Vector2(indentSize, 0f);
				pos.size += new Vector2(indentSize, 0f);
			}

			if (isArray)
			{
				prop.isExpanded = true;
			}

			var tIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = isArray ? 0 : EditorGUI.indentLevel;
			// EditorGUI.indentLevel = isArray ? 0 : EditorGUI.indentLevel;
		
			var typeRect = SliceRow(ref pos);

			if(l != GUIContent.none && !isArray)
			{
				typeRect = EditorGUI.PrefixLabel(typeRect, l);
			}

			SelectorDropdown(typeRect, prop);
			if (prop.managedReferenceValue == null)
			{
				EditorGUI.indentLevel = tIndent;
				return;
			}

			var extraIndent = isArray ? 0 : 1;
			
			EditorGUI.indentLevel += extraIndent;

			if (_fields != null)
			{
				var i = -1;
				foreach (var field in _fields)
				{
					i++;
					var fProp = prop.serializedObject.FindProperty(prop.propertyPath + "." + field.Name);
					var pHeight = EditorGUI.GetPropertyHeight(fProp);
					var fRect = SliceTop(ref pos, pHeight);
					EditorGUI.PropertyField(fRect, fProp);
					if (i < _fields.Count - 1)
					{
						SliceTop(ref pos, EditorGUIUtility.standardVerticalSpacing);
					}
				}
			}

			EditorGUI.indentLevel = tIndent;
		}

		private Type _lastType;
		private IReadOnlyList<FieldInfo> _fields;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var isArray = fieldInfo.FieldType.IsArray || prop.propertyPath.EndsWith($"].{prop.name}");

			var totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			if (isArray)
			{
				// totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			}

			if (_lastType != prop.managedReferenceValue?.GetType())
			{
				_lastType = null;
				_fields = null;
			}

			if (prop.managedReferenceValue == null)
			{
				return totalHeight;
			}

			if (_fields == null)
			{
				_fields = FindInspectorFields<object>(prop.managedReferenceValue.GetType());
				_lastType = prop.managedReferenceValue.GetType();
			}

			foreach (var f in _fields)
			{
				var innerProp = prop.FindPropertyRelative(f.Name);
				totalHeight += EditorGUI.GetPropertyHeight(innerProp);
			}

			totalHeight += (Mathf.Max(_fields.Count - 1, 0f)) * EditorGUIUtility.standardVerticalSpacing;
			
			return totalHeight;
		}

		private void SelectorDropdown(Rect pos, SerializedProperty prop)
		{
			Type currentType = prop.managedReferenceValue?.GetType();
			string defaultLabel = (attribute as InstancedReferenceAttribute)!.emptyValueLabel;
			string label = currentType != null ? GetTypeDisplayName(currentType) : defaultLabel;

			if (!GUI.Button(pos, label, EditorStyles.popup))
			{
				return;
			}
			var m = CreateTypeMenu(GetFieldType(), o =>
			{
				var newType = (Type)o;
				if (newType == currentType)
				{
					return;
				}

				if (o == null)
				{
					prop.managedReferenceValue = null;
					prop.serializedObject.ApplyModifiedProperties();
					return;
				}
				
				prop.managedReferenceValue = Activator.CreateInstance(newType);
				prop.serializedObject.ApplyModifiedProperties();
			}, defaultLabel);
			m.DropDown(pos);
		}

		private Type GetFieldType()
		{
			return !fieldInfo.FieldType.IsArray
			? fieldInfo.FieldType
			: fieldInfo.FieldType.GetElementType();
		}

		private static GenericMenu CreateTypeMenu(Type baseType, GenericMenu.MenuFunction2 fn, string defaultLabel = "(none)")
		{
			var menu = new GenericMenu();

			var types = GetDerivedTypes(baseType);

			Assembly currentAssembly = null;

			menu.AddItem(new GUIContent(defaultLabel), false, fn, null);
			menu.AddSeparator("");

			foreach (var type in types)
			{
				if (type.GetConstructor(Type.EmptyTypes) == null) // new()
				{
					continue;
				}
				if (currentAssembly != type.Assembly)
				{
					if (currentAssembly != null)
					{
						menu.AddSeparator("");
					}
					currentAssembly = type.Assembly;
					menu.AddDisabledItem(new GUIContent(currentAssembly.GetName().Name));
				}

				var dname = new GUIContent(GetTypeDisplayName(type));
				menu.AddItem(dname, false, fn,  type);
			}
			return menu;
		}

		private static IReadOnlyCollection<Type> GetDerivedTypes(Type baseType)
		{
			List<Type> outTypes = new();
			foreach (var t in TypeCache.GetTypesDerivedFrom(baseType))
			{
				if (t.IsAbstract || t.IsValueType)
				{
					continue;
				}
				outTypes.Add(t);
			}
			return outTypes;
		}

		private static string GetTypeDisplayName(Type type)
		{
			if (type == null)
			{
				return "";
			}

			var attr = type.GetCustomAttribute<DisplayNameAttribute>();

			if (attr != null)
			{
				return attr.DisplayName;
			}
			
			return type.Name;
		}
		
		// Find all fields that Unity would default render in the inspector
		internal static IReadOnlyList<FieldInfo> FindInspectorFields<T>(Type owner)
		{
			// NOTE: doesn't work properly for unity components, flags might need to be different

			var baseType = typeof(T);

			List<FieldInfo> fields = new List<FieldInfo>();
			LinkedList<Type> hierarchy = new LinkedList<Type>(); // linked for efficient prepend

			// traverse parent hierarchy, stop at MonoBehaviour
			Type currentType = owner;
			while (currentType != baseType && currentType != null)
			{
				hierarchy.AddFirst(currentType);
				currentType = currentType.BaseType;
			}

			BindingFlags fieldFlags = BindingFlags.NonPublic
			| BindingFlags.Public
			| BindingFlags.DeclaredOnly
			| BindingFlags.Instance;

			// append fields in
			// same order as Unity would normally list them
			foreach (Type htype in hierarchy)
			{
				foreach (FieldInfo field in htype.GetFields(fieldFlags))
				{
					if (!IsInspectorField(field)) { continue; }
					fields.Add(field);
				}
			}
			return fields;
		}
		
		// can field be drawn by inspector
		private static bool IsInspectorField(FieldInfo f)
		{
			// explicitly public but non-serialized
			if (f.IsPublic && f.GetCustomAttribute<NonSerializedAttribute>() != null) { return false; }

			// explicitly hidden
			if (f.GetCustomAttribute<HideInInspector>() != null) { return false; }

			// private, non serialized
			if (!f.IsPublic && f.GetCustomAttribute<SerializeField>() == null) { return false; }

			// at this point, either the field is public, or private and using SerializeField
			return true;
		}
		
		private static Rect SliceRow(ref Rect r)
		{
			var r2 = SliceTop(ref r, EditorGUIUtility.singleLineHeight);
			SliceTop(ref r, EditorGUIUtility.standardVerticalSpacing);
			return r2;
		}
		
		private static Rect SliceTop(ref Rect r, in float h)
		{
			var r2 = r;
			r2.height = h;
			r.height -= h;
			r.position += new Vector2(0f, h);
			return r2;
		}
		
		

	}
}

#endif