// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of scenes in build settings
	///	valid on: string,int
	/// </summary>
	public sealed class ProjectSceneAttribute : __BaseControl
	{
		public ProjectSceneAttribute(bool buildOnly = false)
		{
			this.buildOnly = buildOnly;
		}

		internal bool buildOnly { get; }

	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	[CustomPropertyDrawer(typeof(ProjectSceneAttribute))]
	internal sealed class _ProjectSceneAttribute : __ControlDrawer<ProjectSceneAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String | EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			if (_Attribute.buildOnly && ctx.property.propertyType == SerializedPropertyType.Integer)
			{
				DrawerGUI.MutedInfo(ctx.position, "Invalid");
				return;
			}

			var pos = ctx.position;
			var previewRect = pos.SliceLeft(pos.height);
			BuildScenePopup(pos, ctx.property);
			ScenePreview(previewRect);
		}

		private static void ScenePreview(in Rect pos)
		{
			var img = EditorGUIUtility.IconContent("d_SceneAsset Icon")?.image;
			DrawerGUI.DrawTex(img as Texture2D, pos.Resized(-pos.height * 0.1f));
		}
		
		private void BuildScenePopup(in Rect pos, SerializedProperty prop)
		{
			var label = EConstants.Label.POPUP_DEFAULT;

			var currentValue = GetSceneValue(prop);

			var valid = !(!string.IsNullOrEmpty(currentValue.Item2) && !AssetDatabase.AssetPathExists(currentValue.Item2));

			if (!valid)
			{
				label = "<invalid scene>";
			}
			else if(!string.IsNullOrEmpty(currentValue.Item2))
			{
				label = currentValue.Item3;
				if (currentValue.Item1 >= 0)
				{
					label = "[build] " + label;
				}
				
			}

			var isUnset = string.IsNullOrEmpty(currentValue.Item2);

			if (DrawerGUI.PopupButton(pos, label))
			{
				var options = GetProjectSceneList(_Attribute.buildOnly);
				
				var m = new GenericMenu
				{
					allowDuplicateNames = true
				};
				m.AddItem(new GUIContent(EConstants.Label.POPUP_DEFAULT), isUnset, () =>
				{
					if (prop.IsInt())
					{
						prop.intValue = -1;
					}
					else if (prop.IsString())
					{
						prop.stringValue = string.Empty;
					}
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.AddSeparator(string.Empty);

				if (options.Count == 0)
				{
					m.AddDisabledItem(new GUIContent("No Options"));
				}

				foreach (var (sceneIndex, scenePath, sceneName) in options)
				{
					var active = prop.IsInt() ? sceneIndex == prop.intValue : scenePath == prop.stringValue;

					var cLabel = sceneName;
					
					if (sceneIndex >= 0)
					{
						cLabel = "[build] " + cLabel;
					}
					
					m.AddItem(new GUIContent(cLabel), active, () =>
					{
						if (prop.IsInt())
						{
							prop.intValue = sceneIndex;
						}
						else if (prop.IsString())
						{
							prop.stringValue = scenePath;
						}
						prop.serializedObject.ApplyModifiedProperties();
					});
				}

				m.DropDown(pos);
			}
		}

		private static IReadOnlyList<(int, string, string)> GetProjectSceneList(bool buildOnly)
		{
			List<(int, string, string)> scenes = new();

			if (buildOnly)
			{
				var n = SceneManager.sceneCountInBuildSettings;
				
				for (var i = 0; i < n; i++)
				{
					string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
					string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
					scenes.Add((i, scenePath, sceneName));
				}
				return scenes;
			}

			foreach (var guid in AssetDatabase.FindAssets("t:SceneAsset"))
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
				var bIndex = SceneUtility.GetBuildIndexByScenePath(path);
				scenes.Add((bIndex, path, sceneName));
			}

			return scenes;
		}

		private static (int, string, string) GetSceneValue(SerializedProperty prop)
		{
			var scenePath = "";
			var sceneIndex = -1;

			if (prop.IsInt())
			{
				scenePath = SceneUtility.GetScenePathByBuildIndex(prop.intValue);
				sceneIndex = prop.intValue;
			}
			else if (prop.IsString())
			{
				scenePath = prop.stringValue;
				sceneIndex = SceneUtility.GetBuildIndexByScenePath(prop.stringValue);
			}

			if (string.IsNullOrEmpty(scenePath))
			{
				return (-1, string.Empty, string.Empty);
			}
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			
			return (sceneIndex, scenePath, sceneName);
		}
		
		private static (int,string,string) GetBuildScene(int i)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			return (i, scenePath, sceneName);
		}
	}
}

#endif