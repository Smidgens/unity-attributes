// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	public sealed class BlendShapeAttribute : __BaseControl
	{
		/// <summary>
		/// Name of skinned mesh renderer field
		/// </summary>
		public string RendererField { get; }

		/// <summary>
		/// Init with field of renderer
		/// </summary>
		public BlendShapeAttribute(string field) => RendererField = field;
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
			BlendShapePopup(ctx.position, ctx.property, _Attribute.RendererField);
		}
		
		private static void BlendShapePopup(in Rect pos, SerializedProperty prop, in string rendererField)
		{
			var rendererProp = prop.serializedObject.FindProperty(rendererField);
			if (rendererProp == null)
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_INVALID);
				return;
			}

			if(!rendererProp.IsRefType<SkinnedMeshRenderer>())
			{
				DrawerGUI.MutedInfo(pos, "field ref. ≠ SkinnedMeshRenderer");
				return;
			}

			if (!rendererProp.objectReferenceValue)
			{
				DrawerGUI.MutedInfo(pos, "renderer not set");
				return;
			}

			var mr = rendererProp.objectReferenceValue as SkinnedMeshRenderer;

			if (!mr?.sharedMesh)
			{
				DrawerGUI.MutedInfo(pos, "mesh not set");
				return;
			}

			var shapeCount = mr.sharedMesh.blendShapeCount;

			if (shapeCount == 0)
			{
				DrawerGUI.MutedInfo(pos, "no shape keys in mesh");
				return;
			}

			(int, string) shape = (-1,"");

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

			var label = EConstants.Label.POPUP_DEFAULT;

			if(shape.Item2.Length > 0)
			{
				label = $"{shape.Item1}: {shape.Item2}";
			}

			if (GUI.Button(pos, label, EditorStyles.popup))
			{
				var m = new GenericMenu();

				var isUnset = shape.Item1 < -1;

				m.AddItem(new GUIContent(EConstants.Label.POPUP_DEFAULT), isUnset, () =>
				{
					if (prop.IsInt()) { prop.intValue = -1; }
					else if (prop.IsString()) { prop.stringValue = ""; }
					prop.serializedObject.ApplyModifiedProperties();
				});

				m.AddSeparator("");

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