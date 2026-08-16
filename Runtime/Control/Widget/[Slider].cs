// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	/// <summary>
	/// Numeric slider with step and rounding options
	/// </summary>
	public sealed class SliderAttribute : __BaseControl
	{
		public SliderAttribute(float min, float max, float step = 0f, int precision = 1)
		{
			if (min > max)
			{
				(min, max) = (max, min);
			}
			this.min = min;
			this.max = max;
			this.precision = Mathf.Max(precision, 1);
			this.step = Mathf.Max(step, 0f);
		}

		internal int precision { get; }
		internal float min { get; }
		internal float max { get; }
		internal float step { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	internal sealed class _SliderAttribute : __ControlDrawer<SliderAttribute>
	{
		protected override EFieldType GetValidTypes()
		{
			return EFieldType.Numeric;
		}

		protected override void OnField(in DrawContext ctx)
		{
			DrawSlider(ctx.position, ctx.property, _Attribute.min, _Attribute.max, _Attribute.step, _Attribute.precision);
		}
		private static void DrawSlider(
			in Rect pos,
			SerializedProperty prop,
			in float min,
			in float max,
			in float step = -1f,
			in int precision = -1)
		{
			var val = prop.IsFloat() ? prop.floatValue : prop.intValue;

			EditorGUI.BeginChangeCheck();
			float valueNew = EditorGUI.Slider(pos, val, min, max);
			if (EditorGUI.EndChangeCheck())
			{
				if (precision >= 1)
				{
					valueNew = valueNew.Round(precision);
				}
				if(step > 0f)
				{
					valueNew = ((int)(valueNew / step)) * step;
				}
				valueNew = Mathf.Clamp(valueNew, min, max);

				if (prop.IsFloat())
				{
					prop.floatValue = valueNew;
				}
				else
				{
					prop.intValue = (int)valueNew;
				}
			}
			
		}

	}
}

#endif