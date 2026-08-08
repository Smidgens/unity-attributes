// smidgens @ github

/*
 * TODOS
 *	- move calculation and cache into drawer
 */

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// Draw struct/class fields on one line
	/// </summary>
	public sealed class InlineAttribute : __BaseControl
	{
		public InlineAttribute() { }

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
			Normalize(sizes);
			return sizes;
		}

		private static void Normalize(in float[] sizes)
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

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(InlineAttribute))]
	internal sealed class _InlineAttribute : __ControlDrawer<InlineAttribute>
	{
		protected override void OnInit()
		{
			var opts = fieldInfo.GetCustomAttributes<FieldSizeAttribute>().ToArray();
			_Attribute.Init(fieldInfo.GetItemType(), opts);
			_fields = _Attribute.Fields;
			_sizes = _Attribute.Sizes;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var ti = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// todo: optimize this
			var cols = ctx.position.CalcColumns(2.0, _sizes);

			for (var i = 0; i < _fields.Length; i++)
			{
				var col = cols[i];
				var innerProp = ctx.property.FindPropertyRelative(_fields[i]);
				if (innerProp == null)
				{
					EditorGUI.DrawRect(col, Color.red * 0.3f);
					GUI.Box(col, "?");
					continue;
				}
				EditorGUI.PropertyField(col, innerProp, GUIContent.none);
			}

			EditorGUI.indentLevel = ti;
		}

		private string[] _fields = null;
		private float[] _sizes = null;
	}

}

#endif