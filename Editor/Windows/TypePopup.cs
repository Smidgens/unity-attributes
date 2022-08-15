// smidgens @ github

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	internal class TypePopup : PopupWindowContent
	{
		public static void Open(in Rect pos, Type value, Action<Type> setFn)
		{
			var p = new TypePopup(value, setFn);
			p._pos = pos;
			var clipPos = pos;
			clipPos.position = default;
			PopupWindow.Show(pos, p);

			GetTypes();
		}

		public override Vector2 GetWindowSize()
		{
			return new Vector2(Mathf.Max(220f, _pos.width), 300f);
		}

		public override void OnGUI(Rect rect)
		{
			DrawPage(rect, _currentPage);
			editorWindow.Repaint();
		}

		private Rect _pos = default;
		private Action<Type> _setFn = null;
		private PNode _currentPage = null;
		private static PNode _rootNode = GetTypes();

		private TypePopup(Type value, Action<Type> setFn)
		{
			_setFn = setFn;
			_currentPage = _rootNode;
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
			public static bool TypeIsStatic(Type t) => t.IsAbstract && t.IsSealed;

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

		private static PNode GetTypes()
		{
			var assemblies =
			AppDomain.CurrentDomain.GetAssemblies()
			.OrderBy(x => x.GetName().Name)
			.ToArray();

			var root = new PNode();
			root.name = "Types";

			foreach(var a in assemblies)
			{
				var types = a.GetTypes();

				foreach(var t in types)
				{
					if (!TypeFilter.ShouldInclude(t)) { continue; }

					string catName = TypeCategory.Get(t); ;

					var fp =
					t.Namespace
					+ (catName != null ? $".· {catName} ·." : ".")
					+ t.Name;

					var path = fp.Split('.');

					if(path.Length == 1)
					{
						path = new string[] { "·", t.Name };
					}

					var cn = root;
					foreach(var name in path)
					{
						cn = cn.FindChildOrNew(name);
					}

					cn.type = t;
				}
			}
			root.Sort();
			return root;
		}


		private void Select(Type t)
		{
			try { _setFn?.Invoke(t); }
			finally { }
			editorWindow.Close();
		}

		private static readonly Lazy<GUIStyle> _HEADER_STYLE = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
			s.normal.textColor = Color.white;
			s.fontStyle = FontStyle.Bold;
			return s;
		});

		private static readonly Lazy<GUIStyle> _ITEM_STYLE = new Lazy<GUIStyle>(() =>
		{
			var s = new GUIStyle(EditorStyles.miniLabel);
			s.normal.textColor = Color.white;
			s.fontStyle = FontStyle.Bold;
			return s;
		});

		private static bool DrawHeader(in Rect pos, in string label, bool root = false)
		{
			var lpos = pos;
			lpos.position += new Vector2(5f, 0f);
			lpos.width -= 10f;
			if (!root)
			{
				var ipos = lpos;
				ipos.width = pos.height;
				EditorGUI.LabelField(ipos, "<·", EditorStyles.boldLabel);
			}
			EditorGUI.DrawRect(pos, Color.black * 0.3f);
			EditorGUI.LabelField(lpos, label, _HEADER_STYLE.Value);
			return !root && GUI.Button(pos, "", GUIStyle.none);
		}

		private static bool DrawItem(in Rect pos, in string label, bool leaf = false)
		{
			var lpos = pos;

			if (!leaf)
			{
				lpos.width -= lpos.height;
				var ipos = lpos;

				ipos.position += new Vector2(lpos.width, 0f);
				ipos.width = ipos.height;
				var c = ipos.center;
				ipos.size *= 0.7f;
				ipos.center = c;
				EditorGUI.LabelField(ipos, "·>", EditorStyles.boldLabel);
		
			}

			var cc = lpos.center;
			lpos.width -= 10f;
			lpos.center = cc;

			if (Event.current != null)
			{
				if (pos.Contains(Event.current.mousePosition))
				{
					EditorGUI.DrawRect(pos, Color.cyan * 0.6f);
				}
			}


			EditorGUI.LabelField(lpos, label, _ITEM_STYLE.Value);

			return GUI.Button(pos, "", GUIStyle.none);
		}


		private Vector2 _scroll = default;

		private void DrawPage(in Rect pos, PNode p)
		{
			var rows = pos.SubdivideY(30f, 1f);

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