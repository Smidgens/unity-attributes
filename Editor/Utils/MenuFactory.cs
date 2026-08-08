// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;

	internal static class MenuFactory
	{
		private static readonly Lazy<GUIContent>
		_NO_OPTIONS_LABEL = new (() => new GUIContent(EConstants.Info.NO_POPUP_OPTIONS));

		public static GenericMenu StringifiedValues<T>
		(
			in T value,
			in T[] values,
			Action<T> setFn,
			Func<T, int, string> stringFn = null,
			Func<T,T,bool> compareFn = null
		)
		{
			var m = new GenericMenu();
			m.allowDuplicateNames = true;

			if (values.Length == 0)
			{
				m.AddDisabledItem(_NO_OPTIONS_LABEL.Value);
			}
			for (var i = 0; i < values.Length; i++)
			{
				var v = values[i];
				var l = stringFn == null ? v.ToString() : stringFn.Invoke(v, i);
				var isActive = compareFn == null ? value.Equals(v) : compareFn.Invoke(value, v);
				m.AddItem(new GUIContent(l), isActive, () => setFn.Invoke(v));
			}
			return m;
		}

		public static GenericMenu GetMenu(SP prop, in int[] values, bool showDefault = false, Func<int, string> prefixFn = null)
		{
			return GetIntMenu
			(
				prop,
				values.Stringify(),
				showDefault,
				prefixFn
			);
		}

		public static GenericMenu GetIntMenu(SP prop, in string[] labels, bool showDefault = false, Func<int, string> prefixFn = null)
		{
			return GetIntMenu
			(
				prop.intValue,
				labels,
				v => Set(prop,v),
				showDefault,
				prefixFn
			);
		}

		public static GenericMenu GetIntMenu(in int value, in string[] labels, Action<int> setFn, bool showDefault = false, Func<int, string> prefixFn = null)
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


		private static void Set(SP p, int v)
		{
			p.intValue = v;
			p.serializedObject.ApplyModifiedProperties();
		}


	}

}

#endif