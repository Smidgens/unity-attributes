// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	internal class IsTypeAttribute : BaseAttribute
	{
		public Type[] Types { get; } = null;

		public IsTypeAttribute(Type type) : this(new Type[] { type }) { }

		public IsTypeAttribute(params Type[] types)
		{
			Types = types ?? new Type[0];
		}
	}
}