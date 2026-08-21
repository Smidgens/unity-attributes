// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Unique GUID helper
	/// </summary>
	public sealed class StableGUIDAttribute : __BaseControl
	{
		public StableGUIDAttribute()
		{
	
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using System;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(StableGUIDAttribute))]
	internal sealed class _StableGUIDAttribute : __ControlDrawer<StableGUIDAttribute>
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
			var pos = ctx.position;
			var prop = ctx.property;

			if (_cachedInstanceID < 0)
			{
				_cachedInstanceID = ctx.property.serializedObject.targetObject.GetInstanceID();
				_currentGID = GlobalObjectId.GetGlobalObjectIdSlow(ctx.property.serializedObject.targetObject);
			}

			if (_cachedState.Item1 != prop.stringValue)
			{
				var valid = TryParseGUIDString(ctx.property.stringValue, out var result);
				_cachedState = (prop.stringValue, valid, result);
			}

			if (string.IsNullOrEmpty(ctx.property.stringValue))
			{
				DrawAlert(pos,  _MSG_UNSET, _ALERT_WARR, _LB_FIX, ctx.property, SetGUID);
			}
			else
			{
				if (!_cachedState.Item2)
				{
					DrawAlert(pos,  _MSG_INVALID, _ALERT_WARR, _LB_FIX, ctx.property, SetGUID);
				}
				else
				{
					var duplicate = !_cachedState.Item3.Item2.Equals(_currentGID);
					if (duplicate)
					{
						DrawAlert(pos,  _MSG_DUPLICATE, _ALERT_ERR, _LB_FIX, ctx.property, SetGUID);
					}
					else
					{
						var guid = _cachedState.Item3.Item1;

						GUI.Box(pos, GUIContent.none, _boxStyle);
						
						var inner = pos.Padded(_boxStyle.padding);

						var ctxSize = EditorStyles.iconButton.CalcSize(_ctxIcon);

						var ctxRect = inner.SliceRight(ctxSize.x);
						var ctxCenter = ctxRect.center;
						ctxRect.height = ctxSize.y;
						ctxRect.center = ctxCenter;
						
						GUI.Label(inner, guid, _lbStyle);

						if (GUI.Button(ctxRect, _ctxIcon, EditorStyles.iconButton))
						{
							var m = new GenericMenu();
							m.AddItem(_LB_COPY, false, () =>
							{
								CopyGUID(prop);
							});
							m.DropDown(ctxRect);
						}
					}
				}
			}
		}


		private int _cachedInstanceID = -1;
		private GlobalObjectId _currentGID;

		private (string, bool, (string, GlobalObjectId)) _cachedState;
		private static readonly Color _ALERT_INFO = Color.cyan;
		private static readonly Color _ALERT_WARR = Color.yellow;
		private static readonly Color _ALERT_ERR = Color.red;

		private static readonly GUIContent _LB_COPY = new("Copy");
		private static readonly GUIContent _LB_FIX = new("Fix");

		private const string _MSG_UNSET = "No GUID set";
		private const string _MSG_DUPLICATE = "Duplicate GUID";
		private const string _MSG_INVALID = "Invalid GUID";

		private GUIContent _ctxIcon = EditorGUIUtility.IconContent("_Menu");

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

		public bool IsSceneObject()
		{
			return _currentGID.targetPrefabId == 0;
		}

		private static bool TryParseGUIDString(string stringifiedGUID, out (string, GlobalObjectId) result)
		{
			result = default;

			if (string.IsNullOrEmpty(stringifiedGUID))
			{
				return false;
			}

			var sepIndex = stringifiedGUID.IndexOf(';');
			if (sepIndex < 0 || sepIndex == stringifiedGUID.Length - 1)
			{
				return false;
			}
			var guid = stringifiedGUID.Substring(0, sepIndex);
			if (!guid.IsGUID())
			{
				return false;
			}
			var gidStr = stringifiedGUID.Substring(sepIndex + 1);

			if (!GlobalObjectId.TryParse(gidStr, out var gid))
			{
				return false;
			}
			result = (guid, gid);
			return true;
		}

		private void CopyGUID(SerializedProperty prop)
		{
			GUIUtility.systemCopyBuffer = _cachedState.Item3.Item1;
		}

		private void SetGUID(SerializedProperty prop)
		{
			var newGUID = System.Guid.NewGuid().ToString().Replace("-", "");
			var gid = _currentGID.ToString();
			prop.stringValue = $"{newGUID};{gid}";
		}

		private void DrawAlert<T>(Rect pos, string msg, Color color, GUIContent btnLabel, T arg, Action<T> fn)
		{
			GUI.Box(pos, GUIContent.none, _boxStyle);
			pos = pos.Padded(_boxStyle.padding);

			var tColor = GUI.backgroundColor;
			GUI.backgroundColor = color * 0.6f;

			var showBtn = btnLabel != null && fn != null;

			var w = showBtn ? _btnStyle.CalcSize(btnLabel).x : 0f;
			var fixBtnRect = showBtn ? pos.SliceRight(w) : default;
			var pressed = showBtn && GUI.Button(fixBtnRect, btnLabel, _btnStyle);
			GUI.Label(pos, msg, _lbStyle);
			GUI.backgroundColor = tColor;
			if (pressed)
			{
				fn?.Invoke(arg);
			}
		}
	}
}

#endif