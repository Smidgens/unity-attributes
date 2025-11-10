// smidgens @ github

// resharper disable all

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public sealed class SearchTypeAttribute : __BaseControl
	{
		public readonly bool hideNested = true;
		public readonly bool hideAbstract = false;
		public readonly bool showHidden = false;

		public readonly bool onlyStatic = false;
		public readonly bool onlyInterfaces = false;

		public readonly string[] namespaces = null;
		public readonly string[] assemblies = null;
		public readonly Type[] baseTypes = null;
	}
}