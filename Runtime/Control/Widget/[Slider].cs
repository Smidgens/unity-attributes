// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	/// <summary>
	/// Number slider with rounding support
	/// </summary>
	public sealed class SliderAttribute : __BaseControl
	{
		internal int precision { get; }
		internal float min { get; }
		internal float max { get; }
		internal float step { get; }

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
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SliderAttribute))]
	[CustomPropertyDrawer(typeof(Slider01Attribute))]
	internal sealed class _SliderAttribute : __ControlDrawer<__BaseControl>
	{
		protected override void OnField(in DrawContext ctx)
		{
			if (_Attribute is Slider01Attribute)
			{
				var a01 = (_Attribute as Slider01Attribute)!;
				DrawSlider(ctx.position, ctx.property, 0f, 1f, a01.step, a01.precision);
			}
			else
			{
				var a = (_Attribute as SliderAttribute)!;
				DrawSlider(ctx.position, ctx.property, a.min, a.max, a.step, a.precision);
			}
			
		}

		private static void DrawSlider(
			in Rect pos,
			SerializedProperty prop,
			in float min,
			in float max,
			in float step = -1f,
			in int precision = -1)
		{

			if (!prop.IsNumeric())
			{
				DrawerGUI.MutedInfo(pos, "Field is not numeric");
				return;
			}

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