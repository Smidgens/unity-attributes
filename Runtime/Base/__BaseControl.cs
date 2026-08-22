// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public abstract class __BaseControl : __Base
	{
		protected __BaseControl(){}

		protected __BaseControl(bool buttons, bool collection = false) : base(collection)
		{
			this.buttons = buttons;
		}

		internal bool buttons { get; }
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
				h += GetButtonAreaHeight();
				h += EditorGUIUtility.standardVerticalSpacing;
			}

			return h;
		}

		private float GetButtonAreaHeight()
		{
			var h = 0f;
			var rows = 1;
			var cWidth = 0f;

			foreach (var a in _actions)
			{
				if (a.attribute == null)
				{
					continue;
				}
				if (cWidth + a.attribute.width > 1.0001f)
				{
					rows++;
					cWidth = 0f;
				}
				cWidth += a.attribute.width;
			}

			h += DrawerStyles.ButtonHeightSM * rows;
			h += EditorGUIUtility.standardVerticalSpacing;
			return h;
		}

		public sealed override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			DrawActions(ref pos, prop);

			EditorGUI.BeginProperty(pos, l, prop);

			var tEnabled = GUI.enabled;
			GUI.enabled &= IsFieldEditable();

			var customLabel = GetCustomLabel();

			if (customLabel == null)
			{
				l = GUIContent.none;
			}
			else if (customLabel != string.Empty)
			{
				l.text = customLabel;
			}

			// label
			OnLabel(ref pos, prop, l);

			if (!CanDraw(prop, out var err))
			{
				DrawerGUI.MutedInfo(pos, err, MessageType.Warning);
				return;
			}

			if (_displayIcon.texture)
			{
				var prefixRect = pos.SliceLeft(pos.height).Resized(-pos.height * 0.1f);
				pos.SliceLeft(EditorGUIUtility.standardVerticalSpacing);
				var coords = Mathf.Approximately(_displayIcon.coords.width, 0f)
				? new Rect(0, 0, 1f, 1f)
				: _displayIcon.coords;
				var color = Mathf.Approximately(_displayIcon.color.a, 0f)
				? Color.white
				: _displayIcon.color;

				if (!GUI.enabled)
				{
					color *= 0.8f;
				}
				DrawerGUI.DrawTex(prefixRect, _displayIcon.texture, coords, color);
			}

			var ctx = new DrawContext
			{
				position = pos,
				property = prop,
				label = l,
			};

			OnField(ctx);
			GUI.enabled = tEnabled;
			EditorGUI.EndProperty();
		}

		protected struct DisplayIcon
		{
			public Texture texture;
			public Rect coords;
			public Color color;
		}

		protected virtual DisplayIcon GetFieldDisplayIcon()
		{
			return default;
		}

		private bool IsFieldEditable()
		{
			if (_EditFlags == EFieldUsable.Always)
			{
				return true;
			}

			if (Application.isPlaying && _EditFlags.HasFlag(EFieldUsable.Play))
			{
				return true;
			}

			if (!Application.isPlaying && _EditFlags.HasFlag(EFieldUsable.Editor))
			{
				return true;
			}
			return false;
		}

		protected T _Attribute => attribute as T;

		protected int _ExtraIndent { get; private set; }

		// absolute type of field
		protected Type _FieldType => _fieldType;

		protected EFieldUsable _EditFlags { get; private set; } = EFieldUsable.Always;


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

		private static readonly List<ActionInfo> _EMPTY_ACTIONS = new();

		private bool _init;
		private List<ActionInfo> _actions;
		private string _customLabel = string.Empty;
		private Type _fieldType; // absolute field type, array or no
		private DisplayIcon _displayIcon;

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
					err = PluginConstants.Msg.INVALID_TYPE;
				}
			}
			return result;
		}

		protected virtual void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
		}

		protected virtual void OnField(in DrawContext ctx)
		{
			DrawerGUI.MutedInfo(ctx.position, PluginConstants.Msg.NOT_IMPLEMENTED);
		}

		protected virtual EFieldType GetValidTypes() => EFieldType.Any;
		protected virtual void OnInit() { }
		protected string GetCustomLabel() => _customLabel;

		private struct ActionInfo
		{
			public FieldButtonAttribute attribute;
			public string label;
			public MethodInfo method;

			public void Invoke(object target)
			{
				if (method == null)
				{
					return;
				}

				if (method.IsStatic)
				{
					method.Invoke(null, attribute.args);
				}
				else
				{
					method.Invoke(target, attribute.args);
				}
			}
		}

		private void EnsureInit()
		{
			if (_init)
			{
				return;
			}

			_init = true;
			_fieldType = fieldInfo.FieldType.GetInnermostType();

			_displayIcon = GetFieldDisplayIcon();

			var options = GetMod<FieldOptionsAttribute>();

			if (options != null)
			{
				_ExtraIndent = options.indent;
				_customLabel = options.label;
				_EditFlags = options.useFlags;
			}

			_actions = GetFieldActions();
	
			OnInit();
		}

		protected IEnumerable<MT> GetMods<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttributes<MT>();
		}

		protected MT GetMod<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttribute<MT>();
		}

		private List<ActionInfo> GetFieldActions()
		{
			if (!_Attribute.buttons)
			{
				return _EMPTY_ACTIONS;
			}

			if (!fieldInfo.IsDefined(typeof(FieldButtonAttribute)))
			{
				return _EMPTY_ACTIONS;
			}
			
			var r = new List<ActionInfo>();

			foreach (var attr in GetMods<FieldButtonAttribute>())
			{
				var method = attr.GetMethod(fieldInfo);

				if (method == null)
				{
					// r.Add(default);
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
			if (_actions ==  null || _actions.Count == 0)
			{
				return;
			}
			
			DrawerGUI.IndentRect(ref posx, EditorGUI.indentLevel);

			Rect rowRect = posx.SliceTop(DrawerStyles.ButtonHeightSM);
			var cWidth = 0f;
			foreach (var a in _actions)
			{
				var w = a.attribute.width;

				if (cWidth + w > 1.0001f)
				{
					rowRect = posx.SliceTop(DrawerStyles.ButtonHeightSM);
					cWidth = 0f;
				}
				cWidth += w;
				var btnRect = rowRect.SliceLeft(w * posx.width);

				if (a.method == null)
				{
					DrawerGUI.MutedInfo(btnRect, PluginConstants.Msg.NOT_FOUND);
					continue;
				}

				object target = null;

				if (!a.method.IsStatic)
				{
					if (a.attribute.useInner)
					{
						target = prop.boxedValue;
					}
					else
					{
						// if parent is null then we "should" be at the root serialized object level
						var parentProp = prop.GetParent();
						target = parentProp != null
						? parentProp.boxedValue
						: prop.serializedObject.targetObject;
					}
				}

				var serializedTarget = prop.serializedObject.targetObject;
				bool enabled = (target != null || a.method.IsStatic) && a.attribute.flags.GetUseState(serializedTarget);

				var te = GUI.enabled;
				GUI.enabled = enabled;
				
				if (GUI.Button(btnRect, a.label, DrawerStyles.ButtonSM))
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