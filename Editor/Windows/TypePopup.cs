// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Reflection;

	internal sealed class TypeSearch : PopupWindowContent
	{
		public struct Options
		{
			public ESearchType flags;
			public Type[] baseTypes;
			public Func<Type, bool> typeFilter;
			public Func<Assembly, bool> assemblyFilter;
			public Func<Type, string> labelFn;
		}

		internal readonly struct SearchFilter
		{
			public static SearchFilter Empty => new (string.Empty);
			
			public SearchFilter(string search)
			{
				search ??= string.Empty;
				var s = search.Trim().ToLower();
				len = s.Length;
				segments = s.Split(" ");
			}

			private readonly string[] segments;
			private readonly int len; // Don't call me Len you little #$%*!, I'm a BISHOP!

			public bool MatchString(string str)
			{
				if (len < _MIN_SEARCH_LEN)
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
				return false;
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

		private static readonly Color _UNITY_SELECT_COLOR = new (0.24f, 0.5f, 0.874f);

		private static readonly Color _HOVER_COLOR = PickSkin
		(
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
		= PickSkin(Fade(_UNITY_SELECT_COLOR, 0.3f), Fade(_UNITY_SELECT_COLOR,0.4f));
		
		private static readonly Color _SEP_COLOR
		= PickSkin(Fade(Color.white,0.05f), Color.black * 0.3f);
		
		private const string _ATLAS_GUID = "e769e4d9f339626498a12b64168231ee";
		private const string _SEARCH_FIELD_NAME = "search_field";

		private static readonly Lazy<Texture2D> _TEX_ATLAS =
		new (() => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(_ATLAS_GUID)));

		private static readonly Rect _COORDS_ARROWL = new (0.5f, 0, 0.25f, 0.25f);
		private static readonly Rect _COORDS_ARROWR = new (0.75f, 0, 0.25f, 0.25f);
		private static readonly Rect _COORDS_CLOSE = new (0.25f, 0, 0.25f, 0.25f);
	
		private const float _SEP_WIDTH = 1f;

		private const float _SEARCH_REFRESH_DELAY = 0.2f;

		public static void Open
		(
			in Rect pos,
			Type value,
			MenuNode menuTree,
			Action<Type> setFn
		)
		{
			var p = new TypeSearch(value, menuTree, setFn);
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

			if (_refreshFilter && EditorApplication.timeSinceStartup > (_lastTimeSearched + _SEARCH_REFRESH_DELAY))
			{
				_searchMode = _filterString.Length >= _MIN_SEARCH_LEN;
				_refreshFilter = false;
				_currentNode.Filter(new SearchFilter(_filterString));
				_currentNode = _rootNode;
			}

			if (_rootNode.count < _MAX_FLAT_RESULTS)
			{
				_searchMode = true;
			}

			DrawNode(rect, _currentNode);
			editorWindow.Repaint();
		}

		private bool _searchFocused;
		private readonly Action<Type> _setFn;
		private readonly float _preferredWidth;
		private readonly MenuNode _rootNode;
		private MenuNode _currentNode;
		private readonly Type _currentValue;
		private static (Assembly, Type[])[] _cachedTypes;
		private Vector2 _pageScroll;
		private bool _refreshFilter;
		private string _filterString = string.Empty;
		private double _lastTimeSearched;
		private bool _searchMode;
		private const string _SEARCH_LABEL = "Search";
		private const int _MAX_FLAT_RESULTS = 50; // how many results can be shown without categories when searching
		
		// used to match types with enum flags
		private static readonly (ESearchType, Func<Type, bool>)[] _FLAG_FILTERS =
		{
			(ESearchType.Static, t => t.IsStaticClass()),
			(ESearchType.Delegate, t => t.IsDelegate()),
			(ESearchType.Interface, t => t.IsInterface && !t.IsClass),
			(ESearchType.Abstract, t => t.IsAbstract && !t.IsInterface && !t.IsStaticClass()),
			(ESearchType.Struct, t => t.IsStruct()),
			(ESearchType.Enum, t => t.IsEnum),
			(ESearchType.NonPublic, t => !t.IsVisible),
			(ESearchType.Nested, t => t.IsNested),
			(ESearchType.Class, t => t.IsClass && !t.IsStaticClass()),
			(ESearchType.Primitive, t => t.IsPrimitive && !t.IsEnum),
			(ESearchType.Generic, t => t.IsGenericTypeDefinition)
		};

		// categorize types
		private static readonly (string, Func<Type, bool>)[] _TYPE_CATEGORIES =
		{
			("# Enum", t => t.IsEnum),
			("# Exception", t => t.IsException()),
			("# Attribute", t => t.IsAttribute()),
			("# Delegate", t => t.IsDelegate()),
			("# Static", t => t.IsStaticClass()),
			("# Class", t => t.IsClass),
			("# Struct", t => t.IsStruct()),
			("# Interface", t => t.IsInterface),
			("# Primitive", t => t.IsPrimitive),
		};

		private TypeSearch(Type currentValue, MenuNode menuTree, Action<Type> setFn)
		{
			_setFn = setFn;
			_rootNode = menuTree;
			_currentNode = _rootNode;
			_currentValue = currentValue;

			var selectedNode = _rootNode.FindLeaf(currentValue);

			if (selectedNode != null && _rootNode.count >= _MAX_FLAT_RESULTS)
			{
				_currentNode = selectedNode.parent;
			}

			if (currentValue != null && selectedNode != null)
			{
				var sIndex = 0;
				for (var i = 0; i < selectedNode.parent.children.Count; i++)
				{
					var cNode = selectedNode.parent.children[i];
					if (cNode.filteredCount == 0)
					{
						continue;
					}

					if (cNode.value == currentValue)
					{
						break;
					}
					sIndex++;
				}
				_pageScroll = new Vector2(0f, PopupStyles.ItemHeight * sIndex);
			}
			_preferredWidth = 0f;
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

			var inner = rect.Resized(-_SEARCH_PAD);

			var oldVal = _filterString;

			GUI.SetNextControlName(_SEARCH_FIELD_NAME);
			var newVal = EditorGUI.TextField(inner, GUIContent.none, _filterString, EditorStyles.toolbarSearchField);
	
			if (newVal != _filterString)
			{
				_filterString = newVal;
	
				if (_filterString.Length >= _MIN_SEARCH_LEN || newVal.Length < _MIN_SEARCH_LEN && oldVal.Length >= _MIN_SEARCH_LEN)
				{
					_refreshFilter = true;
					_lastTimeSearched = EditorApplication.timeSinceStartup;
				}
			}

			EditorGUI.DrawRect(sepRect, _SEP_COLOR);
		}

		private static void DrawIcon(in Rect pos, in Rect coords, Color color)
		{
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, pos, coords, color);
		}

		internal class MenuNode : IComparable<MenuNode>
		{
			private MenuNode() {}

			public static MenuNode NewTree(string rName)
			{
				return new MenuNode
				{
					name = rName
				};
			}

			public string name { get; private set; }
			public MenuNode parent  { get; private set; }
			public Type value  { get; private set; }
			public string filterName { get; private set; }
			public int filteredCount { get; private set; }
			public int count { get; private set; }
			public int filteredCountRecursive { get; private set; }
			public ReadOnlyCollection<MenuNode> children => _children.AsReadOnly();

			private readonly List<MenuNode> _children = new ();
			private string _cachedBreadcrumbs;

			public MenuNode FindLeaf(Type lValue)
			{
				if (lValue == null)
				{
					return null;
				}

				if (lValue == value)
				{
					return this;
				}

				if (children.Count == 0)
				{
					return null;
				}

				foreach (var cNode in children)
				{
					var cResult = cNode.FindLeaf(lValue);
					if (cResult != null)
					{
						return cResult;
					}
				}
				return null;
			}

			public void BuildIndex()
			{
				Sort();
				GetCount();
				Filter(SearchFilter.Empty);
			}

			private int GetCount()
			{
				int c = value != null ? 1 : 0;
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
				filteredCountRecursive = 0;
				if (value != null && args.MatchString(filterName))
				{
					filteredCount++;
					filteredCountRecursive++;
				}

				foreach (var c in children)
				{
					if (c.Filter(args))
					{
						filteredCount++;
						filteredCountRecursive += c.filteredCountRecursive;
					}
				}
				return filteredCount > 0;
			}

			private static int CompareValues(Type a, Type b)
			{
				var ret = (a != null) == (b != null) ? 0 : (a != null ? 1 : -1);
				return ret;
			}

			public int CompareTo(MenuNode b)
			{
				int vCmp = CompareValues(value, b.value);
				return vCmp != 0 ? vCmp : string.CompareOrdinal(name, b.name);
			}

			private void Sort()
			{
				_children.Sort();
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

			public void AddValue(string path, Type lValue)
			{
				var cn = this;
				var i = 0;
				var c = 0;
				var si = 0;

				foreach (var ch in path)
				{
					if (ch != '.')
					{
						c++;
					}
					else
					{
						var sub = path.Substring(si, c);
						cn = cn.GetChildOrNew(sub);
						c = 0;
						si = i + 1;
					}
					i++;
				}

				cn = cn.GetChildOrNew(path.Substring(si, c));

				cn.value = lValue;

				if (lValue.IsNested)
				{
					cn.name = cn.name.Replace('+', '.');
				}
				cn.filterName = GetFilterableName(lValue);
			}

			private MenuNode GetChildOrNew(string nodeName)
			{
				var node = _children.Find(x => x.name == nodeName);
				if (node == null)
				{
					node = new MenuNode
					{
						name = nodeName,
						parent = this,
					};
					_children.Add(node);
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

		private static bool CheckFlagFilters(Type t, ESearchType flags)
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

		private static bool FilterAssembly(Assembly assembly, in Options opts)
		{
			if (!opts.flags.HasFlag(ESearchType.EditorAssembly) && assembly.IsEditorAssembly())
			{
				return false;
			}

			if (opts.assemblyFilter != null && !opts.assemblyFilter.Invoke(assembly))
			{
				return false;
			}
			return true;
		}

		private static bool FilterType(Type aType, in Options opts)
		{
			if (aType.IsCompilerGenerated())
			{
				return false;
			}

			if (!aType.IsUserRelevant())
			{
				return false;
			}

			if (!opts.flags.HasFlag(ESearchType.EditorAssembly) && aType.IsEditorType())
			{
				return false;
			}

			if (opts.flags.HasFlag(ESearchType.Serializable) && !aType.IsSerializable())
			{
				return false;
			}
		
			if (!opts.flags.HasFlag(ESearchType.Obsolete) && aType.IsObsolete())
			{
				return false;
			}

			if (!CheckFlagFilters(aType, opts.flags))
			{
				return false;
			}

			if (opts.flags.HasFlag(ESearchType.Newable) && aType.IsClass && !aType.IsNewable())
			{
				return false;
			}

			foreach (var (flag, filter) in _FLAG_FILTERS)
			{
				if (filter.Invoke(aType) && !opts.flags.HasFlag(flag))
				{
					return false;
				}
			}

			if (opts.baseTypes != null && !aType.DerivesFromAny(opts.baseTypes))
			{
				return false;
			}

			if (opts.typeFilter != null && !opts.typeFilter.Invoke(aType))
			{
				return false;
			}

			return true;
		}

		private static string GetMenuPath(Type aType, Func<Type,string> labelFn)
		{
			var tLabel = aType.Name;
			
			if (aType.IsNested)
			{
				tLabel = aType.FullName ?? tLabel;
				if (!string.IsNullOrEmpty(aType.Namespace))
				{
					tLabel = tLabel.Substring(aType.Namespace.Length + 1);
				}
			}

			if (labelFn != null)
			{
				tLabel = labelFn.Invoke(aType) ?? tLabel;
			}

			var categoryName = GetTypeCategory(aType);
			var assemblyName = aType.Assembly.GetName().Name;
			var namespacePrefix = string.IsNullOrEmpty(aType.Namespace)
			? $"(No Namespace).{assemblyName}"
			: aType.Namespace;

			return namespacePrefix
			+ (categoryName != null ? $".{categoryName}." : ".")
			+ tLabel;
		}

		internal static MenuNode CreateTypeMenuTree(in Options opts)
		{
			var root = MenuNode.NewTree("Type");

			// slight optimization, use unity's cache for specific base types
			if (opts.baseTypes != null)
			{
				foreach (var bt in opts.baseTypes)
				{
					foreach (var aType in TypeCache.GetTypesDerivedFrom(bt))
					{
						if (!FilterAssembly(aType.Assembly, opts))
						{
							continue;
						}
						if (!FilterType(aType, opts))
						{
							continue;
						}
						var nodePath = GetMenuPath(aType, opts.labelFn);
						root.AddValue(nodePath, aType);
					}
				}
			}
			else
			{
				foreach (var (assembly, aTypes) in GetAllAssemblyTypes())
				{
					if (!FilterAssembly(assembly, opts))
					{
						continue;
					}

					foreach (var aType in aTypes)
					{
						if (!FilterType(aType, opts))
						{
							continue;
						}
						var nodePath = GetMenuPath(aType, opts.labelFn);
						root.AddValue(nodePath, aType);
					}
				}
			}
			root.BuildIndex();
			return root;
		}

		private static bool DrawHeader(Rect pos, MenuNode node, bool flatMode)
		{
			var sepRect = pos;
			sepRect = sepRect.SliceBottom(_SEP_WIDTH);
			EditorGUI.DrawRect(pos, _HEADER_COLOR);

			var hoverRect = pos;

			bool isRoot = node.parent == null;

			var label = node.name;

			if (!flatMode && label.Length >= 2 && label[0] == '#' && label[1] == ' ')
			{
				label = label.Substring(2);
			}

			var headerLabel = flatMode ? _SEARCH_LABEL : label;

			if (!isRoot) // not a root node
			{
				var icoRect = pos.SliceLeft(pos.height);
				icoRect = icoRect.Resized(-icoRect.height * 0.2f);
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

		private static readonly Color _TYPE_ICO_COLOR =
		PickSkin(Fade(Color.white, 0.75f), Fade(Color.black, 0.75f));

		private static readonly Rect _COORDS_CLASS = new (0.25f, 0.5f, 0.125f, 0.125f);

		private static readonly Dictionary<string, Rect> _TYPE_ICO_COORDS = new()
		{
			{ "Delegate", new (0, 0.25f, 0.125f, 0.125f) },
			{ "Static", new (0.125f, 0.25f, 0.125f, 0.125f) },
			{ "Primitive", new (0.25f, 0.25f, 0.125f, 0.125f) },
			// row2
			{ "Attribute", new (0, 0.375f, 0.125f, 0.125f) },
			{ "Exception", new (0.125f, 0.375f, 0.125f, 0.125f) },
			{ "Enum", new (0.25f, 0.375f, 0.125f, 0.125f) },
			// row 3
			{ "Interface", new (0, 0.5f, 0.125f, 0.125f) },
			{ "Struct", new (0.125f, 0.5f, 0.125f, 0.125f) },
			{ "Class", new (0.25f, 0.5f, 0.125f, 0.125f) },
		};
		
		private static void DrawNodeIcon(Rect pos, string label)
		{
			pos = pos.Resized(-pos.height * 0.1f);
			var c = _TYPE_ICO_COLOR;
			var coords = _TYPE_ICO_COORDS.GetValueOrDefault(label, _COORDS_CLASS);
			DrawIcon(pos, coords, c);
		}

		private static bool DrawItemRow(Rect pos, string label, bool leaf, bool active)
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
				icoRect = icoRect.Resized(-icoRect.height * 0.2f);
				DrawIcon(icoRect, _COORDS_ARROWR, _ARROW_COLOR);
			}

			var categoryNode = label.Length >= 2 && label[0] == '#' && label[1] == ' ';

			var dLabel = categoryNode ? label.Substring(2) : label;

			var style = categoryNode
			? PopupStyles.CategoryLabel
			: PopupStyles.ItemLabel;

			if (categoryNode)
			{
				var catIcoRect = pos.SliceLeft(pos.height);
				DrawNodeIcon(catIcoRect, dLabel);

			}

			var tColor = style.normal.textColor;
			style.normal.textColor = hovered ? Color.white : style.normal.textColor;
			EditorGUI.LabelField(pos, dLabel, style);
			style.normal.textColor = tColor;
			
			return GUI.Button(pos, GUIContent.none, GUIStyle.none);
		}

		private void DrawNodeFlat(Rect pos, MenuNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;

			if (node.value != null)
			{
				var itemRect = pos.SliceTop(rowHeight);
				if (DrawItemRow(itemRect, node.name, true, _currentValue != null && node.value == _currentValue))
				{
					Select(node.value);
				}
			}

			foreach (var cNode in node.children)
			{
				if (cNode.filteredCountRecursive > 0)
				{
					DrawNodeFlat(pos.SliceTop(rowHeight * cNode.filteredCountRecursive), cNode);
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
				if (page.children.Count == 0 && page.value == null)
				{
					continue;
				}
				if (DrawItemRow(rowRect, page.name, page.value != null, _currentValue != null && page.value == _currentValue))
				{
					if (page.value == null)
					{
						_pageScroll = default;
						_currentNode = page;
					}
					else
					{
						Select(page.value);
					}
				}
			}
		}

		private void DrawNode(Rect pos, MenuNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;
			var rowCount = node.filteredCount;

			var flatMode = _searchMode && node.filteredCountRecursive < _MAX_FLAT_RESULTS;

			if (node.count < _MAX_FLAT_RESULTS)
			{
				// flatMode = true;
			}
			
			if (flatMode)
			{
				rowCount = node.filteredCountRecursive;
			}

			MenuNode newPage = null;

			var headerHeight = PopupStyles.HeaderHeight;
			
			if (!flatMode)
			{
				DrawBreadcrumbs(pos.SliceTop(PopupStyles.BreadcrumbHeight), node);
			}
			
			if (DrawHeader(pos.SliceTop(headerHeight), node, flatMode))
			{
				newPage = node.parent;
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
			public static float HeaderHeight => _HEADER_H.Value;
			public static float ItemHeight => _ITEM_H.Value;
			public static float BreadcrumbHeight => _BREADCRUMB_H.Value;
			
			private static readonly Lazy<float> _ITEM_H = 	new(() => ItemLabel.CalcHeight(GUIContent.none, 100));
			private static readonly Lazy<float> _HEADER_H = new(() => HeaderLabel.CalcHeight(GUIContent.none, 100));
			private static readonly Lazy<float> _BREADCRUMB_H = new(() => Breadcrumbs.CalcHeight(GUIContent.none, 100));
			private static readonly Lazy<float> _SCROLLBAR_W = new(() => GUI.skin.verticalScrollbar.CalcSize(GUIContent.none).x);
			
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
				padding = new RectOffset(1,3,3,3),
				fontSize = (int)(ItemLabel.fontSize * 1.1f),
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