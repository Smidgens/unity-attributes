// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public class SearchTypeAttribute : __BaseControl
	{
		public bool hideNested = true;
		public bool hideAbstract = false;
		public bool showHidden = false;
		
		public bool onlyStatic = false;
		public bool onlyInterfaces = false;

		public string[] namespaces = null;
		public string[] assemblies = null;
		public Type[] baseTypes = null;
	}
}