// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEngine;
	using UnityEditor;
	using System.Reflection;

	[CustomPropertyDrawer(typeof(StaticActionAttribute))]
	internal class StaticAction_ : __DecoratorDrawer<StaticActionAttribute>
	{
		protected override void OnInit()
		{
			_label = new GUIContent(_Attribute.Label);
			_method = _Attribute.GetMethod();
		}

		protected override (byte, byte) GetMargin()
		{
			return (0,0);
		}

		protected override (byte, byte, byte, byte) GetPadding()
		{
			return (0, 0, 0, 0);
		}

		protected override float GetHeight(in float w) => 19f;

		protected override void OnContent(in Rect pos)
		{
			var te = GUI.enabled;

			var disabled =
			_method == null
			|| (!Application.isPlaying && _Attribute.onlyPlayMode);

			GUI.enabled = !disabled;
			if(GUI.Button(pos, _label))
			{
				_method.Invoke(null, _Attribute.Args);
			}
			GUI.enabled = te;
		}

		private GUIContent _label = null;
		private MethodInfo _method = null;
	}
}

#endif