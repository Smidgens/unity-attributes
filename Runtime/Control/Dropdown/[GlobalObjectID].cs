// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Retrieves the GlobalObjectId of the serialized object and sets it to the string field
	/// </summary>
	public sealed class GlobalObjectIDAttribute : __BaseControl
	{
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(GlobalObjectIDAttribute))]
	internal sealed class _GlobalObjectIDAttribute : __ControlDrawer<GlobalObjectIDAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			var lbHeight = _lbStyle.CalcSize(GUIContent.none).y;
			var btnHeight = _btnStyle.CalcHeight(GUIContent.none, 100);
			var padding = _boxStyle.padding.top + _boxStyle.padding.bottom;
			return Mathf.Max(btnHeight, lbHeight) + padding;
		}

		protected override void OnField(in DrawContext ctx)
		{
			if (_cachedInstanceID < 0)
			{
				_cachedInstanceID = ctx.property.serializedObject.targetObject.GetInstanceID();
				_currentGID = GlobalObjectId.GetGlobalObjectIdSlow(ctx.property.serializedObject.targetObject);
				_currentGIDStr = _currentGID.ToString();
			}

			if (ctx.property.stringValue != _currentGIDStr)
			{
				ctx.property.stringValue = _currentGIDStr;
			}

			GUI.Box(ctx.position, GUIContent.none, _boxStyle);
			var inner = ctx.position.Padded(_boxStyle.padding);

			var ctxSize = EditorStyles.iconButton.CalcSize(_ICO_CTX);
			var ctxRect = inner.SliceRight(ctxSize.x);
			var ctxCenter = ctxRect.center;
			ctxRect.height = ctxSize.y;
			ctxRect.center = ctxCenter;

			GUI.Label(inner, _currentGIDStr, _lbStyle);

			if (GUI.Button(ctxRect, _ICO_CTX, EditorStyles.iconButton))
			{
				var m = new GenericMenu();
				m.AddItem(new GUIContent("Copy"), false, () =>
				{
					GUIUtility.systemCopyBuffer = _currentGIDStr;
				});
				m.DropDown(ctxRect);
			}
		}

		private int _cachedInstanceID = -1;
		private GlobalObjectId _currentGID;
		private string _currentGIDStr;

		private (string, bool, (string, GlobalObjectId)) _cachedState;

		private readonly GUIContent _ICO_CTX = EditorGUIUtility.IconContent("_Menu");

		private readonly GUIStyle _btnStyle = new (EditorStyles.miniButton)
		{
			fontSize = (int)(EditorStyles.miniButton.fontSize * 0.9f),
			padding = new RectOffset(4,4,1,1)
		};
		
		private readonly GUIStyle _lbStyle = new (EditorStyles.miniLabel)
		{
			fontSize = (int)(EditorStyles.miniLabel.fontSize * 0.9f),
			alignment = TextAnchor.MiddleLeft
		};
		
		private readonly GUIStyle _boxStyle = new (EditorStyles.helpBox)
		{
			padding = new RectOffset(3,2,2,2)
		};
	}
}

#endif