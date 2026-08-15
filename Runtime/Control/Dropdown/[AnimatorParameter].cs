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
	using System.Reflection;
	using Type = System.Type;

	[CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
	internal sealed class _AnimatorParameterAttribute : __ControlDrawer<AnimatorParameterAttribute>
	{
		protected override void OnField(in DrawContext ctx)
		{
			ParameterPopup(ctx.position, ctx.property, _Attribute.field);
		}

		public void ParameterPopup(in Rect pos, SerializedProperty prop, in string animatorFieldPath)
		{
			if (_TYPE_ANIM == null)
			{
				DrawerGUI.MutedInfo(pos, "Missing animation module");
				return;
			}

			var animatorProp = prop.serializedObject.FindProperty(animatorFieldPath);
			if (animatorProp == null)
			{
				DrawerGUI.MutedInfo(pos, "Invalid field type");
				return;
			}

			if (!animatorProp.IsRefType("Animator"))
			{
				DrawerGUI.MutedInfo(pos, "Invalid animator field");
				return;
			}

			if (!animatorProp.objectReferenceValue)
			{
				DrawerGUI.MutedInfo(pos, "animator not set");
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
				btnLabel = new GUIContent(GetAnimatorParameterOption(animatorRef, prop.intValue).Item2);
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
					(name, index) =>
					{
						if (prop.propertyType == SerializedPropertyType.Integer)
						{
							prop.intValue = index;
						}
						else if (prop.propertyType == SerializedPropertyType.String)
						{
							prop.stringValue = name;
						}
						prop.serializedObject.ApplyModifiedProperties();
					}
				);
				m.DropDown(pos);
			}
		}

		private static readonly Type _TYPE_ANIM = Type.GetType("UnityEngine.Animator, UnityEngine.AnimationModule");
		private static readonly Type _TYPE_ANIM_PARAM = Type.GetType("UnityEngine.AnimatorControllerParameter, UnityEngine.AnimationModule");

		private static GenericMenu GetParameterMenu(SerializedProperty animatorProp, EAnimatorParameter types, SerializedProperty prop, System.Action<string, int> setFn)
		{
			Object animatorRef = animatorProp.objectReferenceValue;
			bool isInt = prop.propertyType == SerializedPropertyType.Integer;
			bool isDefault = isInt
			? prop.intValue < 0
			: string.IsNullOrEmpty(prop.stringValue);
			
			var m = new GenericMenu();
			m.AddItem(_POPUP_DEFAULT, isDefault, () => setFn.Invoke(string.Empty, -1));
			m.AddSeparator("");

			var pCount = GetAnimatorParameterCount(animatorRef);
			
			if(pCount == 0)
			{
				m.AddDisabledItem(_POPUP_EMPTY);
			}

			for(var i = 0; i < pCount; i++)
			{
				var (label, name, index, type) = GetAnimatorParameterOption(animatorRef, i);

				if (type[0] == 'F' && !types.HasFlag(EAnimatorParameter.Float))
				{
					continue;
				}
				
				if (type[0] == 'B' && !types.HasFlag(EAnimatorParameter.Bool))
				{
					continue;
				}

				if (type[0] == 'I' && !types.HasFlag(EAnimatorParameter.Int))
				{
					continue;
				}
	
				if (type[0] == 'T' && !types.HasFlag(EAnimatorParameter.Trigger))
				{
					continue;
				}

				var oActive = isInt
				? prop.intValue == index
				: prop.stringValue == name;
				
				m.AddItem(new GUIContent(label), oActive, () => setFn.Invoke(name, index));
			}
			return m;
		}

		private const BindingFlags _BFLAGS_INSTANCE_PROP =
		BindingFlags.Instance
		| BindingFlags.GetProperty
		| BindingFlags.Public;
		
		private const BindingFlags _BFLAGS_INSTANCE_FN =
		BindingFlags.Instance
		| BindingFlags.Public;

		private static MethodInfo _paramGetterFn;
		private static PropertyInfo _paramCountProp;
		private static PropertyInfo _paramTypeProp;
		private static PropertyInfo _paramNameProp;

		private static readonly object[] _paramArray = new object[1];
		
		private static readonly GUIContent _POPUP_DEFAULT = new (PluginConstants.Label.POPUP_UNSET);
		private static readonly GUIContent _POPUP_EMPTY = new (PluginConstants.Label.POPUP_EMPTY);

		private static (GUIContent, string, int, string) GetAnimatorParameterOption(Object animatorRef, int index)
		{
			if (!animatorRef || animatorRef.GetType() != _TYPE_ANIM)
			{
				return default;
			}

			if (_paramTypeProp == null)
			{
				_paramGetterFn = _TYPE_ANIM.GetMethod("GetParameter", _BFLAGS_INSTANCE_FN);
				_paramTypeProp = _TYPE_ANIM_PARAM.GetProperty("type", _BFLAGS_INSTANCE_PROP);
				_paramNameProp = _TYPE_ANIM_PARAM.GetProperty("name", _BFLAGS_INSTANCE_PROP);
			}

			_paramArray[0] = index;
			var param = (_paramGetterFn!).Invoke(animatorRef, _paramArray);
			if (param == null)
			{
				return (_POPUP_DEFAULT, string.Empty, -1, string.Empty);
			}

			var pType = (_paramTypeProp!).GetValue(param);
			var pName = (_paramNameProp!).GetValue(param);
			
			return (new GUIContent($"{pType}/{pName}"), pName.ToString(), index, pType.ToString());
		}
		
		
		private static int GetAnimatorParameterCount(UnityEngine.Object animatorRef)
		{
			if (!animatorRef || animatorRef.GetType() != _TYPE_ANIM)
			{
				return 0;
			}

			if (_paramCountProp == null)
			{
				_paramCountProp = _TYPE_ANIM.GetProperty("parameterCount", _BFLAGS_INSTANCE_PROP);
			}
			return (int)(_paramCountProp!).GetValue(animatorRef);
		}
		
	}
}

#endif