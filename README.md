![](/.github/.banner.png?raw=true "")

## ℹ️ Features

* Collection of useful field attributes and decorators for the Unity inspector.
* Automatically stripped in production: `[Conditional("UNITY_EDITOR")]`

<br/>

<br/>

## 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-attributes.git#<version_tag>`


<br/>

## 🚀 Usage

1. Add an assembly reference to the plugin module.
2. Include module namespace


### Header / Comment


<img src="/.github/preview/decorators.png" />

```cs
[BoxHeader("Some Settings")]
[BoxComment("Settings for something somethings")]
public bool toggle1;
public bool toggle2;
```


### Inlined


<img src="/.github/preview/inlined.png" />

```cs
[Serializable] public struct T1 { public string key; public Texture2D icon; }

[Inlined] public Vector3 inlinedVector;

[FieldSize(nameof(T1.key), 40f)]
[Inlined] public T1 inlinedCustom;

```

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

```cs

[AssemblyType]
public string anyType;

// restrict search to behaviour scripts
[AssemblyType]
[IsType(typeof(MonoBehaviour))]
public string behaviourType;

// restrict search to static classes
[AssemblyType]
[OnlyStatic]
public string staticType;
```
```
UnityEngine.Vector3, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
```

### Tabs

<img src="/.github/preview/tabs.png" />

```cs
[Serializable] public struct ToggleData { public int x; public bool v1, v2, v3; }
[Tabs] public T2 tabs;
```


### Value Dropdowns


<img src="/.github/preview/options.png" />

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

### Layer / Tag


<img src="/.github/preview/layer.png" />

```cs
[Tag] public string _tag = "";
[Layer] public int _layer = -1;
[SortLayer] public int _sortingLayer = -1;
```


### Scene


<img src="/.github/preview/buildscene.png" />

```cs

// project path
[BuildScene(Label = "Scene (path)")]
public string scenePath;

// index in build settings
[BuildScene(Label = "Scene (index)")]
public int _sceneIndex;

```




### Switch

<img src="/.github/preview/switch.png" />

```cs
[Switch("Off", "On")] public bool switch1;
[Switch("Disabled", "Enabled")] public bool switch2;
```

### Hex Color

<img src="/.github/preview/hexcolor.png" />

```cs
[HexColor] public string hexColor = "#f00";
```


### Sliders

<img src="/.github/preview/sliders.png" />

```cs

// decimal precision
[Slider(1f, 10f, 1, Label = "Slider (fixed)")]
public float sliderPrecision = 0f;

// step value
[Slider(1f, 10f, 0.5f, Label = "Slider (step)")]
public float sliderStep = 0f;

// equivalent to [Range(0f,1f)]
[Slider01(Label = "Slider (0 - 1)")] 
public float slider01 = 0f;
```


### Blend Shape

<img src="/.github/preview/blendshape.png" />

```cs
public SkinnedMeshRenderer _skinnedRenderer;

// store name
[BlendShape(nameof(_skinnedRenderer))]
public string _blendShapeName = "";

// store index
[BlendShape(nameof(_skinnedRenderer))]
public int _blendShapeIndex = -1;
```


### Animator Parameter

**Note**: Requires the `ANIMATION_ATTRIBUTES` script define (done this way to remove the need to include the `UnityEngine.Animation` module just for this plugin in case you weren't using it).

<img src="/.github/preview/animatorparameter.png" />

```cs
public Animator _animator;

// store name
[AnimatorParameter(nameof(_animator))]
public string parameterName;

// store index
[AnimatorParameter(nameof(_animator))]
public int parameterIndex;
```


### Renderer Material

<img src="/.github/preview/renderermaterial.png" />

```cs
public Renderer _renderer;

[RendererMaterial(nameof(_renderer))]
public int _materialIndex = -1;
```


