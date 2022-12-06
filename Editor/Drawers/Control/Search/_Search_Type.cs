// smidgens @ github

#if UNITY_EDITOR

namespace Smidgenomics.Unity.Attributes.Editor
{
	using UnityEditor;

	[CustomPropertyDrawer(typeof(SearchTypeAttribute))]
	internal class _Search_Type : __ControlDrawer<SearchTypeAttribute>
	{
		protected override FieldType GetValidTypes() => FieldType.String;

		protected override void OnField(in DrawContext ctx)
		{
			Popup.AssemblyType(ctx.position, ctx.property, GetConstraints);
		}

		private bool _popupInit = false;

		private void InitConstraints()
		{
			_constraints = new TypeSearch.Constraints();
			_constraints.showAbstract = !_Attribute.hideAbstract;
			_constraints.staticOnly = _Attribute.onlyStatic;
			_constraints.derivedTypes = _Attribute.baseTypes;
			_constraints.namespaces = _Attribute.namespaces;
			_constraints.assemblies = _Attribute.assemblies;
			_constraints.includeHidden = _Attribute.showHidden;
			_popupInit = true;
		}

		private TypeSearch.Constraints GetConstraints()
		{
			if (!_popupInit) { InitConstraints(); }
			return _constraints;
		}

		private TypeSearch.Constraints _constraints = default;

	}
}

#endif