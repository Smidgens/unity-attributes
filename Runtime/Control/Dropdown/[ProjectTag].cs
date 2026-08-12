// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of project tags
	/// </summary>
	public sealed class ProjectTagAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using System;
	using UnityEngine;
	using UnityEditorInternal;

	[CustomPropertyDrawer(typeof(ProjectTagAttribute))]
	internal sealed class _ProjectTagAttribute : __ControlDrawer<ProjectTagAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			TagPopup(ctx.position, ctx.property);
		}

		private static void TagPopup(in Rect pos, SerializedProperty prop)
		{
			if (!prop.IsString())
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_STRING);
				return;
			}
			var isEmpty = string.IsNullOrEmpty(prop.stringValue);

			var label = isEmpty
				? EConstants.Label.POPUP_DEFAULT
				: prop.stringValue;

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();

				Action<string> setFn = v =>
				{
					prop.stringValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				};

				// add option
				Action<string, string> optionFn = (l, v) =>
				{
					m.AddItem(new GUIContent(l), prop.stringValue == v, () => setFn.Invoke(v));
				};

				optionFn.Invoke(EConstants.Label.POPUP_DEFAULT, "");

				m.AddSeparator("");

				foreach (var tag in InternalEditorUtility.tags)
				{
					optionFn.Invoke(tag, tag);
				}
				m.DropDown(pos);
			}
		}
	}
}

#endif