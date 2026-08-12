// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	public sealed class StaticActionAttribute : __BaseDecorator
	{
		public StaticActionAttribute
		(
			string method,
			Type type,
			string label = "",
			bool playMode = false
		)
		{
			this.playMode = playMode;
			this.label = label;
			methodName = method;
			_declaringType = type;
		
		}

		internal string label { get; }
		internal bool playMode { get; }

		internal Action GetAction()
		{
			if (!_action.Item2)
			{
				var m =
				_declaringType.GetMethod(methodName, _FLAGS, null, Array.Empty<Type>(), null);

				if (m == null)
				{
					return null;
				}

				if (m.GetParameters().Length != 0)
				{
					return null;
				}

				if(m.ReturnType != typeof(void))
				{
					return null;
				}
				_action = ((Action)m.CreateDelegate(typeof(Action)), true);
			}
			return _action.Item1;
		}

		private (Action, bool) _action;
		private readonly Type _declaringType;
		internal string methodName { get; }

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
			var l = !string.IsNullOrEmpty(_Attribute.label)
			? _Attribute.label
			: _Attribute.methodName.ToSentenceCase();
			_label = new GUIContent(l);
		}

		protected override void OnContent(in Rect pos)
		{
			var te = GUI.enabled;
			var fn = _Attribute.GetAction();
			var disabled = fn == null || (!Application.isPlaying && _Attribute.playMode);

			GUI.enabled = !disabled;

			if(GUI.Button(pos, _label))
			{
				fn?.Invoke();
			}
			GUI.enabled = te;
		}

		private GUIContent _label;
		private MethodInfo _method;
	}
}

#endif