// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Toggle switch
	/// </summary>
	public class SwitchAttribute : BaseAttribute
	{
		public string[] Labels = { "Off", "On" };

		public SwitchAttribute() { }

		public SwitchAttribute(string l0, string l1)
		{
			Labels = new string[] { l0, l1 };
		}
	}
}