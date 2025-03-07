using FlaxEngine;
using System;
using System.Collections.Generic;

namespace Gasimo.CineBlend;

/// <summary>
/// ICineCamera interface. Implement this to create new Virtual Cameras from scratch. Otherwise inherit the VirtualCamera class.
/// </summary>
public interface ICineCamera
{
    /// <summary>
    /// This Cameras non-processed initial properties.
    /// </summary>
    public CameraProperties Properties { get; }

    /// <summary>
    /// This Cameras processed final properties.
    /// </summary>
    public CameraProperties FinalProperties { get; }
    /// <summary>
    /// CineModules which can be added to the camera. They are initialized by the virtual camera and then applied sequentially to the properties.
    /// </summary>
    public Dictionary<Type, ICameraModule> Modules { get; }

    /// <summary>
    /// The priority of the camera. The camera with the highest priority will be the active camera unless overriden by solo.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Name of the camera. Used to improve readability in the editor.
    /// </summary>
    public string Name { get; }

    public CameraUpdateMode CameraUpdateMode { get; }

    /// <summary>
    /// Reference to the world instance of the Virtual Camera. Used by modules to hook into custom logic.
    /// </summary>
    public Actor Actor { get; }

}

public enum CameraUpdateMode
{
    Update,
    FixedUpdate,
    LateUpdate,
    LateFixedUpdate,
    Auto,
    Manual
}

public enum Easing
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut
}

/// <summary>
/// Helper class holding static camera properties, used for creating a fake camera state mid-transition.
/// </summary>
public class StaticCameraProperties : ICineCamera
{
    public CameraProperties Properties { get; }
    public string Name { get; }
    public int Priority => 0;
    public CameraUpdateMode CameraUpdateMode => CameraUpdateMode.Auto;
    public Dictionary<Type, ICameraModule> Modules => new Dictionary<Type, ICameraModule>();
    public CameraProperties FinalProperties => Properties;

    public Actor Actor => throw new NotImplementedException();

    public StaticCameraProperties(CameraProperties properties, string name = "StaticCamera")
    {
        Properties = properties;
        Name = name;
    }
}