// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Linq;
	using System.Reflection;

	using SP = UnityEditor.SerializedProperty;

	[Flags]
	internal enum FieldType
	{
		None = 0,
		Int = 1,
		String = 2,
		Float = 4,
		Bool = 8,
		Object = 16,
		Color = 32,
		Enum = 64,
		Any = ~0
	}

	internal abstract class __ControlDrawer<T> : PropertyDrawer where T : __BaseControl
	{
		public const string DEFAULT_MSG = Config.Info.NOT_IMPLEMENTED;

		public const float BUTTON_MARGIN = 5f;

		public override sealed float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!_init)
			{
				_init = true;
				if (_Attribute.buttons)
				{
					_buttons = GetModifiers<FieldButtonAttribute>();
					if (_buttons.Length > 0)
					{
						_buttonHeight = GUI.skin.button.CalcHeight(new GUIContent("btn"), 200f);
						_totalButtonHeight = _buttonHeight * _buttons.Length;
						_totalButtonHeight += BUTTON_MARGIN;
					}
				}
				OnInit();
			}
			var h = GetHeight(property, label);
			return _totalButtonHeight + h;
		}

		public override sealed void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			if(_buttons != null && _buttons.Length > 0)
			{
				var brect = pos;
				brect.height = _totalButtonHeight - BUTTON_MARGIN;
				pos.height -= _totalButtonHeight;
				pos.position += new Vector2(0f, _totalButtonHeight);
				DrawButtons(brect, prop);
			}

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				var indent = GetIndentLevel();

				using (new EditorGUI.IndentLevelScope(indent))
				{
					// label
					OnLabel(ref pos, l);
					string err = null;

					if (!CanDraw(prop, ref err))
					{
						DrawerGUI.MutedInfo(pos, err);
						return;
					}
					pos = EditorGUI.IndentedRect(pos);
					var ctx = new FieldContext
					{
						position = pos,
						property = prop,
						attribute = (T)attribute,
					};

					if (HasIcon())
					{
						var cols = pos.CalcColumns(2.0, pos.height, 1f);
						ctx.position = cols[1];
						OnIcon(cols[0], ctx);
					}
					OnField(ctx);
				}
			}
		}

		protected T _Attribute => attribute as T;
		private float _buttonHeight = 0f;
		private float _totalButtonHeight = 0f;
		private FieldButtonAttribute[] _buttons = null;

		private void DrawButtons(in Rect rect, SP prop)
		{
			using(new GUI.ClipScope(rect))
			{
				var pos = rect;
				pos.position = default;
				var btnPos = pos;
				btnPos.height = _buttonHeight;
				var target = prop.serializedObject.targetObject;

				var ttype = target.GetType();

				for (var i = 0; i < _buttons.Length; i++)
				{

					var r = btnPos;
					r.height = _buttonHeight;
					r.position = pos.position + new Vector2(0f, i * _buttonHeight);

					var a = _buttons[i];
					var m = a.GetMethod(ttype);

					using(new EditorGUI.DisabledGroupScope(m == null))
					{
						if (GUI.Button(r, a.Label))
						{
							a.Invoke(target);
						}
					}
					//EditorGUI.DrawRect(pos, Color.blue * 0.2f);
				}
			}
		}

		protected struct FieldContext
		{
			public Rect position;
			public SP property;
			public T attribute;
		}

		protected virtual float GetHeight(SP prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, label);
		}


		protected MT[] GetModifiers<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttributes<MT>().ToArray();
		}

		protected RT[] GetModifiers<MT,RT>(Func<MT,RT> fn) where MT : __BaseModifier
		{
			return
			fieldInfo.GetCustomAttributes<MT>().Select(fn).ToArray();
		}

		protected MT GetModifier<MT>() where MT : __BaseModifier
		{
			return fieldInfo.GetCustomAttribute<MT>();
		}

		protected virtual bool HasIcon() => false;
		protected virtual void OnIcon(in Rect pos, in FieldContext ctx) { }

		protected virtual bool CanDraw(SerializedProperty prop, ref string msg)
		{
			var types = GetValidTypes();
			if(types != FieldType.Any)
			{
				return types.HasFlag(GetFlag(prop.propertyType));
			}
			return true;
		}

		protected virtual void OnLabel(ref Rect pos, GUIContent l)
		{
			var pa = _Attribute.Prefix.FromField(fieldInfo);
			var label = pa != null ? pa.Label : l.text;
			if (label == null) { return; }
			l.text = label;
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
		}

		protected virtual void OnField(in FieldContext ctx)
		{
			DrawerGUI.MutedInfo(ctx.position, DEFAULT_MSG);
		}

		protected virtual FieldType GetValidTypes() => FieldType.Any;

		private bool _init = false;

		protected virtual void OnInit()
		{
			
		}


		private int GetIndentLevel()
		{
			var im = _Attribute.Indent.FromField(fieldInfo);
			return im != null ? im.Indent : 0;
		}

		private static FieldType GetFlag(SerializedPropertyType pt)
		{
			switch (pt)
			{
				case SerializedPropertyType.Enum: return FieldType.Enum;
				case SerializedPropertyType.String: return FieldType.String;
				case SerializedPropertyType.Integer: return FieldType.Int;
				case SerializedPropertyType.Float: return FieldType.Float;
				case SerializedPropertyType.Boolean: return FieldType.Bool;
				case SerializedPropertyType.ObjectReference: return FieldType.Object;
				case SerializedPropertyType.Color: return FieldType.Color;
			}
			return FieldType.Any;
		}

	}
}