// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	[AttributeUsage(AttributeTargets.Field)]
	public abstract class __BaseControl : __Base
	{
		protected __BaseControl(){}

		protected __BaseControl(bool showActions)
		{
			this.showActions = showActions;
		}

		internal bool showActions { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	internal abstract class __ControlDrawer<T> : PropertyDrawer where T : __BaseControl
	{
		public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			EnsureInit();
			
			var h = GetHeight(property, label);
			
			if(_actions.Count > 0)
			{
				var aRows = (_actions.Count / 2) + (_actions.Count % 2);
				h += _BTN_HEIGHT.Value * aRows;
				h += EditorGUIUtility.standardVerticalSpacing;
			}

			return h;
		}

		public sealed override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			DrawActions(ref pos, prop);

			EditorGUI.BeginProperty(pos, l, prop);

			// label
			OnLabel(ref pos, l);

			if (!CanDraw(prop, out var err))
			{
				DrawerGUI.MutedInfo(pos, err, MessageType.Warning);
				return;
			}
			
			DrawerGUI.IndentRect(ref pos, _extraIndent);

			pos = EditorGUI.IndentedRect(pos);
			var ctx = new DrawContext
			{
				position = pos,
				property = prop,
				label = l,
			};
			OnField(ctx);

			EditorGUI.EndProperty();
			
		}

		protected T _Attribute => attribute as T;

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

		protected struct DrawContext
		{
			public Rect position;
			public SerializedProperty property;
			public GUIContent label;
		}

		protected virtual float GetHeight(SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, label);
		}

		private bool CanDraw(SerializedProperty prop, out string err)
		{
			var types = GetValidTypes();
			err = null;

			var result = true;
			if (types != EFieldType.Any)
			{
				var validTypes = types.HasFlag(prop.GetTypeFlags());
				result &= validTypes;
				if (!validTypes)
				{
					err = "Unsupported field type";
				}
			}
			return result;
		}

		protected virtual void OnLabel(ref Rect pos, GUIContent l)
		{
			var customLabel = GetCustomLabel();

			if (customLabel == null)
			{
				return;
			}
			if (customLabel != string.Empty)
			{
				l.text = customLabel;
				// DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
				// return;
			}
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
		}

		protected virtual void OnField(in DrawContext ctx)
		{
			DrawerGUI.MutedInfo(ctx.position, EConstants.Info.NOT_IMPLEMENTED);
		}

		protected virtual EFieldType GetValidTypes() => EFieldType.Any;
		protected virtual void OnInit() { }

		protected string GetCustomLabel() => _customLabel;

		private bool _init;
		private List<ActionInfo> _actions;
		private byte _extraIndent;
		private string _customLabel = string.Empty;

		private struct ActionInfo
		{
			public FieldActionAttribute attribute;
			public string label;
			public MethodInfo method;

			public void Invoke(object target)
			{
				if (method == null || target == null)
				{
					return;
				}
				method.Invoke(target, null);
			}
		}

		private void EnsureInit()
		{
			if (_init)
			{
				return;
			}

			_init = true;

			var options = GetMod<FieldOptionsAttribute>();

			if (options != null)
			{
				_extraIndent = options.indent;
				_customLabel = options.label;
			}

			_actions = GetFieldActions();
	
			OnInit();
		}

		private IEnumerable<MT> GetMods<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttributes<MT>();
		}

		private MT GetMod<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttribute<MT>();
		}

		private static readonly List<ActionInfo> _EMPTY_ACTIONS = new();

		private List<ActionInfo> GetFieldActions()
		{
			if (!_Attribute.showActions)
			{
				return _EMPTY_ACTIONS;
			}

			var elType = fieldInfo.FieldType.GetInnermostType();

			// 
			if (elType.IsPrimitive && !elType.IsStruct())
			{
				return _EMPTY_ACTIONS;
			}

			if (!fieldInfo.IsDefined(typeof(FieldActionAttribute)))
			{
				return _EMPTY_ACTIONS;
			}
			
			var r = new List<ActionInfo>();

			foreach (var attr in GetMods<FieldActionAttribute>())
			{
				var method = attr.GetMethod(fieldInfo);

				if (method == null)
				{
					r.Add(default);
					continue;
				}

				var info = new ActionInfo
				{
					attribute = attr,
					label = attr.label ?? method.Name.ToSentenceCase(),
					method = method,
				};
				r.Add(info);
			}
			return r;
		}

		private void DrawActions(ref Rect posx, SerializedProperty prop)
		{
			if (_actions.Count == 0)
			{
				return;
			}

			Rect rowRect = default;
			var left = true;

			var i = -1;
			foreach (var a in _actions)
			{
				i++;
				Rect btnRect;

				if (left)
				{
					rowRect = posx.SliceTop(_BTN_HEIGHT.Value);
					btnRect = i < _actions.Count - 1
					? rowRect.SliceLeft(rowRect.width * 0.5f)
					: rowRect;
				}
				else
				{
					btnRect = rowRect;
				}

				left = !left;

				if (a.method == null)
				{
					DrawerGUI.MutedInfo(btnRect, "Not found");
					continue;
				}

				var target = a.attribute.flags.HasFlag(EFieldAction.OuterObject)
				? prop.serializedObject.targetObject
				: fieldInfo.GetValue(prop.serializedObject.targetObject);

				bool enabled =
				target != null
				&& !(!Application.isPlaying && a.attribute.flags.HasFlag(EFieldAction.PlayMode));
				
				var te = GUI.enabled;
				GUI.enabled = enabled;
				
				if (GUI.Button(btnRect, a.label, _BTN_STYLE.Value))
				{
					a.Invoke(target);
				}
				GUI.enabled = te;
			}
			posx.SliceTop(EditorGUIUtility.standardVerticalSpacing);
		}

	}
}

#endif