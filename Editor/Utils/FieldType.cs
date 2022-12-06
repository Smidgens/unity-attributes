// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;

	// comparison flags for drawer type validation
	[Flags]
	internal enum FieldType
	{
		None = 0,
		Int = 1,
		String = 2,
		Float = 4,
		Bool = 8,
		Object = 16,
		Color = 32,
		Enum = 64,
		Any = ~0
	}
}

#endif