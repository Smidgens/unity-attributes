// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public sealed class AnimatorParameterAttribute : BaseAttribute
	{
		public string AnimatorField { get; }

		public AnimatorParameterAttribute(string field)
		{
			AnimatorField = field;
		}
	}
}