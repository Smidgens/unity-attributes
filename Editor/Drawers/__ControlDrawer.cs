// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using SP = UnityEditor.SerializedProperty;

	// comparison flags for drawer type validation
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

	internal static class SerializedPropertyType_
	{
		public static FieldType GetTypeFlags(this SP prop)
		{
			var pt = prop.propertyType;
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

	internal abstract class __ControlDrawer<T> : PropertyDrawer where T : __BaseControl
	{
		public const string DEFAULT_MSG = Config.Info.NOT_IMPLEMENTED;
		public const float BUTTON_MARGIN = 2f;
		public const float BUTTON_HEIGHT = 19f; // find later

		public override sealed float GetPropertyHeight(SP property, GUIContent label)
		{
			EnsureInit();
			return
			_totalActionHeight
			+ GetHeight(property, label);
		}

		public override sealed void OnGUI(Rect pos, SP prop, GUIContent l)
		{

			DrawActions(ref pos, prop);

			using (new EditorGUI.PropertyScope(pos, l, prop))
			{
				using (new EditorGUI.IndentLevelScope(_extraIndent))
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
					var ctx = new DrawContext
					{
						position = pos,
						property = prop,
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

		protected struct DrawContext
		{
			public Rect position;
			public SP property;
		}

		protected virtual float GetHeight(SP prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, label);
		}

		protected virtual bool HasIcon() => false;
		protected virtual void OnIcon(in Rect pos, in DrawContext ctx) { }

		protected virtual bool CanDraw(SP prop, ref string msg)
		{
			var types = GetValidTypes();
			if (types != FieldType.Any)
			{
				return types.HasFlag(prop.GetTypeFlags());
			}
			return true;
		}

		protected virtual void OnLabel(ref Rect pos, GUIContent l)
		{
			var (cprefix, hasPrefix) = _customLabel;

			if (hasPrefix)
			{
				if(cprefix == null) { return; }
				l.text = cprefix;
			}
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
		}

		protected virtual void OnField(in DrawContext ctx)
		{
			DrawerGUI.MutedInfo(ctx.position, DEFAULT_MSG);
		}

		protected virtual FieldType GetValidTypes() => FieldType.Any;
		protected virtual void OnInit() { }

		protected MT FindMod<MT>() where MT : __BaseModifier
		{
			var i = IndexOfMod<MT>();
			return i > -1 ? _mods[i] as MT : null;
		}

		protected int IndexOfMod<MT>() where MT : __BaseModifier
		{
			for(var i = 0; i < _mods.Count; i++)
			{
				if (_mods[i].GetType() == typeof(MT)) { return i; }
			}
			return -1;
		}

		protected bool HasCustomLabel() => _customLabel.Item2;
		protected string GetCustomLabel() => _customLabel.Item1;

		private bool _init = false;
		private float _totalActionHeight = 0f;
		private List<__BaseModifier> _mods = null;
		private List<ActionInfo> _actions = null;
		private byte _extraIndent = 0;
		private (string, bool) _customLabel = default;

		private struct ActionInfo
		{
			public FieldActionAttribute attribute;
			public string label;
			public MethodInfo method;

			public void Invoke(object target)
			{
				if(method == null) { return; }
				if(target == null) { return; }
				method.Invoke(method.IsStatic ? null : target, attribute.Args);
			}
		}

		private void EnsureInit()
		{
			if (_init) { return; }

			_init = true;

			_mods = new List<__BaseModifier>();
			foreach(var a in fieldInfo.GetCustomAttributes<__BaseModifier>())
			{
				_mods.Add(a);
			}

			var indent = FindMod<IndentAttribute>();
			if(indent != null) { _extraIndent = indent.Indent; }

			var prefix = FindMod<FieldNameAttribute>();
			if(prefix != null) { _customLabel = (prefix.Label, true); }

			_actions = FindActions();

			if(_actions != null)
			{
				_totalActionHeight = BUTTON_HEIGHT * _actions.Count;
				_totalActionHeight += BUTTON_MARGIN;
			}
			OnInit();
		}

		private List<ActionInfo> FindActions()
		{
			if (!_Attribute.buttons) { return null; }
			var i = IndexOfMod<FieldActionAttribute>();
			if(i < 0) { return null; }
			var r = new List<ActionInfo>();
			var methodOwner = fieldInfo.DeclaringType;
	
			for (;i < _mods.Count; i++)
			{
				var m = _mods[i];
				if(m.GetType() != typeof(FieldActionAttribute)) { continue; }
				var am = m as FieldActionAttribute;
				var info = new ActionInfo
				{
					attribute = am,
					label = am.Label,
					method = am.GetMethod(methodOwner),
				};
				r.Add(info);
			}
			return r;
		}

		private void DrawActions(ref Rect posx, SP prop)
		{
			if(_actions == null) { return; }

			var brect = posx;
			brect.height = _totalActionHeight - BUTTON_MARGIN;

			posx.height -= _totalActionHeight;
			posx.position += new Vector2(0f, _totalActionHeight);

			var pos = brect;

			var buttonRow = pos;
			buttonRow.height = BUTTON_HEIGHT;
			var startPos = buttonRow.position;

			var target = prop.serializedObject.targetObject;

			for (var i = 0; i < _actions.Count; i++)
			{
				buttonRow.position =
				startPos + new Vector2(0f, i * BUTTON_HEIGHT);

				var a = _actions[i];

				var te = GUI.enabled;
				GUI.enabled = a.method != null;
				if (GUI.Button(buttonRow, a.label))
				{
					a.Invoke(target);
				}
				GUI.enabled = te;
			}
		}

	}
}