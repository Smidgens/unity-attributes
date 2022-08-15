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

---

### 🟠 Header / Comment

<details>
  <summary>
    ⌨️ Code
  </summary>
  
```cs
[BoxHeader("Some Settings")]
[BoxComment("Settings for something somethings")]
public bool toggle1;
public bool toggle2;
```
</details>

<img src="/.github/preview/decorators.png" />

---


### 🟡 Inlined

<details>
  <summary>
    ⌨️ Code
  </summary>

```cs
[Serializable] public struct T1 { public string key; public Texture2D icon; }

[Inlined] public Vector3 inlinedVector;

[FieldSize(nameof(T1.key), 40f)]
[Inlined] public T1 inlinedCustom;
```
</details>

<img src="/.github/preview/inlined.png" />


---

### 🔵 Tabs

<details>
  <summary>
    ⌨️ Code
  </summary>
  
```cs
[Serializable] public struct ToggleData { public int x; public bool v1, v2, v3; }
[Tabs] public T2 tabs;
```

</details>

<img src="/.github/preview/tabs.png" />


---

### 🔵 Assembly Type

<details>
  <summary>
     ⌨️ Code
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


<img src="/.github/preview/assemblytype.png" />
<img src="/.github/preview/typefind.png" />

```
UnityEngine.Vector3, UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
```


---


### 🔵 Dropdowns

<details>
  <summary>
    ⌨️ Code
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

<img src="/.github/preview/options.png" />

---


### 🔵 Layer / Tag


<details>
  <summary>
    ⌨️ Code
  </summary>
  
```cs
[Tag] public string _tag;
[Layer] public int _layer;
[SortLayer] public int _sortingLayer;
```

</details>

<img src="/.github/preview/layer.png" />

---


### 🔵 Scene

<details>
  <summary>
    ⌨️ Code
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

<img src="/.github/preview/buildscene.png" />


---



### 🔵 Switch


<details>
  <summary>
    ⌨️ Code
  </summary>

```cs
[Switch("Off", "On")] public bool switch1;
[Switch("Disabled", "Enabled")] public bool switch2;
```

</details>

<img src="/.github/preview/switch.png" />

---

### 🔵 Hex Color

<details>
  <summary>
    ⌨️ Code
  </summary>

```cs
[HexColor] public string hexColor = "#f00";
```

</details>

<img src="/.github/preview/hexcolor.png" />

---

### 🔵 Slider

<details>
  <summary>
    ⌨️ Code
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

<img src="/.github/preview/sliders.png" />

---


### 🔵 Blend Shape

<details>
  <summary>
    ⌨️ Code
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

<img src="/.github/preview/blendshape.png" />

---


### 🔵 Animator Parameter

**Note**: Requires script define `ANIMATION_ATTRIBUTES` in project.

<details>
  <summary>
    ⌨️ Code
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

<img src="/.github/preview/animatorparameter.png" />

---

### 🔵 Renderer Material

<details>
  <summary>
    ⌨️ Code
  </summary>
  
```cs
public Renderer _renderer;

[RendererMaterial(nameof(_renderer))]
public int _materialIndex;
```

</details>

<img src="/.github/preview/renderermaterial.png" />


