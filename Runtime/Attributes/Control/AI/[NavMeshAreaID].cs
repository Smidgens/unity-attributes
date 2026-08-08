// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Draws a popup menu of nav agent types
	/// </summary>
	public sealed class NavMeshAreaIDAttribute : __BaseControl
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

	[CustomPropertyDrawer(typeof(NavMeshAreaIDAttribute))]
	internal sealed class _NavMeshAreaIDAttribute : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent l)
		{
			EditorGUI.BeginProperty(pos, l, prop);
			
			if (l != GUIContent.none)
			{
				pos = EditorGUI.PrefixLabel(pos, l);
			}

			if (prop.propertyType != SerializedPropertyType.Integer)
			{
				DrawMessage(pos, "Field type must be int", MessageType.Error);
				return;
			}

			if (!HasAIModule())
			{
				DrawMessage(pos, "Missing AI module", MessageType.Error);
				EditorGUI.EndProperty();
				return;
			}
			
			DrawPopup(pos, prop);

			EditorGUI.EndProperty();
		}

		private static (Type, string, bool) _navmeshType = (null, "UnityEngine.AI.NavMesh, UnityEngine.AIModule", false);
		private static (Type, string, bool) _navmeshHelperType = (null, "UnityEditor.AI.NavMeshEditorHelpers, UnityEditor.CoreModule", false);

		private static (GUIContent, int)[] _cachedMenuOptions;
		private static (GUIContent, int)[] _cachedAreaOptions;
		private static int _cachedSettingsCount;
		private static int _cachedAreasCount;

		private const BindingFlags _BFLAGS_STATIC_FN =
		BindingFlags.Static
		| BindingFlags.Public;

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

				m.AddSeparator("");
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
			return GetType(ref _navmeshType) != null;
		}

		private static void OpenAreaSettings()
		{
			var type = GetType(ref _navmeshHelperType);
			var method = type.GetMethod("OpenAreaSettings", _BFLAGS_STATIC_FN);
			method?.Invoke(null, null);
		}

		private static Func<string, int> _areaFromNameFn;
		private static Func<string[]> _areaNamesFn;

		private static (GUIContent, int)[] GetAreaOptions()
		{
			var nmType = GetType(ref _navmeshType);

			if (_areaFromNameFn == null)
			{
				var namesMethod = nmType.GetMethod("GetAreaNames", _BFLAGS_STATIC_FN);
				var areaFromNameMethod =  nmType.GetMethod("GetAreaFromName", _BFLAGS_STATIC_FN);
				_areaFromNameFn = (Func<string, int>)(areaFromNameMethod!).CreateDelegate(typeof(Func<string, int>));
				_areaNamesFn = (Func<string[]>)(namesMethod!).CreateDelegate(typeof(Func<string[]>));
			}

			var names = _areaNamesFn.Invoke();

			(GUIContent, int)[] options = new (GUIContent, int)[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				var area = _areaFromNameFn.Invoke(names[i]);
				options[i] = (new GUIContent(names[i]), area);
			}
			return options;
		}

		private static Type GetType(ref (Type, string, bool) t)
		{
			if (!t.Item3)
			{
				t = (Type.GetType(t.Item2), t.Item2, true);
			}
			return t.Item1;
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

		private static void DrawMessage(Rect pos, string msg, MessageType type)
		{
			var color = type switch
			{
				MessageType.Error => Color.red * 0.25f,
				MessageType.Warning => Color.yellow * 0.25f,
				MessageType.Info => Color.cyan * 0.25f,
				_ => Color.clear
			};
			EditorGUI.DrawRect(pos, color);
			EditorGUI.LabelField(pos, msg, EditorStyles.centeredGreyMiniLabel);
		}


	}
}


#endif