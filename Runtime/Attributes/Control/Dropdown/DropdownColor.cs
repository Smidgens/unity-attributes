// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public class DropdownColorAttribute : __BaseControl
	{
		public static readonly Color DEFAULT_COLOR = Color.clear;

		internal readonly string[] Labels = null;
		internal readonly Color[] Values = null;
		internal readonly string[] HTMLValues = null;

		public string GetLabel(in int i)
		{
			return i >= 0 && i < Labels.Length ? Labels[i] : "?";
		}

		public DropdownColorAttribute(params string[] htmlColors)
		{
			if (htmlColors == null || htmlColors.Length == 0) { return; }
			var values = new Color[htmlColors.Length];
			Labels = htmlColors;

			for (var i = 0; i < values.Length; i++)
			{
				if (ColorUtility.TryParseHtmlString(htmlColors[i], out Color c))
				{
					values[i] = c;
				}
				else { values[i] = DEFAULT_COLOR; }
			}
			Values = values;
		}

		private static readonly Color[] _EMPTY_ARR_COLOR = new Color[0];
		private static readonly string[] _EMPTY_ARR_STRING = new string[0];
	}
}