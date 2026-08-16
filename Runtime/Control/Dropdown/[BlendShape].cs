// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Shows dropdown of blend shapes in skinned mesh renderer
	/// </summary>
	public sealed class BlendShapeAttribute : __BaseControl
	{
		public BlendShapeAttribute(string field)
		{
			this.field = field;
		}
		internal string field { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(BlendShapeAttribute))]
	internal sealed class _BlendShapeAttribute : __ControlDrawer<BlendShapeAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String | EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			BlendShapePopup(ctx.position, ctx.property, _Attribute.field);
		}
		
		private static void BlendShapePopup(in Rect pos, SerializedProperty prop, in string rendererField)
		{
			var rendererProp = prop.FindSibling(rendererField);
			
			if (rendererProp == null)
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_INVALID);
				return;
			}

			if(!rendererProp.IsRefType<SkinnedMeshRenderer>())
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_INVALID);
				return;
			}

			if (!rendererProp.objectReferenceValue)
			{
				DrawerGUI.MutedInfo(pos, "No renderer");
				return;
			}

			var mr = rendererProp.objectReferenceValue as SkinnedMeshRenderer;

			if (!mr?.sharedMesh)
			{
				DrawerGUI.MutedInfo(pos, "No mesh");
				return;
			}

			var shapeCount = mr.sharedMesh.blendShapeCount;

			if (shapeCount == 0)
			{
				DrawerGUI.MutedInfo(pos, "no blend shapes");
				return;
			}

			(int, string) shape = (-1,string.Empty);

			if (prop.IsInt() && prop.intValue > -1 && prop.intValue < shapeCount)
			{
				shape.Item1 = prop.intValue;
				shape.Item2 = mr.sharedMesh.GetBlendShapeName(prop.intValue);
			}

			if (prop.IsString() && !string.IsNullOrEmpty(prop.stringValue))
			{
				shape.Item1 = mr.sharedMesh.GetBlendShapeIndex(prop.stringValue);
				shape.Item2 = prop.stringValue;
			}

			var label = PluginConstants.Label.POPUP_UNSET;

			if(shape.Item2.Length > 0)
			{
				label = $"{shape.Item1}: {shape.Item2}";
			}

			if (EditorGUI.DropdownButton(pos, new GUIContent(label), FocusType.Keyboard))
			{
				var m = new GenericMenu();

				var isUnset = shape.Item1 < -1;

				m.AddItem(new GUIContent(PluginConstants.Label.POPUP_UNSET), isUnset, () =>
				{
					if (prop.IsInt()) { prop.intValue = -1; }
					else if (prop.IsString()) { prop.stringValue = string.Empty; }
					prop.serializedObject.ApplyModifiedProperties();
				});

				m.AddSeparator(string.Empty);

				for(var i = 0; i < shapeCount; i++)
				{
					var sindex = i;
					var sname = mr.sharedMesh.GetBlendShapeName(i);
					var l = $"[{sindex}] {sname}";

					m.AddItem(new GUIContent(l), shape.Item1 == i, () =>
					{
						if (prop.IsInt()) { prop.intValue = sindex; }
						else if (prop.IsString()) { prop.stringValue = sname; }
						prop.serializedObject.ApplyModifiedProperties();
					});
				}

				m.DropDown(pos);

			}
		}
	}
}

#endif