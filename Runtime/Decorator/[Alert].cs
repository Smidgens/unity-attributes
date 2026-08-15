// smidgens @ github

namespace Smidgenomics.Unity.Attributes
{
	using System;

	/// <summary>
	/// Type supplied to [Alert]
	/// </summary>
	public enum EAlert
	{
		Info,
		Warning,
		Error
	}

	/// <summary>
	/// Draws info/warning/error box
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public sealed class AlertAttribute : __BaseDecorator
	{
		public AlertAttribute
		(
			string text,
			EAlert type = EAlert.Info
		)
		{
			this.text = text ?? string.Empty;
			this.type = type;
		}

		internal EAlert type { get; }
		internal string text { get; }
	}
}

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(AlertAttribute))]
	internal sealed class _AlertAttribute : __DecoratorDrawer<AlertAttribute>
	{
		protected override float GetHeight(in float w)
		{
			var s = GetStyle();
			var h = s.CalcHeight(_label, w - ICO_W);
			return Mathf.Max(h, ICO_W);
		}

		private static readonly float ICO_W = EditorGUIUtility.singleLineHeight * 1.5f;

		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.text);
		}

		private GUIStyle GetStyle() => DrawerStyles.ParagraphBoldSM;
		
		protected override void OnContent(in Rect p)
		{
			var s = GetStyle();
			var pos = p;

			var box = p;
			box.width -= 2f;
			box.center = p.center;

			var (icon, tintColor) = _ALERT_STYLES.GetValueOrDefault(_Attribute.type, default);

			tintColor = tintColor.Fade(0.75f);
			
			EditorGUI.DrawRect(box, Color.black);
			EditorGUI.DrawRect(box, tintColor * 0.7f);
			GUI.Box(pos, GUIContent.none, EditorStyles.helpBox);
			var icoRect = pos.SliceLeft(ICO_W);
			icoRect.height = icoRect.width;
			icoRect = icoRect.Resized(-pos.height * 0.3f);

			var shadowOffset = Vector2.one * -0.75f;
			
			var shadowIcon = icoRect;
			shadowIcon.position += -shadowOffset;
			
			var shadowPos = pos;
			pos.position += shadowOffset;

			var shadowColor = Color.black.Fade(0.5f);
			
			PluginAtlas.DrawIcon(shadowIcon, icon, shadowColor);
			DrawText(shadowPos, _label, s, shadowColor);
			
			PluginAtlas.DrawIcon(icoRect, icon, Color.white);
		
			DrawText(pos, _label, s, Color.white);
		}

		private static readonly Dictionary<EAlert, (EAtlasIcon, Color)> _ALERT_STYLES = new()
		{
			// bootstrap gonna sue...
			{ EAlert.Info, (EAtlasIcon.Info, new Color(0.09f, 0.635f, 0.722f)) },
			{ EAlert.Warning, (EAtlasIcon.Warning, new Color(1f, 0.757f, 0.0275f)) },
			{ EAlert.Error, (EAtlasIcon.Error, new Color(0.863f, 0.208f, 0.2706f)) },
		};

		private GUIContent _label;

		

	}
}

#endif