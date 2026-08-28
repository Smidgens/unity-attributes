// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using Editor;

	[Flags]
	public enum EInstancedReference
	{
		None = 0,
		/// <summary>
		/// Only show types with [Serializable]
		/// </summary>
		Strict = 1,
		/// <summary>
		/// Show dropdown in arrays
		/// </summary>
		ArrayReplace = 2,
		/// <summary>
		/// Should type options be grouped by assembly
		/// </summary>
		GroupByAssembly = 4,
		/// <summary>
		/// Sensible defaults
		/// </summary>
		Default = ArrayReplace|Strict|GroupByAssembly,
		/// <summary>
		/// All
		/// </summary>
		All = ~0
	}
	
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
			string emptyLabel = PluginConstants.Label.POPUP_UNSET,
			string labelFn = null,
			EInstancedReference flags = EInstancedReference.Default
		) : base(true)
		{
			this.labelFn = GetDefaultDisplayName;
			var m = ReflectionUtils.ParseStaticMethodString(labelFn, typeof(string), _labelFnArgs);
			if (m != null)
			{
				this.labelFn = (Func<Type, string>)m.CreateDelegate(typeof(Func<Type, string>), null);
			}
			this.emptyLabel = emptyLabel;
			this.flags = flags;
		}

		// display value for null/empty string
		internal string emptyLabel { get; }

		internal Func<Type, string> labelFn { get; }
		
		internal EInstancedReference flags { get; }
		
		private static string GetDefaultDisplayName(Type type)
		{
			if (type == null)
			{
				return "(none)";
			}
			
			#if UNITY_EDITOR
			var attr = type.GetCustomAttribute<DisplayNameAttribute>();
			return attr != null
			? attr.DisplayName
			: UnityEditor.ObjectNames.NicifyVariableName(type.Name);
			#else
			return "";
			#endif
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
	using UnityEditor.IMGUI.Controls;

	[CustomPropertyDrawer(typeof(InstancedReferenceAttribute))]
	internal sealed class _InstancedReferenceAttribute : __ControlDrawer<InstancedReferenceAttribute>
	{
		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
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

			var isUnset = prop.managedReferenceValue == null;

			var typeRect = pos.SliceTop(EditorGUIUtility.singleLineHeight);

			if(l != GUIContent.none && !isArray)
			{
				typeRect = EditorGUI.PrefixLabel(typeRect, l);
			}

			if (!isUnset && isArray && !_Attribute.flags.HasFlag(EInstancedReference.ArrayReplace))
			{
				GUI.Box(typeRect, GUIContent.none, EditorStyles.helpBox);
				GUI.Box(typeRect, GUIContent.none);
				if (_displayIcon.Item1)
				{
					var iconRect = typeRect;
					iconRect.width = iconRect.height;
					typeRect.SliceLeft(15f);
					DrawDisplayIcon(iconRect);
				}
				typeRect.SliceLeft(EditorGUIUtility.standardVerticalSpacing * 1.5f);
				GUI.Label(typeRect, _typeLabel, EditorStyles.label);
			}
			else
			{
				SelectorDropdown(typeRect, prop);
			}

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

				foreach (var field in _fields)
				{
					pos.SliceTop(EditorGUIUtility.standardVerticalSpacing);
					var fProp = prop.serializedObject.FindProperty(prop.propertyPath + "." + field.Name);
					var pHeight = EditorGUI.GetPropertyHeight(fProp);
					var fRect = pos.SliceTop(pHeight);
					EditorGUI.PropertyField(fRect, fProp);
					
				}
			}
		}

		private Type _lastType;
		private IReadOnlyList<FieldInfo> _fields;
		private (Texture, Rect, Color) _displayIcon;
		private string _typeLabel;

		private void DrawDisplayIcon(Rect pos)
		{
			var iconRect = pos;
			iconRect.width = iconRect.height;
			iconRect = iconRect.Resized(-iconRect.height * 0.15f);
			DrawerGUI.DrawTex(iconRect, _displayIcon.Item1, _displayIcon.Item2, _displayIcon.Item3);
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			// dropdown row
			var totalHeight = EditorGUIUtility.singleLineHeight;

			if (_lastType != prop.managedReferenceValue?.GetType())
			{
				_lastType = null;
				_fields = null;
				_displayIcon = default;
			}

			if (prop.managedReferenceValue == null)
			{
				return totalHeight;
			}

			if (_fields == null)
			{
				_fields = prop.managedReferenceValue.GetType().FindInspectorFields<object>();
				_lastType = prop.managedReferenceValue.GetType();
				_typeLabel = _lastType != null ? _Attribute.labelFn.Invoke(_lastType) : _Attribute.emptyLabel;
				var dIcon = _lastType.GetCustomAttribute<DisplayIconAttribute>();
				if (dIcon != null && dIcon.iconGUID.IsGUID())
				{
					var path = AssetDatabase.GUIDToAssetPath(dIcon.iconGUID);
					var tex = AssetDatabase.LoadAssetAtPath<Texture>(path);
					_displayIcon = (tex, new Rect(0f,0f,1f,1f), Color.white);
				}
			}

			foreach (var f in _fields)
			{
				var innerProp = prop.FindPropertyRelative(f.Name);
				totalHeight += EditorGUI.GetPropertyHeight(innerProp);
			}

			totalHeight += (Mathf.Max(_fields.Count - 1, 0f)) * EditorGUIUtility.standardVerticalSpacing;
			return totalHeight;
		}

		private readonly GUIStyle _iconPopup = new (EditorStyles.popup)
		{
			padding
			= new RectOffset(18, EditorStyles.popup.padding.right, EditorStyles.popup.padding.top, EditorStyles.popup.padding.bottom)
		};
		
		private void SelectorDropdown(Rect pos, SerializedProperty prop)
		{
			Type currentType = prop.managedReferenceValue?.GetType();
			string defaultLabel = _Attribute.emptyLabel;

			var dLabel = prop.managedReferenceValue == null
			? defaultLabel
			: _typeLabel;

			var st = _displayIcon.Item1 ? _iconPopup : EditorStyles.popup;
			
			var dropPressed = EditorGUI.DropdownButton(pos, new GUIContent(dLabel), FocusType.Keyboard, st);
			
			if (_displayIcon.Item1)
			{
				var iconRect = pos;
				iconRect.width = iconRect.height;
				DrawDisplayIcon(iconRect);
			}
			
			if (!dropPressed)
			{
				return;
			}

			if (prop.IsArrayElement())
			{
				defaultLabel = null;
			}

			var dd = CreateTypeDropdown(_FieldType, newType =>
			{
				if (newType == currentType)
				{
					return;
				}
				if (newType == null)
				{
					prop.managedReferenceValue = null;
					prop.serializedObject.ApplyModifiedProperties();
					return;
				}
				prop.managedReferenceValue = Activator.CreateInstance(newType);
				prop.serializedObject.ApplyModifiedProperties();
			}, defaultLabel);
			dd.currentValue = currentType;
			dd.Show(pos, 400f);
		}

		private GenericDropdown<Type> CreateTypeDropdown(Type baseType, Action<Type> fn, string defaultLabel = PluginConstants.Label.POPUP_UNSET)
		{
			var types = GetDerivedTypes(baseType);
			var strict = _Attribute.flags.HasFlag(EInstancedReference.Strict);

			var title = ObjectNames.NicifyVariableName(_FieldType.Name);
			var baseDisplayName = _FieldType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
			title = baseDisplayName ?? title;
			
			var drop = new GenericDropdown<Type>(title)
			{
				onSelected = fn
			};

			if (!string.IsNullOrEmpty(defaultLabel))
			{
				drop.AddItem(defaultLabel, null);
				drop.AddSeparator(string.Empty);
			}

			// Assembly currentAssembly = null;

			foreach (var type in types)
			{
				if (type.GetConstructor(Type.EmptyTypes) == null) // new()
				{
					continue;
				}

				if (strict && !type.IsDefined(typeof(SerializableAttribute), false))
				{
					continue;
				}

				Texture2D icon = null;
				var dIcon = type.GetCustomAttribute<DisplayIconAttribute>();
				if (dIcon != null && !string.IsNullOrEmpty(dIcon.iconGUID))
				{
					icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(dIcon.iconGUID));
				}
				var path = _Attribute.flags.HasFlag(EInstancedReference.GroupByAssembly)
				? $"{type.Assembly.GetName().Name}/{_Attribute.labelFn.Invoke(type)}"
				: _Attribute.labelFn.Invoke(type);
				drop.AddItem(path, type, icon);
			}
			
			return drop;
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