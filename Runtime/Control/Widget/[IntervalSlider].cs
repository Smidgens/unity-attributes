// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Interval slider with min/max handles
	/// </summary>
	public sealed class IntervalSliderAttribute : __BaseControl
	{
		public IntervalSliderAttribute
		(
			float min,
			float max,
			string fMin = "x",
			string fMax = "y",
			float step = 0
		)
		{
			if (min > max)
			{
				(min, max) = (max, min);
			}
			this.min = min;
			this.max = max;
			this.minField = fMin;
			this.maxField = fMax;
			this.step = step;
		}
		
		internal float min { get; }
		internal float max { get; }
		internal string minField { get; }
		internal string maxField { get; }
		internal float step { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(IntervalSliderAttribute))]
	internal sealed class _IntervalSliderAttribute : __ControlDrawer<IntervalSliderAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;

			var (minProp, maxProp) = GetMinMaxProps(ctx.property);

			if (minProp == null)
			{
				DrawerGUI.MutedInfo(pos, "!");
				return;
			}

			var inputWidth = 35f;
			var minSliderWidth = 30f;

			var compact = pos.width - inputWidth * 2f < minSliderWidth;
			
			if (compact)
			{
				inputWidth = pos.width * 0.5f;
			}

			var leftLabel = pos.SliceLeft(inputWidth);
			pos.SliceLeft(EditorGUIUtility.standardVerticalSpacing * 2f);
			var rightLabel = pos.SliceRight(inputWidth);
			pos.SliceRight(EditorGUIUtility.standardVerticalSpacing * 2f);

			var minVal = _Attribute.min;
			var maxVal = _Attribute.max;

			var isFloat = minProp.propertyType == SerializedPropertyType.Float;

			var min = isFloat ? minProp.floatValue : minProp.intValue;
			var max = isFloat ? maxProp.floatValue : maxProp.intValue;

			var step = _Attribute.step;
	
			min = EditorGUI.DelayedFloatField(leftLabel, GUIContent.none, min, _NUM_FIELD.Value);
			min = Mathf.Clamp(min, minVal, max);

			max = EditorGUI.DelayedFloatField(rightLabel, GUIContent.none, max, _NUM_FIELD.Value);
			max = Mathf.Clamp(max, min, maxVal);

			if (!compact)
			{
				EditorGUI.MinMaxSlider(pos, ref min, ref max, minVal, maxVal);
			}

			if(step > 0f)
			{
				min = ((int)(min / step)) * step;
				max = ((int)(max / step)) * step;
			}

			if (isFloat)
			{
				minProp.floatValue = min;
				maxProp.floatValue = max;
			}
			else
			{
				minProp.intValue = (int)min;
				maxProp.intValue = (int)max;
			}
		}

		private static readonly Lazy<GUIStyle> _NUM_FIELD = new(() => new GUIStyle(EditorStyles.numberField)
		{
			fontSize = (int)(EditorStyles.numberField.fontSize * 0.9f),
			alignment = TextAnchor.MiddleLeft
		});

		private (SerializedProperty,SerializedProperty) GetMinMaxProps(SerializedProperty prop)
		{
			var minProp = prop.FindPropertyRelative(_Attribute.minField);
			var maxProp = prop.FindPropertyRelative(_Attribute.maxField);

			if (minProp == null || maxProp == null)
			{
				return default;
			}

			if (!minProp.IsNumeric() || minProp.propertyType != maxProp.propertyType)
			{
				return default;
			}

			return (minProp, maxProp);
		}

	}
}

#endif