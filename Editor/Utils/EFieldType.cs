// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;

	// comparison flags for drawer type validation
	[Flags]
	internal enum EFieldType
	{
		None = 0,
		Int = 1,
		Float = 2,
		Bool = 4,
		Enum = 8,
		String = 16,
		Object = 32, // UnityEngine.Object
		Color = 64,
		Primitive = Int|Float|Bool|Enum,
		Numeric = Int|Float,
		Any = ~0
	}
}

#endif