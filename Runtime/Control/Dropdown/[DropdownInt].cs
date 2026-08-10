// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	public sealed class DropdownIntAttribute : __BaseControl
	{
		internal int[] Values { get; }

		public DropdownIntAttribute(int start, int n)
		{
			Values = GetValues(start, n);
		}

		public DropdownIntAttribute(params int[] values)
		{
			Values = values ?? Array.Empty<int>();
		}

		private static int[] GetValues(int start, int n)
		{
			if (n <= 0)
			{
				return Array.Empty<int>();
			}
			int[] values = new int[n];
			for (var i = 0; i < n; i++)
			{
				values[i] = start + i;
			}
			return values;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using System;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(DropdownIntAttribute))]
	internal sealed class _DropdownIntAttribute : __ControlDrawer<DropdownIntAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			IntegerDropdown(ctx.position, ctx.property, _Attribute.Values);
		}
		
		private static void IntegerDropdown(in Rect pos, SerializedProperty prop, in int[] options)
		{
			// valid type?
			if (prop.propertyType != SerializedPropertyType.Integer)
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			if (GUI.Button(pos, prop.intValue.ToString(), EditorStyles.popup))
			{
				GetIntMenu(prop.intValue, options.Stringify(), v =>
				{
					prop.intValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				})
				.DropDown(pos);
			}
		}
		
		private static GenericMenu GetIntMenu(in int value, string[] labels, Action<int> setFn, bool showDefault = false, Func<int, string> prefixFn = null)
		{
			var m = new GenericMenu();
			m.allowDuplicateNames = true;

			if (showDefault)
			{
				m.AddItem(new GUIContent(EConstants.Label.POPUP_DEFAULT), value == -1, () => setFn.Invoke(-1));
				m.AddSeparator("");
			}

			for (var i = 0; i < labels.Length; i++)
			{
				var lv = i;
				var prefix = prefixFn?.Invoke(i) ?? "";
				var ll = $"{prefix}{labels[i]}";
				m.AddItem(new GUIContent(ll), value == i, () => setFn.Invoke(lv));
			}
			return m;
		}
	}
}

#endif