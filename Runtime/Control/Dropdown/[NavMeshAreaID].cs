// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draws a dropdown menu of nav area types
	/// </summary>
	public sealed class NavMeshAreaIDAttribute : __BaseControl
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.AI;

	[CustomPropertyDrawer(typeof(NavMeshAreaIDAttribute))]
	internal sealed class _NavMeshAreaIDAttribute : __ControlDrawer<NavMeshAreaIDAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;

		protected override void OnField(in DrawContext ctx)
		{
			if (!HasAIModule())
			{
				DrawerGUI.MutedInfo(ctx.position, "Missing AI Module");
				return;
			}
			DrawPopup(ctx.position, ctx.property);
		}
		
		protected override DisplayIcon GetFieldDisplayIcon()
		{
			return new DisplayIcon
			{
				texture = EditorGUIUtility.IconContent("NavMeshData Icon")?.image,
			};
		}

		private static (GUIContent, int)[] _cachedMenuOptions;
		private static (GUIContent, int)[] _cachedAreaOptions;
		private static int _cachedSettingsCount;
		private static int _cachedAreasCount;

		private static void DrawPopup(Rect pos, SerializedProperty prop)
		{
		
			if (_cachedMenuOptions == null)
			{
				_cachedMenuOptions = GetAreaOptions();
			}

			var options = _cachedMenuOptions;
			var label = GUIContent.none;

			var currentIndex = FindIndex(options, o => o.Item2 == prop.intValue);

			if (currentIndex >= 0)
			{
				label = options[currentIndex].Item1;
			}

			if(EditorGUI.DropdownButton(pos, label, FocusType.Keyboard))
			{
				var m = new GenericMenu();

				foreach (var (agentName, agentID) in options)
				{
					var v = agentID;
					m.AddItem(agentName, agentID == prop.intValue, () =>
					{
						prop.intValue = v;
						prop.serializedObject.ApplyModifiedProperties();
					});
				}

				m.AddSeparator(string.Empty);
				m.AddItem(new GUIContent("Open Area Settings..."), false, () =>
				{
					OpenAreaSettings();
					_cachedMenuOptions = null;
				});
				
				m.DropDown(pos);
			}
		}

		private static bool HasAIModule()
		{
#if SM_ATTR_AI
			return true;
#else
			return false;
#endif
		}

		private static void OpenAreaSettings()
		{
#if SM_ATTR_AI
			UnityEditor.AI.NavMeshEditorHelpers.OpenAreaSettings();
#endif
		}

		private static Func<string, int> _areaFromNameFn;
		private static Func<string[]> _areaNamesFn;

		private static (GUIContent, int)[] GetAreaOptions()
		{
#if SM_ATTR_AI

			var names = NavMesh.GetAreaNames();
			(GUIContent, int)[] options = new (GUIContent, int)[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				var area = NavMesh.GetAreaFromName(names[i]);
				options[i] = (new GUIContent(names[i]), area);
			}
			return options;
#else
			return Array.Empty<(GUIContent, int)>();
#endif
		}

		private static int FindIndex<T>(T[] arr, Func<T, bool> fn)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (fn.Invoke(arr[i]))
				{
					return i;
				}
			}
			return -1;
		}

	}
}


#endif