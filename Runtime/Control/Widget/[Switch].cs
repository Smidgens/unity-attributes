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
		internal string label0 { get; }
		internal string label1 { get; }

		public SwitchAttribute(string l0, string l1)
		{
			label0 = l0;
			label1 = l1;
		}

		public SwitchAttribute(string label = "") : this(label, label)
		{
		
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
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

			_isBool = fieldInfo.FieldType == typeof(bool);
			_isFlags = t.IsEnum && t.IsDefined(typeof(FlagsAttribute));

			if (fieldInfo.FieldType.GetInnermostType() == typeof(LayerMask))
			{
				_isFlags = true;
				List<(string, int)> lValues = new();
				
				foreach (var layerIndex in Enumerable.Range(0, 31))
				{
					var lName = LayerMask.LayerToName(layerIndex);
					if (string.IsNullOrEmpty(lName))
					{
						continue;
					}

					lValues.Add((lName, 1 << layerIndex));
				}

				_flagValues = lValues.ToArray();

				return;
			}
			
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
			if (_isFlags)
			{
				DrawFlags(ctx);
			}
			else if(_isBool)
			{
				DrawSingle(ctx);
			}
			else
			{
				DrawerGUI.MutedInfo(ctx.position, "Field should be enum/bool");
			}
		}

		private bool _isBool;
		private bool _isFlags;
		private (string, int)[] _flagValues;

		private void DrawSingle(in DrawContext ctx)
		{
			var prop = ctx.property;
			prop.boolValue = DrawSwitch(ctx.position, prop.boolValue, _Attribute.label0, _Attribute.label1);
		}

		private static bool PointerButton(in Rect pos)
		{
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			return GUI.Button(pos, GUIContent.none, GUIStyle.none);
		}

		private static bool DrawSwitch(Rect pos, bool val, in string l0, in string l1)
		{
			// EditorGUI.indentLevel = 0;
			var label = val ? l1 : l0;

			var id = GUIUtility.GetControlID(FocusType.Keyboard, pos);
			if (PointerButton(pos))
			{
				GUIUtility.keyboardControl = id;
				val = !val;
			}

			var color = DrawerGUI.PickSkin(Color.white * 0.8f, Color.black * 0.5f);

			if (!GUI.enabled)
			{
				color *= 0.8f;
			}

			var focused = id == GUIUtility.keyboardControl;
			
			if (focused)
			{
				color = EditorStyles.label.focused.textColor;
				// ugly hack to get enter key to work like usual when focused
				if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
				{
					val = !val;
				}
			}

			var icoRect = pos.SliceLeft(pos.height * 2);
			var ico = val ? EAtlasIcon.SwitchOn : EAtlasIcon.SwitchOff;
			PluginAtlas.DrawIcon(icoRect, ico, color);
			var s = val ? EditorStyles.boldLabel : EditorStyles.label;

			var tColor = s.normal.textColor;

			if (focused)
			{
				s.normal.textColor = color;
			}

			GUI.Label(pos, label, s);

			s.normal.textColor = tColor;

			// EditorGUI.indentLevel = tIndent;
			return val;
		}
		

		private void DrawFlags(in DrawContext ctx)
		{
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