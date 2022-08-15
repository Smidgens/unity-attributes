// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public static partial class PropRef
	{
		/// <summary>
		/// Select index of material in referenced renderer
		/// </summary>
		public class RendererMaterialAttribute : BaseAttribute
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
}