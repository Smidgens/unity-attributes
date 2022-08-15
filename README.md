![](/.github/.banner.png?raw=true "")

<!--
snippets


<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  

</details>

-->


## ℹ️ Features

* Collection of useful field attributes and decorators for the Unity inspector.
* Simple set-up.
* Combine attributes in a multitude of ways.
* Automatically stripped in production: `[Conditional("UNITY_EDITOR")]`

<br/>

<br/>

## 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-attributes.git#<version_tag>`


<br/>

## 🚀 Usage

**Note:** Requires an explicit assembly reference to `Smidgenomics.Unity.Attributes`.

### Header / Comment


<img src="/.github/preview/decorators.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
[BoxHeader("Some Settings")]
[BoxComment("Settings for something somethings")]
public bool toggle1;
public bool toggle2;
```
</details>

<br/>


### Inlined


<img src="/.github/preview/inlined.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>

```cs
[Serializable] public struct T1 { public string key; public Texture2D icon; }

[Inlined] public Vector3 inlinedVector;

[FieldSize(nameof(T1.key), 40f)]
[Inlined] public T1 inlinedCustom;

```

</details>

<br/>

### Assembly Type

<table>

<tr>
<td>
<img src="/.github/preview/assemblytype.png" />
</td>
<td>
<img src="/.github/preview/typefind.png" />
</td>


</tr>

</table>

```
UnityEngine.Vector3, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
```

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
[AssemblyType]
public string anyType;

// restrict options to behaviour scripts
[IsType(typeof(Component))]
[AssemblyType]
public string behaviourType;

// restrict options to static classes
[IsStatic]
[AssemblyType]
public string staticType;


```
```cs
var t = System.Type.GetType(behaviourType);
Component[] components = GetComponents(t);
```

</details>


<br/>


### Tabs

<img src="/.github/preview/tabs.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
[Serializable] public struct ToggleData { public int x; public bool v1, v2, v3; }
[Tabs] public T2 tabs;
```
  

</details>


<br/>


### Dropdowns

<img src="/.github/preview/options.png" />


<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  

```cs

[StringOptions("option1", "option2")]
public string _string;

[FloatOptions(0.5f, 1.2f, 2.4f)]
public float _float;

[ColorOptions("red", "blue", "cyan")]
public Color _color;

[BoolOptions("Off", "On")]
public bool _bool;

[IntOptions(0, 10)] 
public int _int;

[AssetOptions("Assets/Game/Icons/")]
public Texture2D _texture;
```


</details>


<br/>


### Layer / Tag

<img src="/.github/preview/layer.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
[Tag] public string _tag;
[Layer] public int _layer;
[SortLayer] public int _sortingLayer;
```

</details>

<br/>


### Scene


<img src="/.github/preview/buildscene.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>


```cs
// asset path
[BuildScene]
public string scenePath;

// index in build settings
[BuildScene]
public int sceneIndex;
```

</details>


<br/>



### Switch

<img src="/.github/preview/switch.png" />


<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>

```cs
[Switch("Off", "On")] public bool switch1;
[Switch("Disabled", "Enabled")] public bool switch2;
```



</details>


<br/>

### Hex Color

<img src="/.github/preview/hexcolor.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>

```cs
[HexColor] public string hexColor = "#f00";
```

</details>

<br/>

### Sliders

<img src="/.github/preview/sliders.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>

```cs

// decimal precision
[Slider(0f, 10f, 1)]
public float sliderPrecision;

// step value
[Slider(0, 10f, 0.5f)]
public float sliderStep;

// equivalent to [Range(0f,1f)]
[Slider01] 
public float slider01;
```


</details>


<br/>


### Blend Shape

<img src="/.github/preview/blendshape.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
public SkinnedMeshRenderer _skinnedRenderer;

// string -> name
[BlendShape(nameof(_skinnedRenderer))]
public string _blendShapeName;

// int -> index
[BlendShape(nameof(_skinnedRenderer))]
public int _blendShapeIndex;
```

</details>


<br/>


### Animator Parameter

**Note**: Requires the `ANIMATION_ATTRIBUTES` script define (done this way to remove the need to include the `UnityEngine.Animation` module just for this plugin in case you weren't using it).

<img src="/.github/preview/animatorparameter.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>

```cs
public Animator _animator;

// string -> name
[AnimatorParameter(nameof(_animator))]
public string parameterName;

// int -> index
[AnimatorParameter(nameof(_animator))]
public int parameterIndex;
```


</details>

<br/>


### Renderer Material

<img src="/.github/preview/renderermaterial.png" />

<details>
  <summary>
    <b>⌨️ Code</b>
  </summary>
  
```cs
public Renderer _renderer;

[RendererMaterial(nameof(_renderer))]
public int _materialIndex;
```

</details>




