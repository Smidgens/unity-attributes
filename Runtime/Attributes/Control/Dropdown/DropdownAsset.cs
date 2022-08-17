// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public class DropdownAssetAttribute : __BaseControl
	{
		/// <summary>
		/// Thumbnail mode (0 = none, 1 = mini, 2 = preview)
		/// </summary>
		public byte thumbQuality { get; set; } = 1;

		public readonly static string[] DEFAULT_FOLDERS =
		{
			"Assets/" // project wide
		};

		public DropdownAssetAttribute(params string[] folders) => Folders = folders;

		internal readonly string[] Folders = null;
	}
}