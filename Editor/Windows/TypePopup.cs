// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;

	internal sealed class TypeSearch : PopupWindowContent
	{
		public struct Options
		{
			public bool useDisplayName;
			public ESearchTypeFlags flags;
			public string[] assemblies;
			public string[] namespaces;
			public Type[] baseTypes;
			public Func<Type, bool> customFilter;
		}

		private readonly struct SearchFilter
		{
			public static SearchFilter Empty => new (string.Empty);
			
			public SearchFilter(string search)
			{
				search ??= string.Empty;
				filterString = search.Trim().ToLower();
				segments = filterString.Split(" ");
			}

			private readonly string filterString;
			private readonly string[] segments;

			public bool MatchString(string str)
			{
				if (string.IsNullOrEmpty(filterString)|| filterString.Length < _MIN_SEARCH_LEN)
				{
					return true;
				}

				if (segments is { Length: > 0 })
				{
					foreach (var p in segments)
					{
						var ps = p.Trim();
						if (ps.Length == 0)
						{
							continue;
						}
						if (!str.Contains(ps))
						{
							return false;
						}
					}
					return true;
				}
				return str.Contains(filterString);
			}
		}

		private static readonly float _MIN_WIDTH = Screen.width * 0.4f;
		private const int _MIN_SEARCH_LEN = 3;

		private static T PickSkin<T>(in T dark, in T light)
		{
			return EditorGUIUtility.isProSkin ? dark : light;
		}

		private static Color Fade(Color c, float a)
		{
			c.a = a;
			return c;
		}

		private static readonly Color _UNITY_SELECT_COLOR = new Color(0.24f, 0.5f, 0.874f);

		private static readonly Color _HOVER_COLOR = PickSkin
		(
			// new Color(0.2392157f, 0.3764706f, 0.5686275f) * 0.9f
			Fade(_UNITY_SELECT_COLOR, 0.6f)
			,Fade(_UNITY_SELECT_COLOR, 0.8f)
		);
		
		private static readonly Color _HEADER_HOVER_COLOR
		= PickSkin(Color.white * 0.25f, Fade(Color.white, 0.2f));

		private static readonly Color _HEADER_COLOR
		= PickSkin(Color.black * 0.2f, Color.black * 0.1f);
		
		private static readonly Color _ARROW_COLOR
		= PickSkin(Color.white * 0.5f, Color.black * 0.5f);

		private static readonly Color _ACTIVE_ITEM_COLOR
		= PickSkin(Color.white * 0.5f, Fade(Color.black,0.1f));
		
		private static readonly Color _SEP_COLOR
		= PickSkin(Fade(Color.white,0.05f), Color.black * 0.3f);
		
		private const string _ATLAS_GUID = "e769e4d9f339626498a12b64168231ee";

		private const string _SEARCH_FIELD_NAME = "search_field";

		private static readonly Lazy<Texture2D> _TEX_ATLAS =
		new (() => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(_ATLAS_GUID)));

		private static readonly Rect _COORDS_ARROWL = new (0.5f, 0, 0.25f, 0.25f);
		private static readonly Rect _COORDS_ARROWR = new (0.75f, 0, 0.25f, 0.25f);

		private const float _SEP_WIDTH = 1f;

		public static void Open
		(
			in Rect pos,
			Type value,
			Options options,
			Action<Type> setFn
		)
		{
			var p = new TypeSearch(value, options, setFn);
			PopupWindow.Show(pos, p);
		}

		public override Vector2 GetWindowSize()
		{
			var height = PopupStyles.HeaderHeight + PopupStyles.ItemHeight * 16f;
			return new Vector2(Mathf.Max(_MIN_WIDTH, _preferredWidth * 0.6f), height);
		}

		private static readonly float _SEARCH_PAD = EditorGUIUtility.singleLineHeight * 0.3f;

		public override void OnGUI(Rect rect)
		{
			var searchbarHeight = EditorStyles.toolbarSearchField.CalcHeight(GUIContent.none, 10);
			var searchHeight = searchbarHeight + _SEARCH_PAD * 2f;
			DrawSearchField(rect.SliceTop(searchHeight));

			if (!_searchFocused)
			{
				GUI.FocusControl(_SEARCH_FIELD_NAME);
				_searchFocused = true;
			}

			if (_refreshTree && EditorApplication.timeSinceStartup > (_timeSearched + 0.2f))
			{
				_flatMode = _filterString.Length >= _MIN_SEARCH_LEN;
				_refreshTree = false;
				_currentNode.Filter(new SearchFilter(_filterString));
				_currentNode = _rootNode;
			}

			DrawNode(rect, _currentNode);
			editorWindow.Repaint();
		}

		private bool _searchFocused;
		private readonly Action<Type> _setFn;
		private readonly float _preferredWidth;
		private MenuNode _currentNode;
		private readonly Type _currentValue;
		private readonly MenuNode _rootNode;
		private static (Assembly, Type[])[] _cachedTypes;
		private Vector2 _pageScroll;
		private bool _refreshTree;
		private string _filterString = string.Empty;
		private double _timeSearched;
		private bool _flatMode;
		private const string _SEARCH_LABEL = "Search";
		private const int _MAX_FLAT_RESULTS = 50; // how many results can be shown without categories when searching
		
		// used to match types with enum flags
		private static readonly (ESearchTypeFlags, Func<Type, bool>)[] _FLAG_FILTERS =
		{
			(ESearchTypeFlags.StaticClass, t => t.IsClass && t.IsStatic()),
			(ESearchTypeFlags.Interface, t => t.IsInterface),
			(ESearchTypeFlags.Abstract, t => t.IsAbstract && !t.IsInterface && !t.IsStatic()),
			(ESearchTypeFlags.Struct, t => t.IsStruct()),
			(ESearchTypeFlags.Enum, t => t.IsEnum),
			(ESearchTypeFlags.Private, t => !t.IsVisible),
			(ESearchTypeFlags.Nested, t => t.IsNested),
			(ESearchTypeFlags.Class, t => t.IsClass && !t.IsStatic()),
			(ESearchTypeFlags.Primitive, t => t.IsPrimitive && !t.IsEnum),
			(ESearchTypeFlags.Newable, t => t.GetConstructor(Type.EmptyTypes) != null),
			(ESearchTypeFlags.Generic, t => t.IsGenericTypeDefinition)
		};

		// categorize types
		private static readonly (string, Func<Type, bool>)[] _TYPE_CATEGORIES =
		{
			("# Exception", t => t.IsClass && typeof(Exception).IsAssignableFrom(t)),
			("# Attribute", t => t.IsClass && typeof(Attribute).IsAssignableFrom(t)),
			("# Static", t => t.IsClass && t.IsStatic()),
			("# Class", t => t.IsClass),
			("# Enum", t => t.IsEnum),
			("# Struct", t => t.IsStruct()),
			("# Interface", t => t.IsInterface),
			("# Primitive", t => t.IsPrimitive),
			("# Enum", t => t.IsEnum),
		};

		private TypeSearch(Type currentValue, Options options, Action<Type> setFn)
		{
			_setFn = setFn;
			_rootNode = CreateMenuTree(options, currentValue, out var selectedNode, out var longestLabel);

			_currentNode = _rootNode;
			_currentValue = currentValue;

			if (selectedNode != null && _rootNode.count >= _MAX_FLAT_RESULTS)
			{
				_currentNode = selectedNode.parent;
			}

			if (_currentValue != null && selectedNode != null)
			{
				var sIndex = 0;
				for (var i = 0; i < selectedNode.parent.children.Count; i++)
				{
					var cNode = selectedNode.parent.children[i];
					if (cNode.filteredCount == 0)
					{
						continue;
					}

					if (cNode.leafValue == _currentValue)
					{
						break;
					}
					sIndex++;
				}
				_pageScroll = new Vector2(0f, PopupStyles.ItemHeight * sIndex);
			}
			
			_preferredWidth = PopupStyles.ItemLabel.CalcSize(new GUIContent(longestLabel)).x;
		}

		private void Select(Type t)
		{
			_setFn?.Invoke(t);
			editorWindow.Close();
		}

		private void DrawSearchField(Rect rect)
		{
			var sepRect = rect;
			sepRect = sepRect.SliceBottom(_SEP_WIDTH);
			
			
			EditorGUI.DrawRect(rect, _HEADER_COLOR * 0.6f);

			var inner = rect.Resized(-_SEARCH_PAD*2f);

			var oldVal = _filterString;

			GUI.SetNextControlName(_SEARCH_FIELD_NAME);
			var newVal = EditorGUI.TextField(inner, GUIContent.none, _filterString, EditorStyles.toolbarSearchField);
	
			if (newVal != _filterString)
			{
				_filterString = newVal;
	
				if (_filterString.Length >= _MIN_SEARCH_LEN || newVal.Length < _MIN_SEARCH_LEN && oldVal.Length >= _MIN_SEARCH_LEN)
				{
					_refreshTree = true;
					_timeSearched = EditorApplication.timeSinceStartup;
				}
			}
			
			EditorGUI.DrawRect(sepRect, _SEP_COLOR);
		}

		private static void DrawIcon(in Rect pos, in Rect coords, Color color)
		{
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, pos, coords, color);
		}

		private class MenuNode : IComparable<MenuNode>
		{
			public MenuNode parent;
			public string name;
			public Type leafValue;
			public string filterName;
			public int filteredCount { get; private set; }
			public int count { get; private set; }
			public int filterCountRecursive { get; private set; }
			public readonly List<MenuNode> children = new ();

			private string _cachedBreadcrumbs;
			
			public void RefreshCount()
			{
				GetCount();
			}

			private int GetCount()
			{
				int c = leafValue != null ? 1 : 0;
				foreach (var child in children)
				{
					c += child.GetCount();
				}
				count = c;
				return c;
			}

			public bool Filter(in SearchFilter args)
			{
				filteredCount = 0;
				filterCountRecursive = 0;
				if (leafValue != null && args.MatchString(filterName))
				{
					filteredCount++;
					filterCountRecursive++;
				}

				foreach (var c in children)
				{
					if (c.Filter(args))
					{
						filteredCount++;
						filterCountRecursive += c.filterCountRecursive;
					}
				}
				return filteredCount > 0;
			}

			public int CompareTo(MenuNode b)
			{
				int ret = (leafValue != null) == (b.leafValue != null)
				? 0
				: (leafValue != null ? 1 : -1);
				return ret != 0 ? ret : string.CompareOrdinal(name, b.name);
			}

			public void Sort()
			{
				children.Sort();
				foreach (var c in children)
				{
					c.Sort();
				}
			}

			public string GetBreadcrumbLabel()
			{
				if (_cachedBreadcrumbs != null)
				{
					return _cachedBreadcrumbs;
				}
				var node = this;
				_cachedBreadcrumbs = "";
				var i = 0;
				while (node != null)
				{
					if (node.parent == null)
					{
						break; // skip root
					}

					var currentPath = node.name;
					if (i > 0)
					{
						currentPath += " / ";
					}
					_cachedBreadcrumbs = currentPath + _cachedBreadcrumbs;
					node = node.parent;
					i++;
				}
				return _cachedBreadcrumbs;
			}

			public MenuNode FindChildOrNew(string nodeName)
			{
				var node = children.Find(x => x.name == nodeName);
				if (node == null)
				{
					node = new MenuNode
					{
						name = nodeName,
						parent = this,
					};
					children.Add(node);
				}
				return node;
			}
		}

		private static (Assembly, Type[])[] GetAllAssemblyTypes()
		{
			// TODO: look into optimizing using UnityEditor.TypeCache

			if(_cachedTypes == null)
			{
				List<(Assembly, Type[])> filteredAssemblies = new();

				foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!a.IsUserRelevant())
					{
						continue;
					}

					var types = a.GetTypes();
					if (types.Length == 0)
					{
						continue;
					}
					filteredAssemblies.Add((a, types));
				}

				_cachedTypes = new (Assembly, Type[])[filteredAssemblies.Count];

				for (var i = 0; i < filteredAssemblies.Count; i++)
				{
					_cachedTypes[i] = (filteredAssemblies[i].Item1, filteredAssemblies[i].Item2);
				}
			}
			return _cachedTypes;
		}

		private static bool HasItem(in string[] arr, string name)
		{
			return Array.FindIndex(arr, s => s.StartsWith(name)) > -1;
		}

		private static string GetTypeCategory(Type t)
		{
			foreach (var (cat, fn) in _TYPE_CATEGORIES)
			{
				if (fn.Invoke(t))
				{
					return cat;
				}
			}
			return null;
		}

		private static string GetFilterableName(Type t)
		{
			return (t.FullName ?? t.Name).ToLower();
		}

		private static bool CheckFlagFilters(Type t, ESearchTypeFlags flags)
		{
			foreach (var (flag, filter) in _FLAG_FILTERS)
			{
				if (filter.Invoke(t) && !flags.HasFlag(flag))
				{
					return false;
				}
			}
			return true;
		}

		private static MenuNode CreateMenuTree(in Options opts, Type currentValue, out MenuNode currentNode, out string longestLabel)
		{
			var root = new MenuNode
			{
				name = "Type"
			};

			currentNode = null;

			longestLabel = string.Empty;
			
			var weirdTypes = 0;

			foreach (var (assembly, aTypes) in GetAllAssemblyTypes())
			{
				if (!opts.flags.HasFlag(ESearchTypeFlags.EditorAssembly) && assembly.IsEditorAssembly())
				{
					continue;
				}

				if (opts.assemblies != null && !HasItem(opts.assemblies, assembly.GetName().Name))
				{
					continue;
				}

				foreach (var aType in aTypes)
				{
					if (aType.IsCompilerGenerated())
					{
						continue;
					}

					if (!aType.IsUserRelevant())
					{
						continue;
					}
					
					if (!opts.flags.HasFlag(ESearchTypeFlags.EditorAssembly) && aType.IsEditorType())
					{
						continue;
					}

					// misc generated for unity
					if (aType.FullName.StartsWith("UnitySource"))
					{
						continue;
					}

					if (opts.customFilter != null && !opts.customFilter.Invoke(aType))
					{
						continue;
					}

					if (opts.flags.HasFlag(ESearchTypeFlags.Serializable) && !aType.IsSerializable())
					{
						continue;
					}

					if (!opts.flags.HasFlag(ESearchTypeFlags.Obsolete) && aType.IsObsolete())
					{
						continue;
					}

					if (!CheckFlagFilters(aType, opts.flags))
					{
						continue;
					}

					if (opts.namespaces != null && !HasItem(opts.namespaces, aType.Namespace))
					{
						continue;
					}

					if (opts.baseTypes != null && !aType.DerivesFromAny(opts.baseTypes))
					{
						continue;
					}

					string catName = GetTypeCategory(aType);

					var tLabel = aType.Name;

					if (opts.useDisplayName)
					{
						var dn = aType.GetCustomAttribute<DisplayNameAttribute>();
						if (dn != null)
						{
							tLabel = dn.DisplayName;
						}
					}

					if (aType.IsNested)
					{
						tLabel = aType.FullName ?? tLabel;
						if (!string.IsNullOrEmpty(aType.Namespace))
						{
							tLabel = tLabel.Substring(aType.Namespace.Length + 1);
						}
					}

					if (longestLabel.Length < tLabel.Length)
					{
						longestLabel = tLabel;
					}

					var namespacePrefix = string.IsNullOrEmpty(aType.Namespace)
					? $"(No Namespace).{aType.Assembly.GetName().Name}"
					: aType.Namespace;

					var nodePathString =
					namespacePrefix
					+ (catName != null ? $".{catName}." : ".")
					+ tLabel;

					var nodePath = nodePathString.Split('.');

					if (nodePath.Length == 1)
					{
						nodePath = new [] { ".", tLabel };
					}

					if (aType.IsNested) // nb, kinda shit
					{
						for (int i = 0; i < nodePath.Length; i++)
						{
							nodePath[i] = nodePath[i].Replace("+", ".");
						}
					}
					var cn = root;

					foreach (var name in nodePath)
					{
						cn = cn.FindChildOrNew(name);
					}
					cn.leafValue = aType;
					if (aType == currentValue)
					{
						currentNode = cn;
					}
					cn.filterName = GetFilterableName(aType);
				}
			}
			
#if SM_DEV
			Debug.Log(weirdTypes);
#endif

			root.Sort();
			root.RefreshCount();
			root.Filter(SearchFilter.Empty);
			return root;
		}

		private static bool DrawHeader(Rect pos, MenuNode node, bool flatMode)
		{
			var sepRect = pos;
			sepRect = sepRect.SliceBottom(_SEP_WIDTH);
			EditorGUI.DrawRect(pos, _HEADER_COLOR);

			var hoverRect = pos;

			bool isRoot = node.parent == null;

			var headerLabel = flatMode ? _SEARCH_LABEL : node.name;

			if (!isRoot) // not a root node
			{
				var icoRect = pos.SliceLeft(pos.height);
				icoRect = icoRect.Resized(-icoRect.height * 0.6f);
				DrawIcon(icoRect, _COORDS_ARROWL, _ARROW_COLOR);
			}

			if (hoverRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(hoverRect, _HEADER_HOVER_COLOR);
			}

			EditorGUI.LabelField(hoverRect, headerLabel, PopupStyles.HeaderLabel);
			
			EditorGUI.DrawRect(sepRect, _SEP_COLOR);
			
			return !isRoot && GUI.Button(hoverRect, string.Empty, GUIStyle.none);
		}
		
		private static void DrawBreadcrumbs(Rect pos, MenuNode node)
		{
			var sepRect = pos;
			sepRect = sepRect.SliceBottom(_SEP_WIDTH);
			
			EditorGUI.DrawRect(pos, _HEADER_COLOR * 0.8f);
			var tColor = GUI.color;
			GUI.color = Color.white * 0.9f;
			EditorGUI.LabelField(pos, node.GetBreadcrumbLabel(), PopupStyles.Breadcrumbs);
			GUI.color = tColor;
			EditorGUI.DrawRect(sepRect, _SEP_COLOR);
		}

		private static bool DrawItemRow(Rect pos, in string label, bool leaf, bool active)
		{
			var hoverRect = pos;

			if (active)
			{
				EditorGUI.DrawRect(pos, _ACTIVE_ITEM_COLOR);
			}

			var hovered = hoverRect.Contains(Event.current.mousePosition);

			if (hovered)
			{
				EditorGUI.DrawRect(hoverRect, _HOVER_COLOR);
			}
			if (!leaf)
			{
				var icoRect = pos.SliceRight(pos.height);
				icoRect = icoRect.Resized(-icoRect.height * 0.4f);
				DrawIcon(icoRect, _COORDS_ARROWR, _ARROW_COLOR);
			}

			var style = label.StartsWith("#")
			? PopupStyles.CategoryLabel
			: PopupStyles.ItemLabel;

			var tColor = style.normal.textColor;
			style.normal.textColor = hovered ? Color.white : style.normal.textColor;
			EditorGUI.LabelField(pos, label, style);
			style.normal.textColor = tColor;
			
			return GUI.Button(pos, GUIContent.none, GUIStyle.none);
		}

		private void DrawNodeFlat(Rect pos, MenuNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;

			if (node.leafValue != null)
			{
				var itemRect = pos.SliceTop(rowHeight);
				if (DrawItemRow(itemRect, node.name, true, _currentValue != null && node.leafValue == _currentValue))
				{
					Select(node.leafValue);
				}
			}

			foreach (var cNode in node.children)
			{
				if (cNode.filterCountRecursive > 0)
				{
					DrawNodeFlat(pos.SliceTop(rowHeight * cNode.filterCountRecursive), cNode);
				}
			}
		}

		private void DrawNodeLinked(Rect pos, MenuNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;
			foreach (var page in node.children)
			{
				if (page.filteredCount == 0)
				{
					continue;
				}
				var rowRect = pos.SliceTop(rowHeight);
				if (page.children.Count == 0 && page.leafValue == null)
				{
					continue;
				}
				if (DrawItemRow(rowRect, page.name, page.leafValue != null, _currentValue != null && page.leafValue == _currentValue))
				{
					if (page.leafValue == null)
					{
						_pageScroll = default;
						_currentNode = page;
					}
					else
					{
						Select(page.leafValue);
					}
				}
			}
		}

		private void DrawNode(Rect pos, MenuNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;
			var rowCount = node.filteredCount;

			var flatMode = _flatMode && node.filterCountRecursive < _MAX_FLAT_RESULTS;

			if (node.count < _MAX_FLAT_RESULTS)
			{
				// flatMode = true;
			}
			
			if (flatMode)
			{
				rowCount = node.filterCountRecursive;
			}

			MenuNode newPage = null;

			var headerHeight = PopupStyles.HeaderHeight;
			
			if (DrawHeader(pos.SliceTop(headerHeight), node, flatMode))
			{
				newPage = node.parent;
			}

			if (!flatMode)
			{
				DrawBreadcrumbs(pos.SliceTop(PopupStyles.BreadcrumbHeight), node);
			}

			var scrollRect = new Rect(0f, 0f, pos.width, rowCount * rowHeight);

			if (pos.height < scrollRect.height)
			{
				scrollRect.width -= PopupStyles.ScrollbarWidth;
			}

			_pageScroll = GUI.BeginScrollView(pos, _pageScroll, scrollRect);

			if (flatMode)
			{
				DrawNodeFlat(scrollRect, node);
			}
			else
			{
				DrawNodeLinked(scrollRect, node);
			}

			GUI.EndScrollView();
			if (newPage != null)
			{
				_currentNode = newPage;
			}
		}
		
		private static class PopupStyles
		{
			public static GUIStyle HeaderLabel => _HEADER_STYLE.Value;
			public static GUIStyle ItemLabel => _ITEM_STYLE.Value;
			public static GUIStyle CategoryLabel => _CAT_ITEM_STYLE.Value;
			public static GUIStyle Breadcrumbs => _BREADCRUMB_STYLE.Value;
			public static float ScrollbarWidth => _SCROLLBAR_W.Value;
			public static float HeaderHeight => _HEADER_STYLE.Value.CalcHeight(GUIContent.none, 100);
			public static float ItemHeight => ItemLabel.CalcHeight(GUIContent.none, 100);
			public static float BreadcrumbHeight => Breadcrumbs.CalcHeight(GUIContent.none, 100);

			private static readonly Lazy<float> _SCROLLBAR_W =
			new(() => GUI.skin.verticalScrollbar.CalcSize(GUIContent.none).x);
			
			private static readonly Lazy<GUIStyle> _BREADCRUMB_STYLE =
			new (() => new GUIStyle(EditorStyles.miniLabel)
			{
				padding = new RectOffset(5,5,5,5),
				fontSize = (int)(EditorStyles.miniLabel.fontSize * 0.95f),
			});
		
			private static readonly Lazy<GUIStyle> _ITEM_STYLE =
				new (() => new GUIStyle(EditorStyles.miniLabel)
				{
					padding = new RectOffset(6,3,3,3)
				});
			
			private static readonly Lazy<GUIStyle> _CAT_ITEM_STYLE =
			new (() => new GUIStyle(EditorStyles.miniLabel)
			{
				padding = new RectOffset(6,3,3,3),
				fontStyle = FontStyle.Bold
			});

			private static readonly Lazy<GUIStyle> _HEADER_STYLE = new (() => new GUIStyle(EditorStyles.label)
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(5,5,4,4)
			});
		}
	}
}

#endif