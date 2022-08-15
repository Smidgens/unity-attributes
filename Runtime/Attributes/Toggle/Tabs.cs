// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;
	using System.Linq;
	using UnityEngine;

	/// <summary>
	/// Draw bool fields as tabs
	/// </summary>
	public class TabsAttribute : BaseAttribute
	{
		public TabsAttribute() { }

		internal string[] Fields { get; private set; } = { };
		internal Type Type { get; private set; } = null;

		internal void Init(Type t)
		{
			Fields = FindSerializedFields(t);
			Type = t;
		}

		private const BindingFlags _BFLAGS =
		BindingFlags.Instance
		| BindingFlags.Public
		| BindingFlags.NonPublic;

		private static string[] FindSerializedFields(Type t)
		{
			return t.GetFields(_BFLAGS)
			.Where(x =>
			{
				if (x.IsNotSerialized) { return false; }
				if (x.FieldType != typeof(bool)) { return false; }
				if (x.GetCustomAttribute<HideInInspector>() != null)
				{
					return false;
				}
				return true;
			})
			.Select(x => x.Name)
			.ToArray();
		}
	}
}
