<img src="https://raw.githubusercontent.com/Smidgenomics/unity.plugins/master/attributes/banner.png" width="100%"/>


# ℹ️ Features

* Collection of highly flexible, general-use property attributes and decorators.
* Does not require custom inspector.
* 🤞 Reasonably lightweight.

<br/>

# 📦 Install

> Minimum Unity version: 2022.3

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-attributes.git#<version_tag>`


<br/>


# 🚀 Overview

Attributes break down into four categories: Drawers, Decorators, Modifiers, and Standalone.

* Drawers modify how fields are displayed, and in some cases can be chained.
* Decorators add static elements above fields. Built-in examples include Unity's `[Header]` and `[Space]` attributes
* Modifiers draw nothing but supply additional options to drawers, such as custom labels, indents, or buttons.
* Standalone attributes are simple property drawers that do not inherit from this project's base drawer but modify the drawing of fields in some minor way. 

## ⚡ Drawers

* [`DefaultDrawer`](#defaultdrawer)
* [`EditCondition`](#editcondition)
* [`Expand`](#expand)
* [`Inline`](#inline)
* [`Dropdown`](#dropdown)
* [`InstancedReference`](#instancedreference)
* [`Box`](#box)
* [`Foldout`](#foldout)
* [`Reorderable`](#reorderable) (Unity 6.0+)
* [`SearchType`](#searchtype)
* [`SearchEnum`](#searchenum)
* [`NavMeshAgentID`](#navmeshagentid)
* [`NavMeshAreaID`](#navmeshareaid)
* [`ProjectLayer`](#projectlayer)
* [`ProjectSortLayer`](#projectsortlayer)
* [`ProjectTag`](#projecttag)
* [`ProjectScene`](#projectscene)
* [`ProjectPath`](#projectpath)
* [`BlendShape`](#blendshape)
* [`AnimatorParameter`](#animatorparameter)
* [`RendererMaterial`](#renderermaterial)
* [`HexColor`](#hexcolor)
* [`Slider`](#slider)
* [`IntervalSlider`](#intervalslider)
* [`Progress`](#progress)
* [`Switch`](#switch)
* [`Tabs`](#tabs)
* [`TextBox`](#textbox)

## 🔧 Modifiers

* [`FieldButton`](#fieldbutton)
* [`FieldOptions`](#fieldoptions)
* [`InlineWidth`](#inlinewidth)
* [`InlineHidden`](#inlinehidden)

## ⚡ Decorators

* [`BoxHeader`](#boxheader)
* [`Alert`](#alert)
* [`Comment`](#comment)
* [`Link`](#link)
* [`StaticButton`](#staticbutton)

## ⚡ Standalone

* [`HideLabel`](#hidelabel)

<br/>

# 🚀 Attributes


## ⚙️ Drawers


### `[DefaultDrawer]`

> Fields: Any

Draws the default property drawer. This attribute exists to allow modifier attributes like buttons to work with regular drawers.


### `[EditCondition]`

> Fields: Any

Toggles field hidden/read-only depending on supplied conditional expression.

```cs
[FieldOptions]
public bool hideToggle;

[EditCondition("hideToggle", hide:true)]
[FieldOptions(indent:1)]
public string hiddenOnToggle;

public int intValue;

public TestEnum enumValue;

// show on enum value
[EditCondition("enumValue == Value1")]
public string showIfValue1;

// show using int field
[EditCondition("intValue > 10")]
public string showIfInt;


enum TestEnum
{
	None = 0,
	Value1 ,
	Value2,
	Value3,
}
```


### `[Expand]`

> Fields: Class, Struct

Expands all child fields. Label can be optionally hidden.

```cs
[Serializable]
public struct ExpandableStruct
{
	public string name;
	public Texture2D icon;
}

// show child props indented
[Expand]
public ExpandableStruct expanded1;

// only show child props
[Expand(innerOnly:true)]
public ExpandableStruct expanded2;
```

### `[Inline]`

> Fields: Class, Struct

Inlines all child fields in a single row.

* `[InlineWidth]` can be used to specify the preferred size of specific fields.

* `[InlineHidden]` can be used to exclude fields from being inlined.

```cs
[Inline]
public Vector3 inlinedVector;

[InlineWidth("key", 30f)]
[Inline]
public InlinedType inlinedCustom;

[Serializable]
struct InlinedType
{
	public string key;
	public string name;
	[InlineWidth(40f)]
	public int count;
	[InlineWidth(0.25f)]
	public Texture2D icon1;
	[InlineWidth(0.25f)]
	public Texture2D icon2;
}
```

### `[Dropdown]`

> Fields: Any

Shows a dropdown list of values for field. Values can be supplied directly, or a reference to an options method can be used which returns an IEnumerable of (label,value) tuplies.

Has special behaviour when placed on UnityEngine.Object fields where values can be supplied as folder paths or asset GUIDs.

```cs
[Dropdown("option1", "option2")]
public string stringValue;

[Dropdown(0.5f, 1.2f, 2.4f)]
public float floatValue;

[Dropdown(0, 10)]
public int intValue;

// load asset options
[Dropdown("Assets/Textures/icons/", "460278ced8f4db444b2b4cd02a08f984")]
public Texture2D icon;

// read options from static method
[Dropdown("GetColorOptions;MyType, MyModule")]
public Color colorValue;

class MyType
{
	public static List<(string, Color)> GetColorOptions()
	{
		return new ()
		{
			("White", Color.white),
			("Black", Color.black),
			("Clear", Color.clear),
			("Red", Color.red),
			("Blue", Color.blue),
			("Green", Color.green),
			("Yellow", Color.yellow),
			("Magenta", Color.magenta),
		};
	}
}
```

### `[InstancedReference]`

> Fields: Class

Draws a popup list of new-able types for field with `[SerializeReference]` attribute.

```cs
[InstancedReference]
[SerializeReference]
public BaseClass instance;

[Serializable]
abstract class BaseClass {}

[Serializable]
class ClassA : BaseClass
{
	public int myValueFromA;
}

[Serializable]
class ClassB : BaseClass
{
	public int myValueFromB;
}
```

### `[Box]`

> Fields: Any

Wraps field in outlined box.

Tips:
* Combine with `[Expand(innerOnly:true)]`.

```cs
[Box]
[Expand(innerOnly:true)]
struct GroupedFields fields;

[Serializable]
struct GroupedFields
{
	public int count;
	public string name;
}
```

### `[Foldout]`

> Fields: Any

Wraps field in foldout box.

Options:
* Label. Uses field label by default.
* Icon GUID. Reference to texture asset.
* Icon coords. Coordinates of icon in texture file (texture atlas).

Tips:
* Combine with `[Expand(innerOnly:true)]`.

```cs
[Foldout(iconGUID:"b4508e266a1d41445a0cb18bd9acf8d6")]
[Expand(innerOnly:true)]
public FoldableStruct foldedStruct;

[Serializable]
struct FoldableStruct
{
	public int count;
	public string name;
}
```

### `[Reorderable]` <small>(Unity 6.0+)</small>

> Fields: Array/List

Draws array as reorderable drag list with various customization options and helpers.

```cs
// draws collapsed list of colliders
[Reorderable((EReorderable.Minimal|EReorderable.Foldable))]
public Collider[] _colList;

// hide size input
[Reorderable(EReorderable.Minimal & ~EReorderable.Resizable)]
public string[] stringList;

// draws standard-looking list
[Reorderable(EReorderable.Standard)]
public string[] standardList;
```

### `[SearchType]`

> Fields: String

Provides a searchable popup for assembly types with various filtering options.

```cs
[SearchType]
public string anyType;

// only show component types
[SearchType(baseType: typeof(Component))]
public string componentType;

// only show system module types
[SearchType(assemblies: new string[]{ "mscorlib" })]
public string systemType;
```

### `[SearchEnum]`

> Fields: Enum

Shows a searchable popup of values in enum.

```cs
[SearchTSearchEnumype]
public KeyCode someKey;
```

### `[NavMeshAgentID]`

> Fields: Int

Draws popup of NavMesh agent types in project.

```cs
[NavMeshAgentID]
public int agentID;
```

### `[NavMeshAreaID]`

> Fields: Int

Draws popup of NavMesh area types in project.

```cs
[NavMeshAreaID]
public int areaID;
```

### `[ProjectLayer]`

> Fields: Int

Dropdown of project layer indices.

```cs
[ProjectLayer]
public int layerIndex;
```

### `[ProjectSortLayer]`

> Fields: Int

Dropdown of project sorting layer indices.

```cs
[ProjectSortLayer]
public int sortLayer;
```

### `[ProjectTag]`

> Fields: String

Dropdown of project tags.

```cs
[ProjectTag]
public string sortLayer;
```

### `[ProjectScene]`

> Fields: Int, String

Shows dropdown of scenes in project and saves value as scene path or index in build settings.


```cs
[ProjectScene]
public string scenePath;

[ProjectScene(buildOnly:true)]
public int sceneIndex;
```

### `[ProjectPath]`

> Fields: String

Draws popup of paths relative to project root directory. Can be set to either folder or file paths.

```cs
// show blender files
[ProjectPath(pattern:"*.blend")]
public string filePath;

// show folder paths
[ProjectPath(EProjectPath.Folder)]
public string folderPath;
```

### `[BlendShape]`

> Fields: Int, String

Shows dropdown of blend shapes in referenced skinned mesh renderer. Saves value as either string (shape name) or int (index in renderer array).


```cs
public SkinnedMeshRenderer myRenderer;

[AnimatorParameter("myRenderer")]
public string blendShapeName

[AnimatorParameter("myRenderer")]
public int blendShapeIndex
```

### `[AnimatorParameter]`

> Fields: Int, String

Shows dropdown of parameters in referenced animator. Saves value as either string (param name) or int (param hash).

```cs
public Animator animator;

[AnimatorParameter("animator")]
public string paramName;

[AnimatorParameter("animator")]
public int paramHash;

// restrict to float or int params
[AnimatorParameter("animator", EAnimatorParameter.Float|EAnimatorParameter.Int)]
public string floatParam;
```

### `[RendererMaterial]`

> Fields: Int

Shows dropdown of materials in referenced renderer. Saves value as int index to material in renderer material array.

```cs
public Renderer myRenderer;

[RendererMaterial("myRenderer")]
public int materialIndex
```

### `[HexColor]`

> Fields: String

Draws color picker for string field and saves as hex color value.

```cs
[HexColor]
public string hexColor = "#f00";
```

### `[Slider]`

> Fields: Numeric

Identical to `[Range]` attribute, but provides options for step and precision.

```cs
[Slider(1f,10f,1)]
public float sliderPrecision;

[Slider(1f,10f,0.5f)]
public float sliderStep;

[Slider(1,10)]
public int sliderInt;
```

### `[IntervalSlider]`

> Fields: Class/Struct

Draws Min/Max slider and saves values to two separate child fields.

Options:
* Min/Max values
* Step
* Min/Max fields

```cs
// defaults to x/y fields
[IntervalSlider(0f, 1f)]
public Vector2 vectorInterval;

// specify min/max fields
[IntervalSlider(0f, 1f, fMin:"min", fMax:"max", step:0.25f)]
public MyInterval interval; 

[Serializable]
struct MyInterval
{
	public float min,max;
}
```

### `[Progress]`

> Fields: Numeric

Draws numeric field as a progress bar.

```cs
[Progress(0, 100)]
public float health = 50;

[Progress(0, 100, "Status")]
public float health = 50;
```

### `[Switch]`

> Fields: Enum/Flags, Bool, LayerMask

Draws a toggle switch.

For flag and layermask fields, a switch will be drawn for every value.

```cs
[Switch]
public bool simpleSwitch;

[Switch("Off", "On")]
public bool labeledSwitch;

[Switch]
public EnumFlags enumOptions;

[Flags]
enum EnumFlags
{
    Item1 = 1,
    Item2 = 2,
    Item3 = 4,
}
```

### `[Tabs]`

> Fields: Enum/Flags, Bool

Draws a toolbar of buttons.

```cs
[Flags]
enum Options
{
	Item1 = 1,
	Item2 = 2,
	Item3 = 4,
}

[Tabs]
public Options flagTabs;

[Tabs]
public bool boolTabs;

[Tabs(vertical:true)]
public Options verticalTabs;
```

### `[TextBox]`

> Fields: String

Draws text area that resizes automatically.

```cs
[TextBox(minLines:3)]
public string textArea;
```

<!--==============================================-->
<!--=================MODIFIERS====================-->
<!--==============================================-->


## 🔧 Modifiers

Modifiers work in conjunction with property drawers in that they modify their drawing in some way.

Notes:

* Modifiers only work if at least one attribute from this project is present. `[DefaultDrawer]` can be used to get them to work with regular drawers.


### `[FieldButton]`

> Fields: Any

Draws a button above field. Can reference method relative to field, or any static method with absolute path to type.

Options:
* Width (ratio)
* Label (defaults to method name)
* Usability flags (when is button enabled)

Notes:

* Method in declaring type can be referenced by prefixing name with `~`.
* Methods cannot change values of struct types as their value is always boxed when invoking the function.

```cs
[FieldButton("SetMyValue", "Set=100",  args:new object[]{ 100 }, flags:EFieldUsable.Play, width:0.5f)]
[FieldButton("~OuterMethod", width:0.5f)]
[FieldButton("LogValue;StaticClass, MyModule", width:1f)]
[DefaultDrawer]
public OwnerOfFunctions fieldWithButtons;

private void OuterMethod()
{
	Debug.Log("Outer method called!");
}

[Serializable]
class OwnerOfFunctions
{
	public int myValue = 10;

	public void SetMyValue(int v)
	{
		myValue = v;
	}
}

class StaticClass
{
	public static void LogValue(int v)
	{
		Debug.Log(v);
	}
}
```

### `[FieldOptions]`

> Fields: Any

Allows overrides to be specified for given field.

Options:
* Label. Overrides default display name. Hides if set to `null`.
* Use flags. Controls when field is editable (play mode etc.).
* Indent. Extra indent added to field.


### `[InlineWidth]`

> Fields: With `[Inline]`

Supplies desired field width to `[Inline]` attribute. Can be placed on inlined field with name of child field, or on child field itself.

```cs
[InlineWidth("count", 40f)] // specific inner field
[Inline]
public InlineType inlined;

[Serializable]
struct InlineType
{
	public int count;
	public string text;
	[InlineWidth(40f)]
	public bool check;
}
```

### `[InlineHidden]`

> Fields: With `[Inline]`

Marks specific field to be excluded from being inlined, effectively hiding it when `[Inline]` is used.

```cs
[Inline]
public InlineType inlined;

[Serializable]
struct InlineType
{
	public int count;
	public string text;
	[InlineHidden]
	public bool hideMe;
}
```

<!--==============================================-->
<!--=================DECORATORS===================-->
<!--==============================================-->

## ⚡ Decorators

> Fields: Any

Decorators are simple standalone elements drawn above fields.

### `[BoxHeader]`

Draws large label inside outlined box above field.

```cs
[BoxHeader("My Section")]
public string documentedField1;

[BoxHeader("My Other Section", alignment:TextAnchor.MiddleCenter, style:FontStyle.Normal)]
public string documentedField2;
```

### `[Comment]`

Draws comment paragraph above field.

```cs
[Comment("Something useful")]
public string documentedField;
```

### `[Alert]`

Draws tinted alert with icon over field.

(Uses CSS-Bootstrap-inspired colors.)

```cs
	[Alert("I'm important!", EAlert.Error)]
	[Alert("I'm mildly important.", EAlert.Warning)]
	[Alert("I'm noteworthy.", EAlert.Info)]
	public string documentedValue;
```

### `[Link]`

Draws link to external site above field.

```cs
[Link("https://www.reddit.com/r/lotrmemes/", "Serious Documentation")]
public string documentedField;
```

### `[StaticButton]`

Draws button above field.

StaticButton works almost exactly as FieldButton with some limitations. Bcause it's a decorator, which has no knowledge of the field it's placed on, the method reference must be an absolute path to its type

```cs
[StaticButton("SayHi;StaticGreets, MyModule")]
[StaticButton("LogValue;StaticGreets, MyModule", label: "Log", args: new object[]{ 10 })]
public string documentedField;

class StaticGreets
{
	public static void SayHi()
	{
		Debug.Log("Hello, wurst!");
	}

	public static void LogValue(int v)
	{
		Debug.Log("Your value is: " + v);
	}
}
```

<!--==============================================-->
<!--=================STANDALONE===================-->
<!--==============================================-->


## ⚡ Standalone

### `[HideLabel]`

> Fields: Any

Hides prefix label of field.

```cs
[HideLabel]
public string documentedField;
```