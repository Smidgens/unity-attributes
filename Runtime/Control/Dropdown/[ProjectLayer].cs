// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of layers
	/// </summary>
	public sealed class ProjectLayerAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ProjectLayerAttribute))]
	internal sealed class _ProjectLayerAttribute : __ControlDrawer<ProjectLayerAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			LayerPopup(ctx.position, ctx.property);
		}
		
		public static void LayerPopup(in Rect pos, SerializedProperty prop)
		{
			// invalid type
			if (prop.propertyType != SerializedPropertyType.Integer)
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_NON_INT);
				return;
			}

			var currentValue = prop.intValue;

			string currentName = LayerMask.LayerToName(currentValue);

			var btnLabel = !string.IsNullOrEmpty(currentName)
			? $"{currentValue}: {currentName}"
			: "<none>";

			if (DrawerGUI.PopupButton(pos, btnLabel))
			{
				var m = new GenericMenu();
				foreach (var layerIndex in Enumerable.Range(0, 31))
				{
					var name = LayerMask.LayerToName(layerIndex);
					if (string.IsNullOrEmpty(name))
					{
						continue;
					}

					var v = layerIndex;

					m.AddItem(new GUIContent($"{layerIndex}: {name}"), layerIndex == currentValue, () =>
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