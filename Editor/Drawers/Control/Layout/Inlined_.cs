// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(InlineAttribute))]
	internal class Inline_ : __ControlDrawer<InlineAttribute>
	{
		protected override void OnInit()
		{
			var opts = fieldInfo.GetCustomAttributes<FieldSizeAttribute>().ToArray();
			_Attribute.Init(fieldInfo.GetItemType(), opts);
			_fields = _Attribute.Fields;
			_sizes = _Attribute.Sizes;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var ti = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// todo: optimize this
			var cols = ctx.position.CalcColumns(2.0, _sizes);

			for (var i = 0; i < _fields.Length; i++)
			{
				var col = cols[i];
				var innerProp = ctx.property.FindPropertyRelative(_fields[i]);
				if (innerProp == null)
				{
					EditorGUI.DrawRect(col, Color.red * 0.3f);
					GUI.Box(col, "?");
					continue;
				}
				EditorGUI.PropertyField(col, innerProp, GUIContent.none);
			}

			EditorGUI.indentLevel = ti;
		}

		private string[] _fields = null;
		private float[] _sizes = null;
	}

}

#endif