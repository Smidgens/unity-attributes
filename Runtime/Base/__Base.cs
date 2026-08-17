// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;
	using System.Diagnostics;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class __Base : PropertyAttribute
	{
		protected __Base() {}
	
#if UNITY_6000_0_OR_NEWER
		// unity 6-specific collection support
		protected __Base(bool collection) : base(collection) {}
#endif
	}

}