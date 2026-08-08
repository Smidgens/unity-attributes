// smidgens @ github

/*
 * Maybe
 *	- custom switch icon
 */
namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Switch Widget [bool,flags]
	/// </summary>
	public sealed class SwitchAttribute : __BaseControl
	{
		internal string[] Labels { get; } = { string.Empty, string.Empty };

		public SwitchAttribute() { }

		public SwitchAttribute(string l0, string l1)
		{
			Labels = new [] { l0, l1 };
		}

		public SwitchAttribute(string label)
		: this(label, label) { }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SwitchAttribute))]
	internal sealed class _SwitchAttribute : __ControlDrawer<SwitchAttribute>
	{
		protected override float GetHeight(SerializedProperty property, GUIContent label)
		{
			if (_isFlags)
			{
				return
				Mathf.Max(EditorGUIUtility.singleLineHeight * _flagValues.Length, EditorGUIUtility.singleLineHeight);
			}
			return base.GetHeight(property, label);
		}

		protected override void OnInit()
		{
			var t = fieldInfo.GetItemType();
			_isEnum = t.IsEnum;
			_isBool = fieldInfo.FieldType == typeof(bool);
			_isFlags = t.IsEnum && t.GetCustomAttribute<FlagsAttribute>() != null;

			if (!_isFlags)
			{
				return;
			}

			var vals = (int[])Enum.GetValues(t);
			var labels = Enum.GetNames(t);

			List<(string, int)> fValues = new();

			for (var i = 0; i < vals.Length; i++)
			{
				if (vals[i] == 0 || !Mathf.IsPowerOfTwo(vals[i]))
				{
					continue;
				}
				fValues.Add((labels[i].ToSentenceCase(), vals[i]));
			}
			_flagValues = fValues.ToArray();
		}

		protected override void OnField(in DrawContext ctx)
		{
			if (_isEnum)
			{
				if (_isFlags)
				{
					DrawFlags(ctx);
				}
				else
				{
					DrawerGUI.MutedInfo(ctx.position, "Enum should be flags");
				}
			}
			else
			{
				if (_isBool)
				{
					DrawSingle(ctx);
				}
				else
				{
					DrawerGUI.MutedInfo(ctx.position, "Should be bool or enum");
				}
			}
		}

		private bool _isBool;
		private bool _isEnum;
		private bool _isFlags;
		private (string, int)[] _flagValues;
		private static readonly Rect _SWITCH_0_COORDS = new Rect(0, 0, 0.25f, 0.125f);
		private static readonly Rect _SWITCH_1_COORDS = new Rect(0, 0.125f, 0.25f, 0.125f);

		// icon atlas
		private static readonly Lazy<Texture2D> _TEX_ATLAS = new (() =>
		{
			var path = AssetDatabase.GUIDToAssetPath("e769e4d9f339626498a12b64168231ee");
			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		});

		
		private void DrawSingle(in DrawContext ctx)
		{
			var prop = ctx.property;
			var labels = _Attribute.Labels;
			prop.boolValue = DrawSwitch(ctx.position, prop.boolValue, labels[0], labels[1]);
		}

		private static bool PointerButton(in Rect pos)
		{
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			return GUI.Button(pos, GUIContent.none, GUIStyle.none);
		}

		private static bool DrawSwitch(Rect pos, bool val, in string l0, in string l1)
		{
			var label = val ? l1 : l0;

			var icoRect = pos.SliceLeft(pos.height * 2);

			if (PointerButton(icoRect))
			{
				val = !val;
			}

			var coords = val ? _SWITCH_1_COORDS : _SWITCH_0_COORDS;

			var color = EditorGUIUtility.isProSkin
			? Color.white * 0.8f
			: Color.black * 0.5f;
			
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, icoRect, coords, color);
			
			var s = val ? EditorStyles.boldLabel : EditorStyles.label;
			EditorGUI.LabelField(pos, label, s);
			return val;
		}

		private void DrawFlags(in DrawContext ctx)
		{
			if (!fieldInfo.GetItemType().IsEnum)
			{
				return;
			}

			var pos = ctx.position;
			var evalue = ctx.property.intValue;

			foreach (var (name, value) in _flagValues)
			{
				var row = pos.SliceTop(EditorGUIUtility.singleLineHeight);

				var active = (evalue & value) != 0;
				var nv = DrawSwitch(row, active, name, name);
				if (nv != active)
				{
					if (!nv)
					{
						evalue &= ~value;
					}
					else
					{
						evalue |= value;
					}
				}
			}
			ctx.property.intValue = evalue;
		}

	}
}

#endif