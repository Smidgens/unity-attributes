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
			string prefixLabel = null,
			object[] args = null
		)
		{
			this.flags = flags;
			this.label = label ?? string.Empty;
			this.prefixLabel = prefixLabel ?? String.Empty;
			this.args = args;

			Type[] aTypes = null;
			if (args is { Length: > 0 })
			{
				aTypes = new Type[args.Length];
				for (int i = 0; i < args.Length; i++)
				{
					aTypes[i] = args[i].GetType();
				}
			}
			method = ReflectionUtils.ParseStaticMethodString(methodPath, null, aTypes);
		}
		
		internal object[] args { get; }
		internal string prefixLabel { get; }
		internal string label { get; }
		internal EFieldUsable flags { get; }
		internal MethodInfo method { get; }
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
			var method = GetMethod().method;

			if (method != null)
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
			var mRef = GetMethod();

			if (mRef.method == null)
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
				mRef.Invoke();
				GUIUtility.keyboardControl = id;
			}

			if (GUIUtility.keyboardControl == id)
			{
				EditorGUI.DrawRect(pos, EditorStyles.label.focused.textColor.Fade(0.2f));
				
				if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
				{
					mRef.Invoke();
				}
			}
			GUI.enabled = te;
		}
		
		private (MethodRef, bool) _cache;
		private GUIContent _label = new();
		
		private MethodRef GetMethod()
		{
			if (!_cache.Item2)
			{
				var m = _Attribute.method;
				if (m == null)
				{
					_cache = (default, true);
					return _cache.Item1;
				}
				_cache = (MethodRef.FromMethod(m, _Attribute.args), true);
			}
			return _cache.Item1;
		}

		private struct MethodRef
		{
			public MethodInfo method { get; private set; }
			private object[] _args;
			private Action _onInvoke;

			public static MethodRef FromMethod(MethodInfo m, object[] args)
			{
				var mr = new MethodRef
				{
					method = m,
					_args = args
				};
				return mr;
			}

			public void Invoke()
			{
				if (_onInvoke == null)
				{
					var aCount = _args?.Length ?? 0;
					// optimization for no args
					if (aCount == 0)
					{
						_onInvoke = (Action)method.CreateDelegate(typeof(Action), null);
					}
					// optimization for one arg with common type
					else if (aCount == 1)
					{
						var val = _args![0];
						_onInvoke = TryGetClosureDelegate<int>(val)
						?? TryGetClosureDelegate<float>(val)
						?? TryGetClosureDelegate<bool>(val)
						?? TryGetClosureDelegate<double>(val)
						?? TryGetClosureDelegate<string>(val)
						?? TryGetClosureDelegate<Type>(val);
					}
					_onInvoke ??= InvokeMethodDefault;
				}
				_onInvoke.Invoke();
			}

			// invoke method via reflection
			private void InvokeMethodDefault() => method.Invoke(null, _args);

			private Action TryGetClosureDelegate<T>(object val)
			{
				if (val is not T tVal)
				{
					return null;
				}
				var del = (Action<T>)method.CreateDelegate(typeof(Action<T>), null);
				return () => del.Invoke(tVal);
			}
		}
		
	}
}

#endif