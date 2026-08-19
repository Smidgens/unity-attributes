// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draw bool or enum fields as tabs
	/// </summary>
	public sealed class TabsAttribute : __BaseControl
	{
		public TabsAttribute(bool vertical = false)
		{
			this.vertical = vertical;
		}
		internal bool vertical { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System;
	using System.Collections.Generic;

	[CustomPropertyDrawer(typeof(TabsAttribute))]
	internal class _TabsAttribute : __ControlDrawer<TabsAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Bool | EFieldType.Enum;

			
		// GUI.DoControl
		// 	Rect position,
		// 	int id,
		// 	bool on,
		// 	bool hover,
		// 	GUIContent content,
		// 	GUIStyle style

		
		protected override void OnInit()
		{
			var t = _FieldType;
			_isEnum = t.IsEnum;
			var isFlags = t.IsDefined(typeof(FlagsAttribute));

			if (_isEnum)
			{
				var vals = (int[])Enum.GetValues(t);
				var labels = Enum.GetNames(t);
				List<(string, int)> fValues = new();

				for (var i = 0; i < vals.Length; i++)
				{
					if (isFlags && vals[i] == 0)
					{
						continue;
					}

					if (isFlags && !Mathf.IsPowerOfTwo(vals[i]))
					{
						continue;
					}
					fValues.Add((labels[i].ToSentenceCase(), vals[i]));
				}
				_values = fValues;
			}
		}

		protected override float GetHeight(SerializedProperty property, GUIContent label)
		{
			var itemHeight = EditorStyles.miniButton.CalcSize(GUIContent.none).y;
			if (_Attribute.vertical)
			{
				int rows = _FieldType == typeof(bool)
				? 2
				: _values.Count;
				return itemHeight * rows;
			}
			return Mathf.Max(EditorStyles.label.CalcSize(label).y, itemHeight);
		}
	

		protected override void OnField(in DrawContext ctx)
		{
			if (_isEnum)
			{
				DrawEnum(ctx);
			}
			else
			{
				DrawBool(ctx);
			}
			// GUI.Box(ctx.position, GUIContent.none, EditorStyles.helpBox);
		}
		private void DrawBool(in DrawContext ctx)
		{
			var prop = ctx.property;
			var pos = ctx.position;

			var fRect = _Attribute.vertical
			? pos.SliceTop(EditorStyles.miniButton.CalcSize(GUIContent.none).y)
			: pos.SliceLeft(pos.width * 0.5f);
			
			if (TabButton(fRect, prop.boolValue, "True", EditorStyles.miniButtonLeft))
			{
				prop.boolValue = true;
			}
			if (TabButton(pos, !prop.boolValue, "False", EditorStyles.miniButtonRight))
			{
				prop.boolValue = false;
			}
		}

		private static bool TabButton(in Rect pos, bool v, string l, GUIStyle btnStyle)
		{
			return DrawTabButton(pos, v, l, btnStyle);
		}

		private bool _isEnum;
		private List<(string, int)> _values;

		private static bool DrawTabButton(in Rect pos, bool value, string label, GUIStyle btnStyle)
		{
			var id = GUIUtility.GetControlID(FocusType.Keyboard, pos);

			var hovered = pos.Contains(Event.current.mousePosition);

			if (GUI.Button(pos, GUIContent.none, GUIStyle.none))
			{
				GUIUtility.keyboardControl = id;
				value = !value;
			}
			DrawerGUI.DoControl(pos, id, value, hovered, new GUIContent(label), btnStyle);
			return value;
		}

		private void DrawEnum(in DrawContext ctx)
		{
			if (!fieldInfo.GetItemType().IsEnum)
			{
				return;
			}
			
			var evalue = ctx.property.intValue;
			
			var pos = ctx.position;

			var tabWidth = ctx.position.width / _values.Count;

			var isFLags = fieldInfo.FieldType.GetInnermostType().IsDefined(typeof(FlagsAttribute));

			var itemHeight = EditorStyles.miniButton.CalcSize(GUIContent.none).y;

			int i = -1;
			foreach (var (label, value) in _values)
			{
				i++;

				var btnStyle = EditorStyles.miniButton;

				if (!_Attribute.vertical)
				{
					if (i == 0)
					{
						btnStyle = EditorStyles.miniButtonLeft;
					}
					else if (i == _values.Count - 1)
					{
						btnStyle = EditorStyles.miniButtonRight;
					}
					else
					{
						btnStyle = EditorStyles.miniButtonMid;
					}
				}
	
				var btnRect = _Attribute.vertical
				? pos.SliceTop(itemHeight)
				: pos.SliceLeft(tabWidth);

				var active = isFLags
				? (evalue & value) != 0
				: ctx.property.intValue == value;
				var nv = TabButton(btnRect, active, label, btnStyle);
				if (nv != active)
				{
					if (isFLags)
					{
						if (!nv)
						{
							evalue &= ~value;
						}
						else
						{
							evalue |= value;
						}
						ctx.property.intValue = evalue;
					}
					else
					{
						ctx.property.intValue = value;
					}
				}
			}
		}
	}
}

#endif
