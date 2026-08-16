// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Wraps numeric field as progress bar
	/// </summary>
	public sealed class ProgressAttribute : __BaseControl
	{
		public ProgressAttribute(float min, float max, string label = "") : base(true)
		{
			if (min > max)
			{
				(min, max) = (max, min);
			}
			this.min = min;
			this.max = max;
			this.label = label;
		}

		internal float min { get; }
		internal float max { get; }
		internal string label { get; }
		
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(ProgressAttribute))]
	internal sealed class _ProgressAttribute : __ControlDrawer<ProgressAttribute>
	{
		protected override EFieldType GetValidTypes()
		{
			return EFieldType.Numeric;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;
			var val = prop.IsFloat() ? prop.floatValue : prop.intValue;
			val = Mathf.Clamp(val, _Attribute.min, _Attribute.max);
			var t = Mathf.InverseLerp(_Attribute.min, _Attribute.max, val);
			EditorGUI.ProgressBar(ctx.position, t, _Attribute.label);
		}

	}
}

#endif