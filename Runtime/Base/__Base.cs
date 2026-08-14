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
		protected __Base(bool collection = false) : base(collection)
		{
			
		}
	}

}