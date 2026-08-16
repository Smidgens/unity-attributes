// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;
	using System.Text.RegularExpressions;

	public sealed class EditConditionAttribute : __BaseControl
	{
		internal bool hide { get; }
		internal (bool, string, string, string) parsed { get; }

		public EditConditionAttribute(string expression, bool hide = true)
		{
			this.hide = hide;
			parsed = EParse.ParseExpression(expression.Replace(" ", ""));
		}
		
		private static class EParse
		{
			public static (bool, string, string, string) ParseExpression(string expression)
			{
				if (!HasCompareToken(expression))
				{
					var pattern = $"^{REG_VAR}$";
					if (!Regex.IsMatch(expression, pattern))
					{
						return default;
					}
					return (true, expression, string.Empty, string.Empty);
				}
				
				var rMatch = REGEX.Value.Match(expression);
				if (rMatch.Success)
				{
					var n = rMatch.Groups["name"].Value;
					var o = rMatch.Groups["op"].Value;
					var v = rMatch.Groups["value"].Value;
					return (true, n, o, v);
				}
				return default;
			}

			private static bool HasCompareToken(string expression)
			{
				return
				expression.Contains('<')
				|| expression.Contains('>')
				|| expression.Contains('=');
			}

			private const string REG_VAR = "((!?)[a-zA-Z_]+([a-zA-Z_]|[0-9])*)";
			private const string REG_OP = "(?<op>(\\<|\\>|==|\\<=|\\>=|\\!=))";
			private const string REG_FLOAT = "([+-]?([0-9]*[.])?[0-9]+)";

			private static readonly Lazy<Regex> REGEX = new(() =>
			{
				var lSide = $"(?<name>{REG_VAR})";
				var rSide = $"(?<value>({REG_FLOAT}|{REG_VAR})+)";
				var op = REG_OP;
				var pattern = $"^{lSide}{op}{rSide}$";
				return new Regex(pattern);
			});
		}
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System;
	using System.Reflection;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(EditConditionAttribute))]
	internal sealed class _EditConditionAttribute : __ControlDrawer<EditConditionAttribute>
	{
		protected override void OnInit()
		{
			
		}

		private (bool, string, string, string) _expressionParse;

		protected override float GetHeight(SerializedProperty prop, GUIContent label)
		{
			if (prop == null)
			{
				return 0;
			}

			var toggleState = GetState(prop);

			if (!toggleState && _Attribute.hide)
			{
				return 0f;
			}
			return EditorGUI.GetPropertyHeight(prop);
		}

		protected override void OnLabel(ref Rect pos, SerializedProperty prop, GUIContent l)
		{
			var toggleState = GetState(prop);
			
			if (!toggleState && _Attribute.hide)
			{
				return;
			}

			EditorGUI.indentLevel += _ExtraIndent;
			base.OnLabel(ref pos, prop, l);
			EditorGUI.indentLevel -= _ExtraIndent;
		}

		protected override void OnField(in DrawContext ctx)
		{
			var pos = ctx.position;
			var prop = ctx.property;

			if (pos.height == 0)
			{
				return;
			}

			var toggleState = GetState(prop);

			if (!toggleState && _Attribute.hide)
			{
				return;
			}

			var tEnabled = GUI.enabled;
			GUI.enabled &= toggleState;
			EditorGUI.PropertyField(pos, prop, GUIContent.none);
			GUI.enabled = tEnabled;
		}

		private (FieldInfo, bool) _otherField;

		private bool GetState(SerializedProperty currentProp)
		{
			if (!HasValidExpression())
			{
				return false;
			}

			var (_, lhs, op, rhs) = _Attribute.parsed;

			var lhsIndex = 0;

			var negateLeft = lhs.StartsWith('!');
			
			if (negateLeft)
			{
				lhsIndex++;
			}

			// note: cache if prop exists
			var lProp = currentProp.FindSibling(lhs.Substring(lhsIndex));
			if (lProp == null)
			{
				return false;
			}

			var bFlags = BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;

			if (!_otherField.Item2)
			{
				_otherField = (fieldInfo.DeclaringType!.GetField(lProp.name, bFlags), true);
			}

			if (_otherField.Item1 == null)
			{
				return false;
			}

			var otherType = _otherField.Item1.FieldType;

			if (lProp.propertyType == SerializedPropertyType.ObjectReference)
			{
				if (!string.IsNullOrEmpty(op))
				{
					return false;
				}
				return negateLeft ? lProp.objectReferenceValue : !lProp.objectReferenceValue;
			}

			// bool is lhs only
			if (lProp.IsBool())
			{
				if (!string.IsNullOrEmpty(op))
				{
					return false;
				}
				return negateLeft ? !lProp.boolValue : lProp.boolValue;
			}

			if (lProp.IsInt())
			{
				return EvalNumber(lProp.intValue, op, rhs);
			}

			if (lProp.IsFloat())
			{
				return EvalNumber(lProp.doubleValue, op, rhs);
			}

			if (lProp.IsEnum())
			{
				if (!Enum.TryParse(otherType, rhs, false, out var e))
				{
					return false;
				}

				if (op == "==")
				{
					return lProp.intValue == (int)e;
				}
				return lProp.intValue != (int)e;
			}

			return false;
		}

		private static bool EvalNumber(double lhs, string op, string rhsStr)
		{
			if (!double.TryParse(rhsStr, out var rhs))
			{
				Debug.Log("cock");
				return false;
			}
			
			if (op == "==")
			{
				return KindaSame(lhs, rhs);
			}

			if (op == "<=")
			{
				return lhs < rhs || KindaSame(lhs, rhs);
			}

			if (op == "!=")
			{
				return lhs < rhs || KindaSame(lhs, rhs);
			}

			if (op == ">=")
			{
				return lhs > rhs || KindaSame(lhs, rhs);
			}
			
			if (op == ">")
			{
				return lhs > rhs;
			}
			
			if (op == "<")
			{
				return lhs < rhs;
			}
			return false;
		}

		private static bool KindaSame(double v1, double v2)
		{
			// (double) Mathf.Abs(b - a) < (double) Mathf.Max(1E-06f * Mathf.Max(Mathf.Abs(a), Mathf.Abs(b)), Mathf.Epsilon * 8f);
			return Math.Abs(v1 - v2) < .00001;
		}

		private bool HasValidExpression()
		{
			return _Attribute.parsed.Item1;
		}
	}

}

#endif