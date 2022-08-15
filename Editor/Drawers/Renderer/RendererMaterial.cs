// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(PropRef.RendererMaterialAttribute))]
	internal class MaterialIndex_ : PropertyDrawer
	{
		public const string EMPTY_LABEL = Config.Label.POPUP_DEFAULT;
		public const string NO_RENDERER_MSG = "no renderer";
		public const string NO_MATERIALS_MSG = "no material slots";
		public const string NULL_LABEL = "(null)";

		public override void OnGUI(Rect pos, SP prop, GUIContent l)
		{
			// label
			DrawerGUI.PrefixLabel(ref pos, l, fieldInfo);

			var ctx = new DrawerContext
			{
				attribute = (PropRef.RendererMaterialAttribute)attribute,
				property = prop,
			};

			ctx.renderer = GetRendererValue(ctx.attribute.RendererFieldPath, prop);
			DrawPopup(pos, ctx);
		}


		private struct DrawerContext
		{
			public Renderer renderer;
			public PropRef.RendererMaterialAttribute attribute;
			public SP property;
		}

		private static Renderer GetRendererValue(in string field, SP prop)
		{
			if (prop.propertyType != SerializedPropertyType.Integer) { return null; }
			var rendererProp = prop.serializedObject.FindProperty(field);
			if (rendererProp == null) { return null; }
			if (rendererProp.propertyType != SerializedPropertyType.ObjectReference) { return null; }
			return rendererProp.objectReferenceValue as Renderer;
		}

		private static void DrawPopup(Rect r, in DrawerContext ctx)
		{
			var prop = ctx.property;
			var renderer = ctx.renderer;

			// renderer-related errors
			if(HasError(renderer, out string err))
			{
				DrawerGUI.MutedInfo(r, err); return;
			}

			var materials = renderer ? renderer.sharedMaterials : Empty.Array.MATERIAL;

			string l = prop.intValue > -1 && prop.intValue < materials.Length
			? GetLabel(prop.intValue, materials[prop.intValue]?.name)
			: EMPTY_LABEL;

			if (GUI.Button(r, l, EditorStyles.popup))
			{
				GetMenu(ctx)
				.DropDown(r);
			}
		}

		private static bool HasError(Renderer r, out string msg)
		{
			msg = null;
			if (!r) { msg = NO_RENDERER_MSG; return true; }
			if(r.sharedMaterials.Length == 0) { msg = NO_MATERIALS_MSG; return true; }
			return false;
		}

		private static string GetLabel(in int i, in string txt) => $"[{i}] {txt ?? NULL_LABEL}";

		private static GenericMenu GetMenu(in DrawerContext ctx)
		{
			var menu = new GenericMenu();

			var prop = ctx.property;

			Action<int> setFn = v =>
			{
				prop.intValue = v;
				prop.serializedObject.ApplyModifiedProperties();
			};

			menu.AddItem(new GUIContent(EMPTY_LABEL), prop.intValue == -1, () => setFn.Invoke(-1));
			menu.AddSeparator("");

			var mats = ListMaterials(ctx);

			if (mats.Length == 0) { menu.AddDisabledItem(new GUIContent(NO_MATERIALS_MSG)); }

			for(var i = 0; i < mats.Length; i++)
			{
				var l = mats[i];
				var index = i;
				menu.AddItem(new GUIContent(l), prop.intValue == i, () => setFn.Invoke(index));
			}

			return menu;
		}

		private static string[] ListMaterials(in DrawerContext ctx)
		{
			Renderer renderer = ctx.renderer;
			Material[] materials = renderer?.sharedMaterials ?? Empty.Array.MATERIAL;
			var labels = new string[materials.Length];

			for(var i = 0; i < materials.Length; i++)
			{
				var label = GetLabel(i, materials[i]?.name);
				labels[i] = label;
			}
			return labels;
		}
	}
}