// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class AssetOptionsAttribute : BaseAttribute
	{
		public readonly static string[] DEFAULT_FOLDERS =
		{
			"Assets/" // project wide
		};

		/// <summary>
		/// Thumbnail mode (0 = none, 1 = mini, 2 = preview)
		/// </summary>
		public int ThumbQuality { get; set; } = 1;
		public string[] Folders { get; } = DEFAULT_FOLDERS;

		public AssetOptionsAttribute(params string[] folders) => Folders = folders;
	}
}