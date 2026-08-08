// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	public sealed class StaticActionAttribute : __BaseDecorator
	{
		public bool onlyPlayMode;

		public StaticActionAttribute
		(
			string methodName,
			Type declaringType,
			params object[] args
		) : this(methodName, methodName, declaringType, args) { }

		public StaticActionAttribute
		(
			string label,
			string methodName,
			Type declaringType,
			params object[] args
		)
		{
			order = 2;

			Label = label;
			_methodName = methodName;
			_declaringType = declaringType;
			Args = args;
			_argTypes = new Type[args.Length];
			for(var i = 0; i < args.Length; i++)
			{
				_argTypes[i] = args[i].GetType();
			}
		}

		internal readonly object[] Args = null;
		internal readonly string Label = null;

		internal MethodInfo GetMethod()
		{
			if (!_method.Item2)
			{
				var m =
				_declaringType.GetMethod(_methodName, _FLAGS, null, _argTypes, null);

				_method = (m, true);
			}
			return _method.Item1;
		}

		private readonly Type _declaringType = null;
		private readonly string _methodName = null;
		private readonly Type[] _argTypes = null;
		private (MethodInfo, bool) _method = default; // lazy cache

		private const BindingFlags _FLAGS =
		BindingFlags.Public
		| BindingFlags.NonPublic
		| BindingFlags.Static;

	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;

	[CustomPropertyDrawer(typeof(StaticActionAttribute))]
	internal sealed class _StaticActionAttribute : __DecoratorDrawer<StaticActionAttribute>
	{
		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.Label);
			_method = _Attribute.GetMethod();
		}

		protected override (byte, byte) GetMargin()
		{
			return (0,0);
		}

		protected override (byte, byte, byte, byte) GetPadding()
		{
			return (0, 0, 0, 0);
		}

		protected override float GetHeight(in float w) => 19f;

		protected override void OnContent(in Rect pos)
		{
			var te = GUI.enabled;

			var disabled =
				_method == null
				|| (!Application.isPlaying && _Attribute.onlyPlayMode);

			GUI.enabled = !disabled;
			if(GUI.Button(pos, _label))
			{
				_method.Invoke(null, _Attribute.Args);
			}
			GUI.enabled = te;
		}

		private GUIContent _label = null;
		private MethodInfo _method = null;
	}
}

#endif