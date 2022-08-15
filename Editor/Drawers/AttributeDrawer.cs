// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;

	[Flags]
	internal enum PType
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

	internal abstract class AttributeDrawer<T> : PropertyDrawer where T : BaseAttribute
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			using(new EditorGUI.PropertyScope(pos, l, prop))
			{
				// label
				DrawPrefixLabel(ref pos, l);
				string err = null;

				if (!Validate(prop, ref err))
				{
					DrawerGUI.MutedInfo(pos, err);
					return;
				}

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
					DrawIcon(cols[0], ctx);
				}
				DrawField(ctx);
			}
		}

		protected T _Attribute => attribute as T;

		protected struct FieldContext
		{
			public Rect position;
			public SerializedProperty property;
			public T attribute;
		}

		protected virtual bool HasIcon()
		{
			return false;
		}

		protected virtual void DrawIcon(in Rect pos, in FieldContext ctx)
		{
		}

		protected virtual bool Validate(SerializedProperty prop, ref string msg)
		{
			var types = GetSupportedTypes();
			if(types != PType.Any)
			{
				return types.HasFlag(GetTypeFlag(prop.propertyType));
			}
			return true;
		}

		protected virtual void DrawPrefixLabel(ref Rect pos, GUIContent l)
		{
			var lt = ((T)attribute).Label;
			if (!string.IsNullOrEmpty(lt))
			{
				l.text = lt;			}

			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);
		}

		protected virtual void DrawField(in FieldContext ctx)
		{
			DrawerGUI.MutedInfo(ctx.position, Config.Info.NOT_IMPLEMENTED);
		}

		protected virtual PType GetSupportedTypes()
		{
			return PType.Any;
		}

		private static PType GetTypeFlag(SerializedPropertyType pt)
		{
			switch (pt)
			{
				case SerializedPropertyType.String: return PType.String;
				case SerializedPropertyType.Integer: return PType.Int;
				case SerializedPropertyType.Float: return PType.Float;
				case SerializedPropertyType.Boolean: return PType.Bool;
				case SerializedPropertyType.ObjectReference: return PType.Object;
				case SerializedPropertyType.Color: return PType.Color;
			}
			return PType.Any;
		}

	}
}