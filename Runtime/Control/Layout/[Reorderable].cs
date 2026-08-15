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
		/// Every option
		/// </summary>
		All = ~0,
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
		/// Shows controls next to label
		/// </summary>
		Compact = 128,
		/// <summary>
		/// Can drop items
		/// </summary>
		Drop = 256,
		/// <summary>
		/// Draws typical looking list
		/// </summary>
		Standard = All & ~Foldable & ~Compact,
		/// <summary>
		/// 
		/// </summary>
		Minimal = All & ~Resizable & ~Index & ~Foldable,
	}

	/// <summary>
	/// Draws array as reorderable list
	/// </summary>
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
	using System.ComponentModel;
	using System.Reflection;
	using UnityEditor;
	using UnityEditorInternal;
	using UnityEngine;
	using Component = UnityEngine.Component;
	using Object = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(ReorderableAttribute))]
	internal sealed class _ReorderableAttribute : __ControlDrawer<ReorderableAttribute>
	{
		protected override void OnInit()
		{
			// _focusedIndex = -1;
		}

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			GetList(prop);
			
			var h = _PAD_LIST_BOTTOM;

			if (_IsFoldable)
			{
				var labelHeight = DrawerStyles.FoldoutHeight;
				var lHeight = prop.isExpanded
				? GetList(prop).GetHeight() + _FOLDOUT_SPACING
				: 0f;
				var padHeight = _FOLDOUT_PAD * 2f;
				h += labelHeight + lHeight + padHeight;
			}
			else
			{
				h += GetList(prop).GetHeight();
			}
			return h;
		}

		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l) {}
		
		protected override void OnField(in DrawContext ctx)
		{
			var property = ctx.property;
			var position = ctx.position;

			position.SliceBottom(_PAD_LIST_BOTTOM);
			

			if (CheckFlag(EReorderable.Foldable))
			{
				GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
				GUI.Box(position, GUIContent.none);
				var inner = position.Resized(-_FOLDOUT_PAD);

				var labelHeight = DrawerStyles.FoldoutHeight;
				var foldoutBox = inner.SliceTop(labelHeight);

				var dropArea = foldoutBox;
				
				DrawCompactControls(ref foldoutBox, property);

				var label = property.displayName;

				if (!CheckFlag(EReorderable.Resizable))
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

		private bool _IsFoldable => CheckFlag(EReorderable.Foldable);
		private bool _IsUnityObjectField => typeof(Object).IsAssignableFrom(_FieldType);
		private ReorderableList _list;

		private bool CheckFlag(EReorderable flag)
		{
			return _Attribute.flags.HasFlag(flag);
		}

		private static readonly float _PAD_LIST_BOTTOM = EditorGUIUtility.standardVerticalSpacing * 1f;
		private static readonly float _PAD_ITEM_TOP = EditorGUIUtility.standardVerticalSpacing * 0.5f;
		private static readonly float _PAD_ITEM_BOTTOM = EditorGUIUtility.standardVerticalSpacing * 0.5f;
		private static readonly float _FOLDOUT_PAD = EditorGUIUtility.standardVerticalSpacing * 2f;
		private static readonly float _FOLDOUT_SPACING = EditorGUIUtility.standardVerticalSpacing * 2f;

		private static readonly Lazy<GUIStyle> _SIZE_BOX_STYLE = new(() => new GUIStyle(EditorStyles.numberField)
		{
			fontSize = EditorStyles.miniLabel.fontSize,
			alignment = TextAnchor.MiddleLeft
		});

		private static readonly Lazy<GUIContent> _MISSING_REFS_LB = new(() =>
		{
			var warnIcon = EditorGUIUtility.IconContent("console.warnicon");
			return new GUIContent(string.Empty)
			{
				tooltip = "List contains missing objects",
				image = warnIcon?.image
			};
		});

		private static void DrawWarningIcon(in Rect pos)
		{
			DrawerGUI.DrawTex(_MISSING_REFS_LB.Value?.image, pos);
		}

		private float GetListItemHeight(int i)
		{
			var item = _list?.serializedProperty?.GetArrayElementAtIndex(i);
			if (item == null)
			{
				return 0f;
			}
			var h = EditorGUI.GetPropertyHeight(item, GUIContent.none);
			return h + _PAD_ITEM_BOTTOM + _PAD_ITEM_TOP;
		}

		private void DrawListItem(Rect r, int i, bool focused, bool active)
		{
			var prop = _list?.serializedProperty?.GetArrayElementAtIndex(i);
			if (prop == null)
			{
				return;
			}

			var box = r;
			
			r.SliceTop(_PAD_ITEM_TOP);
			r.SliceBottom(_PAD_ITEM_BOTTOM);

			if (CheckFlag(EReorderable.Index))
			{
				var pl = r.SliceLeft(EditorGUIUtility.singleLineHeight * 1.2f);
				EditorGUI.LabelField(pl, i.ToString(), EditorStyles.miniLabel);
			}
			EditorGUI.PropertyField(r, prop, GUIContent.none);
			
			
			if (_IsUnityObjectField && prop.HasMissingReference())
			{
				var icoRect = box;
				icoRect.width = icoRect.height;
				icoRect = icoRect.Resized(-icoRect.height * 0.25f);
				icoRect.position -= new Vector2(r.height, 0f);
				DrawWarningIcon(icoRect);
			}
		}

		private static bool IconButton(ref Rect pos, GUIContent lb, bool enabled = true)
		{
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

		private static Lazy<GUIContent> GetLazyUnityIcon(string name)
		{
			return new Lazy<GUIContent>(() => EditorGUIUtility.IconContent(name));
		}

		private static readonly Lazy<GUIContent> _ICO_MENU = GetLazyUnityIcon("_Menu");
		private static readonly Lazy<GUIContent> _ICO_ADD = GetLazyUnityIcon("CreateAddNew");
		private static readonly Lazy<GUIContent> _ICO_ADD_DROP = GetLazyUnityIcon("Toolbar Plus More");
		private static readonly Lazy<GUIContent> _ICO_REMOVE = GetLazyUnityIcon("Toolbar Minus");
		private static readonly GUIContent _LABEL_CLEAR = new ("Clear");
		private static readonly GUIContent _LABEL_CLEAR_MISSING = new ("Clear Missing References");

		private void DrawCompactControls(ref Rect pos, SerializedProperty prop, bool add = true, bool rm = true, bool size = true)
		{
			var drawCompact = CheckFlag(EReorderable.Compact);

			GetList(prop);

			var pad = EditorGUIUtility.standardVerticalSpacing * 1.5f;

			if (IconButton(ref pos, _ICO_MENU.Value, true))
			{
				var m = new GenericMenu();
				if (prop.arraySize > 0)
				{
					m.AddItem(_LABEL_CLEAR, false, () =>
					{
						EditorApplication.delayCall += () =>
						{
							prop.arraySize = 0;
							prop.serializedObject.ApplyModifiedProperties();
						};
					});
				}
				else
				{
					m.AddDisabledItem(_LABEL_CLEAR);
				}

				if (typeof(Object).IsAssignableFrom(_FieldType))
				{
					var missingRefs = prop.CountMissingArrayItemRefs();

					if (missingRefs > 0)
					{
						m.AddItem(_LABEL_CLEAR_MISSING, false, () =>
						{
							EditorApplication.delayCall += () =>
							{
								for (int i = prop.arraySize - 1; i >= 0; i--)
								{
									var iProp = prop.GetArrayElementAtIndex(i);
									if (iProp.HasMissingReference())
									{
										prop.DeleteArrayElementAtIndex(i);
									}
									prop.serializedObject.ApplyModifiedProperties();
								}
							};
						});
					}
				}
				m.ShowAsContext();
			}

			if (drawCompact && rm && CheckFlag(EReorderable.Remove))
			{
				var canRemove = _list.index >= 0 && _list.index < _list.count;
;				if (IconButton(ref pos, _ICO_REMOVE.Value, canRemove))
				{
					if (_list.index >= 0 && _list.index < _list.count)
					{
						prop.DeleteArrayElementAtIndex(_list.index);
					}
				}
				pos.SliceRight(pad);
			}

			if (drawCompact && add && CheckFlag(EReorderable.Add))
			{
				var ico = IsManagedReferenceField() ? _ICO_ADD_DROP.Value : _ICO_ADD.Value;
				if (IconButton(ref pos, ico))
				{
					OnAddButton(prop);
				}
				pos.SliceRight(pad);
			}

			if (CheckFlag(EReorderable.Resizable))
			{
				var sizeBox = pos.SliceRight(EditorGUIUtility.singleLineHeight * 2f);
				var sc = sizeBox.center;
				sizeBox.center = sc;
				DrawSizeField(sizeBox, prop);
				pos.SliceRight(pad);
			}

			if (CheckFlag(EReorderable.Foldable) && _IsUnityObjectField && prop.CountMissingArrayItemRefs() > 0)
			{
				var tColor = GUI.color;
				GUI.color *= prop.isExpanded ? 0.7f : 1f;
				var iconRect = pos.SliceRight(EditorGUIUtility.singleLineHeight);
				iconRect.height = iconRect.width;
				iconRect = iconRect.Resized(-iconRect.height * 0.1f);
				GUI.Box(iconRect.Resized(-iconRect.height * 0.1f), _MISSING_REFS_LB.Value, GUIStyle.none);
				GUI.color = tColor;
			}
		}

		private bool IsManagedReferenceField()
		{
			return
			!_IsUnityObjectField
			&& fieldInfo.IsDefined(typeof(SerializeReference), false)
			&& fieldInfo.IsDefined(typeof(InstancedReferenceAttribute), false);
		}

		private void OnAddButton(SerializedProperty arrProp)
		{
			if (IsManagedReferenceField())
			{
				var m = CreateTypeMenu(_FieldType, o =>
				{
					var newType = (Type)o;

					EditorApplication.delayCall += () =>
					{
						var i = arrProp.arraySize;
						arrProp.InsertArrayElementAtIndex(i);
						var newItem = arrProp.GetArrayElementAtIndex(i);

						object newVal = newType != null
						? Activator.CreateInstance(newType)
						: null;

						newItem.managedReferenceValue = newVal;
						newItem.serializedObject.ApplyModifiedProperties();
						newItem.serializedObject.Update();
					};
				});
				
				m.ShowAsContext();
			}
			else
			{
				arrProp.InsertArrayElementAtIndex(arrProp.arraySize);
			}
		}

		private GenericMenu CreateTypeMenu(Type baseType, GenericMenu.MenuFunction2 fn, bool showNull = false)
		{
			var menu = new GenericMenu();

			var types = TypeCache.GetTypesDerivedFrom(baseType);

			Assembly currentAssembly = null;

			if (showNull)
			{
				menu.AddItem(new GUIContent(PluginConstants.Label.POPUP_UNSET), false, fn, null);
				menu.AddSeparator(string.Empty);
			}

			foreach (var type in types)
			{
				if (type.GetConstructor(Type.EmptyTypes) == null) // new()
				{
					continue;
				}

				if (!type.IsClass || type.IsAbstract)
				{
					continue;
				}
				if (currentAssembly != type.Assembly)
				{
					if (currentAssembly != null)
					{
						menu.AddSeparator(string.Empty);
					}
					currentAssembly = type.Assembly;
					menu.AddDisabledItem(new GUIContent(currentAssembly.GetName().Name));
				}
				var dAttribute = type.GetCustomAttribute<DisplayNameAttribute>();
				var label = dAttribute != null ? dAttribute.DisplayName : type.Name;
				var dname = new GUIContent(label);
				menu.AddItem(dname, false, fn,  type);
			}
			return menu;
		}

		private void DrawListHeader(Rect rect)
		{
			var prop = _list?.serializedProperty;

			if (prop == null)
			{
				return;
			}

			var showHeader = CheckFlag(EReorderable.Header) && !_IsFoldable;
			showHeader |= CheckFlag(EReorderable.Compact) && !_IsFoldable;

			if (!showHeader)
			{
				return;
			}

			var label = prop.displayName;

			if (!CheckFlag(EReorderable.Resizable))
			{
				label = $"{label} ({prop.arraySize})";
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
			GUI.enabled = CheckFlag(EReorderable.Resizable);
			var newSize = EditorGUI.DelayedIntField(pos, GUIContent.none, prop.arraySize, _SIZE_BOX_STYLE.Value);
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
				var isCompact = CheckFlag(EReorderable.Compact);
				var showListheader = isCompact || CheckFlag(EReorderable.Header);

				// hide standard list header when foldable
				showListheader &= !_IsFoldable;

				// toggle add/rm for standard list footer
				var showAdd = !isCompact && CheckFlag(EReorderable.Add);
				var showRemove = !isCompact && CheckFlag(EReorderable.Remove);
				var allowDrag = CheckFlag(EReorderable.Drag);

				_list = new ReorderableList(prop.serializedObject, prop, allowDrag, showListheader, showAdd, showRemove)
				{
					elementHeightCallback = GetListItemHeight,
					drawElementCallback = DrawListItem,
				};

				// effectively hide standard footer if no buttons are enabled
				if (!showAdd && !showRemove)
				{
					_list.footerHeight = 0f;
				}

				// show dropdown variant if field has [SerializeReference]
				if (showAdd && IsManagedReferenceField())
				{
					_list.onAddDropdownCallback = (r, l) => OnAddButton(prop);
				}

				// register header drawing if enabled
				if (showListheader)
				{
					_list.drawHeaderCallback = DrawListHeader;
				}
			}
			return _list;
		}

		private void DoDropArea(Rect area, SerializedProperty prop)
		{
			// can't drop, adding or drop disabled
			if (!CheckFlag(EReorderable.Drop&EReorderable.Add))
			{
				return;
			}
			// can't drop non-unity objects
			if (!_IsUnityObjectField)
			{
				return;
			}
			DrawerGUI.DragDrop(area, prop, HandleDrop, null);
		}

		private void HandleDrop(SerializedProperty arrProp, Object[] dropped)
		{
			if (dropped.Length == 0)
			{
				return;
			}

			var isComponentType = typeof(Component).IsAssignableFrom(_FieldType);
			var isGameObjectType = typeof(GameObject).IsAssignableFrom(_FieldType);

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
					addOb = GetComponentDrop(_FieldType, ob);
				}
				else if(_FieldType.IsAssignableFrom(dropType))
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

		// retrieve assignable object from dropped gameobject/component
		private static Object GetComponentDrop(Type fieldType, Object ob)
		{
			// field is gameobject
			if (fieldType == typeof(GameObject) && ob is GameObject)
			{
				return ob;
			}

			// field type is plain component -> default to transform component
			if (fieldType == typeof(Component) && ob is GameObject gameObject)
			{
				return gameObject.transform;
			}

			// get sibling component
			if (ob is Component component && typeof(Component).IsAssignableFrom(fieldType))
			{
				return component.GetComponent(fieldType);
			}

			return null;
		}
		
		
	}
}

#endif