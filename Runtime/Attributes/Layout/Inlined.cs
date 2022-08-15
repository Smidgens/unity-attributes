// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// Set size of specific inlined field
	/// </summary>
	public class FieldSizeAttribute : BaseOptionAttribute
	{
		public FieldSizeAttribute(string name, float size)
		{
			Name = name;
			Size = size;
		}
		internal string Name { get; }
		internal float Size { get; } = -1f;
	}

	/// <summary>
	/// Draw struct/class fields on one line
	/// </summary>
	public class InlinedAttribute : BaseAttribute
	{
		public InlinedAttribute() { }

		/// <summary>
		/// Names of serialized fields to display inlined
		/// </summary>
		internal Type Type { get; private set; }
		internal string[] Fields { get; private set; } = { };
		internal float[] Sizes { get; private set; } = { };

		internal void Init(Type t, FieldSizeAttribute[] options)
		{
			Type = t;
			Init(t, options, out var fields, out var sizes);
			Fields = fields;
			Sizes = sizes;
		}

		private const BindingFlags _BFLAGS =
		BindingFlags.Instance
		| BindingFlags.Public
		| BindingFlags.NonPublic;

		private static void Init(
			Type t,
			FieldSizeAttribute[] options,
			out string[] names, out float[] sizes
		)
		{
			var fields = t.GetFields(_BFLAGS)
			.Where(x =>
			{
				if (x.IsNotSerialized) { return false; }
				if (x.GetCustomAttribute<HideInInspector>() != null)
				{
					return false;
				}
				return true;
			})
			.Select(x => x.Name)
			.ToArray();

			sizes = ComputeSizes(fields, options);
			names = fields;
		}

		private static float[] ComputeSizes
		(
			in string[] fields,
			in FieldSizeAttribute[] options
		)
		{
			if(fields.Length == 0) { return new float[0]; }
			var sizes = fields.Select(x => -1f).ToArray();
			float defWidth = 1f / fields.Length;
			for (var i = 0; i < fields.Length; i++)
			{
				var fn = fields[i];
				var oi = Array.FindIndex(options, x => x.Name == fn);
				sizes[i] = oi > -1 ? options[oi].Size : defWidth;
			}
			NormalizeSizes(sizes);
			return sizes;
		}

		private static void NormalizeSizes(in float[] sizes)
		{
			if (sizes.Length == 0) { return; }
			float rtotal = 0f;
			var ratio = new List<int>();
			var flex = new List<int>();
			for (var i = 0; i < sizes.Length; i++)
			{
				var w = sizes[i];
				if (w > 1f) { continue; }
				if (w <= 0f) { flex.Add(i); continue; }
				rtotal += w;
				ratio.Add(i);
			}
			float flexRemainder = 1f - rtotal;
			if (flexRemainder > 0f && flex.Count > 0)
			{
				var fw = flexRemainder / flex.Count;
				foreach (var fi in flex) { sizes[fi] = fw; }
				rtotal += flexRemainder;
			}
			foreach (var ri in ratio)
			{
				sizes[ri] = sizes[ri] / rtotal;
			}
		}
	}
}