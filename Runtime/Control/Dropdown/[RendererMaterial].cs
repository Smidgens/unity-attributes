// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Select index of material in referenced renderer
	/// </summary>
	public sealed class RendererMaterialAttribute : __BaseControl
	{
		/// <summary>
		/// Init with field of renderer
		/// </summary>
		public RendererMaterialAttribute(string field)
		{
			RendererFieldPath = field;
		}
		
		internal string RendererFieldPath { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;

	[CustomPropertyDrawer(typeof(RendererMaterialAttribute))]
	internal sealed class _RendererMaterialAttribute : __ControlDrawer<RendererMaterialAttribute>
	{
		public const string NO_RENDERER_MSG = "no renderer";
		public const string NO_MATERIALS_MSG = "no material slots";
		public const string NULL_LABEL = "(null)";

		protected override EFieldType GetValidTypes() => EFieldType.Int;
		
		protected override DisplayIcon GetFieldDisplayIcon()
		{
			return new DisplayIcon
			{
				texture = EditorGUIUtility.IconContent("Material Icon")?.image,
			};
		}

		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;
			var prop = ctx.property;

			var ctx2 = new DrawerContext
			{
				property = prop,
				renderer = GetRendererValue(_Attribute.RendererFieldPath, prop)
			};

			DrawPopup(pos, ctx2);
		}

		private struct DrawerContext
		{
			public Renderer renderer;
			public SP property;
		}

		private static readonly GUIContent _label = new();

		private static Renderer GetRendererValue(in string field, SP prop)
		{
			var rendererProp = prop.FindSibling(field);
			if (rendererProp == null || !rendererProp.IsObjectRef())
			{
				return null;
			}
			return rendererProp.objectReferenceValue as Renderer;
		}

		private static void DrawPopup(Rect r, in DrawerContext ctx)
		{
			var prop = ctx.property;
			var renderer = ctx.renderer;

			// renderer-related errors
			if(Validate(renderer, out string err))
			{
				DrawerGUI.MutedInfo(r, err); return;
			}

			var materials = renderer ? renderer.sharedMaterials : Array.Empty<Material>();

			string l = prop.intValue > -1 && prop.intValue < materials.Length
			? GetLabel(prop.intValue, materials[prop.intValue]?.name)
			: PluginConstants.Label.POPUP_UNSET;

			_label.text = l;
			if (EditorGUI.DropdownButton(r, _label, FocusType.Keyboard))
			{
				GetMenu(ctx)
				.DropDown(r);
			}
		}

		private static bool Validate(Renderer r, out string msg)
		{
			msg = null;
			if (!r)
			{
				msg = NO_RENDERER_MSG;
				return true;
			}

			if (r.sharedMaterials.Length == 0)
			{
				msg = NO_MATERIALS_MSG;
				return true;
			}
			return false;
		}

		private static string GetLabel(in int i, in string txt) => $"{i}: {txt ?? NULL_LABEL}";

		private static GenericMenu GetMenu(in DrawerContext ctx)
		{
			var prop = ctx.property;

			Action<int> setFn = v =>
			{
				prop.intValue = v;
				prop.serializedObject.ApplyModifiedProperties();
			};

			var menu = new GenericMenu
			{
				allowDuplicateNames = true
			};
			menu.AddItem(new GUIContent(PluginConstants.Label.POPUP_UNSET), prop.intValue == -1, () => setFn.Invoke(-1));
			menu.AddSeparator(string.Empty);

			var mats = ListMaterials(ctx);

			if (mats.Length == 0)
			{
				menu.AddDisabledItem(new GUIContent(NO_MATERIALS_MSG));
			}

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
			Material[] materials = renderer?.sharedMaterials ?? Array.Empty<Material>();
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

#endif