// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Display icon for type, similar to System.ComponentModel.DisplayName
	///
	/// Used for example by [InstancedReference]
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct)]
	public sealed class DisplayIconAttribute : __BaseModifier
	{
		public DisplayIconAttribute
		(
			string iconGUID,
			float x = 0,
			float y = 0,
			float w = 1f,
			float h = 1f
		)
		{
			this.iconGUID = iconGUID;
			iconCoords = new Rect(x, y, w, h);
		}

		public string iconGUID { get; }
		public Rect iconCoords { get; }
	}
}