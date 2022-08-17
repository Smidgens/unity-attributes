// smidgens @ github

/*
 * TODOS
 *	- custom switch icon
 */

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Switch Widget [bool,flags]
	/// </summary>
	public sealed class SwitchAttribute : __BaseControl
	{
		internal string[] Labels = { "", "" };

		public SwitchAttribute() { }

		public SwitchAttribute(string l0, string l1)
		{
			Labels = new string[] { l0, l1 };
		}

		public SwitchAttribute(string label)
		: this(label, label) { }
	}
}