// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditorInternal;
	using System;
	using SP = UnityEditor.SerializedProperty;
	using Menu = UnityEditor.GenericMenu;

	internal static class MenuFactory
	{

		private static readonly Lazy<GUIContent>
		_NO_OPTIONS_LABEL = new Lazy<GUIContent>(() =>
		{
			return new GUIContent(EConstants.Info.NO_POPUP_OPTIONS);
		});

		public static Menu StringifiedValues<T>
		(
			in T value,
			in T[] values,
			Action<T> setFn,
			Func<T, int, string> stringFn = null,
			Func<T,T,bool> compareFn = null
		)
		{
			var m = new Menu();
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


		public static Menu SortingLayers(in int value, Action<int> setFn)
		{
			var m = new Menu();

			m.AddItem(new GUIContent(EConstants.Label.POPUP_DEFAULT), value == -1, () => setFn.Invoke(-1));
			m.AddSeparator("");

			var layers = SortingLayer.layers;

			foreach(var l in layers)
			{
				var id = l.id;
				var label = $"{l.id}: {l.name}";
				m.AddItem(new GUIContent(label), id == value, () => setFn.Invoke(id));
			}
			return m;
		}

#if ATTRIBUTES_ANIMATION_1
		public static Menu AnimatorParameters(
			Animator animator,
			in string value,
		Action<int> setFn
		)
		{
			var m = new Menu();
			m.AddItem(new GUIContent(Config.Label.POPUP_DEFAULT), value == "", () => setFn.Invoke(-1));
			m.AddSeparator("");
			if(animator.parameterCount == 0)
			{
				m.AddDisabledItem(new GUIContent(Config.Info.NO_POPUP_OPTIONS));
			}

			for(var i = 0; i < animator.parameterCount; i++)
			{
				var pi = i;
				var p = animator.GetParameter(i);
				var val = p.name;
				var label = $"{p.type}/{p.name}";
				m.AddItem(new GUIContent(label), p.name == value, () => setFn.Invoke(pi));
			}


			//foreach (var p in animator.parameters)
			//{
			//	var val = p.name;
			//	var label = $"{p.type}/{p.name}";
			//	m.AddItem(new GUIContent(label), value == val, () => setFn.Invoke(val));
			//}
			return m;
		}

#endif

		public static Menu Layers(in int value, Action<int> setFn)
		{
			return GetIntMenu
			(
				value,
				InternalEditorUtility.layers.Stringify(),
				setFn,
				true,
				i => $"{i}: "
			);
		}

		public static Menu GetMenu(SP prop, in int[] values, bool showDefault = false, Func<int, string> prefixFn = null)
		{
			return GetIntMenu
			(
				prop,
				values.Stringify(),
				showDefault,
				prefixFn
			);
		}

		public static Menu GetIntMenu(SP prop, in string[] labels, bool showDefault = false, Func<int, string> prefixFn = null)
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

		public static Menu GetIntMenu(in int value, in string[] labels, Action<int> setFn, bool showDefault = false, Func<int, string> prefixFn = null)
		{
			var m = new Menu();
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