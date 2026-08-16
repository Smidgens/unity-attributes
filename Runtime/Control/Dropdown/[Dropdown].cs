// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Displays list of values in dropdown
	/// </summary>
	public sealed class DropdownAttribute : __BaseControl
	{
		public DropdownAttribute()
		{
			boxedValues = Array.Empty<object>();
		}
	
		public DropdownAttribute(string optionFn)
		{
			this.optionFn = optionFn;
			boxedValues = Array.Empty<object>();
		}

		public DropdownAttribute(params object[] boxedValues)
		{
			this.boxedValues = boxedValues ?? Array.Empty<object>();
		}

		internal object[] boxedValues { get; }
		internal string optionFn { get; }
		
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEditor;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DropdownAttribute))]
	internal sealed class _DropdownAttribute : __ControlDrawer<DropdownAttribute>
	{
		protected override void OnInit()
		{
			var optValueType = _FieldType;

			if (typeof(UnityEngine.Object).IsAssignableFrom(_FieldType))
			{
				optValueType = typeof(string);
			}
			_optionFn = GetStaticOptionEnumerator(_Attribute.optionFn, optValueType);

			var isColor = typeof(Color) == _FieldType;
			
			if (_optionFn.Item1 != null)
			{
				foreach (var it in _optionFn.Item1.Invoke())
				{
					var key = _optionFn.Item2.GetValue(it);
					var val = _optionFn.Item3.GetValue(it);
					var str = isColor ? ColorUtility.ToHtmlStringRGBA((Color)val) : val.ToString();
					_options.Add((new GUIContent((string)key), val, str));
				}
			}

			foreach (var boxedValue in _Attribute.boxedValues)
			{
				if (boxedValue.GetType() != optValueType)
				{
					continue;
				}
				var str = isColor ? ColorUtility.ToHtmlStringRGBA((Color)boxedValue) : boxedValue.ToString();
				_options.Add((new GUIContent(boxedValue.ToString()), boxedValue, str));
			}
			
			if (_FieldType == typeof(bool))
			{
				if (_Attribute.boxedValues.Length == 0 && _options.Count == 0)
				{
					_options.Add((new GUIContent("False"), false, "false"));
					_options.Add((new GUIContent("True"), true, "true"));
				}
			}
		}

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;
			var attr = _Attribute;
			var currentValStr = GetCurrentValueLabel(ctx.property);
			var pos = ctx.position;

			var hasPreview = prop.propertyType
			is SerializedPropertyType.Color
			or SerializedPropertyType.ObjectReference;

			Rect previewRect = new();

			if (hasPreview)
			{
				previewRect = pos.SliceLeft(pos.height);
			}
			
			if (DrawerGUI.PopupButton(pos, currentValStr))
			{
				GetMenu(ctx.property, attr)
				.DropDown(ctx.position);
			}

			if (hasPreview)
			{
				DrawValuePreview(previewRect, ctx.property);
			}
		}

		private void DrawValuePreview(Rect pos, SerializedProperty prop)
		{
			if (_FieldType == typeof(Color))
			{
				EditorGUI.DrawRect(pos.Resized(-pos.height * 0.2f), prop.colorValue);
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(_FieldType))
			{
				DrawAssetThumbnail(pos.Resized(-pos.height * 0.1f), prop.objectReferenceValue);
			}
		}

		private readonly List<(GUIContent, object, string)> _options = new();

		private GUIContent FindValueLabel(object o)
		{
			var isColor = _FieldType == typeof(Color);

			var oStr = _FieldType == typeof(Color) ? ColorUtility.ToHtmlStringRGBA((Color)o) : o.ToString();
			
			foreach (var (label, value, str) in _options)
			{
				// if (value.GetHashCode() == o.GetHashCode())
				// {
				// 	return label;
				// }
				if (oStr == str)
				{
					return label;
				}
			}
			return null;
		}

		private static void DrawAssetThumbnail(Rect pos, UnityEngine.Object o)
		{
			if (!o)
			{
				GUI.Box(pos, GUIContent.none);
				return;
			}

			var c = pos.center;
			var tex = AssetPreview.GetMiniThumbnail(o);
			var ratio = (float)tex.height / tex.width;
			pos.height *= ratio;
			pos.center = c;

			GUI.DrawTexture(pos, AssetPreview.GetMiniThumbnail(o));

			if (GUI.Button(pos, GUIContent.none, GUIStyle.none))
			{
				EditorGUIUtility.PingObject(o);
			}
		}
		
		
		private const BindingFlags _BF_INSTANCE
		= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private delegate System.Collections.IEnumerable OptionFn();
		private (OptionFn, FieldInfo, FieldInfo) _optionFn;

		private static (OptionFn, FieldInfo, FieldInfo) GetStaticOptionEnumerator(string name, Type valueType)
		{
			var m = ReflectionUtils.ParseStaticMethodString(name, typeof(System.Collections.IEnumerable));

			if (m == null)
			{
				return default;
			}
			var rType = m.ReturnType;
			var genericListArgs = rType.GenericTypeArguments;
			if (genericListArgs.Length != 1)
			{
				return default;
			}

			var itemType = genericListArgs[0]; // should be tuple (string,<valueType>)
			var itemKeyType = itemType.GenericTypeArguments[0];
			var itemValueType = itemType.GenericTypeArguments[1];

			if (itemValueType != valueType || itemKeyType != typeof(string))
			{
				return default;
			}

			var fItem1 = itemType.GetField("Item1", _BF_INSTANCE)!;
			var fItem2 = itemType.GetField("Item2", _BF_INSTANCE)!;

			if (fItem1 == null || fItem2 == null)
			{
				return default;
			}
			var del = (OptionFn)m.CreateDelegate(typeof(OptionFn));
			return (del, fItem1, fItem2);
		}

		private List<(GUIContent, string)> LoadAssetOptions(DropdownAttribute attr, Type fieldType)
		{
			List<(GUIContent, string)> paths = new();
			List<string> values = new();

			foreach (var option in _options)
			{
				values.Add((string)option.Item2);
			}

			var folderPaths = new List<string>();

			foreach (var value in values)
			{
				if (value.IsGUID())
				{
					var path = AssetDatabase.GUIDToAssetPath(value);
					var aType = AssetDatabase.GetMainAssetTypeAtPath(path);
					if (aType == fieldType)
					{
						paths.Add((new GUIContent(GetPathLabel(path)), path));
					}
					continue;
				}
				folderPaths.Add(value);
			}

			foreach (var aGUID in AssetDatabase.FindAssets($"t:{fieldType.Name}", folderPaths.ToArray()))
			{
				var path = AssetDatabase.GUIDToAssetPath(aGUID);
				paths.Add((new GUIContent(GetPathLabel(path)), path));
			}

			return paths;
		}
		private string GetCurrentValueLabel(SerializedProperty prop)
		{
			if (prop.propertyType == SerializedPropertyType.ObjectReference)
			{
				var ob = prop.objectReferenceValue;
				if (prop.HasMissingReference())
				{
					return PluginConstants.Label.MISSING;
				}
				return ob?.name ?? PluginConstants.Label.POPUP_UNSET;
			}

			// if (prop.IsColor())
			// {
			// 	return $"#{ColorUtility.ToHtmlStringRGBA(prop.colorValue)}";
			// }
			if (prop.IsString())
			{
				if (string.IsNullOrEmpty(prop.stringValue))
				{
					return PluginConstants.Label.POPUP_UNSET;
				}
				return FindValueLabel(prop.stringValue)?.text ?? prop.stringValue;
			}

			var lb = FindValueLabel(prop.boxedValue);
			return lb != null ? lb.text : prop.boxedValue.ToString();
			
		}

		private static string GetPathLabel(string path)
		{
			var dotIndex = path.LastIndexOf('.');
			var nIndex = path.LastIndexOf('/') + 1;
			var len = dotIndex - nIndex;
			return path.Substring(nIndex, len);
		}

		private GenericMenu GetMenu(SerializedProperty p, DropdownAttribute attr)
		{
			var m = new GenericMenu
			{
				allowDuplicateNames = true
			};

			if (p.propertyType == SerializedPropertyType.ObjectReference)
			{
				var currentAssetPath = p.objectReferenceValue
				? AssetDatabase.GetAssetPath(p.objectReferenceValue)
				: null;

				var isUnset = !p.objectReferenceValue && !p.HasMissingReference();

				m.AddItem(new GUIContent(PluginConstants.Label.POPUP_UNSET), isUnset, () =>
				{
					p.objectReferenceValue = null;
					p.serializedObject.ApplyModifiedProperties();
				});
				
				m.AddSeparator(string.Empty);

				foreach (var (label, path) in LoadAssetOptions(attr, _FieldType))
				{
					m.AddItem(new GUIContent(label), path == currentAssetPath, () =>
					{
						p.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, this._FieldType);
						p.serializedObject.ApplyModifiedProperties();
					});
				}
				if (m.GetItemCount() == 2)
				{
					m.AddDisabledItem(new GUIContent(PluginConstants.Label.POPUP_EMPTY));
				}
				return m;
			}

			foreach (var (label, val, str) in _options)
			{
				var active = val.GetHashCode() == p.boxedValue?.GetHashCode();
				m.AddItem(label, active, () =>
				{
					p.boxedValue = val;
					p.serializedObject.ApplyModifiedProperties();
				});
			}

			return m;
		}

	
	}
}

#endif