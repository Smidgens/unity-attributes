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
		#if UNITY_6000_0_OR_NEWER
		protected __Base(bool collection = false) : base(collection) {}
		#else
		protected __Base(bool collection = false){}
		#endif
	}

}