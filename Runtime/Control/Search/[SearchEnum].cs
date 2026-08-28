// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Popup search for enum
	/// </summary>
	public sealed class SearchEnumAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SearchEnumAttribute))]
	internal sealed class _SearchEnumAttribute : __ControlDrawer<SearchEnumAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Enum;

		protected override void OnInit()
		{
			var vals = new List<(string, int)>();
			var names = Enum.GetNames(_FieldType);
			var values = (int[])Enum.GetValues(_FieldType);
			for (int i = 0; i < names.Length; i++)
			{
				vals.Add((ObjectNames.NicifyVariableName(names[i]), values[i]));
			}
			_values = vals;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;
			var label = Enum.GetName(_FieldType, prop.intValue);

			if (!string.IsNullOrEmpty(label))
			{
				label = ObjectNames.NicifyVariableName(label);
			}
			_popupLabel.text = label ?? PluginConstants.Label.POPUP_UNSET;
			if (EditorGUI.DropdownButton(ctx.position, _popupLabel, FocusType.Keyboard))
			{
				var title = ObjectNames.NicifyVariableName(_FieldType.Name);
				var drop = new GenericDropdown<int>(title, prop.intValue)
				{
					onSelected = v =>
					{
						prop.intValue = v;
						prop.serializedObject.ApplyModifiedProperties();
					}
				};
				foreach (var (l, v) in _values)
				{
					drop.AddItem(l, v);
				}
				drop.Show(ctx.position, 300f);
			}
		}

		private IReadOnlyList<(string, int)> _values;
		private readonly GUIContent _popupLabel = new();
	}
}

#endif