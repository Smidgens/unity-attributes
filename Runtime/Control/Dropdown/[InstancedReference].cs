// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using Editor;

	// 
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class InstancedReferenceAttribute : __BaseControl
	{
		private static readonly Type[] _labelFnArgs =
		{
			typeof(Type)
		};

		public InstancedReferenceAttribute
		(
			string emptyLabel = EConstants.Label.POPUP_DEFAULT,
			string labelFn = null
		) : base(true)
		{
			this.labelFn = GetByDisplayName;
			var m = ReflectionUtils.ParseStaticMethodString(labelFn, typeof(string), _labelFnArgs);
			if (m != null)
			{
				this.labelFn = (Func<Type, string>)m.CreateDelegate(typeof(Func<Type, string>), null);
			}
			this.emptyLabel = emptyLabel;
		}

		// display value for null/empty string
		internal string emptyLabel { get; }

		internal Func<Type, string> labelFn { get; }
		
		private static string GetByDisplayName(Type type)
		{
			if (type == null)
			{
				return "";
			}

			var attr = type.GetCustomAttribute<DisplayNameAttribute>();

			return attr != null
			? attr.DisplayName
			: type.Name;
		}
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

			if (prop.propertyType != SerializedPropertyType.ManagedReference)
			{
				if (!isArray)
				{
					pos = EditorGUI.PrefixLabel(pos, l);
					DrawerGUI.MutedInfo(pos, "Invalid type");
					return;
				}
			}

			var typeRect = pos.SliceTop(EditorGUIUtility.singleLineHeight);
			pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);

			if(l != GUIContent.none && !isArray)
			{
				typeRect = EditorGUI.PrefixLabel(typeRect, l);
			}

			SelectorDropdown(typeRect, prop);
			if (prop.managedReferenceValue == null)
			{
				return;
			}

			if (_fields != null)
			{
				if (!isArray)
				{
					DrawerGUI.IndentRect(ref pos, 1);
				}

				var i = -1;
				foreach (var field in _fields)
				{
					i++;
					var fProp = prop.serializedObject.FindProperty(prop.propertyPath + "." + field.Name);
					var pHeight = EditorGUI.GetPropertyHeight(fProp);
					var fRect = pos.SliceTop(pHeight);
					EditorGUI.PropertyField(fRect, fProp);
					if (i < _fields.Count - 1)
					{
						pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
					}
				}

			}
		}

		private Type _lastType;
		private IReadOnlyList<FieldInfo> _fields;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

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
				_fields = prop.managedReferenceValue.GetType().FindInspectorFields<object>();
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
			string defaultLabel = (attribute as InstancedReferenceAttribute)!.emptyLabel;

			// string label = currentType != null ? GetTypeDisplayName(currentType) : defaultLabel;
			string label = currentType != null ? _Attribute.labelFn.Invoke(currentType) : defaultLabel;

			if (!EditorGUI.DropdownButton(pos, new GUIContent(label), FocusType.Keyboard))
			{
				return;
			}

			if (prop.IsArrayElement())
			{
				defaultLabel = null;
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

				EditorApplication.delayCall += () =>
				{
					prop.serializedObject.Update();
				};

			}, defaultLabel);
			m.DropDown(pos);
		}

		private Type GetFieldType()
		{
			return !fieldInfo.FieldType.IsArray
			? fieldInfo.FieldType
			: fieldInfo.FieldType.GetElementType();
		}

		private GenericMenu CreateTypeMenu(Type baseType, GenericMenu.MenuFunction2 fn, string defaultLabel = EConstants.Label.POPUP_DEFAULT)
		{
			var menu = new GenericMenu();

			var types = GetDerivedTypes(baseType);

			Assembly currentAssembly = null;

			if (defaultLabel != null)
			{
				menu.AddItem(new GUIContent(defaultLabel), false, fn, null);
				menu.AddSeparator("");
			}

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
				// var dname = new GUIContent(GetTypeDisplayName(type));
				var dname = new GUIContent(_Attribute.labelFn.Invoke(type));
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
		

	}
}

#endif