// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using UnityEngine;

	public sealed class FoldoutAttribute : __BaseControl
	{
		public FoldoutAttribute
		(
			string name = null,
			string iconGUID = null,
			string iconColor = null,
			float[] iconCoords = null
		): base(false)
		{
			this.name = name;
			this.iconGUID = iconGUID;

			if (!string.IsNullOrEmpty(iconColor))
			{
				if(ColorUtility.TryParseHtmlString(iconColor, out var c))
				{
					this.iconColor = c;
				}
			}

			if (iconCoords is { Length: >= 4 })
			{
				this.iconCoords = new Rect(iconCoords[0], iconCoords[1], iconCoords[2], iconCoords[3]);
			}
		}

		internal string name { get; }
		internal string iconGUID { get; }
		internal Color iconColor { get; } = Color.white;
		internal Rect iconCoords { get; } = new (0f, 0f, 1f, 1f);
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	[CustomPropertyDrawer(typeof(FoldoutAttribute))]
	internal sealed class _FoldoutAttribute : __ControlDrawer<FoldoutAttribute>
	{
		protected override void OnInit()
		{
			if (!string.IsNullOrEmpty(_Attribute.name))
			{
				_customLabel = new GUIContent(_Attribute.name);
			}
			_isEvent = typeof(UnityEventBase).IsAssignableFrom(_FieldType);

			_icon = (true, null);

			if (_Attribute.iconGUID.IsGUID())
			{
				_icon.Item2 = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(_Attribute.iconGUID));
			}
		}

		private (bool, Texture) _icon;

		private static readonly float _PAD = EditorGUIUtility.standardVerticalSpacing * 2f;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var h = DrawerStyles.FoldoutHeight;
			if (prop.isExpanded)
			{
				h += _PAD;
				h += EditorGUI.GetPropertyHeight(prop, label);
			}
			h += EditorStyles.helpBox.padding.top + EditorStyles.helpBox.padding.bottom;
			return h;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var property = ctx.property;
			var position = ctx.position;

			GUI.Box(position, GUIContent.none, EditorStyles.helpBox);
			var foldoutHeight = DrawerStyles.FoldoutHeight;

			position = position.Padded(EditorStyles.helpBox.padding);
			var foldoutRect = position.SliceTop(foldoutHeight);

			var foldLabel = ctx.label;

			if (_customLabel != null)
			{
				foldLabel = _customLabel;
			}

			if (_icon.Item2)
			{
				var iconRect = foldoutRect.SliceLeft(foldoutRect.height);
				iconRect = iconRect.Resized(-iconRect.height * 0.1f);
				var iconColor = _Attribute.iconColor;
				DrawerGUI.DrawTex(_icon.Item2, iconRect, _Attribute.iconCoords, iconColor);
			}

			EditorGUI.indentLevel++;
			property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, foldLabel, true, DrawerStyles.Foldout);
			EditorGUI.indentLevel--;

			if (_isEvent)
			{
				var elCount = property.GetEventListenerCount();
				var tAlignment = DrawerStyles.LabelSM.alignment;
				DrawerStyles.LabelSM.alignment = TextAnchor.MiddleRight;
				GUI.Label(foldoutRect, elCount.ToString(), DrawerStyles.LabelSM);
				DrawerStyles.LabelSM.alignment = tAlignment;
			}

			if (property.isExpanded)
			{
				position.SliceTop(_PAD);
				position.SliceLeft(DrawerGUI.INDENT_W * 0.5f);

				if (_isEvent)
				{
					EditorGUI.PropertyField(position, property, GUIContent.none);
				}
				else
				{
					EditorGUI.PropertyField(position, property);
				}
			}
		}
		
		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
		
		}

		private GUIContent _customLabel;
		private bool _isEvent;
	}
}

#endif