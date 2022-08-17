// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Select index of material in referenced renderer
	/// </summary>
	public class RendererMaterialAttribute : __BaseControl
	{
		/// <summary>
		/// Name of renderer field
		/// </summary>
		public string RendererFieldPath { get; }

		/// <summary>
		/// Init with field of renderer
		/// </summary>
		public RendererMaterialAttribute(string field) => RendererFieldPath = field;
	}
}