// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using Editor;

	public sealed class DropdownLabelsAttribute : __BaseModifier
	{
		public DropdownLabelsAttribute(params string[] labels)
		{
			this.labels = (labels ?? Array.Empty<string>()).ToGUIContent();
		}

		internal GUIContent[] labels { get; }
	}

	public sealed class DropdownAttribute : __BaseControl
	{
		public DropdownAttribute()
		{
			values = new List<(GUIContent,Type,DropdownValue)>();
		}
	
		public DropdownAttribute(params int[] values)
		{
			this.values = GetOptions(values, s => new DropdownValue
			{
				intValue = s
			});
		}

		public DropdownAttribute(params float[] values)
		{
			this.values = GetOptions(values, v => new DropdownValue
			{
				floatValue = v
			});
		}

		// generic fallback variant - TBD
		internal DropdownAttribute(params object[] values)
		{
			
		}

		public DropdownAttribute(params string[] values)
		{
			this.values = GetOptions(values, s => new DropdownValue
			{
				stringRef = s
			});
		}

		public DropdownAttribute(params Type[] values)
		{
			this.values = GetOptions(values, t => new DropdownValue
			{
				stringRef = t.AssemblyQualifiedName
			});
		}

		internal IReadOnlyList<(GUIContent,Type,DropdownValue)> values { get; }

		internal Type GetOptionType()
		{
			if (values.Count == 0)
			{
				return null;
			}
			return values[0].Item2;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct DropdownValue
		{
			[FieldOffset(0)] public string stringRef;
			[FieldOffset(0)] public int intValue;
			[FieldOffset(0)] public bool boolValue;
			[FieldOffset(0)] public float floatValue;
		}

		private static IReadOnlyList<(GUIContent,Type,DropdownValue)> GetOptions<T>(T[] values, Func<T,DropdownValue> vFn, Func<T,string> lFn = null)
		{
			var l = new List<(GUIContent, Type, DropdownValue)>();

			if (values == null)
			{
				return l;
			}
			foreach (var v in values)
			{
				var label = lFn != null ? lFn.Invoke(v) : v.ToString();
				var val = vFn?.Invoke(v) ?? default;
				l.Add((new GUIContent(label), typeof(T), val));
			}
			return l;
		}
		
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

	// TBD: refactor using SerializedProperty.boxedValue
	[CustomPropertyDrawer(typeof(DropdownAttribute))]
	internal sealed class _DropdownAttribute : __ControlDrawer<DropdownAttribute>
	{
		protected override EFieldType GetValidTypes()
		=> EFieldType.Primitive|EFieldType.String|EFieldType.Color| EFieldType.Object;

		protected override void OnInit()
		{
			var labelAttr = GetMod<DropdownLabelsAttribute>();
			if (labelAttr != null)
			{
				_labels = labelAttr.labels;
			}
		}

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;
			var attr = _Attribute;
			if (_FieldType == typeof(bool))
			{
				if (_boolOptions == null)
				{
					if (attr.GetOptionType() == typeof(string) && _labels.Length >= 2)
					{
						_boolOptions = new()
						{
							(new GUIContent(_labels[0]), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = false }),
							(new GUIContent(_labels[1]), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = true }),
						};
					}
					else
					{
						_boolOptions = _defaultBoolOptions;
					}
				}
			}

			var currentValStr = GetCurrentValueLabel(ctx.property, attr.GetOptionType());
			var pos = ctx.position;


			bool hasPreview =
			prop.propertyType == SerializedPropertyType.Color
			|| prop.propertyType == SerializedPropertyType.ObjectReference;

			Rect previewRect = new();

			if (hasPreview)
			{
				previewRect = pos.SliceLeft(pos.height);
			}
			
			if (DrawerGUI.PopupButton(pos, currentValStr))
			{
				GetMenu(ctx.property, currentValStr, attr)
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

		private static List<string> LoadAssetOptions(DropdownAttribute attr, Type fieldType)
		{
			List<string> paths = new();

			if (attr.GetOptionType() != typeof(string))
			{
				return paths;
			}

			var folderPaths = new List<string>();

			foreach (var (_, _, value) in attr.values)
			{
				if (value.stringRef.IsGUID())
				{
					var path = AssetDatabase.GUIDToAssetPath(value.stringRef);
					var aType = AssetDatabase.GetMainAssetTypeAtPath(path);
					if (aType == fieldType)
					{
						paths.Add(path);
					}
					continue;
				}
				folderPaths.Add(value.stringRef);
			}

			foreach (var aGUID in AssetDatabase.FindAssets($"t:{fieldType.Name}", folderPaths.ToArray()))
			{
				paths.Add(AssetDatabase.GUIDToAssetPath(aGUID));
			}

			return paths;
		}
		private string GetCurrentValueLabel(SerializedProperty prop, Type oType)
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

			if (prop.IsColor())
			{
				return $"#{ColorUtility.ToHtmlStringRGBA(prop.colorValue).ToLower()}";
			}

			if (prop.IsString())
			{
				if (string.IsNullOrEmpty(prop.stringValue))
				{
					return PluginConstants.Label.POPUP_UNSET;
				}
				if (oType == typeof(Type))
				{
					var type = Type.GetType(prop.stringValue);
					return type?.FullName ?? PluginConstants.Label.MISSING;
				}
				return prop.stringValue;
			}

			if (prop.IsInt())
			{
				return prop.intValue.ToString();
			}

			if (prop.IsFloat())
			{
				return prop.floatValue.ToString(CultureInfo.InvariantCulture);
			}

			if (prop.IsBool())
			{
				return _boolOptions[prop.boolValue ? 1 : 0].Item1.text;
			}
			return PluginConstants.Label.POPUP_UNSET;
		}

		private static void SetValue(SerializedProperty prop, Type oType, in DropdownAttribute.DropdownValue value)
		{
			if (prop.IsColor())
			{
				if (oType == typeof(string))
				{
					if (ColorUtility.TryParseHtmlString(value.stringRef, out var color))
					{
						prop.colorValue = color;
					}
				}
			}

			if (prop.IsString())
			{
				if(oType == typeof(Type) || oType == typeof(string))
				{
					prop.stringValue = value.stringRef;
				}
			}

			if (prop.IsFloat())
			{
				if (oType == typeof(int))
				{
					prop.floatValue = value.intValue;
				}
				else if (oType == typeof(float))
				{
					prop.floatValue = value.floatValue;
				}
			}

			if (prop.IsInt())
			{
				if (oType == typeof(int))
				{
					prop.intValue = value.intValue;
				}
				else if (oType == typeof(float))
				{
					prop.intValue = (int)value.floatValue;
				}
			}

			if (prop.IsBool())
			{
				prop.boolValue = value.boolValue;
			}
		}

		private GUIContent[] _labels = Array.Empty<GUIContent>();

		private readonly List<(GUIContent, Type, DropdownAttribute.DropdownValue)> _defaultBoolOptions = new()
		{
			(new GUIContent("false"), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = false}),
			(new GUIContent("true"), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = true}),
		};

		private List<(GUIContent, Type, DropdownAttribute.DropdownValue)> _boolOptions;

		private GenericMenu GetMenu(SerializedProperty p, string currentLabel, DropdownAttribute attr)
		{
			var m = new GenericMenu
			{
				allowDuplicateNames = true
			};

			var values = attr.values;

			if (_FieldType == typeof(bool))
			{
				values = _boolOptions;
			}
			else if (p.propertyType == SerializedPropertyType.ObjectReference)
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

				foreach (var path in LoadAssetOptions(attr, _FieldType))
				{
					var aPath = path;
					var dotIndex = path.LastIndexOf('.');
					var nIndex = path.LastIndexOf('/') + 1;
					var len = dotIndex - nIndex;
					var label = path.Substring(nIndex, len);
					m.AddItem(new GUIContent(label), aPath == currentAssetPath, () =>
					{
						p.objectReferenceValue = AssetDatabase.LoadAssetAtPath(aPath, this._FieldType);
						p.serializedObject.ApplyModifiedProperties();
					});
				}

				if (m.GetItemCount() == 2)
				{
					m.AddDisabledItem(new GUIContent(PluginConstants.Label.POPUP_EMPTY));
				}
				
				return m;
			}

			int i = -1;
			foreach (var (label, type, value) in values)
			{
				i++;
				var v = value;
				var l = i < _labels.Length - 1 ? _labels[i] : label;
				m.AddItem(l, currentLabel == label.text, () =>
				{
					SetValue(p, type, v);
					p.serializedObject.ApplyModifiedProperties();
				});
			}

			return m;
		}

	
	}
}

#endif