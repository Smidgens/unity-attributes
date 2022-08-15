// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.SceneManagement;
	using USortingLayer = UnityEngine.SortingLayer;
	using SP = UnityEditor.SerializedProperty;
	using System;
	using UnityEditorInternal;

	internal static class Popup
	{
		public static void AssemblyType(in Rect pos, SP prop)
		{
			var label = "...";

			Type t = null;

			var missing = false;

			if (!string.IsNullOrEmpty(prop.stringValue))
			{
				t = Type.GetType(prop.stringValue, false);
				label = t != null
				? t.FullName
				: "<type missing>";

				missing = t == null;
			}

			var brect = pos;
			brect.width -= pos.height + 2f;

			var clearRect = pos;
			clearRect.width = pos.height;
			clearRect.position += new Vector2(brect.width + 2f, 0f);

			GUI.Box(brect, "", EditorStyles.helpBox);

			EditorGUIUtility.AddCursorRect(brect, MouseCursor.Link);

			var c = GUI.color;
			GUI.color *= 0.5f;
			if (GUI.Button(brect, t == null ? label : ""))
			{
				TypePopup.Open(brect, t, v =>
				{
					prop.stringValue = v?.AssemblyQualifiedName ?? "";
					prop.serializedObject.ApplyModifiedProperties();
				});
			}
			GUI.color = c;

			if(t != null)
			{
				var lpos = brect;
				lpos.width -= 10f;
				lpos.position += new Vector2(5f, 0f);
				EditorGUI.LabelField(lpos, label, EditorStyles.miniLabel);
			}

			using (new EditorGUI.DisabledGroupScope(!missing))
			{
				if (GUI.Button(clearRect, "x", EditorStyles.miniButton))
				{
					prop.stringValue = "";
					prop.serializedObject.ApplyModifiedProperties();
				}
			}
		}

		public static void SLayer(in Rect pos, SP prop)
		{
			if (!prop.IsInt())
			{
				DrawerGUI.MutedInfo(pos, Config.Info.FIELD_NON_INT);
				return;
			}

			var label = prop.intValue >= 0
			? $"{prop.intValue}: {USortingLayer.IDToName(prop.intValue)}"
			: Config.Label.POPUP_DEFAULT;

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = MenuFactory.SortingLayers(prop.intValue, id =>
				{
					prop.intValue = id;
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.DropDown(pos);
			}
		}

		public static void Scene(in Rect pos, SP prop)
		{
			var n = SceneManager.sceneCountInBuildSettings;
			Scene scene = default;
			var label = "<invalid scene>";
			if (prop.IsInt() && prop.intValue > -1 && prop.intValue < n)
			{
				scene = SceneManager.GetSceneByBuildIndex(prop.intValue);
			}
			
			if(prop.IsString() && !string.IsNullOrEmpty(prop.stringValue))
			{
				scene = SceneManager.GetSceneByPath(prop.stringValue);
			}

			var isUnset = scene.buildIndex == -1;

			if (isUnset) { label = Config.Label.POPUP_DEFAULT; }
			else if (scene.IsValid()) { label = $"{scene.buildIndex}: {scene.name}"; }

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();
				m.allowDuplicateNames = true;
				m.AddItem(new GUIContent(Config.Label.POPUP_DEFAULT), isUnset, () =>
				{
					if (prop.IsInt()) { prop.intValue = -1; }
					else if (prop.IsString()) { prop.stringValue = ""; }
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.AddSeparator("");
				if(n == 0) { m.AddDisabledItem(new GUIContent("No Options")); }
				for (var i = 0; i < n; i++)
				{
					var cs = SceneManager.GetSceneByBuildIndex(i);
					var active = prop.IsInt() ? i == prop.intValue : cs.path == prop.stringValue;
					m.AddItem(new GUIContent(cs.name), active, () =>
					{
						if (prop.IsInt()) { prop.intValue = cs.buildIndex; }
						else if (prop.IsString()) { prop.stringValue = cs.path; }
						prop.serializedObject.ApplyModifiedProperties();
					});
				}
				m.DropDown(pos);
			}
		}

		public static void Tag(in Rect pos, SP prop)
		{
			if (!prop.IsString())
			{
				DrawerGUI.MutedInfo(pos, Config.Info.FIELD_NON_STRING);
				return;
			}
			var isEmpty = string.IsNullOrEmpty(prop.stringValue);

			var label = isEmpty
			? Config.Label.POPUP_DEFAULT
			: prop.stringValue;

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();

				Action<string> setFn = v =>
				{
					prop.stringValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				};

				// add option
				Action<string, string> optionFn = (l, v) =>
				{
					m.AddItem(new GUIContent(l), prop.stringValue == v, () => setFn.Invoke(v));
				};

				optionFn.Invoke(Config.Label.POPUP_DEFAULT, "");

				m.AddSeparator("");

				foreach (var tag in InternalEditorUtility.tags)
				{
					optionFn.Invoke(tag, tag);
				}
				m.DropDown(pos);
			}
		}
	}
}