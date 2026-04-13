# Sim.Faciem.uGUI

> **uGUI Extension that brings Runtime Data Binding to the MonoBehaviour World**

`Sim.Faciem.uGUI` is a Unity 6 package that enables **MVVM-style, reactive data binding** for classic uGUI and MonoBehaviour workflows — without needing UI Toolkit. Powered by [R3](https://github.com/Cysharp/R3), it lets you connect any data source to any UI component property entirely through the Inspector or with minimal code.

---

## ✨ Why Use It?

Unity's built-in data binding is tied to UI Toolkit and `INotifyBindablePropertyChanged`. If you are working with **uGUI** (Canvas, TextMeshPro, Image, Slider, etc.) and **MonoBehaviours**, keeping UI in sync with game state usually means writing a lot of boilerplate subscription code.

`Sim.Faciem.uGUI` solves this by providing:

- **One-way reactive bindings** from any `Observable<T>` or `ReactiveProperty<T>` on a data source directly to any component property — zero subscription boilerplate.
- **Editor-first workflow**: configure bindings visually in the Inspector with a dedicated Binding Window and custom property drawers.
- **Type-safe converters**: plug in converter components to transform values between your data model and your UI without changing either.
- **Deep property path traversal**: navigate nested objects and hop through reactive observables using a simple dot-and-`$` path syntax.
- **`SimAutoBindingComponent`**: bind any component's property to a data source completely without writing a single line of custom code.

---

## 📦 Package Info

| Field | Value |
|---|---|
| Package name | `com.sim.faciem-ugui` |
| Version | `1.0.0-alpha.1` |
| Unity | 6000.0+ |
| Dependency | `com.sim.faciem` |

---

## 🔧 Installation

### Step 1 — Prerequisites: R3

`Sim.Faciem.uGUI` relies on **R3** for reactive primitives (`Observable<T>`, `ReactiveProperty<T>`, etc.). R3 has its own setup steps that must be completed first.

#### 1a. Install NuGetForUnity

R3 uses NuGet packages internally. Add NuGetForUnity via UPM by opening  
**Edit → Project Settings → Package Manager** and adding a scoped registry, **or** add it directly to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity"
  }
}
```

#### 1b. Install UniTask

R3 depends on UniTask. Add it to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
  }
}
```

#### 1c. Install R3

Add R3 itself to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.cysharp.r3": "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity"
  }
}
```

After Unity reimports, open **NuGet → Manage NuGet Packages** and install the `R3` NuGet package (version ≥ 1.x).

---

### Step 2 — Install Sim.Faciem (Core)

`Sim.Faciem.uGUI` depends on the Sim.Faciem core package. Add it to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.sim.faciem": "https://github.com/Simplicitas-Studios/Sim.Faciem.git"
  }
}
```

---

### Step 3 — Install Sim.Faciem.uGUI

Finally, add this package to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.sim.faciem-ugui": "https://github.com/Simplicitas-Studios/Sim.Faciem.uGUI.git"
  }
}
```

Your final `manifest.json` dependencies section should look similar to:

```json
{
  "dependencies": {
    "com.github-glitchenzo.nugetforunity": "https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity",
    "com.cysharp.unitask":                 "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask",
    "com.cysharp.r3":                      "https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity",
    "com.sim.faciem":                      "https://github.com/Simplicitas-Studios/Sim.Faciem.git",
    "com.sim.faciem-ugui":                 "https://github.com/Simplicitas-Studios/Sim.Faciem.uGUI.git"
  }
}
```

---

## 🧩 Core Concepts

### `SimDataSourceMonoBehaviour` — The Data Source (ViewModel)

Your data source is any `MonoBehaviour` that inherits from `SimDataSourceMonoBehaviour`. This serves as the **ViewModel** layer. Expose reactive properties using R3's `ReactiveProperty<T>` or `Observable<T>` and mark them with `[CreateProperty]` so the binding system can discover them via Unity's property path resolution.

```csharp
using R3;
using Sim.Faciem.uGUI;
using Unity.Properties;
using UnityEngine;

public class PlayerViewModel : SimDataSourceMonoBehaviour
{
    // A reactive property whose changes can be observed
    [CreateProperty]
    public ReactiveProperty<int> Score { get; } = new(0);

