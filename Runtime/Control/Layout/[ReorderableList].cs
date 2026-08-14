// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Diagnostics;
	using UnityEngine;

	[Flags]
	public enum EReorderable
	{
		/// <summary>
		/// Show add button
		/// </summary>
		Add = 1,
		/// <summary>
		/// Show remove button
		/// </summary>
		Remove = 2,
		/// <summary>
		/// Allow drag
		/// </summary>
		Drag = 4,
		/// <summary>
		/// Show default header label
		/// </summary>
		Header = 8,
		/// <summary>
		/// Show element indices
		/// </summary>
		Index = 16,
		/// <summary>
		/// Show size control
		/// </summary>
		Resizable = 32,
		/// <summary>
		/// Wrap list in foldout
		/// </summary>
		Foldable = 64,
		/// <summary>
		/// Move buttons to header
		/// </summary>
		Compact = 128,
		/// <summary>
		/// Can drop items
		/// </summary>
		Drop = 256,
		All = ~0,
		Minimal = All & ~Resizable & ~Index & ~Foldable,
	}

	/// <summary>
	/// Draws array as reorderable list
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ReorderableAttribute : __BaseControl
	{
		public ReorderableAttribute(EReorderable flags = EReorderable.Minimal) : base(true, true)
		{
			this.flags = flags;
		}
		internal EReorderable flags { get; }
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

	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	internal sealed class _ReorderableAttribute : __ControlDrawer<ReorderableAttribute>
	{
		protected override void OnInit()
		{
			_focusedIndex = -1;
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			GetList(prop);
			
			var a = (attribute as ReorderableAttribute)!;

			if (a.flags.HasFlag(EReorderable.Foldable))
			{
				var labelHeight = DrawerStyles.FoldoutHeight;
				var lHeight = prop.isExpanded
				? GetList(prop).GetHeight() + _FOLDOUT_SPACING
				: 0f;
				var padHeight = _FOLDOUT_PAD * 2f;
				return labelHeight + lHeight + padHeight;

			}
			return GetList(prop).GetHeight();
		}

		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l) {}

		protected override void OnField(in DrawContext ctx)
		{
			var property = ctx.property;
			var position = ctx.position;
			
			var a = (attribute as ReorderableAttribute)!;

			if (a.flags.HasFlag(EReorderable.Foldable))
			{
				GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
				GUI.Box(position, GUIContent.none);
				var inner = position.Resized(-_FOLDOUT_PAD);

				var labelHeight = DrawerStyles.FoldoutHeight;
				var foldoutBox = inner.SliceTop(labelHeight);

				var dropArea = foldoutBox;
				
				DrawCompactControls(ref foldoutBox, property);

				var label = property.displayName;

				if (!a.flags.HasFlag(EReorderable.Resizable))
				{
					label += $" ({property.arraySize})";
				}

				var tIndent = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 1;
				property.isExpanded = EditorGUI.Foldout(foldoutBox, property.isExpanded, label, true, DrawerStyles.Foldout);
				EditorGUI.indentLevel = tIndent;

				DoDropArea(dropArea, property);

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
		}

		private ReorderableList _list;

		private int _focusedIndex;

		private static readonly float _FOLDOUT_PAD = EditorGUIUtility.standardVerticalSpacing * 2f;
		private static readonly float _FOLDOUT_SPACING = EditorGUIUtility.standardVerticalSpacing * 2f;

		private static readonly Lazy<GUIStyle> _sizeBoxStyle = new(() => new GUIStyle(EditorStyles.numberField)
		{
			fontSize = EditorStyles.miniLabel.fontSize
		});

		private float GetListItemHeight(int i)
		{
			var prop = _list?.serializedProperty?.GetArrayElementAtIndex(i);
			if (prop == null)
			{
				return 0f;
			}

			var h = EditorGUI.GetPropertyHeight(prop, GUIContent.none);
			return h + EditorGUIUtility.standardVerticalSpacing * 1f;
		}

		private void DrawListItem(Rect r, int i, bool focused, bool active)
		{
			var prop = _list?.serializedProperty?.GetArrayElementAtIndex(i);
			if (prop == null)
			{
				return;
			}

			if (active && focused)
			{
				_focusedIndex = i;
			}

			var a = (attribute as ReorderableAttribute)!;
			r.SliceBottom(EditorGUIUtility.standardVerticalSpacing);
			if (a.flags.HasFlag(EReorderable.Index))
			{
				var pl = r.SliceLeft(EditorGUIUtility.singleLineHeight * 1.2f);
				EditorGUI.LabelField(pl, i.ToString(), EditorStyles.miniLabel);
			}
			EditorGUI.PropertyField(r, prop, GUIContent.none);
		}

		private static bool IconButton(ref Rect pos, string iconName, bool enabled = true)
		{
			var lb = EditorGUIUtility.IconContent(iconName);

			if (lb == null)
			{
				return false;
			}
			

			var size = EditorStyles.iconButton.CalcSize(lb);
			var width = size.x;
			var btnRect = pos.SliceRight(width);
			var center = btnRect.center;
			btnRect.height = size.y;
			btnRect.center = center;

			var tEnabled = GUI.enabled;
			GUI.enabled &= enabled;
			var pressed = GUI.Button(btnRect, lb, EditorStyles.iconButton);
			GUI.enabled = tEnabled;

			return pressed;
		}

		private void DrawCompactControls(ref Rect pos, SerializedProperty prop, bool add = true, bool rm = true, bool size = true)
		{
			GetList(prop);

			var pad = EditorGUIUtility.standardVerticalSpacing * 1.5f;

			if (rm && _Attribute.flags.HasFlag(EReorderable.Remove))
			{
				var canRemove = _focusedIndex >= 0 && _focusedIndex < _list.count;
				if (IconButton(ref pos, "d_Toolbar Minus", canRemove))
				{
					if (_list.index >= 0 && _list.index < _list.count)
					{
						prop.DeleteArrayElementAtIndex(_list.index);
					}
				}
				pos.SliceRight(pad);
			}

			if (add && _Attribute.flags.HasFlag(EReorderable.Add))
			{
				if (IconButton(ref pos, "CreateAddNew"))
				{
					prop.InsertArrayElementAtIndex(prop.arraySize);
				}
				pos.SliceRight(pad);
			}

			if (_Attribute.flags.HasFlag(EReorderable.Resizable))
			{
				var sizeBox = pos.SliceRight(EditorGUIUtility.singleLineHeight * 2f);
				var sc = sizeBox.center;
				sizeBox.center = sc;
				DrawSizeField(sizeBox, prop);
			}
		}

		private void DrawListHeader(Rect rect)
		{
			var prop = _list?.serializedProperty;

			if (prop == null)
			{
				return;
			}

			var foldable = _Attribute.flags.HasFlag(EReorderable.Foldable);

			var showHeader =
			_Attribute.flags.HasFlag(EReorderable.Header)
			&& !_Attribute.flags.HasFlag(EReorderable.Foldable);

			if (_Attribute.flags.HasFlag(EReorderable.Compact) && !foldable)
			{
				showHeader = true;
			}

			if (!showHeader)
			{
				return;
			}

			var label = prop.displayName;
			
			if (!_Attribute.flags.HasFlag(EReorderable.Resizable))
			{
				label += $" ({prop.arraySize})";
			}
	
			var pos = rect;

			var controlRect = pos;
			controlRect.height *= 0.8f;
			controlRect.center = pos.center;

			DrawCompactControls(ref controlRect, prop);

			pos.width -= pos.width - controlRect.width;
			
			GUI.Label(pos, label);
			
			DoDropArea(rect, prop);
		}

		private void DrawSizeField(Rect pos, SerializedProperty prop)
		{
			var tEnabled = GUI.enabled;
			EditorGUI.BeginChangeCheck();
			GUI.enabled = _Attribute.flags.HasFlag(EReorderable.Resizable);
			var newSize = EditorGUI.DelayedIntField(pos, GUIContent.none, prop.arraySize, _sizeBoxStyle.Value);
			GUI.enabled = tEnabled;
			if (EditorGUI.EndChangeCheck() && newSize >= 0)
			{
				prop.arraySize = newSize;
			}
		}

		private ReorderableList GetList(SerializedProperty prop)
		{
			if (_list == null)
			{
				var a = (attribute as ReorderableAttribute)!;

				var header = a.flags.HasFlag(EReorderable.Header);

				if (a.flags.HasFlag(EReorderable.Foldable))
				{
					header = false;
				}

				var compact = a.flags.HasFlag(EReorderable.Compact);
				var add = !compact && a.flags.HasFlag(EReorderable.Add);
				var rm = !compact && a.flags.HasFlag(EReorderable.Remove);
				var drag = a.flags.HasFlag(EReorderable.Drag);

				_list = new ReorderableList(prop.serializedObject, prop, drag, header, add, rm)
				{
					elementHeightCallback = GetListItemHeight,
					drawElementCallback = DrawListItem,
				};

				if (!add && !rm)
				{
					_list.footerHeight = 0f;
				}

				if (compact || a.flags.HasFlag(EReorderable.Header))
				{
					_list.drawHeaderCallback = DrawListHeader;
				}
			}
			return _list;
		}

		private void DoDropArea(Rect area, SerializedProperty prop)
		{
			if (!_Attribute.flags.HasFlag(EReorderable.Drop&EReorderable.Add))
			{
				return;
			}
			if (!typeof(Object).IsAssignableFrom(_FieldType))
			{
				return;
			}

			DrawerGUI.DragDrop(area, obs =>
			{
				EditorApplication.delayCall += () =>
				{
					HandleDrop(_FieldType, prop, obs);
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