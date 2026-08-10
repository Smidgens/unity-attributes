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
			var m = new GenericMenu
			{
				allowDuplicateNames = true
			};

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

	}

}

#endif