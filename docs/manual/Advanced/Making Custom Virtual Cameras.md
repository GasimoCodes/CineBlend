# Creating Custom Virtual Cameras in CineBlend

## Overview
CineBlend provides two ways to create custom Virtual Cameras:
1. Inheriting from the `VirtualCamera` base class (recommended)
2. Implementing the `ICineCamera` interface from scratch

This guide covers both approaches, with a focus on the interface implementation for advanced customization needs.

## The ICineCamera Interface
The `ICineCamera` interface defines the core functionality required for any Virtual Camera in the CineBlend system:

```csharp
public interface ICineCamera
{
    CameraProperties Properties { get; }
    CameraProperties FinalProperties { get; }
    Dictionary<Type, ICameraModule> Modules { get; }
    int Priority { get; }
    string Name { get; }
    CameraUpdateMode CameraUpdateMode { get; }
    public Actor Actor { get; }
}
```

### Key Properties

#### Properties and FinalProperties
- `Properties`: Holds the camera's initial, unprocessed state
- `FinalProperties`: Returns the final camera state after all modules have processed it
  - Should apply all modules sequentially to the initial properties
  - Requested by CineblendMaster during blends or when this camera is active/previewed.

#### Modules
- Dictionary of camera modules indexed by their Type
- Modules are processed in the order they were added (in the order they appear in the Editor scripts tab, top to bottom)
- Should include at least the `CineTransformModule` at 0th index to set the initial state. This module is to be added into the dictionary by scripting.
- Modules are to be initialized sequentially by calling [Initialize](/api/Gasimo.CineBlend.ICameraModule.html#Gasimo_CineBlend_ICameraModule_Initialize_Gasimo_CineBlend_VirtualCamera_)

#### Priority
- Determines camera precedence in the CineBlend system
- Higher priority cameras override lower priority ones
- Should trigger a priority update in CineblendMaster when changed

#### CameraUpdateMode
Controls when the camera updates its state:
```csharp
public enum CameraUpdateMode
{
    Update,            // Standard update
    FixedUpdate,       // Physics-based update
    LateUpdate,        // After standard updates
    LateFixedUpdate,   // After physics updates
    Auto,              // Determined by camera context
    Manual             // Explicit update calls required
}
```

#### Actor
Reference to the world instance of the Virtual Camera. Used by modules to hook into custom logic.

## Implementation Guide

### 1. Basic Implementation
Here's a minimal implementation of `ICineCamera`:

```csharp
public class CustomVirtualCamera : ICineCamera
{
    private CameraProperties properties = new();
    private Dictionary<Type, ICameraModule> modules;
    private int priority;
    
    public CustomVirtualCamera()
    {
        modules = new Dictionary<Type, ICameraModule>()
        {
            { typeof(CineTransformModule), new CineTransformModule() }
        };
    }

    public CameraProperties Properties => properties;
    
    public CameraProperties FinalProperties
    {
        get
        {
            var finalState = (CameraProperties)properties.Clone();
            foreach (var module in Modules.Values)
            {
                module.PostProcessProperties(ref finalState);
            }
            return finalState;
        }
    }

    public Dictionary<Type, ICameraModule> Modules => modules;
    public int Priority => priority;
    public string Name { get; set; }
    public CameraUpdateMode CameraUpdateMode => CameraUpdateMode.Update;
    public Actor Actor => this;
}
```

### 2. Required Implementation Details

#### Module Management
Implement methods for module handling:
```csharp
public void AddModule<T>(T module) where T : ICameraModule
{
    module.Initialize(this);
    Modules[typeof(T)] = module;
}

public T GetModule<T>() where T : class, ICameraModule
{
    Modules.TryGetValue(typeof(T), out var module);
    return module as T;
}

public void RemoveModule<T>() where T : ICameraModule
{
    Modules.Remove(typeof(T));
}
```

#### CineblendMaster Integration
Handle registration with the CineBlend system:
```csharp
public void OnEnable()
{
    CineblendMaster.Instance?.RegisterVirtualCamera(this);
}

public void OnDisable()
{
    CineblendMaster.Instance?.UnregisterVirtualCamera(this);
}

private void OnPriorityChanged(int oldPriority, int newPriority)
{
    CineblendMaster.Instance?.UpdateVirtualCameraPriority(this, oldPriority, newPriority);
}
```


## Editor Integration
For better editor support, implement these attributes:
```csharp
[ActorContextMenu("New/CineBlend/Custom Virtual Camera")]
[ActorToolbox("Visuals")]
public class CustomVirtualCamera : ICineCamera
{
    [ShowInEditor, ReadOnly]
    [EditorDisplay("Virtual Camera Status")]
    public bool IsActive => (CineblendMaster.Instance?.currentVirtualCamera == this);
    
    // ... rest of implementation
}
```