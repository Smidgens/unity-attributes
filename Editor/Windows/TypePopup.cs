// smidgens @ github

/*
 * TODO
 * - optimize
 *		- lazy type loading
 *		- gui layout
 * - show namespace breadcrumbs
 * - show currently set value
 * - show search filter
 */

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class TypeSearch : PopupWindowContent
	{
		[Flags]
		public enum VType
		{
			None = 0,
			Class = 1,
			Struct = 2,
			Enum = 4,
			Primitive = 8,
			All = ~0,
		}

		public struct Constraints
		{
			public Type[] types; // use specific types, ignore rest
			public Type[] derivedTypes;
			public VType vtypes;
			public bool staticOnly;
			public bool onlyInterfaces;
			public bool showAbstract;
			public bool includeHidden;
			public string[] assemblies;
			public string[] namespaces;
		}

		public const float
		MIN_WIDTH = 200f,
		MAX_HEIGHT = 300f;

		public static readonly Color
		HOVER_COLOR = Color.cyan * 0.6f,
		HEADER_HOVER_COLOR = Color.white * 0.25f,
		HEADER_COLOR = Color.black * 0.3f;

		public static void Open
		(
			in Rect pos,
			Type value,
			Constraints options,
			Action<Type> setFn
		)
		{
			var p = new TypeSearch(value, setFn);
			p._preferredWidth = pos.width;
			var clipPos = pos;
			clipPos.position = default;
			p._options = options;
			p._currentPage = FilterTypes(options);
			PopupWindow.Show(pos, p);
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(Mathf.Max(MIN_WIDTH, _preferredWidth), MAX_HEIGHT);
		}

		public override void OnGUI(Rect rect)
		{
			DrawPage(rect, _currentPage);
			editorWindow.Repaint();
		}

		private float _preferredWidth = 1f;
		private Action<Type> _setFn = null;
		private PNode _currentPage = null;
		private Vector2 _scroll = default;
		private Constraints _options = default;

		private TypeSearch(Type value, Action<Type> setFn)
		{
			// todo: display currently set value
			_setFn = setFn;
		}

		private void Select(Type t)
		{
			try { _setFn?.Invoke(t); }
			finally { }
			editorWindow.Close();
		}

		private class PNode
		{
			public PNode parent;
			public string name;

			public Type type;

			public Dictionary<string, PNode>
			links = new Dictionary<string, PNode>();

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

			public PNode FindChildOrNew(string name)
			{
				if (!links.TryGetValue(name, out var node))
				{
					node = new PNode();
					node.name = name;
					node.parent = this;
					links[name] = node;
				}
				return node;
			}
		}

		private static class TypeFilter
		{
			public static bool ShouldInclude(Type t)
			{
				for(var i = 0; i < _SKIP_PREDICATES.Length; i++)
				{
					if (_SKIP_PREDICATES[i].Invoke(t)) { return false; }
				}
				return true;
			}
			public static bool TypeIsNested(Type t) => t.IsNested;
			public static bool TypeHasWeirdPrefix(Type t) =>
			t.Name[0] == '<'
			|| t.Name.StartsWith("__");

			private static Func<Type, bool>[] _SKIP_PREDICATES =
			{
				TypeIsNested,
				TypeHasWeirdPrefix
			};
		}

		private static class TypeCategory
		{
			public static string Get(Type t)
			{
				if (t.IsClass)
				{
					if (typeof(Exception).IsAssignableFrom(t)) { return _EXCEPTIONS; }
					else { return _CLASSES; }
				}
				if (t.IsEnum) { return _ENUMS; }
				if (t.IsPrimitive) { return _PRIMITIVES; }
				if (t.IsValueType && !t.IsPrimitive) { return _STRUCTS; }
				if (t.IsInterface) { return _INTERFACES;  }
				return null;
			}
			private const string
			_CLASSES = "Classes",
			_EXCEPTIONS = "Exceptions",
			_STRUCTS = "Structs",
			_INTERFACES = "Structs",
			_PRIMITIVES = "Primitives",
			_ENUMS = "Enums";
		}

		private static Type[] _cachedTypes = null;

		private static Type[] GetAllTypes()
		{
			if(_cachedTypes == null)
			{
				var tl = new List<Type>();
				var assemblies =
				AppDomain.CurrentDomain.GetAssemblies()
				.OrderBy(x => x.GetName().Name)
				.ToArray();
				foreach (var a in assemblies)
				{
					var types = a.GetTypes();
					foreach (var t in types)
					{
						if (!TypeFilter.ShouldInclude(t)) { continue; }
						tl.Add(t);
					}
					_cachedTypes = tl.ToArray();
				}
			}
			return _cachedTypes;
		}

		private static bool HasItem(in string[] arr, in string name)
		{
			return Array.IndexOf(arr, name) > -1;
		}

		private static PNode FilterTypes(in Constraints opts)
		{
			var root = new PNode();
			root.name = "Types";

			Type[] types = opts.types != null
			? opts.types
			: GetAllTypes();

			var applyConstraints = opts.types == null;

			foreach (var t in types)
			{
				if (applyConstraints)
				{
					if (opts.staticOnly && !t.IsStatic())
					{
						continue;
					}

					if(opts.onlyInterfaces && !t.IsInterface)
					{
						continue;
					}

					if (!opts.includeHidden && !t.IsVisible)
					{
						continue;
					}

					if(!opts.showAbstract && t.IsAbstract)
					{
						continue;
					}

					if (opts.namespaces != null && !HasItem(opts.namespaces, t.Namespace))
					{
						continue;
					}

					if (opts.assemblies != null && !HasItem(opts.assemblies, t.Assembly.GetName().Name))
					{
						continue;
					}
					if (opts.derivedTypes != null && !t.DerivesFrom(opts.derivedTypes))
					{
						continue;
					}
				}

				string catName = TypeCategory.Get(t); ;

				var fp =
				t.Namespace
				+ (catName != null ? $".· {catName} ·." : ".")
				+ t.Name;

				var path = fp.Split('.');

				if (path.Length == 1)
				{
					path = new string[] { ".", t.Name };
				}

				var cn = root;
				foreach (var name in path)
				{
					cn = cn.FindChildOrNew(name);
				}
				cn.type = t;
			}

			root.Sort();
			return root;
		}

		private static bool DrawHeader(in Rect pos, in string label, bool root = false)
		{
			EditorGUI.DrawRect(pos, HEADER_COLOR);

			if (!root)
			{
				var ipos = pos;
				ipos.width = pos.height;
				ipos.position += Vector2.right * 5f;
				SpriteGUI.DrawIcon(ipos, AtlasIcon.ArrowLeft);
			}

			if (pos.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(pos, HEADER_HOVER_COLOR);
			}

			EditorGUI.LabelField(pos.ResizeW(-5f), label, PopupStyles.HeaderLabel);
			return !root && GUI.Button(pos, "", GUIStyle.none);
		}

		private static bool DrawItem(in Rect pos, in string label, bool leaf = false)
		{
			var (rl, rr) = pos.GetColumns(1f, pos.height, 2);

			if (!leaf)
			{
				SpriteGUI.DrawIcon(rr, AtlasIcon.ArrowRight);
			}
			if (pos.Contains(Event.current.mousePosition))
			{
				EditorGUI.DrawRect(pos, HOVER_COLOR);
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

				{
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