    [CreateProperty]
    public ReactiveProperty<string> PlayerName { get; } = new("Hero");

    // Plain property (bound directly, not reactive)
    [CreateProperty]
    public string StatusText { get; set; }
}
```

> ℹ️ The `[CreateProperty]` attribute (from `Unity.Properties`) is required so the binding system can resolve property paths at runtime using Unity's `PropertyContainer` API.

---

### `BindableProperty<T>` — Typed Bindable Field

`BindableProperty<T>` is a **serializable field** you add to any MonoBehaviour. It holds a value of type `T` and can optionally be connected to a data source at edit time via the Inspector. At runtime, whenever the bound observable emits a new value, `BindableProperty<T>` automatically updates.

```csharp
using R3;
using Sim.Faciem.uGUI;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private BindableProperty<int> _score;

    private void Awake()
    {
        _score.CreateBinding(); // activates the binding configured in the Inspector

        _score.ObserveChanges()
              .Subscribe(value => Debug.Log($"Score updated: {value}"))
              .AddTo(this);

        _score.AddTo(this); // ties the binding lifetime to this GameObject
    }
}
```

In the Inspector you will see the field with a **binding icon** (🔗) when a binding is configured, plus a binding details summary.

---

### `SimAutoBindingComponent` — Codeless Binding

`SimAutoBindingComponent` is a ready-made component that lets you connect **any component property** on the same or a child GameObject to a data source entirely through the Inspector — no custom script required.

**How to use:**
1. Add a `SimAutoBindingComponent` to a GameObject.
2. Right-click the component in the Inspector and choose **Add Binding**.
3. In the Binding Window, select:
   - **Target**: the component and property to write to (e.g., `TMP_Text.text`).
   - **Data Source**: the `SimDataSourceMonoBehaviour` instance to read from.
   - **Property Path**: the path on the data source (e.g., `Score.$`).
4. Optionally attach **Converter** components to transform the value.

At runtime, `SimAutoBindingComponent` creates all bindings in `Awake` and manages their lifetimes automatically.

---

### `TMPBindable` — TextMeshPro Text Binding

A drop-in component for `TMP_Text`. Add it alongside a `TMP_Text` (or `TextMeshProUGUI`) component and configure the `_text` binding in the Inspector to a `string` observable path. The component automatically updates the text whenever the bound value changes.

```
GameObject
 ├─ TextMeshProUGUI
 └─ TMPBindable          ← add this, configure binding in Inspector
```

---

### `SimConverterBehaviour<TFrom, TTo>` — Value Converters

Converters transform a value from the data source type into the type expected by the UI before it is applied. They are regular `MonoBehaviour` components, which means you can configure them in the Inspector and reuse them across multiple bindings.

```csharp
using Sim.Faciem.uGUI;

// Converts a bool health state to a display string
public class BoolToStatusConverter : SimConverterBehaviour<bool, string>
{
    public override string Convert(bool isAlive)
    {
        return isAlive ? "Alive" : "Dead";
    }
}
```

Attach the converter MonoBehaviour to any GameObject in the scene, then add it to the **Converters** list of a binding. Multiple converters can be chained — they are applied in order.

---

### Property Path Syntax

Binding paths are dot-separated property names, identical to Unity's `PropertyPath` format. The `$` symbol acts as a **subscription separator**: it tells the binding engine to *subscribe* to the observable at that point in the path and re-evaluate everything that follows whenever it emits.

| Path | Meaning |
|---|---|
| `Score.$` | Subscribe to `Score` (a `ReactiveProperty<int>`) and forward its values. |
| `PlayerName.$` | Subscribe to `PlayerName` (a `ReactiveProperty<string>`) and forward its values. |
| `NestedReactiveItem$.TestPath` | Subscribe to `NestedReactiveItem` (a `ReactiveProperty<NestedItem>`), then read `TestPath` from each emitted `NestedItem`. |
| `NestedReactiveItem$.Child$.Value` | Multi-hop: subscribe to an outer reactive, then subscribe to a nested reactive within each emitted value, and finally read `Value`. |

> Properties that are **not** observables (plain `[CreateProperty]` values) are read once at binding creation and are not re-evaluated reactively. Wrap them in a `ReactiveProperty<T>` and use `$` if you need live updates.

---

## 🚀 Usage Examples

### Example 1 — Display a counter with TMPBindable

**ViewModel:**

```csharp
using R3;
using Sim.Faciem.uGUI;
using Unity.Properties;
using UnityEngine;

