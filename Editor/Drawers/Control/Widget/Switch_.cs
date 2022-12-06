// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SwitchAttribute))]
	internal class Switch_ : __ControlDrawer<SwitchAttribute>
	{
		private bool _isFlags = false;
		private int _fcount = 0;
		private int[] _flagValues = { };

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

		private void DrawSingle(in DrawContext ctx)
		{
			var prop = ctx.property;
			var labels = _Attribute.Labels;
			prop.boolValue =
			DrawerGUI.Switch(ctx.position, prop.boolValue, labels[0], labels[1]);
		}

		private void DrawFlags(in DrawContext ctx)
		{
			if (!fieldInfo.GetItemType().IsEnum) { return; }
			if(_fcount == 0) { return; }
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
				DrawerGUI.Switch(r, active, l, l);
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