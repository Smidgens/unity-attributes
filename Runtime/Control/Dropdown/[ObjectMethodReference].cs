// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Config flags for [ObjectMethodReference]
	/// </summary>
	[Flags]
	public enum EOBjectMethodRef
	{
		/// <summary>
		/// Include nothing (pointless innit?)
		/// </summary>
		None = 0,
		/// <summary>
		/// Include private/protected methods
		/// </summary>
		NonPublic = 1,
		/// <summary>
		/// Include property methods
		/// </summary>
		Property = 2,
		/// <summary>
		/// Standard behaviour, similar to UnityEvent
		/// </summary>
		Standard = Property,
		/// <summary>
		/// Include everything
		/// </summary>
		All = ~0
	}

	/// <summary>
	/// Draws popup of methods on referenced object
	///
	/// Note: passing null delegate type will include every method
	/// </summary>
	public sealed class ObjectMethodReferenceAttribute : __BaseControl
	{
		public ObjectMethodReferenceAttribute
		(
			string field,
			Type delegateType,
			EOBjectMethodRef flags = EOBjectMethodRef.Standard,
			bool hideIcon = false
		)
		{
			this.field = field;
			this.flags = flags;
			this.delegateType = delegateType;
			this.hideIcon = hideIcon;
		}
		
		/// <summary>
		/// Alternative con
		/// </summary>
		public ObjectMethodReferenceAttribute
		(
			string field,
			string delegateTypeFn,
			EOBjectMethodRef flags = EOBjectMethodRef.Standard,
			bool hideIcon = false
		)
		{
			this.field = field;
			this.flags = flags;
			this.delegateTypeFn = delegateTypeFn;
			this.hideIcon = hideIcon;
		}
		
		internal bool hideIcon { get; }
		internal string field { get; }
		internal Type delegateType { get; }
		internal string delegateTypeFn { get; }
		internal EOBjectMethodRef flags { get; }

		internal static (Type, Type[]) GetDelegateTypes(Type delType)
		{
			if (delType == null || !typeof(Delegate).IsAssignableFrom(delType))
			{
				return default;
			}

			var invoke = delType.GetMethod(nameof(Action.Invoke))!;
			var pars = invoke.GetParameters();
			
			var rType = invoke.ReturnType;
			var aTypes = new Type[pars.Length];
			for (int i = 0; i < pars.Length; i++)
			{
				aTypes[i] = pars[i].ParameterType;
			}
			return (rType, aTypes);
		}
		
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using System.Text;

	[CustomPropertyDrawer(typeof(ObjectMethodReferenceAttribute))]
	internal sealed class _ObjectMethodReferenceAttribute : __ControlDrawer<ObjectMethodReferenceAttribute>
	{
		protected override EFieldType GetValidTypes() => EFieldType.String;

		private void EnsureInit(SerializedProperty prop)
		{
			if (_init)
			{
				return;
			}

			Type dType = _Attribute.delegateType;
			if (!string.IsNullOrEmpty(_Attribute.delegateTypeFn))
			{
				var m = GetTypeMethod(_Attribute.delegateTypeFn, fieldInfo.DeclaringType);
				if (m != null)
				{
					var target = prop.GetParent().boxedValue;
					dType = (Type)m.Invoke(target, null);
				}
			}

			var (rType, aTypes) = ObjectMethodReferenceAttribute.GetDelegateTypes(dType);

			_optionCtx = new OptionContext
			{
				returnType = rType,
				argTypes = aTypes,
				flags = _Attribute.flags
			};
		}

		private static MethodInfo GetTypeMethod(string method, Type tType)
		{
			var m = tType.GetMethod(method, ANY_INSTANCE_MEMBER);
			if (m == null || m.GetParameters().Length != 0 || m.ReturnType != typeof(Type))
			{
				return null;
			}
			return m;
		}

		private bool _init;
		private (bool, Type, Type[]) _cachedTypeFn;

		protected override void OnField(in DrawContext ctx)
		{
			EnsureInit(ctx.property);
			
			var prop = ctx.property;
			var pos = ctx.position;

			var obProp = prop.FindSibling(_Attribute.field);

			if (obProp == null || obProp.propertyType != SerializedPropertyType.ObjectReference)
			{
				DrawerGUI.MutedInfo(pos, "Invalid field");
				return;
			}

			if (!obProp.objectReferenceValue)
			{
				var tEnabled = GUI.enabled;
				GUI.enabled = false;

				if (!_Attribute.hideIcon)
				{
					DrawerGUI.DrawControlPrefixIcon(ref pos, EAtlasIcon.Parentheses);
				}
				
				GUI.Box(pos, _NO_FN_LABEL, EditorStyles.popup);
				GUI.enabled = tEnabled;
				return;
			}

			var obRef = obProp.objectReferenceValue;

			if (_cachedMethod.Item1.GetHashCode() != prop.stringValue.GetHashCode())
			{
				_cachedMethod = (string.Empty, null);
			}

			if (_cachedMethod.Item2 == null)
			{
				_cachedMethod = (prop.stringValue, ParseMethodString(prop.stringValue));
			}

			var currentMethod = _cachedMethod.Item2;
			_btnLabel.text = _NO_FN_LABEL.text;
			
			if (currentMethod != null)
			{
				if (ValidateMethodTarget(obRef, currentMethod.ReflectedType))
				{
					_btnLabel.text = $"{currentMethod.ReflectedType?.Name}.{GetDisplayName(currentMethod)}";
				}
				else
				{
					_btnLabel.text = "(Invalid target)";
				}
				
			}

			if (!_Attribute.hideIcon)
			{
				DrawerGUI.DrawControlPrefixIcon(ref pos, EAtlasIcon.Parentheses);
			}

			if (EditorGUI.DropdownButton(pos, _btnLabel, FocusType.Keyboard))
			{
				var m = GetMenu(obProp.objectReferenceValue, currentMethod, m =>
				{
					prop.stringValue = m != null
					? StringifyMethod(m)
					: string.Empty;
					prop.serializedObject.ApplyModifiedProperties();
				});
				m.DropDown(ctx.position);
			}
		}
		
		private readonly GUIContent _btnLabel = new();
		private static readonly GUIContent _NO_FN_LABEL = new ("No Function");
		private static readonly CallableInfo[] _singleItem = new CallableInfo[1];
		private (string, MethodInfo) _cachedMethod = (string.Empty, null);

		private OptionContext _optionCtx;

		private struct OptionContext
		{
			public Type returnType;
			public Type[] argTypes;
			public EOBjectMethodRef flags;
		}

		private struct CallableInfo
		{
			public Type targetType;
			public IReadOnlyList<MethodInfo> properties;
			public IReadOnlyList<MethodInfo> methods;
		}

		private UnityEngine.Object ValidateMethodTarget(UnityEngine.Object obRef, Type type)
		{
			if (obRef.GetType() == type)
			{
				return obRef;
			}

			var isComponent = typeof(Component).IsAssignableFrom(type);
			var isGameObject = type == typeof(GameObject);

			if (!isComponent && !isGameObject)
			{
				if (obRef.GetType() != type)
				{
					return null;
				}
			}
			// target is already a game object
			if (isGameObject && obRef is GameObject)
			{
				return obRef;
			}
		
			if (isGameObject && obRef is Component c1)
			{
				return c1.gameObject;
			}
			if (isComponent && obRef is GameObject go)
			{
				return go.GetComponent(type);
			}
			if (isComponent && obRef is Component c2)
			{
				return c2.GetComponent(type);
			}
			
			return null;
		}

		private GenericMenu GetMenu(UnityEngine.Object owner, MethodInfo currentMethod, Action<MethodInfo> fn)
		{
			var m = new GenericMenu
			{
				allowDuplicateNames = true
			};

			var items = FindAllCallables(owner, _optionCtx);
			
			m.AddItem(_NO_FN_LABEL, false, () => fn.Invoke(null));

			m.AddSeparator(string.Empty);
			
			var groupNames = new HashSet<string>();

			foreach(var it in items)
			{
				var tn = it.targetType.Name;

				var groupName = tn;

				EnsureUnique(groupNames, ref groupName);

				foreach (var pm in it.properties)
				{
					var methodName = GetMenuDisplayName(pm);
					var active = pm == currentMethod;
					m.AddItem(new GUIContent($"{groupName}/{methodName}"), active, () =>
					{
						fn.Invoke(pm);
					});
				}

				if (it.properties.Count > 0 && it.methods.Count > 0)
				{
					m.AddSeparator(groupName + "/");
				}
				
				foreach (var pm in it.methods)
				{
					var methodName = GetMenuDisplayName(pm);
					var active = pm == currentMethod;
					m.AddItem(new GUIContent($"{groupName}/{methodName}"), active, () =>
					{
						fn.Invoke(pm);
					});
				}
			}

			if (m.GetItemCount() == 2)
			{
				m.AddDisabledItem(_NO_OPTS_LABEL);
			}
			return m;
		}

		private static readonly GUIContent _NO_OPTS_LABEL = new ("(no options)");

		private static MethodInfo ParseMethodString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			var segments = str.Split(';');

			if (segments.Length != 4)
			{
				return null;
			}

			var returnType = Type.GetType(segments[1]);
			if (returnType == null)
			{
				return null;
			}
			
			var outerType = Type.GetType(segments[3]);
			if (outerType == null)
			{
				return null;
			}

			Type[] argTypes = Array.Empty<Type>();

			if (!string.IsNullOrEmpty(segments[2]))
			{
				var argTypeNames = segments[2].Split(';');
				argTypes = new Type[argTypeNames.Length];
			
				for (int i = 0; i < argTypes.Length; i++)
				{
					argTypes[i] = Type.GetType(argTypeNames[i]);
					if (argTypes[i] == null)
					{
						return null;
					}
				}
			}

			var methodName = segments[0];
			var m = outerType.GetMethod(methodName, ANY_INSTANCE_MEMBER, null, argTypes, null);

			if (m?.ReturnType != returnType)
			{
				return null;
			}
			return m;
		}

		private static string StringifyMethod(MethodInfo m)
		{
			var oType = m.ReflectedType!;
			var sb = new StringBuilder();
			sb.Append(m.Name);
			sb.Append(';');
			sb.Append(m.ReturnType);
			sb.Append(',');
			sb.Append(m.ReturnType.Assembly.GetName().Name);
			sb.Append(';');
			var i = -1;
			var pars = m.GetParameters();
			foreach (var par in pars)
			{
				i++;
				sb.Append(par.ParameterType.FullName);
				sb.Append(',');
				sb.Append(par.ParameterType.Assembly.GetName().Name);
				if (i < pars.Length - 1)
				{
					sb.Append('|');
				}
			}
			sb.Append(';');
			sb.Append(oType.FullName);
			sb.Append(',');
			sb.Append(oType.Assembly.GetName().Name);
			return sb.ToString();
		}

		private static CallableInfo[] FindAllCallables(UnityEngine.Object ob, in OptionContext ctx)
		{
			if (!ob)
			{
				return Array.Empty<CallableInfo>();
			}

			if (!TryGetGameObject(ob, out GameObject go))
			{
				_singleItem[0] = GetCallables(ob.GetType(), ctx);
				return _singleItem;
			}

			var components = go.GetComponents<Component>();
			var targets = new CallableInfo[components.Length + 1];
			targets[0] = GetCallables(typeof(GameObject), ctx);
			for (var i = 0; i < components.Length; i++)
			{
				targets[i + 1] = GetCallables(components[i].GetType(), ctx);
			}
			return targets;
		}
		
		public const BindingFlags ANY_INSTANCE_MEMBER
		= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		
		public const BindingFlags PUBLIC_INSTANCE_MEMBER
		= BindingFlags.Instance | BindingFlags.Public;

		private static CallableInfo GetCallables(Type targetType, in OptionContext ctx)
		{
			List<MethodInfo> props = new();
			List<MethodInfo> methods = new();

			var bFlags = ctx.flags.HasFlag(EOBjectMethodRef.NonPublic)
			? ANY_INSTANCE_MEMBER
			: PUBLIC_INSTANCE_MEMBER;
			
			foreach (var m in targetType.GetMethods(bFlags))
			{
				if (m.DeclaringType == targetType && m.IsPrivate)
				{
					continue;
				}

				if (m.IsPrivate && !ctx.flags.HasFlag(EOBjectMethodRef.NonPublic))
				{
					continue;
				}

				if (m.ReturnType != ctx.returnType)
				{
					continue;
				}

				var isProperty = IsPropertyMethod(m);
				
				if (isProperty && !ctx.flags.HasFlag(EOBjectMethodRef.Property))
				{
					continue;
				}

				if (ctx.argTypes != null)
				{
					var pars = m.GetParameters();

					if (pars.Length != ctx.argTypes.Length)
					{
						continue;
					}

					var validPars = true;
					for (int i = 0; i < pars.Length; i++)
					{
						if (pars[i].ParameterType != ctx.argTypes[i])
						{
							validPars = false;
							break;
						}
					}

					if (!validPars)
					{
						continue;
					}
				}

				(isProperty ? props : methods).Add(m);
			}

			return new CallableInfo
			{
				targetType = targetType,
				properties = props,
				methods = methods,
			};
		}
		
		private static bool IsPropertyMethod(MethodInfo m)
		{
			return
			m.Name.Length > 4
			&& m.IsSpecialName
			&& m.Name[3] == '_';
		}

		private static void EnsureUnique(HashSet<string> set, ref string key)
		{
			var pi = 1;
			var initial = key;
			while (set.Contains(key))
			{
				key = $"{initial} ({pi})"; pi++;
			}
			set.Add(key);
		}
		
		private static bool TryGetGameObject(UnityEngine.Object ob, out GameObject go)
		{
			go = null;
			if (!ob)
			{
				return false;
			}
			var tt = ob.GetType();

			if (typeof(Component).IsAssignableFrom(tt))
			{
				go = (ob as Component)!.gameObject;
			}
			else if (tt == typeof(GameObject))
			{
				go = ob as GameObject;
			}
			return go;
		}
		
		private const string _ACCESS_TOKENS = "-#?+";

		public static string GetMenuDisplayName(MethodInfo m)
		{
			var rt = GetNameOrAlias(m.ReturnType);
			var n = GetDisplayName(m);

			var parameters = m.GetParameters();

			var pnames =
			string.Join(", ", parameters
			.Select(x => GetNameOrAlias(x.ParameterType)));

			var accessToken = _ACCESS_TOKENS[GetAccessLevel(m)];

			var accessPrefix = $"[{accessToken}]";

			if (m.IsSpecialName)
			{
				if (m.ReturnType == typeof(void))
				{
					return $"{accessPrefix} {GetNameOrAlias(parameters[0].ParameterType)} {n}";
				}
				return $"{accessPrefix} {rt} {n}";
			}
			return $"{accessPrefix} {rt} {n} ({pnames})";
		}
		
		private static string GetDisplayName(MethodInfo m)
		{
			return IsPropertyMethod(m) ? GetOwningPropertyName(m) : m.Name;
		}
		
		private static byte GetAccessLevel(MethodInfo m)
		{
			if (m.IsFamily) { return 1; }
			if (m.IsPrivate) { return 0; }
			return 3;
		}
		
		private static string GetOwningPropertyName(MethodInfo m)
		{
			return m.Name[4..]; // get_ or set_
		}
		
		private static string GetNameOrAlias(Type t) => _TYPE_ALIAS.GetValueOrDefault(t) ?? t.Name;
		
		private static readonly Dictionary<Type, string> _TYPE_ALIAS = new()
		{
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
			{ typeof(int), "int" },
			{ typeof(string), "string" },
			{ typeof(double), "double" },
			{ typeof(float), "float" },
			{ typeof(bool), "bool" },
			{ typeof(long), "long" },
			{ typeof(void), "void" },
			{ typeof(object), "object" },
		};

	
	}
}

#endif