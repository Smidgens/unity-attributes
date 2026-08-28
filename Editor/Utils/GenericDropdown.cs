// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using UnityEditor.IMGUI.Controls;

	/// <summary>
	/// Basic dropdown
	/// </summary>
	[Serializable]
	internal sealed class GenericDropdown<T> : AdvancedDropdown
	{
		public GenericDropdown(string title, T currentValue = default, AdvancedDropdownState state = null) : base(state ?? new AdvancedDropdownState())
		{
			_title = title;
		}

		public T currentValue;
		public Action<T> onSelected;

		public void AddItem(string label, T value, Texture2D icon = null)
		{
			_options.Add(new Option
			{
				label = label,
				icon = icon,
				value = value,
				enabled = true
			});
		}
		
		public void AddSeparator(string path)
		{
			_options.Add(new Option
			{
				label = null,
			});
		}

		public void AddDisabledItem(string label, Texture2D icon = null)
		{
			_options.Add(new Option
			{
				label = label,
				icon = icon,
			});
		}

		public void Show(Rect pos, float maxHeight)
		{
			var titleWidth = EditorStyles.boldLabel.CalcSize(new GUIContent(_title)).x;
			Show(pos);
			var maxWidth = Mathf.Max(pos.width * 2f, Mathf.Max(200f, titleWidth));
			SetLastDropdownHeight(pos, maxHeight, maxWidth);
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var root = new AdvancedDropdownItem(_title);
			foreach (var opt in _options)
			{
				if (string.IsNullOrEmpty(opt.label))
				{
					root.AddSeparator();
					continue;
				}
				var item = new TypedDropdownItem(opt.label, opt.value)
				{
					enabled = opt.enabled && !AreEqual(currentValue, opt.value),
					icon = opt.icon,
				};
				root.AddChild(item);
			}
			return root;
		}

		private readonly List<Option> _options = new();
		private readonly string _title;

		private struct Option
		{
			public string label;
			public T value;
			public Texture2D icon;
			public bool enabled;
		}

		private sealed class TypedDropdownItem : AdvancedDropdownItem
		{
			public TypedDropdownItem(string name, T value) : base(name)
			{
				this.value = value;
			}
			public T value { get; }
		}

		// hardly the most robust comparison, should switch to comparable later
		private bool AreEqual(T v1, T v2)
		{
			var h1 = v1 == null ? 0 : v1.GetHashCode();
			var h2 = v2 == null ? 0 : v2.GetHashCode();
			return h1 == h2;
		}

		protected override void ItemSelected(AdvancedDropdownItem item)
		{
			if (item is TypedDropdownItem it)
			{
				onSelected?.Invoke(it.value);
			}
		}

		// hack to force height
		private static void SetLastDropdownHeight(Rect rect, float maxHeight, float maxWidth = 0f)
		{
			var window = EditorWindow.focusedWindow;

			if(!window || window.GetType().Name != "AdvancedDropdownWindow")
			{
				return;
			}

			var position = window.position;

			position.height = Mathf.Min(maxHeight, position.height);
			
			if (!Mathf.Approximately(0f, maxWidth))
			{
				position.width = Mathf.Min(maxWidth, position.width);
			}
			window.minSize = position.size;
			window.maxSize = position.size;
			window.position = position;
			window.ShowAsDropDown(GUIUtility.GUIToScreenRect(rect), position.size);
		}

		
	}
}

#endif