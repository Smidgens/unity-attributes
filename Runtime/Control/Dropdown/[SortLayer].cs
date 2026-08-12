// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of sorting layers
	/// </summary>
	public sealed class SortLayerAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SortLayerAttribute))]
	internal sealed class _SortLayerAttribute : __ControlDrawer<SortLayerAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			SortLayerPopup(ctx.position, ctx.property);
		}

		private static void SortLayerPopup(in Rect pos, SerializedProperty prop)
		{
			if (!prop.IsInt())
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			var lName = SortingLayer.IDToName(prop.intValue);
			var valid = !string.IsNullOrEmpty(lName);

			var label = valid
				? $"{SortingLayer.GetLayerValueFromID(prop.intValue)}: {lName}"
				: "<none>";

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();
				foreach (var sLayer in SortingLayer.layers)
				{
					var v = sLayer.id;
					m.AddItem(new GUIContent($"{sLayer.value}: {sLayer.name}"), prop.intValue == v, () =>
					{
						prop.intValue = v;
						prop.serializedObject.ApplyModifiedProperties();
					});
				}
				m.DropDown(pos);
			}
		}
	}
}

#endif