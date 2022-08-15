// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;
	using UnityObject = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(AssetOptionsAttribute))]
	internal class DropdownAsset_ : AttributeDrawer<AssetOptionsAttribute>
	{
		protected override PType GetSupportedTypes() => PType.Object;

		protected override bool HasIcon()
		{
			return Mathf.Clamp(_Attribute.ThumbQuality, 0, 2) > 0;
		}

		protected override void DrawIcon(in Rect pos, in FieldContext ctx)
		{
			DrawerGUI.AssetThumbnail(pos, ctx.property.objectReferenceValue, _Attribute.ThumbQuality == 2);
		}

		protected override void DrawField(in FieldContext ctx)
		{
			var label = ctx.property.objectReferenceValue?.name ?? Config.Label.POPUP_DEFAULT;

			if (GUI.Button(ctx.position, label, EditorStyles.popup))
			{
				GetMenu(ctx.property, Config.Label.POPUP_DEFAULT)
				.DropDown(ctx.position);
			}
		}

		private GenericMenu GetMenu(SP prop, string empty)
		{
			var m = new GenericMenu();
			m.allowDuplicateNames = true;

			var typeLabel = fieldInfo.GetInnerType().Name;

			m.AddDisabledItem(new GUIContent(typeLabel));

			m.AddItem(new GUIContent(empty), !prop.objectReferenceValue, () =>
			{
				prop.objectReferenceValue = null;
				prop.serializedObject.ApplyModifiedProperties();
			});

			m.AddSeparator("");

			string currentGUID = prop.objectReferenceValue.GetAssetGUID();
			var assets = ListAssets();
			foreach (var (name,path,guid) in assets)
			{
				if(guid == null) { continue; }
				var active = currentGUID == guid;
				m.AddItem(new GUIContent(name), active, () =>
				{
					prop.objectReferenceValue = Load(path, typeof(UnityObject));
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			if(assets.Length == 0)
			{
				m.AddDisabledItem(new GUIContent(Config.Info.NO_POPUP_OPTIONS));
			}
			return m;
		}

		private static UnityObject Load(in string p, Type t) => AssetDatabase.LoadAssetAtPath(p, t);

		private (string, string, string)[] ListAssets()
		{
			var tname = !fieldInfo.FieldType.IsArray
			? fieldInfo.FieldType.Name
			: fieldInfo.FieldType.Name.Slice(-2);
			var folders = _Attribute.Folders;
			var guids = AssetDatabase.FindAssets($"t:{tname}", folders);
			var items = new (string, string, string)[guids.Length];
			for(var i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				var name = path.TrimExtension();
				items[i] = (name, path, guids[i]);
			}
			return items;
		}
	}
}