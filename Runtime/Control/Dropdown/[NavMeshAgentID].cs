// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draws a dropdown menu of nav agent types
	/// </summary>
	public sealed class NavMeshAgentIDAttribute : __BaseControl
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.AI;

	[CustomPropertyDrawer(typeof(NavMeshAgentIDAttribute))]
	internal sealed class _NavAgentIDAttribute :  __ControlDrawer<NavMeshAgentIDAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Int;
		
		protected override void OnField(in DrawContext ctx)
		{
			if (!HasAIModule())
			{
				DrawerGUI.MutedInfo(ctx.position, "Missing AI Module");
				return;
			}
			DrawAgentPopup(ctx.position, ctx.property);
		}
		
		protected override DisplayIcon GetFieldDisplayIcon()
		{
			return new DisplayIcon
			{
				texture = EditorGUIUtility.IconContent("NavMeshAgent Icon")?.image,
			};
		}

		private static (GUIContent, int)[] _cachedAgentOptions;
		private static int _cachedSettingsCount;

		private static void DrawAgentPopup(Rect pos, SerializedProperty prop)
		{
			var currentSettingsCount = GetNavMeshSettingsCount();

			if (currentSettingsCount != _cachedSettingsCount)
			{
				_cachedSettingsCount = currentSettingsCount;
				_cachedAgentOptions = null;
			}

			if (_cachedAgentOptions == null)
			{
				_cachedAgentOptions = GetAgentOptions();
			}

			var options = _cachedAgentOptions;
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
				m.AddItem(new GUIContent("Open Agent Settings..."), false, () =>
				{
					OpenAgentSettings();
					_cachedAgentOptions = null;
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

		private static void OpenAgentSettings()
		{
#if SM_ATTR_AI
			UnityEditor.AI.NavMeshEditorHelpers.OpenAgentSettings(-1);
#endif
		}

		private static (GUIContent, int)[] GetAgentOptions()
		{
#if SM_ATTR_AI
			var count = GetNavMeshSettingsCount();
			(GUIContent, int)[] options = new (GUIContent, int)[count];
			for (int i = 0; i < count; i++)
			{
				var settings = NavMesh.GetSettingsByIndex(i);
				var name = NavMesh.GetSettingsNameFromID(settings.agentTypeID);
				options[i] = (new GUIContent(name), settings.agentTypeID);
			}
			return options;
#else
			return Array.Empty<(GUIContent, int)>();
#endif
		}

		private static int GetNavMeshSettingsCount()
		{
#if SM_ATTR_AI
			return NavMesh.GetSettingsCount();
#else
			return 0;
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