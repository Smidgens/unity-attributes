// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;
	using System;
	using System.Reflection;
	using UnityObject = UnityEngine.Object;
	using SP = UnityEditor.SerializedProperty;

	internal static class DrawerGUI
	{
		public const float PAD_FULL = 2f;
		public const float PAD_MINI = 5f;

		// icon atlas
		private readonly static Lazy<Texture> _SWITCH_ICON = new Lazy<Texture>(() =>
		{
			return Resources.Load<Texture>(EConstants.Resource.ICON_SWITCH);
		});

		public static bool PointerButton(in Rect pos)
		{
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			return GUI.Button(pos, "", GUIStyle.none);
		}

		public static void ColorPreview(in Rect pos, in Color c)
		{
			EditorGUI.DrawRect(pos, c);
		}

		public static bool PopupButton(in Rect pos, in string label)
		{
			return GUI.Button(pos, label, EditorStyles.popup);
		}

		public static bool Switch(in Rect pos, bool val, in string l0, in string l1)
		{
			var (rl,rr) = pos.GetColumns(pos.height * 2f, 1f, 2);
			var label = val ? l1 : l0;
			if (PointerButton(pos)) { val = !val; }
			SpriteGUI.AtlasRow(rl, _SWITCH_ICON.Value, 2, val.ToInt());
			var s = val ? EditorStyles.boldLabel : EditorStyles.label;
			EditorGUI.LabelField(rr, label, s);
			return val;
		}


#if ATTRIBUTES_ANIMATION_1
		public static void AnimatorParameter(in Rect pos, SP prop, in string animatorFieldPath)
		{
			var animatorProp = prop.serializedObject.FindProperty(animatorFieldPath);
			if (animatorProp == null)
			{
				MutedInfo(pos, Config.Info.FIELD_INVALID);
				return;
			}

			if (!animatorProp.IsRefType("Animator"))
			{
				MutedInfo(pos, Config.Info.FIELD_NON_ANIMATOR);
				return;
			}

			if (!animatorProp.objectReferenceValue)
			{
				MutedInfo(pos, "animator not set");
				return;
			}
			var animator = animatorProp.objectReferenceValue as Animator;


			(int, string, string) parameter = (-1,"","");

			if (prop.IsString())
			{
				parameter.Item2 = prop.stringValue;
			}

			if (prop.IsInt())
			{
				parameter.Item1 = prop.intValue;
			}

			if (parameter.Item1 > -1 || !string.IsNullOrEmpty(parameter.Item2))
			{
				for(var i = 0; i < animator.parameterCount; i++)
				{
					var ap = animator.GetParameter(i);
					if(ap.name == parameter.Item2 || i == parameter.Item1)
					{
						parameter = (i, ap.name, ap.type.ToString());
						break;
					}
				}
			}

			var isUnset = parameter.Item1 < 0 || parameter.Item2.Length == 0;

			var btnLabel = Config.Label.POPUP_DEFAULT;

			if (!isUnset)
			{
				btnLabel = $"{parameter.Item1}: {parameter.Item2} ({parameter.Item3})";
			}

			if(GUI.Button(pos, btnLabel, EditorStyles.popup))
			{

				var m = MenuFactory.AnimatorParameters
				(
					animator,
					parameter.Item2,
					v =>
					{
						if (prop.IsInt()) { prop.intValue = v; }
						else if (prop.IsString())
						{
							prop.stringValue = v > -1
							? animator.GetParameter(v).name
							: "";
							
						}

						prop.serializedObject.ApplyModifiedProperties();
					}
				);
				m.DropDown(pos);
			}
		}

#endif

		public static void BlendShape(in Rect pos, SP prop, in string rendererField)
		{
			var rendererProp = prop.serializedObject.FindProperty(rendererField);
			if (rendererProp == null)
			{
				MutedInfo(pos, EConstants.Info.FIELD_INVALID);
				return;
			}

			if(!rendererProp.IsRefType<SkinnedMeshRenderer>())
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_SKINNEDRENDERER);
				return;
			}

			if (!rendererProp.objectReferenceValue)
			{
				MutedInfo(pos, "renderer not set");
				return;
			}

			var mr = rendererProp.objectReferenceValue as SkinnedMeshRenderer;

			if (!mr.sharedMesh)
			{
				MutedInfo(pos, "mesh not set");
				return;
			}

			var shapeCount = mr.sharedMesh.blendShapeCount;

			if (shapeCount == 0)
			{
				MutedInfo(pos, "no shape keys in mesh");
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

		public static void Slider(
			in Rect pos,
			SP prop,
			in float min,
			in float max,
			in float step = -1f,
			in int precision = -1)
		{
			if (!prop.IsFloat() && !prop.IsInt())
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_NUM);
				return;
			}

			using (var check = new EditorGUI.ChangeCheckScope())
			{

				var val = prop.IsFloat() ? prop.floatValue : prop.intValue;

				float valueNew = EditorGUI.Slider(pos, val, min, max);
				if (check.changed)
				{
					if (precision >= 1) { valueNew = valueNew.Round(precision); }
					if(step > 0f)
					{
						valueNew = ((int)(valueNew / step)) * step;
					}
					valueNew = Mathf.Clamp(valueNew, min, max);

					if (prop.IsFloat())
					{
						prop.floatValue = valueNew;
					}
					else
					{
						prop.intValue = (int)valueNew;
					}


				}
			}
		}

		public static void PrefixLabel(ref Rect pos, GUIContent l, FieldInfo fo)
		{
			if(l.IsEmpty() || fo.IsArray()) { return; }
			pos = EditorGUI.PrefixLabel(pos, l);
		}

		public static bool BoolDropdown(in Rect pos, in bool value, in string[] labels)
		{
			return EditorGUI.Popup(pos, value.ToInt(), labels).ToBool();
		}

		public static void IntegerDropdown(in Rect pos, SP prop, in int[] options)
		{
			// valid type?
			if (!prop.IsInt())
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			if (GUI.Button(pos, prop.intValue.ToString(), EditorStyles.popup))
			{
				MenuFactory
				.GetMenu(prop, options)
				.DropDown(pos);
			}
		}

		public static void Layer(in Rect pos, SP prop)
		{
			// invalid type
			if (!prop.IsInt())
			{
				MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			string[] layers = InternalEditorUtility.layers;

			string btnLabel = prop.intValue >= 0 && prop.intValue < layers.Length
			? $"{prop.intValue}: {layers[prop.intValue]}"
			: EConstants.Label.POPUP_DEFAULT;

			if (GUI.Button(pos, btnLabel, EditorStyles.popup))
			{
				MenuFactory.Layers(prop.intValue, v =>
				{
					prop.intValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				})
				.DropDown(pos);
			}
		}

		public static void MutedInfo(in Rect pos, in string msg)
		{
			GUI.Box(pos, "");
			EditorGUI.LabelField(pos, msg, EditorStyles.centeredGreyMiniLabel);
		}

		public static void AssetThumbnail(in Rect pos, UnityObject o, in bool full = false)
		{
			GUI.Box(pos, "", GUI.skin.box);
			if (!o) { return; }

			if (full) { GUI.DrawTexture(pos.Resize(-PAD_FULL), AssetPreview.GetAssetPreview(o)); }
			else { GUI.DrawTexture(pos.Resize(-PAD_MINI), AssetPreview.GetMiniThumbnail(o)); }

			var ping = GUI.Button(pos, "", GUIStyle.none);
			if (ping) { EditorGUIUtility.PingObject(o); }
		}

		public static bool TabButton(in Rect pos, bool value, in string label)
		{
			// background
			EditorGUI.DrawRect(pos, _TOGGLE_COLORS[value.ToInt()]);
			if (GUI.Button(pos, "", _ToggleTabStyle.Value))
			{
				value = !value;
			}
			EditorGUIUtility.AddCursorRect(pos, MouseCursor.Link);
			EditorGUI.LabelField(pos, label, _ToggleTabStyle.Value);
			return value;
		}

		private static readonly Color[] _TOGGLE_COLORS =
		{
			Color.black * 0.1f, // false
			Color.white * 0.5f, // true
		};

		private static Lazy<GUIStyle> _ToggleTabStyle = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle();
			s.alignment = TextAnchor.MiddleCenter;
			s.normal.textColor = Color.white;
			s.fontStyle = FontStyle.Bold;
			s.fontSize = 10;
			return s;
		});
	}
}

#endif