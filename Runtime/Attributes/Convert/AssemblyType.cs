// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// System.Type.AssemblyQualifiedName
	/// </summary>
	public class AssemblyTypeAttribute : BaseAttribute
	{
		public Type BaseType { get; set; } = null;
		public bool Namespace { get; set; } = false;
		public bool ShowAbstract { get; set; } = false;
		public AssemblyTypeAttribute(Type baseType) => BaseType = baseType;
	}
}