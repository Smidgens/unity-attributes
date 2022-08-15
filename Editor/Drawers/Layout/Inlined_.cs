﻿// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(InlinedAttribute))]
	internal class Inlined_ : AttributeDrawer<InlinedAttribute>
	{
		protected override void DrawField(in FieldContext ctx)
		{
			if (_Attribute.Type == null)
			{
				var opts = fieldInfo.GetCustomAttributes<FieldSizeAttribute>().ToArray();
				_Attribute.Init(fieldInfo.GetInnerType(), opts);
			}
			var fields = _Attribute.Fields;
			var sizes = _Attribute.Sizes;
			var cols = ctx.position.CalcColumns(2.0, sizes);

			for (var i = 0; i < fields.Length; i++)
			{
				var innerProp = ctx.property.FindPropertyRelative(fields[i]);
				if (innerProp == null)
				{
					EditorGUI.DrawRect(cols[i], Color.red * 0.3f);
					GUI.Box(cols[i], "?");
					continue;
				}
				EditorGUI.PropertyField(cols[i], innerProp, GUIContent.none);
			}
		}
	}

}