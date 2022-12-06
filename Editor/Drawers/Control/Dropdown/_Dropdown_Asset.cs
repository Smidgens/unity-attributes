// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using SP = UnityEditor.SerializedProperty;
	using UnityObject = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(DropdownAssetAttribute))]
	internal class _Dropdown_Asset : __ControlDrawer<DropdownAssetAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.Object;

		protected override bool HasIcon()
		{
			return Mathf.Clamp(_Attribute.thumbQuality, 0, 2) > 0;
		}

		protected override void OnIcon(in Rect pos, in DrawContext ctx)
		{
			DrawerGUI.AssetThumbnail(pos, ctx.property.objectReferenceValue, _Attribute.thumbQuality == 2);
		}

		protected override void OnField(in DrawContext ctx)
		{
			var label = ctx.property.objectReferenceValue?.name ?? EConstants.Label.POPUP_DEFAULT;

			if (GUI.Button(ctx.position, label, EditorStyles.popup))
			{
				GetMenu(ctx.property, EConstants.Label.POPUP_DEFAULT)
				.DropDown(ctx.position);
			}
		}

		private GenericMenu GetMenu(SP prop, string empty)
		{
			var m = new GenericMenu();
			m.allowDuplicateNames = true;

			var typeLabel = fieldInfo.GetItemType().Name;

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
				m.AddDisabledItem(new GUIContent(EConstants.Info.NO_POPUP_OPTIONS));
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
				var name = path.ToFileName();
				items[i] = (name, path, guids[i]);
			}
			return items;
		}
	}
}

#endif