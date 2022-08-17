// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	// common empties
	internal static class Empty
	{
		public static class Array
		{
			public static readonly Material[] MATERIAL = Get<Material>();
			public static readonly float[] FLOAT = Get<float>();
			public static readonly string[] STRING = Get<string>();
			public static readonly int[] INT = Get<int>();
			public static readonly object[] OBJECT = Get<object>();
		}
		private static T[] Get<T>() => new T[0];
	}

}