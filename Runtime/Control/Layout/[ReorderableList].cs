// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;
	using UnityEngine;

	[Flags]
	public enum EReorderableList
	{
		All = ~0,
		Add = 1,
		Remove = 2,
		Drag = 4,
		Header = 8,
		Index = 16,
		Resizable = 32,
		Collapsible = 64,
		Minimal = All & ~Resizable & ~Index & ~Collapsible,
	}

	/// <summary>
	/// Draws array as list
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ReorderableListAttribute : PropertyAttribute
	{
		public ReorderableListAttribute(EReorderableList flags = EReorderableList.Minimal) : base(true)
		{
			this.flags = flags;
		}

		internal EReorderableList flags { get; }
		
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using Object = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
	internal sealed class _ReorderableListAttribute : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var a = (attribute as ReorderableListAttribute)!;

			if (a.flags.HasFlag(EReorderableList.Collapsible))
			{
				var labelHeight = _foldoutLabel.Value.CalcHeight(GUIContent.none, 100);
				var lHeight = property.isExpanded
				? GetList(property).GetHeight() + _FOLDOUT_SPACING
				: 0f;
				var padHeight = _FOLDOUT_PAD * 2f;
				return labelHeight + lHeight + padHeight;

			}
			return GetList(property).GetHeight();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
			var a = (attribute as ReorderableListAttribute)!;

			if (a.flags.HasFlag(EReorderableList.Collapsible))
			{
				GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
				GUI.Box(position, GUIContent.none);
				var inner = position.Resized(-_FOLDOUT_PAD);

				var labelHeight = _foldoutLabel.Value.CalcHeight(GUIContent.none, 100);
				var foldoutBox = inner.SliceTop(labelHeight);

				var dropBox = foldoutBox;

				var tEnabled = GUI.enabled;

				GUI.enabled = a.flags.HasFlag(EReorderableList.Resizable);
				var sizeBox = foldoutBox.SliceRight(EditorGUIUtility.singleLineHeight * 2f);
				DrawSizeField(sizeBox, property);

				GUI.enabled = tEnabled;

				var tIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 1;
				property.isExpanded = EditorGUI.Foldout(foldoutBox, property.isExpanded, property.displayName, true, _foldoutLabel.Value);
				EditorGUI.indentLevel = tIndent;
				
				DoDropArea(dropBox, property);

				if (property.isExpanded)
				{
					inner.SliceTop(_FOLDOUT_SPACING);
					inner.SliceLeft(_FOLDOUT_PAD);
					var bgRect = inner;
					bgRect.height -= GetList(property).footerHeight;
					GUI.Box(bgRect, GUIContent.none);
					GetList(property).DoList(inner);
				}
			}
			else
			{
				GetList(property).DoList(position);
			}

			EditorGUI.EndProperty();
		}

		private ReorderableList _list;

		private static readonly float _FOLDOUT_PAD = EditorGUIUtility.standardVerticalSpacing * 2f;
		private static readonly float _FOLDOUT_SPACING = EditorGUIUtility.standardVerticalSpacing * 2f;

		private static readonly Lazy<GUIStyle> _sizeBoxStyle = new(() =>
		{
			return new GUIStyle(EditorStyles.numberField)
			{
				fontSize = EditorStyles.miniLabel.fontSize
			};
		});
		
		private static readonly Lazy<GUIStyle> _foldoutLabel = new(() =>
		{
			return new GUIStyle(EditorStyles.foldout)
			{
				fontSize = (int)(EditorStyles.foldout.fontSize * 1.2f)
			};
		});

		private void DrawSizeField(Rect pos, SerializedProperty prop)
		{
			EditorGUI.BeginChangeCheck();
			var newSize = EditorGUI.DelayedIntField(pos, GUIContent.none, prop.arraySize, _sizeBoxStyle.Value);

			if (EditorGUI.EndChangeCheck() && newSize >= 0)
			{
				prop.arraySize = newSize;
			}
		}

		private ReorderableList GetList(SerializedProperty prop)
		{
			if (_list == null)
			{
				var a = (attribute as ReorderableListAttribute)!;

				var header = a.flags.HasFlag(EReorderableList.Header);

				if (a.flags.HasFlag(EReorderableList.Collapsible))
				{
					header = false;
				}
				
				var add = a.flags.HasFlag(EReorderableList.Add);
				var rm = a.flags.HasFlag(EReorderableList.Remove);
				var drag = a.flags.HasFlag(EReorderableList.Drag);

				var elType = fieldInfo.FieldType.GetInnermostType();

				var isUnityType = typeof(Object).IsAssignableFrom(elType);

				_list = new ReorderableList(prop.serializedObject, prop, drag, header, add, rm)
				{
					elementHeightCallback = i =>
					{
						var h = EditorGUI.GetPropertyHeight(prop.GetArrayElementAtIndex(i), GUIContent.none);
						return h + EditorGUIUtility.standardVerticalSpacing * 1f;
					},
					drawElementCallback = (r, i, _, _) =>
					{
						// r.SliceTop(EditorGUIUtility.standardVerticalSpacing);
						r.SliceBottom(EditorGUIUtility.standardVerticalSpacing);
						
						if (a.flags.HasFlag(EReorderableList.Index))
						{
							var pl = r.SliceLeft(EditorGUIUtility.singleLineHeight * 1.2f);
							EditorGUI.LabelField(pl, i.ToString(), EditorStyles.miniLabel);
						}
						
						var iProp = prop.GetArrayElementAtIndex(i);

						EditorGUI.PropertyField(r, iProp, GUIContent.none);
					}
				};

				if (a.flags.HasFlag(EReorderableList.Collapsible))
				{
					_list.showDefaultBackground = false;
				}
				

				if (a.flags.HasFlag(EReorderableList.Header))
				{
					_list.drawHeaderCallback = rect =>
					{
						var pos = rect;

						if (a.flags.HasFlag(EReorderableList.Resizable))
						{
							var sizeBox = pos.SliceRight(EditorGUIUtility.singleLineHeight * 2f);

							var sc = sizeBox.center;
							sizeBox.height *= 0.8f;
							sizeBox.center = sc;
				
							EditorGUI.BeginChangeCheck();
							var newSize = EditorGUI.DelayedIntField(sizeBox, GUIContent.none, prop.arraySize, _sizeBoxStyle.Value);

							if (EditorGUI.EndChangeCheck() && newSize >= 0)
							{
								prop.arraySize = newSize;
							}
						}

						var tIndent = EditorGUI.indentLevel;
						EditorGUI.indentLevel = 0;
						EditorGUI.LabelField(pos, prop.displayName);
						EditorGUI.indentLevel = tIndent;

						if (!isUnityType)
						{
							return;
						}

						DoDropArea(rect, prop);

						// DrawerGUI.DragDrop(rect, (Object[] obs) =>
						// {
						// 	EditorApplication.delayCall += () =>
						// 	{
						// 		HandleDrop(elType, prop, obs);
						// 	};
						// }, null);
						
					};
				}
				
				
			}
			return _list;
		}

		private void DoDropArea(Rect area, SerializedProperty prop)
		{
			var elType = fieldInfo.FieldType.GetInnermostType();
			
			DrawerGUI.DragDrop(area, (Object[] obs) =>
			{
				EditorApplication.delayCall += () =>
				{
					HandleDrop(elType, prop, obs);
				};
			}, null);
		}

		private void HandleDrop(Type elType, SerializedProperty arrProp, Object[] dropped)
		{
			if (dropped.Length == 0)
			{
				return;
			}

			var isComponentType = typeof(Component).IsAssignableFrom(elType);
			var isGameObjectType = typeof(GameObject).IsAssignableFrom(elType);

			int count = 0;

			var ownerIsAsset = AssetDatabase.Contains(arrProp.serializedObject.targetObject);

			foreach (var ob in dropped)
			{
				var dropType = ob.GetType();

				var droppedIsScene = !AssetDatabase.Contains(ob);
				
				if (ownerIsAsset && droppedIsScene)
				{
					continue;
				}

				Object addOb = null;
				
				if (isGameObjectType || isComponentType)
				{
					addOb = GetGameObjectDrop(elType, ob);
				}
				else if(elType.IsAssignableFrom(dropType))
				{
					addOb = ob;
				}
				if (addOb)
				{
					var i = arrProp.arraySize;
					arrProp.InsertArrayElementAtIndex(i);
					var item = arrProp.GetArrayElementAtIndex(i);
					item.objectReferenceValue = addOb;
					count++;
				}
			}

			if (count > 0)
			{
				arrProp.serializedObject.ApplyModifiedProperties();
				arrProp.serializedObject.Update();
			}
		}

		private static Object GetGameObjectDrop(Type arrType, Object ob)
		{
			if (arrType == typeof(Component) && ob is GameObject)
			{
				return ((GameObject)ob).transform;
			}
			if (arrType == typeof(GameObject) && ob is GameObject)
			{
				return ob;
			}
			if (ob is Component component && typeof(Component).IsAssignableFrom(arrType))
			{
				return component.GetComponent(arrType);
			}
			return null;
		}
		
		
	}
}

#endif