// smidgens @ github

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