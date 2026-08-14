// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	
	internal static class Array_
	{
		public static string[] Stringify<T>(this T[] arr)
		{
			if (typeof(T) == typeof(string))
			{
				return arr as string[];
			}

			if (arr.Length == 0)
			{
				return Array.Empty<string>();
			}

			var labels = new string[arr.Length];
			for (var i = 0; i < arr.Length; i++)
			{
				labels[i] = arr[i].ToString();
			}
			return labels;
		}

		public static GUIContent[] ToGUIContent(this string[] arr)
		{
			if (arr.Length == 0)
			{
				return Array.Empty<GUIContent>();
			}
			GUIContent[] labels = new GUIContent[arr.Length];
			for (int i = 0; i < labels.Length; i++)
			{
				labels[i] = new GUIContent(arr[i]);
			}
			return labels;
		}
	}
}

#endif