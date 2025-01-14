using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// ICineCamera interface.
/// </summary>
public interface ICineCamera
{
    public CameraProperties Properties { get; }
    public CameraProperties FinalProperties { get; }
    public Dictionary<Type, ICameraModule> Modules { get; }
    public int Priority { get; }
    public string Name { get; }

}
