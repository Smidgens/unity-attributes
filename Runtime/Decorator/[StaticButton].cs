// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Reflection;

	public sealed class StaticButtonAttribute : __BaseDecorator
	{
		public StaticButtonAttribute
		(
			string methodPath,
			EFieldUsable flags = EFieldUsable.Always,
			string label = null,
			string prefixLabel = null
		)
		{
			this.flags = flags;
			this.label = label ?? string.Empty;
			this.prefixLabel = prefixLabel ?? String.Empty;
			_method = ReflectionUtils.ParseStaticMethodString(methodPath);
		}

		internal string prefixLabel { get; }
		internal string label { get; }
		internal EFieldUsable flags { get; }
		internal readonly MethodInfo _method;
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;

	[CustomPropertyDrawer(typeof(StaticButtonAttribute))]
	internal sealed class _StaticButtonAttribute : __DecoratorDrawer<StaticButtonAttribute>
	{
		protected override void OnInit()
		{
			var (fn, method) = GetAction();

			if (fn != null)
			{
				var l = !string.IsNullOrEmpty(_Attribute.label)
				? _Attribute.label
				: method.Name.ToSentenceCase();
				_label = new GUIContent(l);
			}
		}
		
		protected override (float, float) GetVerticalMargins()
		{
			return default;
		}

		protected override float GetHeight(in float w)
		{
			return DrawerStyles.ButtonHeightSM;
		}

		protected override void OnContent(in Rect p)
		{
			var pos = p;
			var te = GUI.enabled;
			var (fn, method) = GetAction();

			if (fn == null)
			{
				DrawerGUI.MutedInfo(p, PluginConstants.Msg.NOT_FOUND);
				return;
			}

			if (!string.IsNullOrEmpty(_Attribute.prefixLabel))
			{
				pos = EditorGUI.PrefixLabel(pos, new GUIContent(_Attribute.prefixLabel));
			}

			GUI.enabled = _Attribute.flags.GetUseState();

			var id = GUIUtility.GetControlID(FocusType.Keyboard, pos);

			if(GUI.Button(pos, _label, DrawerStyles.ButtonSM))
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
		
		internal (Action, MethodInfo) GetAction()
		{
			if (!_action.Item2)
			{
				var m = _Attribute._method;
				if (m == null)
				{
					_action = ((null, null), true);
					return _action.Item1;
				}
				_action = (((Action)m.CreateDelegate(typeof(Action)), m), true);
			}
			return _action.Item1;
		}
		
		private ((Action, MethodInfo), bool) _action;

		private GUIContent _label = new();
	}
}

#endif