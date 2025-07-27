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
    /// Processes this cameras properties.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void ProcessProperties(float deltaTime);

    /// <summary>
    /// Notifies the camera that it is being blended from one snapshot to another.
    /// </summary>
    /// <param name="fromSnapshot">Camera we're blending from</param>
    /// <param name="toSnapshot">Camera we're blending to</param>
    /// <param name="t">progress of blend (eased)</param>
    public void OnBlend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {
        foreach (var item in Modules)
        {
            item.Value.Blend(fromSnapshot, toSnapshot, t);
        }
    }

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
    Manual,
    OnRender,
    OnDraw
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

    ICineCamera Camfrom;
    ICineCamera Camto;

    public Dictionary<Type, ICameraModule> Modules => new();
    public CameraProperties FinalProperties => Properties;
    public Actor Actor => throw new NotImplementedException();

    public void OnBlend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {
        foreach (var item in Camfrom.Modules)
        {
            item.Value.Blend(fromSnapshot, toSnapshot, t);
        }

        foreach (var item in Camto.Modules)
        {
            item.Value.Blend(fromSnapshot, toSnapshot, t);
        }
    }

    public StaticCameraProperties(CameraProperties properties, ICineCamera CamFrom, ICineCamera CamTo, string name = "StaticCamera")
    {
        Properties = properties;
        Name = name;
        this.Camfrom = CamFrom;
        this.Camto = CamTo;
    }

    public void ProcessProperties(float deltaTime)
    {
    }
}