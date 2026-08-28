// smidgens @ github

#if UNITY_2023_3_OR_NEWER

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of rendering layers
	/// </summary>
	public sealed class ProjectRenderLayerAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ProjectRenderLayerAttribute))]
	internal sealed class _ProjectRenderLayerAttribute : __ControlDrawer<ProjectRenderLayerAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			LayerPopup(ctx.position, ctx.property);
		}

		private static void LayerPopup(in Rect pos, SerializedProperty prop)
		{
			if (!prop.IsInt())
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_NON_INT);
				return;
			}

			var lName = RenderingLayerMask.RenderingLayerToName(prop.intValue);
			var valid = !string.IsNullOrEmpty(lName);

			var label = valid
			? $"{prop.intValue}: {lName}"
			: "<none>";

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();
				foreach (var (l,v) in GetOptions())
				{
					var val = v;
					m.AddItem(new GUIContent($"{v}: {l}"), prop.intValue == v, () =>
					{
						prop.intValue = val;
						prop.serializedObject.ApplyModifiedProperties();
					});
				}
				m.DropDown(pos);
			}
		}
		private static IReadOnlyList<(string, int)> GetOptions()
		{
			var l = new List<(string, int)>();
			var names = RenderingLayerMask.GetDefinedRenderingLayerNames();
			foreach (var n in names)
			{
				var val = RenderingLayerMask.NameToRenderingLayer(n);
				l.Add((n, val));
			}
			return l;
		}
		
	}
}

#endif

#endif