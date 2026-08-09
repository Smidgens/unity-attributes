// smidgens @ github

/*
 * TODO
 * - filter bar
 * Maybe
 * - show namespace breadcrumbs
 * - show currently selected type
 */

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
			public bool searchbar;
			public ESearchTypeFlags flags;
			public string[] assemblies;
			public string[] namespaces;
			public Type[] baseTypes;
		}

		private static readonly float _MIN_WIDTH = Screen.width * 0.4f;
		private static readonly float _MAX_HEIGHT = Screen.height * 0.25f;
		private const int _MIN_SEARCH_LEN = 3;

		private static readonly Color _HOVER_COLOR = new Color(0.2392157f, 0.3764706f, 0.5686275f) * 0.9f;
		private static readonly Color _HEADER_HOVER_COLOR = Color.white * 0.25f;

		private static readonly Color _HEADER_COLOR = EditorGUIUtility.isProSkin
		? Color.black * 0.3f
		: Color.black * 0.1f;

		public static void Open
		(
			in Rect pos,
			Type value,
			Options options,
			Action<Type> setFn
		)
		{
			var p = new TypeSearch(value, options, setFn)
			{
				_preferredWidth = pos.width
			};
			PopupWindow.Show(pos, p);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(Mathf.Max(_MIN_WIDTH, _preferredWidth), _MAX_HEIGHT);
		}

		private static readonly float _SEARCH_PAD = EditorGUIUtility.singleLineHeight * 0.3f;

		public override void OnGUI(Rect rect)
		{
			if (_options.searchbar)
			{
				var searchbarHeight = EditorStyles.toolbarSearchField.CalcHeight(GUIContent.none, 10);
				var searchHeight = searchbarHeight + _SEARCH_PAD * 2f;
				DrawSearchField(rect.SliceTop(searchHeight));

				if (!_focused)
				{
					GUI.FocusControl(_SEARCH_FIELD_NAME);
					_focused = true;
				}
			}

			if (_refreshTree && EditorApplication.timeSinceStartup > (_timeSearched + 0.2f))
			{
				_flatMode = _filterString.Length >= _MIN_SEARCH_LEN;
				_refreshTree = false;
				_currentPage.Filter(_filterString.ToLower());
				
				_currentPage = _rootNode;
			}

			DrawNode(rect, _currentPage);
			editorWindow.Repaint();
		}

		private bool _focused;

		private readonly Options _options;
		private readonly Action<Type> _setFn;
		private float _preferredWidth = 1f;
		private TypeNode _currentPage;
		private readonly TypeNode _rootNode;
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
			(ESearchTypeFlags.Static, t => t.IsClass && t.IsStatic()),
			(ESearchTypeFlags.Interface, t => t.IsInterface),
			(ESearchTypeFlags.Abstract, t => t.IsAbstract && !t.IsInterface && !t.IsStatic()),
			(ESearchTypeFlags.Struct, t => t.IsStruct()),
			(ESearchTypeFlags.Enum, t => t.IsEnum),
			(ESearchTypeFlags.Hidden, t => !t.IsVisible),
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

		private const string _ATLAS_GUID = "e769e4d9f339626498a12b64168231ee";

		private const string _SEARCH_FIELD_NAME = "search_field";

		private static readonly Lazy<Texture2D> _TEX_ATLAS =
		new (() => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(_ATLAS_GUID)));

		private static readonly Rect _ARROWL_COORDS = new (0.5f, 0, 0.25f, 0.25f);
		private static readonly Rect _ARROWR_COORDS = new (0.75f, 0, 0.25f, 0.25f);

		private TypeSearch(Type currentValue, Options options, Action<Type> setFn)
		{
			_setFn = setFn;
			_options = options;
			_currentPage = CreateTypeTree(options, _filterString);
			_currentPage.RefreshCount();
			_rootNode = _currentPage;
		}

		private void Select(Type t)
		{
			_setFn?.Invoke(t);
			editorWindow.Close();
		}
		
		private void DrawSearchField(Rect rect)
		{
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
		}

		private static void DrawIcon(in Rect pos, in Rect coords, Color color)
		{
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, pos, coords, color);
		}

		private class TypeNode : IComparable<TypeNode>
		{
			public TypeNode parent;
			public string name;
			public Type type;
			public string filterName;
			public int filteredCount { get; private set; }
			public int count { get; private set; }
			public int filterCountRecursive { get; private set; }
			public readonly List<TypeNode> children = new ();

			public void RefreshCount()
			{
				GetCount();
			}

			private int GetCount()
			{
				int c = type != null ? 1 : 0;
				foreach (var child in children)
				{
					c += child.GetCount();
				}
				count = c;
				return c;
			}

			public bool Filter(string searchString)
			{
				filteredCount = 0;
				filterCountRecursive = 0;
				if (type != null && MatchFilter(filterName, searchString))
				{
					filteredCount++;
					filterCountRecursive++;
				}

				foreach (var c in children)
				{
					if (c.Filter(searchString))
					{
						filteredCount++;
						filterCountRecursive += c.filterCountRecursive;
					}
				}

				return filteredCount > 0;
			}

			public int CompareTo(TypeNode b)
			{
				int ret = (type != null) == (b.type != null)
				? 0
				: (type != null ? 1 : -1);
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

			public TypeNode FindChildOrNew(string nodeName)
			{
				var node = children.Find(x => x.name == nodeName);
				if (node == null)
				{
					node = new TypeNode
					{
						name = nodeName,
						parent = this
					};
					children.Add(node);
				}
				return node;
			}
		}

		private static (Assembly, Type[])[] GetAllAssemblyTypes()
		{
			if(_cachedTypes == null)
			{
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				_cachedTypes = new (Assembly, Type[])[assemblies.Length];
				for (var i = 0; i < assemblies.Length; i++)
				{
					_cachedTypes[i] = (assemblies[i], assemblies[i].GetTypes());
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

		private static bool MatchFilter(string text, string searchString)
		{
			if (searchString.Length < _MIN_SEARCH_LEN)
			{
				return true;
			}
			return text.Contains(searchString);
		}

		private static string GetFilterableName(Type t)
		{
			return (t.FullName ?? t.Name).ToLower();
		}

		private static TypeNode CreateTypeTree(in Options opts, string filterString = "")
		{
			var root = new TypeNode
			{
				name = "Type"
			};
			int weirdName = 0;

			foreach (var (assembly, aTypes) in GetAllAssemblyTypes())
			{
				if (opts.assemblies != null && !HasItem(opts.assemblies, assembly.GetName().Name))
				{
					continue;
				}

				foreach (var aType in aTypes)
				{
					if (aType.Name.StartsWith("<"))
					{
						weirdName++;
						continue;
					}

					if (aType.IsNested && aType.FullName != null && aType.FullName.StartsWith("<"))
					{
						weirdName++;
						continue;
					}
					
					var include = true;

					foreach (var (flag, filter) in _FLAG_FILTERS)
					{
						if (filter.Invoke(aType) && !opts.flags.HasFlag(flag))
						{
							include = false;
							break;
						}
					}

					if (!include)
					{
						continue;
					}

					if (opts.namespaces != null && !HasItem(opts.namespaces, aType.Namespace))
					{
						continue;
					}

					if (opts.baseTypes != null && !aType.DerivesFrom(opts.baseTypes))
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

					var namespacePrefix = string.IsNullOrEmpty(aType.Namespace)
					? $"(No Namespace).{aType.Assembly.GetName().Name}"
					: aType.Namespace;

					var pagePathString =
					namespacePrefix
					+ (catName != null ? $".{catName}." : ".")
					+ tLabel;

					var pagePath = pagePathString.Split('.');

					if (pagePath.Length == 1)
					{
						pagePath = new [] { ".", tLabel };
					}
					
					if (aType.IsNested) // nb, kinda shit
					{
						for (int i = 0; i < pagePath.Length; i++)
						{
							pagePath[i] = pagePath[i].Replace("+", ".");
						}
					}
					var cn = root;
					foreach (var name in pagePath)
					{
						cn = cn.FindChildOrNew(name);
					}
					cn.type = aType;
					cn.filterName = GetFilterableName(aType);
				}
			}
			
			root.Sort();
			root.Filter("");
			return root;
		}

		private static bool DrawHeader(Rect pos, in string label, bool root = false)
		{
			EditorGUI.DrawRect(pos, _HEADER_COLOR);

			var hoverRect = pos;
			
			if (!root)
			{
				var icoRect = pos.SliceLeft(pos.height);
				icoRect = icoRect.Resized(-icoRect.height * 0.4f);
				DrawIcon(icoRect, _ARROWL_COORDS, Color.white * 0.5f);
			}

			if (hoverRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(hoverRect, _HEADER_HOVER_COLOR);
			}

			EditorGUI.LabelField(hoverRect, label, PopupStyles.HeaderLabel);
			return !root && GUI.Button(hoverRect, string.Empty, GUIStyle.none);
		}

		private static bool DrawItemRow(Rect pos, in string label, bool leaf = false)
		{
			var hoverRect = pos;

			if (hoverRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(hoverRect, _HOVER_COLOR);
			}
			if (!leaf)
			{
				var icoRect = pos.SliceRight(pos.height);
				icoRect = icoRect.Resized(-icoRect.height * 0.4f);
				DrawIcon(icoRect, _ARROWR_COORDS, Color.white * 0.5f);
			}
			
			EditorGUI.LabelField(pos, label, PopupStyles.ItemLabel);
			return GUI.Button(pos, "", GUIStyle.none);
		}

		private void DrawNodeFlat(Rect pos, TypeNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;

			if (node.type != null)
			{
				var itemRect = pos.SliceTop(rowHeight);
				if (DrawItemRow(itemRect, node.name, true))
				{
					Select(node.type);
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

		private void DrawNodeLinked(Rect pos, TypeNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;
			foreach (var page in node.children)
			{
				if (page.filteredCount == 0)
				{
					continue;
				}
				var rowRect = pos.SliceTop(rowHeight);
				if (page.children.Count == 0 && page.type == null)
				{
					continue;
				}
				if (DrawItemRow(rowRect, page.name, page.type != null))
				{
					if (page.type == null)
					{
						_pageScroll = default;
						_currentPage = page;
					}
					else
					{
						Select(page.type);
					}
				}
			}
		}


		private void DrawNode(Rect pos, TypeNode node)
		{
			var rowHeight = PopupStyles.ItemHeight;
			var rowCount = node.filteredCount;

			var flatMode = _flatMode && node.filterCountRecursive < _MAX_FLAT_RESULTS;

			if (node.count < _MAX_FLAT_RESULTS)
			{
				flatMode = true;
			}
			
			if (flatMode)
			{
				rowCount = node.filterCountRecursive;
			}

			TypeNode newPage = null;

			var headerLabel = _flatMode ? _SEARCH_LABEL : node.name;

			var headerHeight = PopupStyles.HeaderHeight;
			
			if (DrawHeader(pos.SliceTop(headerHeight), headerLabel, node.parent == null))
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
				_currentPage = newPage;
			}
		}
		
		private static class PopupStyles
		{
			public static GUIStyle HeaderLabel => _HEADER_STYLE.Value;
			public static GUIStyle ItemLabel => _ITEM_STYLE.Value;
			public static float ScrollbarWidth => _SCROLLBAR_W.Value;

			public static float HeaderHeight => _HEADER_STYLE.Value.CalcHeight(GUIContent.none, 100);
			public static float ItemHeight => ItemLabel.CalcHeight(GUIContent.none, 100);

			private static readonly Lazy<float> _SCROLLBAR_W =
				new(() => GUI.skin.verticalScrollbar.CalcSize(GUIContent.none).x);
		
			private static readonly Lazy<GUIStyle> _ITEM_STYLE =
				new (() => new GUIStyle(EditorStyles.miniLabel)
				{
					padding = new RectOffset(6,3,3,3)
				});

			private static readonly Lazy<GUIStyle> _HEADER_STYLE = new (() => new GUIStyle(EditorStyles.label)
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(5,5,7,7)
			});
		}
	}
}

#endif