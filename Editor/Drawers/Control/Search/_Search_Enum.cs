﻿// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SearchEnumAttribute))]
	internal class _Search_Enum : __ControlDrawer<SearchEnumAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Enum;

		protected override void OnField(in DrawContext ctx)
		{
			base.OnField(ctx);
		}
	}
}

#endif