public class CounterViewModel : SimDataSourceMonoBehaviour
{
    [CreateProperty]
    public ReactiveProperty<int> Counter { get; } = new(0);

    private void Start()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ => Counter.Value++)
            .AddTo(this);
    }
}
```

**Setup in the Scene:**
1. Create a GameObject with `CounterViewModel`.
2. Create a UI Text GameObject with `TextMeshProUGUI` + `TMPBindable`.
3. In the Inspector of `TMPBindable`, configure the `_text` binding:
   - **Data Source** → `CounterViewModel` instance
   - **Property Path** → `Counter.$`
   - **Converter** → `IntToStringConverter` (a custom `SimConverterBehaviour<int, string>`)

The text will update every second automatically.

---

### Example 2 — Bind any property with SimAutoBindingComponent

Bind the `interactable` property of a `Button` to a `bool` observable without writing any code:

1. Add `SimAutoBindingComponent` to the Button GameObject.
2. Right-click → **Add Binding**.
3. Set **Target** to `Button.interactable`.
4. Set **Data Source** and **Property Path** to your `ReactiveProperty<bool>` path (e.g., `IsLoggedIn.$`).

---

### Example 3 — BindableProperty<T> in a custom component

```csharp
using R3;
using Sim.Faciem.uGUI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    // Configure the binding (Data Source + Path) in the Inspector
    [SerializeField] private BindableProperty<float> _health;

    private void Awake()
    {
        _health.CreateBinding();

        _health.ObserveChanges()
               .Subscribe(hp => _slider.value = hp)
               .AddTo(this);

        _health.AddTo(this);
    }
}
```

---

### Example 4 — Navigating nested reactive objects

```csharp
// ViewModel
public class GameViewModel : SimDataSourceMonoBehaviour
{
    [CreateProperty]
    public ReactiveProperty<PlayerData> CurrentPlayer { get; } = new();
}

// Data class (POCO, no MonoBehaviour required)
public class PlayerData
{
    [CreateProperty] public string Name { get; set; }
    [CreateProperty] public int Level { get; set; }
}
```

In the binding window, set the path to `CurrentPlayer$.Name` — whenever `CurrentPlayer` emits a new `PlayerData` object, the binding automatically reads `Name` from it and pushes the value to the UI.

---

### Example 5 — Custom converter

```csharp
using Sim.Faciem.uGUI;
using UnityEngine;

public class IntToRomanConverter : SimConverterBehaviour<int, string>
{
    public override string Convert(int number)
    {
        // ... conversion logic ...
        return result;
    }
}
```

Attach `IntToRomanConverter` to any GameObject and add it to the binding's **Converters** list in the Inspector. It will be applied automatically every time the source value changes.

---

## 🖥️ Editor Support

The package ships with a fully custom editor experience:

- **`BindableProperty<T>` Property Drawer** — shows the current bound value (read-only while bound) and a binding icon when a binding is active.
- **`SimAutoBindingComponent` Inspector** — lists all configured bindings with target component, property path, type, and data source at a glance. Right-click a binding to **Edit** or **Delete** it.
- **Binding Window** — a dedicated editor window that guides you through selecting a data source, navigating its property paths (including reactive hops), and configuring converters.

---

## 📋 Requirements Summary

| Requirement | Source |
|---|---|
| Unity 6000.0+ | — |
| NuGetForUnity | `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity` |
| UniTask | `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask` |
| R3 (Unity) | `https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity` |
| R3 (NuGet) | Install via NuGet → Manage NuGet Packages |
| Sim.Faciem | `https://github.com/Simplicitas-Studios/Sim.Faciem.git` |
| TextMeshPro | Bundled with Unity (required for `TMPBindable`) |

---

## 📄 License

See [LICENSE](LICENSE) for details.

---

*Made with ❤️ by [Simplicitas Studios](https://github.com/Simplicitas-Studios)*
