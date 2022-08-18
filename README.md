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

* Collection of general use drawer and decorator attributes.
* Stripped in production: `[Conditional("UNITY_EDITOR")]`
* 🤞 Reasonably lightweight.


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

### 🟢 Decorators

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX HEADER -->

<details>
    <summary><b>📝 BoxHeader</b></summary>

<br/>

<img src="/.github/preview/boxheader.png" />

```cs
[BoxHeader("Example Header")]
[TextArea]
public string documentedField;
```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX COMMENT -->

<details>
    <summary><b>📝 BoxComment</b></summary>

<br/>

<img src="/.github/preview/boxcomment.png" />

```cs
[BoxComment("Some information about bla")]
[TextArea]
public string documentedField;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<!-- BOX LINK -->

<details>
    <summary><b>📝 BoxLink</b></summary>

<br/>

<img src="/.github/preview/boxlink.png" />

```cs
[BoxLink("Documentation", "https://en.wikipedia.org/wiki/Slartibartfast")]
[TextArea]
public string documentedField;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔳 StaticAction</b></summary>

<br/>

<img src="/.github/preview/staticaction.png" />

```cs
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

[StaticAction("Say Hi", "SayHi", typeof(StaticGreets))]
[StaticAction("Log 10", "LogValue", typeof(StaticGreets), 10)]
public string staticActionField;

```

</details>


<!--======================================================-->
<!--###################### DRAWERS #######################-->
<!--======================================================-->


### 🟠 Property Drawers

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>📏 Inline</b></summary>
    

<br/>

<img src="/.github/preview/inline.png" />

```cs

[System.Serializable]
public struct T1 { public string name; public Texture2D icon; }

[Inline] public Vector3 inlined1 = default;
[FieldSize("name", 40f)]
[Inline] public T1 inlined2 = default;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>📏 Expand</b></summary>
    

<br/>


<img src="/.github/preview/expand.png" />

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

<img src="/.github/preview/slider.png" />


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

<img src="/.github/preview/slider01.png" />


```cs
[Slider01]
public float slider01;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->



<details>
    <summary><b>🎨 HexColor</b></summary>

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

<img src="/.github/preview/searchtype.png" />
<br/>
<img src="/.github/preview/typefind.png" />


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

<img src="/.github/preview/dropdown.png" />


```cs
[DropdownString("option1", "option2")]
public string _string;

[DropdownFloat(0.5f, 1.2f, 2.4f)]
public float _float;

[DropdownColor("red", "blue", "cyan")]
public Color _color;

[DropdownBool("Off", "On")]
public bool _bool;

[DropdownInt(0, 10)]
public int _int;

[DropdownAsset("Assets/Demo/")]
public Texture2D _texture;
```

</details>





<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔻 Layer</b></summary>

<br/>

<img src="/.github/preview/layer.png" />


```cs
[Layer]
public int _layer;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 SortLayer</b></summary>

<br/>

<img src="/.github/preview/sortlayer.png" />


```cs
[SortLayer]
public int _sortingLayer;
```

</details>

<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 Tag</b></summary>

<br/>

<img src="/.github/preview/tag.png" />


```cs
[Tag]
public string _tag;
```

</details>


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->


<details>
    <summary><b>🔻 BuildScene</b></summary>

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



### 🔵 Modifiers


<!--======================================================-->
<!--######################################################-->
<!--======================================================-->

<details>
    <summary><b>🔳 FieldAction</b></summary>

<br/>

<img src="/.github/preview/fieldaction.png" />


```cs
[System.Serializable]
internal class OwnerOfFunctions
{
    public int myValue = 10;

    public void SetMyValue(int v)
    {
        myValue = v;
    }

    public void CallMe()
    {
        Debug.Log("Yay!");
    }

    public void CallMeAsWell()
    {
        Debug.Log("OMG YAY");
    }
}


class MyScript : MonoBehaviour
{
    [FieldAction("Action 1", "CallMe")]
    [FieldAction("Action 2", "CallMeAsWell")]
    [FieldAction("Set value: 100", "SetMyValue", 100, onlyPlayMode = true)]
    [FieldAction("Call Target", "ScriptMethod", callRoot = true)]
    [Expand]
    public OwnerOfFunctions fieldWithActions;

    private void ScriptMethod()
    {
        Debug.Log("Script method called!");
    }
}

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





