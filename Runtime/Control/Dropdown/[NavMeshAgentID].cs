// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draws a popup menu of nav agent types
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

		private static (Type, string, bool) _navmeshType = (null, "UnityEngine.AI.NavMesh, UnityEngine.AIModule", false);
		private static (Type, string, bool) _navmeshHelperType = (null, "UnityEditor.AI.NavMeshEditorHelpers, UnityEditor.CoreModule", false);
		private static (Delegate, Type, string, bool) _settingsCountFn = (null, typeof(Func<int>), "GetSettingsCount", false);

		private static (GUIContent, int)[] _cachedAgentOptions;
		private static int _cachedSettingsCount;

		private const BindingFlags _BFLAGS_STATIC_FN =
		BindingFlags.Static
		| BindingFlags.Public;

		private const BindingFlags _BFLAGS_INSTANCE_PROP =
		BindingFlags.Instance
		| BindingFlags.GetProperty
		| BindingFlags.Public;

		private static Texture _agentIcon;
		
		private static void DrawAgentPopup(Rect pos, SerializedProperty prop)
		{
			if (!_agentIcon)
			{
				_agentIcon = EditorGUIUtility.IconContent("NavMeshAgent Icon")?.image;
			}

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

			var prefixRect = pos.SliceLeft(pos.height).Resized(-pos.height * 0.1f);
			pos.SliceLeft(EditorGUIUtility.standardVerticalSpacing);
			DrawerGUI.DrawTex(prefixRect, _agentIcon);

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

				m.AddSeparator("");
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
			return GetType(ref _navmeshType) != null;
		}

		private static void OpenAgentSettings()
		{
			var type = GetType(ref _navmeshHelperType);
			var method = type.GetMethod("OpenAgentSettings", _BFLAGS_STATIC_FN);
			method?.Invoke(null, new object[] { -1 });
		}

		private static (GUIContent, int)[] GetAgentOptions()
		{
			var count = GetNavMeshSettingsCount();
			(GUIContent, int)[] options = new (GUIContent, int)[count];
			for (int i = 0; i < count; i++)
			{
				var (agentName, agentID) = GetAgentTypeAtSettingsIndex(i);
				options[i] = (new GUIContent(agentName), agentID);
			}
			return options;
		}

		private static (string, int) GetAgentTypeAtSettingsIndex(int i)
		{
			var nmType = GetType(ref _navmeshType);

			var settingsFn = (nmType!).GetMethod("GetSettingsByIndex", _BFLAGS_STATIC_FN);
			var settings = (settingsFn!).Invoke(null, new object[]{ i });

			if (settings == null)
			{
				return ("", -1);
			}
			var nameFn = (nmType!).GetMethod("GetSettingsNameFromID", _BFLAGS_STATIC_FN);
			var prop = settings.GetType().GetProperty("agentTypeID", _BFLAGS_INSTANCE_PROP);
			var agentTypeID = (prop!).GetValue(settings);
			var name = (nameFn!).Invoke(null, new object[] { agentTypeID });
			return (name as string, (int)agentTypeID);

		}
		
		private static int GetNavMeshSettingsCount()
		{
			var fn = GetDelegate(GetType(ref _navmeshType), ref _settingsCountFn, _BFLAGS_STATIC_FN) as Func<int>;
			return (fn!).Invoke();
		}
		
		private static Type GetType(ref (Type, string, bool) t)
		{
			if (!t.Item3)
			{
				t = (Type.GetType(t.Item2), t.Item2, true);
			}
			return t.Item1;
		}
		
		private static Delegate GetDelegate(Type type, ref (Delegate, Type, string, bool) d, BindingFlags flags)
		{
			var fnInfo = (type!).GetMethod(d.Item3, flags);
			return fnInfo?.CreateDelegate(d.Item2);
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