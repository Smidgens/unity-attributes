// smidgens @ github

// ReSharper disable StaticMemberInGenericType
#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	internal sealed class SearchPopup<ST> : PopupWindowContent
	{
		public struct Options
		{
			public Func<ST, string> labelFn;
			public Func<ST, string> filterNameFn;
			public Func<ST, ST, bool> equalsFn;
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

				if (str == null)
				{
					return false;
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

		private static readonly Color _UNITY_SELECT_COLOR = new (0.24f, 0.5f, 0.874f);

		private static readonly Color _HOVER_COLOR = DrawerGUI.PickSkin
		(
			_UNITY_SELECT_COLOR.Fade(0.6f)
			,_UNITY_SELECT_COLOR.Fade(0.8f)
		);

		private static readonly Color _HEADER_HOVER_COLOR
		= DrawerGUI.PickSkin(Color.white * 0.25f, Color.white.Fade(0.2f));

		private static readonly Color _HEADER_COLOR
		= DrawerGUI.PickSkin(Color.black * 0.2f, Color.black * 0.1f);
		
		private static readonly Color _ARROW_COLOR
		= DrawerGUI.PickSkin(Color.white * 0.5f, Color.black * 0.5f);

		private static readonly Color _ACTIVE_ITEM_COLOR
		= DrawerGUI.PickSkin(_UNITY_SELECT_COLOR.Fade(0.3f), _UNITY_SELECT_COLOR.Fade(0.4f));
		
		private static readonly Color _SEP_COLOR
		= DrawerGUI.PickSkin(Color.white.Fade(0.05f), Color.black * 0.3f);
		
		private static readonly Color _TYPE_ICO_COLOR =
		DrawerGUI.PickSkin(Color.white.Fade(0.75f), Color.white.Fade(0.75f));

		private const string _SEARCH_FIELD_NAME = "search_field";
		private const float _SEP_WIDTH = 1f;
		private const float _SEARCH_REFRESH_DELAY = 0.2f;
		private static readonly float _SEARCH_PAD = EditorGUIUtility.singleLineHeight * 0.3f;

		public static SearchPopup<ST> Create
		(
			ST value,
			MenuNode menuTree,
			Action<ST> setFn
		)
		{
			return new SearchPopup<ST>(value, menuTree, setFn);
		}

		public void Show(in Rect pos)
		{
			PopupWindow.Show(pos, this);
		}

		public override Vector2 GetWindowSize()
		{
			var height = PopupStyles.HeaderHeight + PopupStyles.ItemHeight * 16f;
			return new Vector2(Mathf.Max(_MIN_WIDTH, _preferredWidth * 0.6f), height);
		}

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
		private readonly Action<ST> _setFn;
		private readonly float _preferredWidth;
		private readonly MenuNode _rootNode;
		private MenuNode _currentNode;
		private readonly ST _currentValue;
		private Vector2 _pageScroll;
		private bool _refreshFilter;
		private string _filterString = string.Empty;
		private double _lastTimeSearched;
		private bool _searchMode;
		private const string _SEARCH_LABEL = "Search";
		private const int _MAX_FLAT_RESULTS = 50; // how many results can be shown without categories when searching
		
		private SearchPopup(ST currentValue, MenuNode menuTree, Action<ST> setFn)
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

			if (currentValue != null && selectedNode != null && selectedNode.parent != null)
			{
				var sIndex = 0;
				for (var i = 0; i < selectedNode.parent.children.Count; i++)
				{
					var cNode = selectedNode.parent.children[i];
					if (cNode.filteredCount == 0)
					{
						continue;
					}
					if (cNode.Equals(currentValue))
					{
						break;
					}
					sIndex++;
				}
				_pageScroll = new Vector2(0f, PopupStyles.ItemHeight * sIndex);
			}
			_preferredWidth = 0f;
		}

		private void Select(ST t)
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

		internal class MenuNode : IComparable<MenuNode>
		{
			private MenuNode() {}

			public static MenuNode NewTree(string rName, in Options opts)
			{
				var n = new MenuNode
				{
					name = rName,
					_labelFn = opts.labelFn,
					// _compareFn = opts.compareFn,
					_equalsFn = opts.equalsFn,
					_filterNameFn = opts.filterNameFn
				};
				n.root = n;
				return n;
			}

			public string name { get; private set; }
			public MenuNode root  { get; private set; }
			public MenuNode parent  { get; private set; }
			public ST value  { get; private set; }
			public string filterName { get; private set; }
			public int filteredCount { get; private set; }
			public int count { get; private set; }
			public int filteredCountRecursive { get; private set; }

			public string GetDisplayLabel()
			{
				if (string.IsNullOrEmpty(_displayName))
				{
					if (value == null)
					{
						_displayName = name;
					}
					else
					{
						_displayName = root?._labelFn?.Invoke(value) ?? name;
					}
				}

				return _displayName;
			}

			private Func<ST, string> _labelFn;
			private Func<ST, ST, int> _compareFn;
			private Func<ST, ST, bool> _equalsFn;
			private Func<ST, string> _filterNameFn;
			public ReadOnlyCollection<MenuNode> children => _children.AsReadOnly();
			private readonly List<MenuNode> _children = new ();
			private string _cachedBreadcrumbs;
			private string _displayName;

			public MenuNode FindLeaf(ST lValue)
			{
				// if (lValue == null)
				// {
				// 	return null;
				// }
				
				

				// if (CompareTo(lValue) == 0)
				// {
				// 	return this;
				// }

				if (Equals(lValue))
				{
					return this;
				}

				// if (root._compareFn.Invoke(lValue, value) == 0)
				// {
				// 	return this;
				// }

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

			public bool Equals(ST otherValue)
			{
				return root._equalsFn?.Invoke(value, otherValue) ?? false;
			}

			public int CompareTo(MenuNode b)
			{
				var ac = children.Count;
				var bc = b.children.Count;
				var cmp = (ac == 0) == (bc == 0) ? 0 : (ac == 0 ? 1 : -1);
				return cmp != 0 ? cmp : string.CompareOrdinal(name, b.name);
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
				_cachedBreadcrumbs = string.Empty;
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

			public void AddValue(string path, ST lValue)
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
				cn.filterName = root._filterNameFn?.Invoke(lValue) ?? path;
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
						root = root,
					};
					_children.Add(node);
				}
				return node;
			}
		}

		internal static MenuNode CreateTypeMenuTree(string rootName, in Options opts)
		{
			var root = MenuNode.NewTree(rootName, opts);
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
				PluginAtlas.DrawIcon(icoRect, EAtlasIcon.ArrowLeft, _ARROW_COLOR);
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

		private static readonly Dictionary<string, EAtlasIcon> _TYPE_ICO_COORDS = new()
		{
			{ "Delegate", EAtlasIcon.Delegate },
			{ "Static", EAtlasIcon.Static },
			{ "Primitive", EAtlasIcon.Primitive },
			// row2
			{ "Attribute", EAtlasIcon.Attribute },
			{ "Exception", EAtlasIcon.Exception },
			{ "Enum", EAtlasIcon.Enum },
			// row 3
			{ "Interface", EAtlasIcon.Interface },
			{ "Struct", EAtlasIcon.Struct },
			{ "Class", EAtlasIcon.Class },
		};

		private static void DrawNodeIcon(Rect pos, string label)
		{
			pos = pos.Resized(-pos.height * 0.1f);
			var c = _TYPE_ICO_COLOR;
			var icon = _TYPE_ICO_COORDS.GetValueOrDefault(label, EAtlasIcon.Class);
			PluginAtlas.DrawIcon(pos, icon, c);
		}

		private static bool DrawItemRow(Rect pos, MenuNode node, bool leaf, bool active)
		{
			var label = node.GetDisplayLabel();
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
				PluginAtlas.DrawIcon(icoRect, EAtlasIcon.ArrowRight, _ARROW_COLOR);
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
				// var active = _currentValue != null && node.CompareTo(_currentValue) == 0;
				// var active = node.CompareTo((ST)default) != 0 && node.CompareTo(_currentValue) == 0;
				var active = !node.Equals(default) && node.Equals(_currentValue);
				if (DrawItemRow(itemRect, node, true, active))
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
			foreach (var childNode in node.children)
			{
				if (childNode.filteredCount == 0)
				{
					continue;
				}
				var rowRect = pos.SliceTop(rowHeight);
				if (childNode.children.Count == 0 && childNode.value == null)
				{
					continue;
				}
				var active = !childNode.Equals(default) && childNode.Equals(_currentValue);
				if (DrawItemRow(rowRect, childNode, childNode.value != null, active))
				{
					if (childNode.value == null)
					{
						_pageScroll = default;
						_currentNode = childNode;
					}
					else
					{
						Select(childNode.value);
					}
				}
			}
		}

		private void DrawNode(Rect pos, MenuNode node)
		{
			if (node == null)
			{
				return;
			}
			
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