// smidgens @ github

/*
 * TODOS
 *	- move calculation and cache into drawer
 */

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// Draw struct/class fields on one line
	/// </summary>
	public sealed class InlineAttribute : __BaseControl
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using System;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(InlineAttribute))]
	internal sealed class _InlineAttribute : __ControlDrawer<InlineAttribute>
	{
		protected override void OnInit()
		{
			var fieldType = fieldInfo.FieldType.GetInnermostType();
			InitFields(fieldType);
		}

		private void InitFields(Type type)
		{
			List<(FieldInfo, float)> fields = new();

			var norm = 0f;

			foreach (var f in type.FindInspectorFields<object>())
			{
				var wAttr = f.GetCustomAttribute<InlineWidthAttribute>();
				var w = wAttr?.width ?? 0f;
				if (w < 1f)
				{
					norm += w;
				}

				if (Mathf.Approximately(w, 0f))
				{
					_flexFields++;
				}

				fields.Add((f, w));
			}

			if (norm > 1f)
			{
				for (int i = 0; i < fields.Count; i++)
				{
					if (fields[i].Item2 >= 1f)
					{
						var val = fields[i];
						val.Item2 = fields[i].Item2 / norm;
						fields[i] = val;
					}
				}
			}
			_fields = fields;
			_currentWidths = new float[_fields.Count];
		}

		private static readonly float _PAD = EditorGUIUtility.standardVerticalSpacing * 1.5f;

		protected override float GetHeight(SP prop, GUIContent label)
		{
			var max = EditorGUIUtility.singleLineHeight;
			foreach (var (f, _) in _fields)
			{
				var p = prop.FindPropertyRelative(f.Name);
				var h = EditorGUI.GetPropertyHeight(p, GUIContent.none);
				if (h > max)
				{
					max = h;
				}
			}
			return max;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var ti = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var pos = ctx.position;
		
			var usableWidth = pos.width - Mathf.Max(0f, _fields.Count - 1) * _PAD;
			
			var remainingWidth = usableWidth;

			for(int i = 0; i < _currentWidths.Length; i++)
			{
				var width = _fields[i].Item2;
				var fWidth = width > 1f ? width : width * usableWidth;
				_currentWidths[i] = fWidth;
				remainingWidth -= fWidth;
				i++;
			}

			var flexWidth = _flexFields > 0 ? remainingWidth / _flexFields : 0f;

			for(int i = 0; i < _currentWidths.Length; i++)
			{
				var w = Mathf.Approximately(_currentWidths[i], 0f)
				? flexWidth
				: _currentWidths[i];
				
				var fRect = pos.SliceLeft(w);
				var field = _fields[i].Item1;
				
				var prop = ctx.property.FindPropertyRelative(field.Name);

				var height = EditorGUI.GetPropertyHeight(prop, GUIContent.none);
				fRect.height = height;

				EditorGUI.PropertyField(fRect, prop, GUIContent.none);

				if (i != _fields.Count - 1)
				{
					pos.SliceLeft(_PAD);
				}
			}
			EditorGUI.indentLevel = ti;
		}

		private int _flexFields;
		private List<(FieldInfo, float)> _fields;
		private float[] _currentWidths;
	}

}

#endif