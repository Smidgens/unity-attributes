// smidgens @ github

/*
 * TODOS
 *	- support arrays
 */

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityObject = UnityEngine.Object;

	[CustomPropertyDrawer(typeof(ExpandAttribute))]
	internal class Expand_ : __ControlDrawer<ExpandAttribute>
	{
		public const byte ROW_MARGIN = 2;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			return
			+_rows.Count * (ROW_MARGIN + EditorGUIUtility.singleLineHeight);
		}

		protected override void OnLabel(ref Rect pos, GUIContent l) { }

		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;
			var row = pos;
			row.height = EditorGUIUtility.singleLineHeight;
			var start = row.position;

			for (var i = 0; i < _rows.Count; i++)
			{
				var oy = i * (row.height + ROW_MARGIN);
				row.position = start + new Vector2(0, oy);
				DrawFieldAt(row, _rows[i], ctx);
			}
		}

		protected override void OnInit()
		{
			_rows = GetRows(fieldInfo);
			_customLabel = GetCustomLabel();
		}

		private string _customLabel = null;
		private List<FieldRow> _rows = null;

		private const BindingFlags _BFLAGS =
		BindingFlags.Instance
		| BindingFlags.Public
		| BindingFlags.NonPublic;

		private struct FieldRow
		{
			public byte depth; // indent
			public string path; // prop path
			public bool isGroup; // child fields
			public bool isArray;
		}

		private void DrawFieldAt(in Rect pos, in FieldRow r, in DrawContext ctx)
		{
			if (r.isArray)
			{
				DrawerGUI.MutedInfo(pos, "array expand not implemented");
				return;
			}

			var p =
			r.depth == 0
			? ctx.property
			: ctx.property.FindPropertyRelative(r.path);

			if (p == null)
			{
				EditorGUI.DrawRect(pos, Color.red * 0.3f);
				GUI.Box(pos, "?");
				return;
			}

			var l = p.displayName;

			if(r.depth == 0 && _customLabel != null) { l = _customLabel; }

			using (new EditorGUI.IndentLevelScope(r.depth))
			{
				if (r.isGroup)
				{
					EditorGUI.LabelField(pos, l, EditorStyles.boldLabel);
				}
				else
				{
					EditorGUI.PropertyField(pos, p);
				}
			}
		}

		private static List<FieldRow> GetRows(FieldInfo fi)
		{
			var r = new List<FieldRow>();
			GetRows(0, "", fi, r);
			return r;
		}

		private static void GetRows(in byte depth, in string path, FieldInfo fi, List<FieldRow> rows)
		{
			if (fi.IsNotSerialized) { return; }

			if (fi.IsArray())
			{
				rows.Add(new FieldRow
				{
					depth = depth,
					isArray = true,
					path = path,
				});
				return;
			}

			if (fi.GetCustomAttribute<HideInInspector>() != null) { return; }

			var t = fi.FieldType;

			var isGroup = t.IsClassOrStruct();

			if (t.DerivesFrom(typeof(UnityObject)))
			{
				isGroup = false;
			}
			else if(t == typeof(string))
			{
				isGroup = false;
			}


			var row = new FieldRow
			{
				depth = depth,
				isGroup = isGroup,
				path = path,
			};

			rows.Add(row);

			if (row.isGroup)
			{
				foreach (var sub in t.GetFields(_BFLAGS))
				{
					var prefix = depth > 0 ? path + "." : "";
					GetRows((byte)(depth + 1), prefix + sub.Name, sub, rows);
				}
			}
		}
	}
}

#endif