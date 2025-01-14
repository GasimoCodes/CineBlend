using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// Represents the blend state between two cameras
/// </summary>
public class CameraBlendState
{
    public ICameraModule FromCamera { get; set; }
    public ICameraModule ToCamera { get; set; }
    public float BlendTime { get; set; }
    public float CurrentTime { get; set; }
    public bool IsBlending => CurrentTime < BlendTime;
}