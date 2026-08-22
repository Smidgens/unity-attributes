// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	/// <summary>
	/// Types for [AnimatorParameter]
	/// </summary>
	[System.Flags]
	public enum EAnimatorParameter
	{
		Bool = 1,
		Float = 2,
		Int = 4,
		Trigger = 8,
		All = ~0
	}

	public sealed class AnimatorParameterAttribute : __BaseControl
	{
		internal string field { get; }
		internal EAnimatorParameter types { get; }

		public AnimatorParameterAttribute(string animatorRefField, EAnimatorParameter types = EAnimatorParameter.All)
		{
			field = animatorRefField;
			this.types = types;
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	internal sealed class _AnimatorParameterAttribute : __ControlDrawer<AnimatorParameterAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			ParameterPopup(ctx.position, ctx.property, _Attribute.field);
		}

		protected override DisplayIcon GetFieldDisplayIcon()
		{
			return new DisplayIcon
			{
				texture = EditorGUIUtility.IconContent("Animator Icon")?.image,
			};
		}

		public void ParameterPopup(in Rect position, SerializedProperty prop, in string animatorFieldPath)
		{
			var pos = position;
			// 

			if (!HasAnimationModule())
			{
				DrawerGUI.MutedInfo(pos, "Missing animation module");
				return;
			}
			
			SerializedProperty animatorProp;

			// absolute path from root object
			if (animatorFieldPath.StartsWith('~'))
			{
				animatorProp = prop.serializedObject.FindProperty(animatorFieldPath.Substring(1));
			}
			else
			{
				animatorProp = prop.FindSibling(animatorFieldPath);
			}

			if (animatorProp == null || !IsAnimatorRef(animatorProp.objectReferenceValue))
			{
				DrawerGUI.MutedInfo(pos, PluginConstants.Msg.FIELD_INVALID);
				return;
			}

			var animatorRef = animatorProp.objectReferenceValue;
			
			bool isInt = prop.propertyType == SerializedPropertyType.Integer;
			bool isDefault = isInt
			? prop.intValue < 0
			: string.IsNullOrEmpty(prop.stringValue);

			var btnLabel = _POPUP_DEFAULT;

			if (isInt && !isDefault)
			{
				btnLabel = new GUIContent(GetAnimatorParameterOption(animatorRef, prop.intValue).name);
			}
			else if (!isInt && !isDefault)
			{
				btnLabel = new GUIContent(prop.stringValue);
			}

			if(EditorGUI.DropdownButton(pos, btnLabel, FocusType.Keyboard))
			{
				var m = GetParameterMenu
				(
					animatorProp,
					_Attribute.types,
					prop,
					(opt) =>
					{
						if (prop.propertyType == SerializedPropertyType.Integer)
						{
							prop.intValue = opt.index;
						}
						else if (prop.propertyType == SerializedPropertyType.String)
						{
							prop.stringValue = opt.name;
						}
						prop.serializedObject.ApplyModifiedProperties();
					}
				);
				m.DropDown(pos);
			}
		}

		private static bool IsAnimatorRef(Object ob)
		{
#if SM_ATTR_ANIMATION
			return typeof(Animator) == ob?.GetType();
#else
			return false;
#endif
		}

		private static bool HasAnimationModule()
		{
#if SM_ATTR_ANIMATION
			return true;
#else
			return false;
#endif
		}

		private static GenericMenu GetParameterMenu(SerializedProperty animatorProp, EAnimatorParameter types, SerializedProperty prop, System.Action<AnimOption> setFn)
		{
			Object animatorRef = animatorProp.objectReferenceValue;
			bool isInt = prop.propertyType == SerializedPropertyType.Integer;
			bool isDefault = isInt
			? prop.intValue < 0
			: string.IsNullOrEmpty(prop.stringValue);
			
			var m = new GenericMenu();
			m.AddItem(_POPUP_DEFAULT, isDefault, () => setFn.Invoke(new AnimOption
			{
				name = string.Empty,
				index = -1
			}));
			m.AddSeparator("");

			var pCount = GetAnimatorParameterCount(animatorRef);
			
			if(pCount == 0)
			{
				m.AddDisabledItem(_POPUP_EMPTY);
			}

			for(var i = 0; i < pCount; i++)
			{
				var opt = GetAnimatorParameterOption(animatorRef, i);

				if (!types.HasFlag(opt.typeFlag))
				{
					continue;
				}
				
				var oActive = isInt
				? prop.intValue == opt.index
				: prop.stringValue == opt.name;
				
				m.AddItem(opt.menuLabel, oActive, () => setFn.Invoke(opt));
			}
			return m;
		}

		private static readonly GUIContent _POPUP_DEFAULT = new (PluginConstants.Label.POPUP_UNSET);
		private static readonly GUIContent _POPUP_EMPTY = new (PluginConstants.Label.POPUP_EMPTY);

		private struct AnimOption
		{
			public GUIContent menuLabel;
			public string name;
			public int index;
			public int nameHash;
			public EAnimatorParameter typeFlag;
		}

		private static AnimOption GetAnimatorParameterOption(Object animatorRef, int index)
		{
#if !SM_ATTR_ANIMATION
			return default;
#else

			if (!animatorRef || animatorRef.GetType() != typeof(Animator))
			{
				return default;
			}

			var animator = (animatorRef as Animator)!;

			var par = animator.GetParameter(index);
			

			if (par == null)
			{
				return new AnimOption
				{
					menuLabel = _POPUP_DEFAULT,
					index = -1,
				};
			}

			var typeFlag = par.type switch
			{
				AnimatorControllerParameterType.Bool => EAnimatorParameter.Bool,
				AnimatorControllerParameterType.Float => EAnimatorParameter.Float,
				AnimatorControllerParameterType.Int => EAnimatorParameter.Int,
				AnimatorControllerParameterType.Trigger => EAnimatorParameter.Trigger,
				_ => EAnimatorParameter.All
			};
			

			return new AnimOption
			{
				menuLabel = new GUIContent($"{par.type.ToString()}/{par.name}"),
				nameHash = par.nameHash,
				index = index,
				typeFlag = typeFlag,
			};
#endif
		}

		private static int GetAnimatorParameterCount(Object animatorRef)
		{
#if SM_ATTR_ANIMATION
			if (!animatorRef || animatorRef is not Animator a)
			{
				return 0;
			}
			return a.parameterCount;
#else
			return 0;
#endif

		}
		
	}
}

#endif