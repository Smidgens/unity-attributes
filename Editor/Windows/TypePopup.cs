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
	using System.Linq;
	using System.Reflection;

	internal sealed class TypeSearch : PopupWindowContent
	{
		public struct Constraints
		{
			public ESearchTypeFlags flags;
			public string[] assemblies;
			public string[] namespaces;
			public Type[] baseTypes;
		}

		private const float _MIN_WIDTH = 240f;
		private const float _MAX_HEIGHT = 300f;

		private static readonly Color _HOVER_COLOR = new Color(0.2392157f, 0.3764706f, 0.5686275f) * 0.9f;
		private static readonly Color _HEADER_HOVER_COLOR = Color.white * 0.25f;

		private static readonly Color _HEADER_COLOR = EditorGUIUtility.isProSkin
		? Color.black * 0.3f
		: Color.black * 0.1f;

		public static void Open
		(
			in Rect pos,
			Type value,
			Constraints options,
			Action<Type> setFn
		)
		{
			var p = new TypeSearch(value, setFn)
			{
				_preferredWidth = pos.width
			};
			var clipPos = pos;
			clipPos.position = default;
			p._currentPage = FilterTypes(options);
			PopupWindow.Show(pos, p);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(Mathf.Max(_MIN_WIDTH, _preferredWidth), _MAX_HEIGHT);
		}

		public override void OnGUI(Rect rect)
		{
			DrawPage(rect, _currentPage);
			editorWindow.Repaint();
		}

		private readonly Action<Type> _setFn;
		private float _preferredWidth = 1f;
		private PNode _currentPage;
		private Vector2 _scroll;
		private static (Assembly, Type[])[] _cachedTypes;
		
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
			(ESearchTypeFlags.Newable, t => t.GetConstructor(Type.EmptyTypes) != null)
		};

		private static readonly (string, Func<Type, bool>)[] _TYPE_CATEGORIES =
		{
			("# Exception", t => t.IsClass && typeof(Exception).IsAssignableFrom(t)),
			("# Static", t => t.IsClass && t.IsStatic()),
			("# Class", t => t.IsClass),
			("# Enum", t => t.IsEnum),
			("# Struct", t => t.IsStruct()),
			("# Interface", t => t.IsInterface),
			("# Primitive", t => t.IsPrimitive),
			("# Enum", t => t.IsEnum),
		};

		private static readonly Lazy<Texture2D> _TEX_ATLAS = new (() =>
		{
			var path = AssetDatabase.GUIDToAssetPath("e769e4d9f339626498a12b64168231ee");
			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
		});

		private static readonly Rect _ARROWL_COORDS = new (0.5f, 0, 0.25f, 0.25f);
		private static readonly Rect _ARROWR_COORDS = new (0.75f, 0, 0.25f, 0.25f);

		private TypeSearch(Type value, Action<Type> setFn)
		{
			_setFn = setFn;
		}

		private void Select(Type t)
		{
			try { _setFn?.Invoke(t); }
			finally { }
			editorWindow.Close();
		}

		private static void DrawIcon(in Rect pos, in Rect coords, Color color)
		{
			DrawerGUI.DrawTex(_TEX_ATLAS.Value, pos, coords, color);
		}

		private class PNode
		{
			public PNode parent;
			public string name;
			public Type type;
			public Dictionary<string, PNode> links = new ();

			public void Sort()
			{
				links = links
				.OrderBy(x => x.Value.type != null)
				.ThenBy(x => x.Key)
				.ToDictionary(x => x.Key, x => x.Value);

				foreach(var l in links)
				{
					l.Value.Sort();
				}
			}

			public PNode FindChildOrNew(string cName)
			{
				if (!links.TryGetValue(cName, out var node))
				{
					node = new PNode
					{
						name = cName,
						parent = this
					};
					links[cName] = node;
				}
				return node;
			}
		}

		private static (Assembly, Type[])[] GetAllAssemblyTypes()
		{
			if(_cachedTypes == null)
			{
				List<(Assembly, Type[])> aTypes = new();
				var assemblies =
				AppDomain.CurrentDomain.GetAssemblies()
				.OrderBy(x => x.GetName().Name)
				.ToArray();
				foreach (var a in assemblies)
				{
					List<Type> tList = new();
					foreach (var t in a.GetTypes())
					{
						tList.Add(t);
					}
					aTypes.Add((a, tList.ToArray()));
				}
				_cachedTypes = aTypes.ToArray();
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

		private static PNode FilterTypes(in Constraints opts)
		{
			var root = new PNode();
			root.name = "Types";

			foreach (var (assembly, aTypes) in GetAllAssemblyTypes())
			{
				if (opts.assemblies != null && !HasItem(opts.assemblies, assembly.GetName().Name))
				{
					continue;
				}

				foreach (var aType in aTypes)
				{
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

					var fp =
					aType.Namespace
					+ (catName != null ? $".{catName}." : ".")
					+ tLabel;

					var path = fp.Split('.');

					if (path.Length == 1)
					{
						path = new [] { ".", tLabel };
					}

					var cn = root;
					foreach (var name in path)
					{
						cn = cn.FindChildOrNew(name);
					}
					cn.type = aType;
				}
			}

			root.Sort();
			return root;
		}

		private static bool DrawHeader(Rect pos, in string label, bool root = false)
		{
			EditorGUI.DrawRect(pos, _HEADER_COLOR);

			var hoverRect = pos;
			
			if (!root)
			{
				var icoRect = pos.SliceLeft(pos.height);
				icoRect = icoRect.Resize(-icoRect.height * 0.4f);
				DrawIcon(icoRect, _ARROWL_COORDS, Color.white * 0.5f);
			}

			if (hoverRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(hoverRect, _HEADER_HOVER_COLOR);
			}

			EditorGUI.LabelField(hoverRect, label, PopupStyles.HeaderLabel);
			return !root && GUI.Button(hoverRect, "", GUIStyle.none);
		}

		private static bool DrawItem( Rect pos, in string label, bool leaf = false)
		{
			var hoverRect = pos;
			
			pos.SliceLeft(pos.height * 0.25f);
			
			if (!leaf)
			{
				var icoRect = pos.SliceRight(pos.height);
				icoRect = icoRect.Resize(-icoRect.height * 0.4f);
				DrawIcon(icoRect, _ARROWR_COORDS, Color.white * 0.5f);

			}
			var (rl, rr) = pos.GetColumns(1f, pos.height, 2);
			if (hoverRect.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(hoverRect, _HOVER_COLOR);
			}
			EditorGUI.LabelField(rl.ResizeW(-5f), label, PopupStyles.ItemLabel);
			return GUI.Button(pos, "", GUIStyle.none);
		}

		private void DrawPage(in Rect pos, PNode p)
		{
			var rows = pos.CalcRows(30f, 1f);
			var count = Mathf.Max(p.links.Count, 1);
			var ih = EditorGUIUtility.singleLineHeight + 5f;
			var itemRect = rows[1];
			itemRect.height = 30f + count * ih;
			itemRect.width -= 15f;

			PNode newPage = null;

			if (DrawHeader(rows[0], p.name, p.parent == null))
			{
				newPage = p.parent;
			}

			using (var s = new GUI.ScrollViewScope(rows[1], _scroll, itemRect))
			{
				var offset = 30f;
				var ci = 0;
				foreach (var it in p.links)
				{
					if(it.Value.links.Count == 0 && it.Value.type == null)
					{
						continue;
					}

					var c = it.Value;

					var posy = offset;

					var min = posy;
					var max = posy + ih;

					var shouldDraw =
					max >= _scroll.y
					&& max <= _scroll.y + itemRect.height;

					if (shouldDraw)
					{
						var itemPos = itemRect;
						itemPos.height = ih;
						itemPos.position = new Vector2(0f, offset);

						if (DrawItem(itemPos, c.name, c.type != null))
						{
							if (c.type == null)
							{
								_scroll = default;
								
								_currentPage = c;
							}
							else
							{
								Select(c.type);
							}
						}
					}

					offset += ih;
					ci++;
				}
			
				_scroll = s.scrollPosition;
			}

			if (newPage != null)
			{
				_currentPage = newPage;
			}
		}
	}
}

#endif