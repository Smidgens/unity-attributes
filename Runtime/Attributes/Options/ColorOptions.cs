// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public class ColorOptionsAttribute : BaseAttribute
	{
		public static readonly Color DEFAULT_COLOR = Color.clear;

		public string[] Labels { get; } = _EMPTY_ARR_STRING;
		public Color[] Values { get; } = _EMPTY_ARR_COLOR;
		public string[] HTMLValues { get; } = _EMPTY_ARR_STRING;

		public string GetLabel(in int i)
		{
			return i >= 0 && i < Labels.Length ? Labels[i] : "?";
		}

		public ColorOptionsAttribute(params string[] htmlColors)
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