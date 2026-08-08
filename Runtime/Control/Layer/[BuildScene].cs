// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Dropdown of scenes in build settings
	///	valid on: string,int
	/// </summary>
	public sealed class BuildSceneAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	[CustomPropertyDrawer(typeof(BuildSceneAttribute))]
	internal sealed class _BuildSceneAttribute : __ControlDrawer<BuildSceneAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String | EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			BuildScenePopup(ctx.position, ctx.property);
		}
		
		public static void BuildScenePopup(in Rect pos, SerializedProperty prop)
		{
			var n = SceneManager.sceneCountInBuildSettings;

			(int, string, string) currentScene = (-1,null,null);

			var label = "<invalid scene>";

			int targetIndex = -1;


			if (prop.IsInt() && prop.intValue > -1 && prop.intValue < n)
			{
				targetIndex = prop.intValue;
			}
			
			if(prop.IsString() && !string.IsNullOrEmpty(prop.stringValue))
			{
				targetIndex = SceneUtility.GetBuildIndexByScenePath(prop.stringValue);
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
		
		private static (int,string,string) GetScene(int i)
		{
			string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
			string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
			return (i, scenePath, sceneName);
		}
	}
}

#endif