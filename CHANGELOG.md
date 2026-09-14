# 0.6.0

## Added

* ProjectRenderLayer attribute.
* Add InstancedReference flag to configure if types should be grouped by assembly.

## Fixed

* Fix bug in AnimatorParameter.
* Fix build errors caused by editor code in runtime.

## Changed

* Overhauled Dropdown, InstancedReference, and SearchEnum attributes to use advanced dropdown drawer.



# 0.5.0

## Added

### Attributes

* Dropdown (generic)
* EditCondition
* Box
* Foldout
* Reorderable
* StableGUID
* GlobalObjectID
* ObjectMethodReference
* SearchEnum
* NavMeshAgentID
* NavMeshAreaID
* IntervalSlider
* Progress
* TextBox
* Texture
* Divider
* FieldLabel
* FieldIndent
* ProjectPath
* Alert

## Changed

### Renamed Attributes

* BoxComment -> Comment.
* BoxLink -> Link.
* StaticAction -> StaticButton.
* FieldAction -> FieldButton.
* BuildScene -> ProjectScene.

## Removed

* Dropdown_ variants (replaced with single Dropdown).
* ProjectFile (replaced with ProjectPath).
* ProjectFolder (replaced with ProjectPath).
