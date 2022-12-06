// smidgens @ github

#if UNITY_EDITOR

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
		public static void AssemblyType(in Rect pos, SP prop, Func<TypeSearch.Constraints> optsFn)
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
				var opts = optsFn.Invoke();
				TypeSearch.Open(brect, t, opts, v =>
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

			using (new EditorGUI.DisabledGroupScope(t == null))
			{
				if (GUI.Button(clearRect, "", EditorStyles.miniButton))
				{
					prop.stringValue = "";
					prop.serializedObject.ApplyModifiedProperties();
				}
				
			}
			EditorGUI.LabelField(clearRect, "x", EditorStyles.centeredGreyMiniLabel);
		}

		public static void SLayer(in Rect pos, SP prop)
		{
			if (!prop.IsInt())
			{
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_INT);
				return;
			}

			var label = prop.intValue >= 0
			? $"{prop.intValue}: {USortingLayer.IDToName(prop.intValue)}"
			: EConstants.Label.POPUP_DEFAULT;

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

		private static (int,string,string) GetScene(int i)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			return (i, scenePath, sceneName);
		}


		public static void Scene(in Rect pos, SP prop)
		{
			var n = SceneManager.sceneCountInBuildSettings;
			//Scene scene = default;




			(int, string, string) currentScene = (-1,null,null);

			var label = "<invalid scene>";

			int targetIndex = -1;


			if (prop.IsInt() && prop.intValue > -1 && prop.intValue < n)
			{
				//currentScene = GetScene(prop.intValue);
				targetIndex = prop.intValue;
				//scene = SceneManager.GetSceneByBuildIndex(prop.intValue);
			}
			
			if(prop.IsString() && !string.IsNullOrEmpty(prop.stringValue))
			{
				targetIndex = SceneUtility.GetBuildIndexByScenePath(prop.stringValue);
				//scene = SceneManager.GetSceneByPath(prop.stringValue);
			}

			if(targetIndex > -1)
			{
				currentScene = GetScene(targetIndex);
			}


			var isUnset = currentScene.Item1 == -1;

			bool isValid = !isUnset && currentScene.Item2 != null;

			if (isUnset) { label = EConstants.Label.POPUP_DEFAULT; }
			else if (isValid) { label = $"{currentScene.Item1}: {currentScene.Item3}"; }

			if (DrawerGUI.PopupButton(pos, label))
			{
				var m = new GenericMenu();
				m.allowDuplicateNames = true;
				m.AddItem(new GUIContent(EConstants.Label.POPUP_DEFAULT), isUnset, () =>
				{
					if (prop.IsInt()) { prop.intValue = -1; }
					else if (prop.IsString()) { prop.stringValue = ""; }
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.AddSeparator("");

				if(n == 0) { m.AddDisabledItem(new GUIContent("No Options")); }

				for (var i = 0; i < n; i++)
				{
					int sceneIndex = i;
					string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
					string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

					var active = prop.IsInt() ? i == prop.intValue : scenePath == prop.stringValue;
					m.AddItem(new GUIContent(sceneName), active, () =>
					{
						if (prop.IsInt()) { prop.intValue = sceneIndex; }
						else if (prop.IsString()) { prop.stringValue = scenePath; }
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
				DrawerGUI.MutedInfo(pos, EConstants.Info.FIELD_NON_STRING);
				return;
			}
			var isEmpty = string.IsNullOrEmpty(prop.stringValue);

			var label = isEmpty
			? EConstants.Label.POPUP_DEFAULT
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

				optionFn.Invoke(EConstants.Label.POPUP_DEFAULT, "");

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

#endif