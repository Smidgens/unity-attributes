// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System.Linq;

	/// <summary>
	/// Base class for all modifier attributes
	/// </summary>
	public abstract class __BaseModifier : __Base
	{
		internal virtual bool HasGUI => false;
		internal virtual float GetDrawerHeight() => 0f;

		/// <summary>
		/// 
		/// </summary>
		internal virtual void Modify(ref ModContext ctx) { }
	}

	internal struct ModContext
	{
		public int indent;
	}

}