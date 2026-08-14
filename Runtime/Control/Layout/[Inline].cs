// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draw struct/class fields on one line
	/// </summary>
	public sealed class InlineAttribute : __BaseControl
	{
		public InlineAttribute() : base(true)
		{
			
		}
	}
}

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Set size of specific inlined field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class InlineWidthAttribute : __BaseModifier
	{
		public InlineWidthAttribute(float w)
		{
			width = Mathf.Max(w, 0f);
		}

		/// <summary>
		/// Specify width of inner field
		/// </summary>
		public InlineWidthAttribute(string field, float w) : this(w)
		{
			this.fieldName = field;
		}
		internal float width { get; }
		internal string fieldName { get; }
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
			
			// loop through 
			foreach (var f in type.FindInspectorFields<object>())
			{
				var wAttr = f.GetCustomAttribute<InlineWidthAttribute>();

				if (wAttr == null)
				{
					wAttr = GetFieldOverride(f.Name);
				}
				var w = wAttr?.width ?? 0f;

				if (Mathf.Approximately(w, 0f))
				{
					_flexFields++;
				}

				fields.Add((f, w));
			}
			_fields = fields;
			_currentWidths = new float[_fields.Count];
		}

		private static readonly float _PAD = EditorGUIUtility.standardVerticalSpacing;

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

		private Dictionary<string, InlineWidthAttribute> _outerFieldOverrides = null;

		private InlineWidthAttribute GetFieldOverride(string name)
		{
			if (_outerFieldOverrides == null)
			{
				_outerFieldOverrides = new();

				foreach (var attr in fieldInfo.GetCustomAttributes<InlineWidthAttribute>())
				{
					if (!string.IsNullOrEmpty(attr.fieldName))
					{
						_outerFieldOverrides[attr.fieldName] = attr;
					}
				}
			}
			return _outerFieldOverrides.GetValueOrDefault(name, null);
		}

		protected override void OnField(in DrawContext ctx)
		{
			if (_currentWidths.Length == 0)
			{
				DrawerGUI.MutedInfo(ctx.position, "No fields", MessageType.Warning);
				return;
			}

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