![](/.github/.banner.png?raw=true "")

<!--
snippets

<details>
    <summary><b>TITLE</b></summary>
    <img src="/.github/preview/IMAGE.png" />
    <p></p>

```cs

```

</details>

-->


# ℹ️ Features

* Collection of useful field attributes and decorators for the Unity inspector.
* Simple set-up.
* Combine attributes in a multitude of ways.
* Automatically stripped in production: `[Conditional("UNITY_EDITOR")]`

<br/>

<br/>

# 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-attributes.git#<version_tag>`


<br/>

# 🚀 Usage

<!--======================================================-->
<!--#################### DECORATORS ######################-->
<!--======================================================-->

## Decorators

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX HEADER -->

<details>
    <summary><b>📝 BoxHeader</b></summary>
    <img src="/.github/preview/decorators.png" />
    <p></p>

```cs

```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX COMMENT -->

<details>
    <summary><b>📝 BoxComment</b></summary>
    <img src="/.github/preview/decorators.png" />

```cs
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX LINK -->

<details>
    <summary><b>📝 BoxLink</b></summary>
    <img src="/.github/preview/decorators.png" />
    <p></p>

```cs

```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔳 StaticAction</b></summary>
    <img src="/.github/preview/IMAGE.png" />
    <p></p>

```cs

```

</details>




<!--======================================================-->
<!--###################### DRAWERS #######################-->
<!--======================================================-->

<br/>

## Property Drawers

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>📏 Inline</b></summary>
    <img src="/.github/preview/inlined.png" />

<br/>

* `object`

<br/>

```cs

```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>📏 Expand</b></summary>
    <img src="/.github/preview/expand.png" />

<br/>

* `object`

<br/>


```cs

[Serializable]
public struct T1
{
    public string name;
    public Texture2D icon;
}

[Serializable]
public struct T2
{
    public int someValue;
    public T1 nested;
}

[Expand]
public T1 expanded1;

[Expand]
public T2 expanded2;
```

</details>




<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔘 Tabs</b></summary>

<br/>

* `object` -> displays a button a for each immediate bool field
* `enum flags` -> displays a button for each flag

<br/>


<img src="/.github/preview/tabs.png" />

```cs

[System.Serializable]
struct ToggleData
{
    public bool item1,item2,item3;
}

[Tabs]
public ToggleData options;

```

```cs
[System.Flags]
enum Options
{
    Item1 = 1,
    Item2 = 2,
    Item3 = 4,
}

[Tabs]
public Options options;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔘 Switch</b></summary>

<br/>

* `bool` -> displays a single switch
* `enum flags` -> displays a switch for each flag

<br/>

<img src="/.github/preview/switch.png" />

```cs
[Switch]
public bool switch1;

[Switch("Off", "On")]
public bool switch2;

[Switch("Disabled", "Enabled")]
public bool switch3;
```
```cs
[System.Flags]
enum Options
{
    Item1 = 1,
    Item2 = 2,
    Item3 = 4,
}

[Switch]
public Options options;
```

</details>



<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🎚️ Slider</b></summary>

<br/>

* `int`
* `float`
* `double`

<br/>

<img src="/.github/preview/sliders.png" />


```cs
[Slider(1f,10f,1)]
public float precisionSlider;

[Slider(1f,10f,0.5f)]
public float stepSlider;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🎚️ Slider01</b></summary>

<br/>

`float`

<br/>

<img src="/.github/preview/sliders.png" />


```cs
[Slider01]
public float precisionSlider;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->



<details>
    <summary><b>🎨 HexColor</b></summary>

<br/>

`string` -> saves stringified hex color

<br/>

<img src="/.github/preview/hexcolor.png" />


```cs

[HexColor]
public string stringColor;

```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔎 SearchType</b></summary>

<br/>

`string` -> saves assembly qualified name of type

<br/>

<img src="/.github/preview/options.png" />


```cs

[SearchType]
public string anyType;

// only show static classes
[SearchType(onlyStatic = true)]
public string anyType;

// only show system types
[SearchType(assemblies = new string[]{ "mscorlib" })]
public string systemType;

// only show component types
[SearchType(baseTypes = new Type[]{ typeof(Component) })]
public string componentType;


```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔻 Dropdown__</b></summary>

<br/>

Variants:

* DropdownInt
* DropdownFloat
* DropdownBool
* DropdownString
* DropdownColor
* DropdownAsset
* DropdownType

<br/>

<img src="/.github/preview/options.png" />


```cs
[DropdownString("option1","option2")]
public string stringOption;

[DropdownAsset("Assets/Icons/")]
public string stringOption;

```

</details>





<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔻 Layer</b></summary>

<br/>

* `int`

<br/>

<img src="/.github/preview/layer.png" />


```cs
[Layer]
public int someLayer;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 SortLayer</b></summary>

<br/>

* `int`

<br/>

<img src="/.github/preview/layer.png" />


```cs
[SortLayer]
public int someSortingLayer;
```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 Tag</b></summary>

<br/>

* `string`

<br/>

<img src="/.github/preview/layer.png" />


```cs
[SortLayer]
public int someSortingLayer;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 BuildScene</b></summary>

<br/>

* `string` -> scene path
* `int` -> index in build settings

<br/>

<img src="/.github/preview/buildscene.png" />


```cs
[BuildScene]
public string scenePath;

[BuildScene]
public int sceneIndex;
```

</details>



<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔻 AnimatorParameter</b></summary>

<br/>

* `string` -> parameter name
* `int` -> parameter index

<br/>

<img src="/.github/preview/animatorparameter.png" />


```cs

public Animator myAnimator;

[AnimatorParameter("myAnimator")]
public string parameterName;

[AnimatorParameter("myAnimator")]
public int parameterIndex;
```

</details>



<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔻 RendererMaterial</b></summary>

<br/>

* `int` -> index of referenced renderer field 

<br/>

<img src="/.github/preview/renderermaterial.png" />


```cs
public Renderer myRenderer;

[AnimatorParameter("myRenderer")]
public int materialIndex
```

</details>



<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 BlendShape</b></summary>

<br/>

* `int` -> index of blend shape in ref. renderer
* `string` -> name of blend shape in ref. renderer

<br/>

<img src="/.github/preview/blendshape.png" />


```cs
public SkinnedMeshRenderer myRenderer;

[AnimatorParameter("myRenderer")]
public string shapeIndex

[AnimatorParameter("myRenderer")]
public int shapeName
```

</details>

<!--======================================================-->
<!--##################### MODIFIERS ######################-->
<!--======================================================-->

<br/>

## Modifiers


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔳 FieldAction</b></summary>

<br/>

<img src="/.github/preview/fieldaction.png" />


```cs

[Indent(1)]
[DefaultDrawer]
public int iAmIndented;

[System.Serializable]
class MyFancyType
{
    public int value;

    public void SetValue(int v)
    {
        value = v;
    }

    public void SayHi()
    {
        Debug.Log("Hi!");
    }
}

[FieldAction("SayHi")]
[FieldAction("SetValue", 10)]
[DefaultDrawer]
public MyFancyType fieldStuff;


```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>📏 Indent</b></summary>

<br/>

<img src="/.github/preview/indent.png" />


```cs

[Indent(1)]
[DefaultDrawer]
public int iAmIndented;

[Indent(2)]
[DefaultDrawer]
public int iAmMoreSo;

```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->





