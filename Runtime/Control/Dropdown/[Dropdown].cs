// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;

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
	using UnityEngine;
	using System.Text.RegularExpressions;

	[CustomPropertyDrawer(typeof(DropdownAttribute))]
	internal sealed class _DropdownAttribute : __ControlDrawer<DropdownAttribute>
	{
		protected override EFieldType GetValidTypes()
			=> EFieldType.Int | EFieldType.Float | EFieldType.String | EFieldType.Bool | EFieldType.Color |
			EFieldType.Object;

		protected override void OnField(in DrawContext ctx)
		{
			var attr = (DropdownAttribute)attribute;
			if (_FieldType == typeof(bool))
			{
				if (_boolOptions == null)
				{
					if (attr.GetOptionType() == typeof(string) && attr.values.Count >= 2)
					{
						_boolOptions = new()
						{
							(attr.values[0].Item1, typeof(bool), new DropdownAttribute.DropdownValue { boolValue = false}),
							(attr.values[1].Item1, typeof(bool), new DropdownAttribute.DropdownValue { boolValue = true}),
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

			var cPreview = pos.SliceLeft(pos.height);
			
			if (DrawerGUI.PopupButton(pos, currentValStr))
			{
				GetMenu(ctx.property, currentValStr, attr)
				.DropDown(ctx.position);
			}
			
			DrawValuePrefix(cPreview, ctx.property);
			
		}

		private Type _FieldType => fieldInfo.FieldType.GetInnermostType();

		private void DrawValuePrefix(Rect pos, SerializedProperty prop)
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

		private static Color ParseColor(string hex)
		{
			if (ColorUtility.TryParseHtmlString(hex, out var c))
			{
				return c;
			}
			return Color.clear;
		}
		
		private static void DrawAssetThumbnail(in Rect pos, UnityEngine.Object o)
		{
			if (!o)
			{
				return;
			}

			GUI.DrawTexture(pos, AssetPreview.GetMiniThumbnail(o));

			if (GUI.Button(pos, GUIContent.none, GUIStyle.none))
			{
				EditorGUIUtility.PingObject(o);
			}
		}

		private static bool IsGUID(string str)
		{
			return Regex.IsMatch(str, "^([a-f]|[0-9]){32}$");
		}

		private static List<string> LoadAssetOptions(DropdownAttribute attr, Type fieldType)
		{
			List<string> paths = new();

			if (attr.GetOptionType() != typeof(string))
			{
				return paths;
			}

			var folderPaths = new List<string>();

			foreach (var (label, _, value) in attr.values)
			{
				if (IsGUID(value.stringRef))
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
				return prop.objectReferenceValue ? prop.objectReferenceValue.name : "<none>";
			}

			if (prop.IsColor())
			{
				return $"#{ColorUtility.ToHtmlStringRGBA(prop.colorValue).ToLower()}";
			}

			if (prop.IsString())
			{
				if (string.IsNullOrEmpty(prop.stringValue))
				{
					return "<none>";
				}
				
				if (oType == typeof(Type))
				{
					var type = Type.GetType(prop.stringValue);
					return type?.FullName ?? "<missing>";
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
			return "<none>";
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

		private readonly List<(GUIContent, Type, DropdownAttribute.DropdownValue)> _defaultBoolOptions = new()
		{
			(new GUIContent("False"), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = false}),
			(new GUIContent("True"), typeof(bool), new DropdownAttribute.DropdownValue { boolValue = true}),
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

				m.AddItem(new GUIContent("None"), !p.objectReferenceValue, () =>
				{
					p.objectReferenceValue = null;
					p.serializedObject.ApplyModifiedProperties();
				});
				
				m.AddSeparator("");

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
					m.AddDisabledItem(new GUIContent("No options"));
				}
				
				return m;
			}

			foreach (var (label, type, value) in values)
			{
				var v = value;
				m.AddItem(label, currentLabel == label.text, () =>
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