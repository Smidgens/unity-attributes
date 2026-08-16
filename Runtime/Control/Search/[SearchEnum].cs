// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Popup search for enum
	/// </summary>
	public sealed class SearchEnumAttribute : __BaseControl { }
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(SearchEnumAttribute))]
	internal sealed class _SearchEnumAttribute : __ControlDrawer<SearchEnumAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.Enum;

		protected override void OnInit()
		{
			_enumValues = (int[])Enum.GetValues(_FieldType);
		}

		private string[] _enumNames;
		private int[] _enumValues;

		protected override void OnField(in DrawContext ctx)
		{
			var prop = ctx.property;

			if (_enumNames == null)
			{
				_enumNames = prop.enumDisplayNames;
			}

			_popupLabel.text = PluginConstants.Label.POPUP_UNSET;
			
			var ci = Array.IndexOf(_enumValues, prop.intValue);

			if (ci > -1)
			{
				_popupLabel.text = _enumNames[ci];
			}
			
			var enumType = _FieldType;

			if (EditorGUI.DropdownButton(ctx.position, _popupLabel, FocusType.Keyboard))
			{
				if (_cachedTree == null)
				{
					_cachedTree = SearchPopup<int>.CreateTypeMenuTree(_FieldType.Name.ToSentenceCase(), new SearchPopup<int>.Options
					{
						labelFn = v =>
						{
							var i = Array.IndexOf(_enumValues, v);

							return i >= 0 ? _enumNames[i] : "-";
						},
						filterNameFn = v =>
						{
							var i = Array.IndexOf(_enumValues, v);

							return i >= 0 ? _enumNames[i].ToLower() : v.ToString();
						},
						equalsFn = (a,b) => a == b
					});

					for (int i = 0; i < _enumNames.Length; i++)
					{
						var n = _enumNames[i];
						var v = _enumValues[i];
						_cachedTree.AddValue(n, v);
					}
				}

				_cachedTree.Filter(SearchPopup<int>.SearchFilter.Empty);

				var menu = SearchPopup<int>.Create(0, _cachedTree, v =>
				{
					prop.intValue = v;
					prop.serializedObject.ApplyModifiedProperties();
				});
				
				menu.Show(ctx.position);
				
				
			}
		}

		private readonly GUIContent _popupLabel = new();
		private SearchPopup<int>.MenuNode _cachedTree;
	}
}

#endif