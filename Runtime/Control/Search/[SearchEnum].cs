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
	using UnityEditor.IMGUI.Controls;

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
				var enumDrop = new EnumDropdown(title, _values, prop.intValue, new AdvancedDropdownState())
				{
					onSelectValue = v =>
					{
						prop.intValue = v;
						prop.serializedObject.ApplyModifiedProperties();
					}
				};
				enumDrop.Show(ctx.position);
				SetLastDropdownHeight(ctx.position, 300f);
			}
		}

		private IReadOnlyList<(string, int)> _values;
		private readonly GUIContent _popupLabel = new();

		private sealed class EnumDropdown : AdvancedDropdown
		{
			private sealed class EnumDropdownItem : AdvancedDropdownItem
			{
				public int value { get; }
				public EnumDropdownItem(string name, int value) : base(name)
				{
					this.value = value;
				}
			}

			public Action<int> onSelectValue;

			public EnumDropdown(string title, IReadOnlyList<(string, int)> values, int active, AdvancedDropdownState state) : base(state)
			{
				_values = values;
				_active = active;
				_title = title;
			}

			protected override void ItemSelected(AdvancedDropdownItem item)
			{
				onSelectValue?.Invoke(((EnumDropdownItem)(item)).value);
			}

			protected override AdvancedDropdownItem BuildRoot()
			{
				var root = new AdvancedDropdownItem(_title);

				foreach (var (label, value) in _values)
				{
					var item = new EnumDropdownItem(label, value)
					{
						enabled = _active != value,
						id = value
					};
					root.AddChild(item);
				}
				return root;
			}

			private readonly IReadOnlyList<(string, int)> _values;
			private readonly int _active;
			private readonly string _title;
		}

		// hack to control size of advanced dropdown window
		private static void SetLastDropdownHeight(Rect rect, float maxHeight)
		{
			var window = EditorWindow.focusedWindow;

			if(!window || window.GetType().Name != "AdvancedDropdownWindow")
			{
				return;
			}
			var position = window.position;
			if(position.height <= maxHeight)
			{
				return;
			}
			position.height = maxHeight;
			window.minSize = position.size;
			window.maxSize = position.size;
			window.position = position;
			window.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), position.size);
		}
	}
}

#endif