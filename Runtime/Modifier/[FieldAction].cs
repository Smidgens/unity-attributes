// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System.Reflection;
	using System;
	using Editor;

	public enum EFieldAction
	{
		/// <summary>
		/// Play mode only
		/// </summary>
		PlayMode = 1,
		/// <summary>
		/// Call method on outer type
		/// </summary>
		DeclaringType = 2,
		/// <summary>
		/// Sensible defaults
		/// </summary>
		Default = PlayMode
	}

	/// <summary>
	/// Displays action above specific field
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class FieldActionAttribute : __BaseModifier
	{
		public FieldActionAttribute
		(
			string methodName,
			EFieldAction flags = EFieldAction.Default,
			string label = null
		)
		{
			_methodName = methodName;
			this.label = label;
			this.flags = flags;
		}

		internal string label { get; }
		internal EFieldAction flags { get; }

		internal MethodInfo GetMethod(FieldInfo field)
		{
			var type = field.FieldType.GetInnermostType();
			if (flags.HasFlag(EFieldAction.DeclaringType))
			{
				type = field.DeclaringType;
			}
			var m = (type!).GetMethod(_methodName, _FLAGS, null, Array.Empty<Type>(), null);
			if (m == null || m.ReturnType != typeof(void))
			{
				return null;
			}
			return m;
		}

		private string _methodName { get; }

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Instance;
	}
}