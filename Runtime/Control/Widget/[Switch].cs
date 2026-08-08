// smidgens @ github

/*
 * TODOS
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
				return EditorGUIUtility.singleLineHeight * _fcount;
			}
			return base.GetHeight(property, label);
		}

		protected override void OnInit()
		{
			var t = fieldInfo.GetItemType();
			_isFlags =
			t.IsEnum
			&& t.GetCustomAttribute<FlagsAttribute>() != null;

			if (!_isFlags) { return; }

			_flagValues = (int[])Enum.GetValues(t);
			var n = _flagValues.Length;
			if (_flagValues[0] == 0) { n--; }
			if (_flagValues[_flagValues.Length - 1] == -1) { n--; }
			_fcount = n;
		}

		protected override void OnField(in DrawContext ctx)
		{
			if (_isFlags) { DrawFlags(ctx); }
			else { DrawSingle(ctx); }
		}

		private bool _isFlags;
		private int _fcount = 0;
		private int[] _flagValues = { };
		private const string _SWITCH_GUID = "e769e4d9f339626498a12b64168231ee";

		private static readonly Rect _SWITCH_0_COORDS = new Rect(0, 0, 0.25f, 0.125f);
		private static readonly Rect _SWITCH_1_COORDS = new Rect(0, 0.125f, 0.25f, 0.125f);

		// icon atlas
		private static readonly Lazy<Texture2D> _SWITCH_ICON = new (() =>
		{
			var path = AssetDatabase.GUIDToAssetPath(_SWITCH_GUID);
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
			// var (rl,rr) = pos.GetColumns(pos.height * 2f, 1f, 2);
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
			
			DrawerGUI.DrawTex(_SWITCH_ICON.Value, icoRect, coords, color);
			
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

			if (_fcount == 0)
			{
				return;
			}
			var evalue = ctx.property.intValue;
			var values = _flagValues;
			var dnames = ctx.property.enumDisplayNames;
			var frow = ctx.position;
			frow.height = ctx.position.height / _fcount;
			for (var i = 0; i < _fcount; i++)
			{
				var r = frow;
				r.position += new Vector2(0f, r.height * i);
				var l = dnames[i + 1];
				var v = values[i + 1];
				var active = (evalue & v) != 0;
				var nv =
				DrawSwitch(r, active, l, l);
				if (nv != active)
				{
					if (!nv)
					{
						evalue &= ~v;
					}
					else { evalue |= v; }
				}
			}
			ctx.property.intValue = evalue;
		}

	}
}

#endif