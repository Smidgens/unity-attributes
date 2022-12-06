// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;
	using System;

	[CustomPropertyDrawer(typeof(TabsAttribute))]
	internal class _Tabs : __ControlDrawer<TabsAttribute>
	{
		protected override float GetHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetHeight(property, label);
		}

		protected override void OnField(in DrawContext ctx)
		{
			if (_isFlags) { DrawFlags(ctx); }
			else { DrawDefault(ctx); }
		}
		private void DrawDefault(in DrawContext ctx)
		{
			if (_Attribute.Type == null)
			{
				_Attribute.Init(fieldInfo.GetItemType());
			}

			var fields = _Attribute.Fields;

			if (fields.Length == 0)
			{
				GUI.Box(ctx.position, "");
				return;
			}

			Rect col = ctx.position;
			col.width = ctx.position.width / fields.Length;
			var spos = ctx.position.position;

			for (var i = 0; i < fields.Length; i++)
			{
				col.position = spos + new Vector2(i * col.width, 0);
				var tProp = ctx.property.FindPropertyRelative(fields[i]);
				if (tProp == null || tProp.propertyType != SerializedPropertyType.Boolean)
				{
					EditorGUI.DrawRect(col, Color.red * 0.2f);
					continue;
				}
				tProp.boolValue = DrawerGUI.TabButton(col, tProp.boolValue, tProp.displayName);
			}
		}

		private static bool TabButton(in Rect pos, in bool v, in string l)
		{
			return DrawerGUI.TabButton(pos, v, l);
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

		private bool _isFlags = false;
		private int _fcount = 0;
		private int[] _flagValues = null;

		private void DrawFlags(in DrawContext ctx)
		{
			if (!fieldInfo.GetItemType().IsEnum) { return; }
			if (_fcount == 0) { return; }

			var maxWidth =
			ctx.position.width;

			var evalue = ctx.property.intValue;
			var values = _flagValues;
			var dnames = ctx.property.enumDisplayNames;
			var frow = ctx.position;
			frow.width = maxWidth / _fcount;

			var coffset = maxWidth / _fcount;

			for (var i = 0; i < _fcount; i++)
			{
				var ox = coffset * i;
				var r = frow;
				r.position += new Vector2(ox, 0);
				var l = dnames[i + 1];
				var v = values[i + 1];
				var active = (evalue & v) != 0;

				var nv = TabButton(r, active, l);
				if (nv != active)
				{
					if (!nv) { evalue &= ~v; }
					else { evalue |= v; }
				}
			}
			ctx.property.intValue = evalue;
		}
	}
}

#endif