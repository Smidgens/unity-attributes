// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(IsChildAttribute))]
	internal class SelectChildComponent_Drawer : AttributeDrawer<IsChildAttribute>
	{
		protected override bool Validate(SerializedProperty prop, ref string msg)
		{
			if(prop.propertyType != SerializedPropertyType.ObjectReference)
			{
				msg = Config.Info.FIELD_NON_COMPONENT;
				return false;
			}
			return true;
		}

		protected override void DrawField(in FieldContext ctx)
		{
			base.DrawField(ctx);
		}
	}
}