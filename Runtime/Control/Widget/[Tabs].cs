// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Draw bool or enum fields as tabs
	/// </summary>
	public sealed class TabsAttribute : __BaseControl
	{
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
		protected override EFieldType GetValidTypes()
		{
			return EFieldType.Bool | EFieldType.Enum;
		}

		protected override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetHeight(property, label);
		}

		protected override void OnField(in DrawContext ctx)
		{
			var id = GUIUtility.GetControlID(FocusType.Keyboard, ctx.position);
			
			GUI.Box(ctx.position, GUIContent.none);

			if (_isEnum)
			{
				DrawEnum(ctx);
			}
			else
			{
				DrawDefault(ctx);
			}
			GUI.Box(ctx.position, GUIContent.none, EditorStyles.helpBox);
			
		}
		private void DrawDefault(in DrawContext ctx)
		{
			var prop = ctx.property;
			var pos = ctx.position;

			if (TabButton(pos.SliceLeft(pos.width * 0.5f), prop.boolValue, "True", EditorStyles.miniButtonLeft))
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

		protected override void OnInit()
		{
			var t = fieldInfo.GetItemType();
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
				_values = fValues.ToArray();
			}
		}

		private bool _isEnum;
		private (string, int)[] _values;

		private static bool DrawTabButton(in Rect pos, bool value, string label, GUIStyle btnStyle)
		{
			// background
			// EditorGUI.DrawRect(pos, value ? _ACTIVE_COLOR : _INACTIVE_COLOR);

			var bgColor = value ? _ACTIVE_COLOR : _INACTIVE_COLOR;
			
			var tColorGUIBG = GUI.backgroundColor;
			GUI.backgroundColor = value ? _ACTIVE_COLOR : _INACTIVE_COLOR;

			var id = GUIUtility.GetControlID(FocusType.Keyboard, pos);
			
			if (GUI.Button(pos, string.Empty, btnStyle))
			{
				GUIUtility.keyboardControl = id;
				value = !value;
			}

			var focused = GUIUtility.keyboardControl == id;

			if (focused)
			{

				// var fRect = pos;
				// fRect.height = 2f;
				// EditorGUI.DrawRect(fRect, EditorStyles.label.focused.textColor.Fade(0.9f));
				
				if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
				{
					value = !value;
				}
			}

			GUI.backgroundColor = tColorGUIBG;

			var tColor = GUI.color;
			GUI.color = (focused || value) ? _LABEL_COL_ACTIVE : _LABEL_COL_INACTIVE;
			_tabLabelStyle.Value.fontStyle = value ? FontStyle.Bold : FontStyle.Normal;
			GUI.Label(pos, label, _tabLabelStyle.Value);
			GUI.color = tColor;

			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
		
			return value;
		}

		

		private static readonly Color _INACTIVE_COLOR
		= DrawerGUI.PickSkin(Color.black.Fade(0.01f), Color.white);
		
		private static readonly Color _ACTIVE_COLOR
		= DrawerGUI.PickSkin(Color.black.Fade(0.6f), Color.black.Fade(0.7f));

		private static readonly Color _LABEL_COL_ACTIVE
		= DrawerGUI.PickSkin(Color.white.Fade(1f), Color.white.Fade(0.9f));
		
		private static readonly Color _LABEL_COL_INACTIVE
		= DrawerGUI.PickSkin(Color.white.Fade(0.8f), Color.black.Fade(0.9f));

		private static readonly Lazy<GUIStyle> _tabLabelStyle = new (() =>
		{
			return new GUIStyle(EditorStyles.miniLabel)
			{
				alignment = TextAnchor.MiddleCenter,
				hover =
				{
					textColor = Color.white
				},
				normal =
				{
					textColor = Color.white
				},
				fontSize = (int)(EditorStyles.miniLabel.fontSize * 0.95f)
			};
		});

		private void DrawEnum(in DrawContext ctx)
		{
			if (!fieldInfo.GetItemType().IsEnum)
			{
				return;
			}
			
			var evalue = ctx.property.intValue;
			
			var pos = ctx.position;

			var tabWidth = ctx.position.width / _values.Length;

			var isFLags = fieldInfo.FieldType.GetInnermostType().IsDefined(typeof(FlagsAttribute));

			int i = -1;
			foreach (var (label, value) in _values)
			{
				i++;

				var btnStyle = EditorStyles.miniButtonMid;

				if (i == 0)
				{
					btnStyle = EditorStyles.miniButtonLeft;
				}
				else if (i == _values.Length - 1)
				{
					btnStyle = EditorStyles.miniButtonRight;
				}
				
				var col = pos.SliceLeft(tabWidth);
				var active = isFLags
				? (evalue & value) != 0
				: ctx.property.intValue == value;
				var nv = TabButton(col, active, label, btnStyle);
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
