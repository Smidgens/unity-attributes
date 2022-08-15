// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;
	using System.Diagnostics;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public abstract class BaseAttribute : PropertyAttribute
	{
		public string Label { get; set; } = null;
	}
}

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;
	using System.Diagnostics;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public abstract class BaseOptionAttribute : PropertyAttribute { }
}


namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;
	using System;
	using System.Diagnostics;

	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, Inherited = true)]
	public abstract class BaseDecorator : PropertyAttribute { }
}