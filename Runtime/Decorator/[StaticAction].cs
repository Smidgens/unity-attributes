// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	public enum EStaticAction
	{
		/// <summary>
		/// Play mode only
		/// </summary>
		PlayMode = 1,
		/// <summary>
		/// Sensible defaults
		/// </summary>
		Default = PlayMode,
	}

	public sealed class StaticActionAttribute : __BaseDecorator
	{
		public StaticActionAttribute
		(
			string methodPath,
			EStaticAction flags = EStaticAction.Default,
			string label = "",
			string prefixLabel = ""
		)
		{
			this.flags = flags;
			this.label = label;
			this.prefixLabel = prefixLabel;
			_method = ReflectionUtils.ParseStaticMethodString(methodPath);
		}

		internal string prefixLabel { get; }
		internal string label { get; }
		internal EStaticAction flags { get; }

		internal (Action, MethodInfo) GetAction()
		{
			if (!_action.Item2)
			{
				var m = _method;
				if (m == null)
				{
					_action = ((null, null), true);
					return _action.Item1;
				}
				_action = (((Action)m.CreateDelegate(typeof(Action)), m), true);
			}
			return _action.Item1;
		}

		private readonly MethodInfo _method;
		private ((Action, MethodInfo), bool) _action;

	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;

	[CustomPropertyDrawer(typeof(StaticActionAttribute))]
	internal sealed class _StaticActionAttribute : __DecoratorDrawer<StaticActionAttribute>
	{
		protected override void OnInit()
		{
			_label = GUIContent.none;

			var (fn, method) = _Attribute.GetAction();

			if (fn != null)
			{
				var l = !string.IsNullOrEmpty(_Attribute.label)
				? _Attribute.label
				: method.Name.ToSentenceCase();
				_label = new GUIContent(l);
			}
		}
		
		private static readonly Lazy<GUIStyle> _BTN_STYLE = new(() =>
		{
			return new GUIStyle(EditorStyles.miniButton)
			{
				fontSize = (int)(EditorStyles.miniButton.fontSize * 0.9f)
			};
		});

		private static readonly Lazy<float> _BTN_HEIGHT = new (() =>
		{
			return _BTN_STYLE.Value.CalcHeight(GUIContent.none, 100);
		});

		protected override float GetHeight(in float w)
		{
			return _BTN_HEIGHT.Value;
		}

		protected override void OnContent(in Rect p)
		{
			var pos = p;
			var te = GUI.enabled;
			var (fn, method) = _Attribute.GetAction();

			if (fn == null)
			{
				DrawerGUI.MutedInfo(p, "Not found");
				return;
			}

			if (!string.IsNullOrEmpty(_Attribute.prefixLabel))
			{
				pos = EditorGUI.PrefixLabel(pos, new GUIContent(_Attribute.prefixLabel));
			}

			GUI.enabled = !(!Application.isPlaying && _Attribute.flags.HasFlag(EStaticAction.PlayMode));

			var id = GUIUtility.GetControlID(FocusType.Keyboard, pos);

			if(GUI.Button(pos, _label, _BTN_STYLE.Value))
			{
				fn?.Invoke();
				GUIUtility.keyboardControl = id;
			}

			if (GUIUtility.keyboardControl == id)
			{
				EditorGUI.DrawRect(pos, EditorStyles.label.focused.textColor.Fade(0.2f));
				
				if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
				{
					fn?.Invoke();
				}
			}
			
			GUI.enabled = te;
		}

		private GUIContent _label;
		private MethodInfo _method;
	}
}

#endif