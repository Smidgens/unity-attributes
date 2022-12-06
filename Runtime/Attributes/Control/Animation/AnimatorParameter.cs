// smidgens @ github

#if ATTRIBUTES_ANIMATION_1

namespace Smidgenomics.Unity.Attributes
{
	public sealed class AnimatorParameterAttribute : __BaseControl
	{
		public string AnimatorField { get; }

		public AnimatorParameterAttribute(string field)
		{
			AnimatorField = field;
		}
	}
}

#endif