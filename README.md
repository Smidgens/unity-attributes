![](/.github/.banner.png?raw=true "")

## ℹ️ Features

* Collection of useful inspector attributes and decorators
* 🤞 Reasonably lightweight.
<br/>

<br/>

## 📦 Install

1. Open Package Manager
2. Paste GitHub URL:\
`https://github.com/Smidgens/unity-attributes.git#<version_tag>`


<br/>

## 🚀 Usage


### Decorators


<img src="/.github/preview/decorators.png" />

```cs
[BoxHeader("Some Settings")]
[BoxComment("Settings for something somethings")]
public bool toggle1;
public bool toggle2;
```


### Layout


<img src="/.github/preview/layout.png" />

```cs
[Serializable] public struct T1 { public string key; public Texture2D icon; }
[Serializable] public struct T2 { public int x; public bool v1, v2, v3; }

[Inlined] public Vector3 inlinedVector;

[FieldSize(nameof(T1.key), 40f)]
[Inlined] public T1 inlinedCustom;

[Tabs] public T2 tabs;
```


### Value Dropdowns


<img src="/.github/preview/options.png" />

```cs
[StringOptions("option1", "option2")] public string _string;
[FloatOptions(0.5f, 1.2f, 2.4f)] public float _float;
[ColorOptions("red", "blue", "cyan")] public Color _color;
[BoolOptions("Off", "On")] public bool _bool;
[IntOptions(0, 10)] public int _int;
[AssetOptions("Assets/Demo/")] public Texture2D _texture;
```


### Widgets

<img src="/.github/preview/widgets.png" />

```cs
[Switch("Off", "On")] public bool switch1;
[Switch("Disabled", "Enabled")] public bool switch2;
```
