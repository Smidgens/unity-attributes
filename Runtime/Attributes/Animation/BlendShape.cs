// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class BlendShapeAttribute : BaseAttribute
	{
		/// <summary>
		/// Name of skinned mesh renderer field
		/// </summary>
		public string RendererField { get; }

		/// <summary>
		/// Init with field of renderer
		/// </summary>
		public BlendShapeAttribute(string field) => RendererField = field;
	}
}