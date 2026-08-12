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
			if (_isEnum)
			{
				DrawEnum(ctx);
			}
			else
			{
				DrawDefault(ctx);
			}
		}
		private void DrawDefault(in DrawContext ctx)
		{
			var prop = ctx.property;
			var pos = ctx.position;

			if (TabButton(pos.SliceLeft(pos.width * 0.5f), prop.boolValue, "True"))
			{
				prop.boolValue = true;
			}
			if (TabButton(pos, !prop.boolValue, "False"))
			{
				prop.boolValue = false;
			}

		}

		private static bool TabButton(in Rect pos, in bool v, in string l)
		{
			return DrawTabButton(pos, v, l);
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
				_flagValues = fValues.ToArray();
			}
		}

		private bool _isEnum;
		private (string, int)[] _flagValues;
		
		private static bool DrawTabButton(in Rect pos, bool value, in string label)
		{
			// background
			EditorGUI.DrawRect(pos, _TOGGLE_COLORS[value.ToInt()]);
			if (GUI.Button(pos, "", _ToggleTabStyle.Value))
			{
				value = !value;
			}
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			EditorGUI.LabelField(pos, label, _ToggleTabStyle.Value);
			return value;
		}

		private static readonly Color[] _TOGGLE_COLORS =
		{
			Color.black * 0.1f,
			Color.white * 0.5f,
		};
		
		private static readonly Lazy<GUIStyle> _ToggleTabStyle = new (() =>
		{
			return new GUIStyle(EditorStyles.miniLabel)
			{
				alignment = TextAnchor.MiddleCenter,
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

			var tabWidth = ctx.position.width / _flagValues.Length;

			var isFLags = fieldInfo.FieldType.GetInnermostType().IsDefined(typeof(FlagsAttribute));

			foreach (var (label, value) in _flagValues)
			{
				var col = pos.SliceLeft(tabWidth);
				var active = isFLags
				? (evalue & value) != 0
				: ctx.property.intValue == value;
				var nv = TabButton(col, active, label);
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
