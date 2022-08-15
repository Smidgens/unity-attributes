// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TabsAttribute))]
	internal class Tabs_ : AttributeDrawer<TabsAttribute>
	{
		protected override void DrawField(in FieldContext ctx)
		{
			var a = (TabsAttribute)attribute;

			if(_Attribute.Type == null)
			{
				_Attribute.Init(fieldInfo.GetInnerType());
			}

			var fields = a.Fields;

			if (fields.Length == 0)
			{
				GUI.Box(ctx.position, "");
				return;
			}

			var cols = ctx.position.CalcColumns(fields.Length, 2.0);

			for (var i = 0; i < cols.Length; i++)
			{
				var tProp = ctx.property.FindPropertyRelative(fields[i]);
				if (tProp == null || tProp.propertyType != SerializedPropertyType.Boolean)
				{
					EditorGUI.DrawRect(cols[i], Color.red * 0.2f);
					continue;
				}

				tProp.boolValue = DrawerGUI.TabButton(cols[i], tProp.boolValue, tProp.displayName);
			}
		}
	}
}