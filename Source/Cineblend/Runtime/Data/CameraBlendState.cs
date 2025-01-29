using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend;

/// <summary>
/// Represents the blend state between two cameras
/// </summary>
public class CameraBlendState
{

    /// <summary>
    /// Camera to blend from
    /// </summary>
    public ICameraModule FromCamera { get; set; }
    
    /// <summary>
    /// Camera to blend to
    /// </summary>
    public ICameraModule ToCamera { get; set; }
    
    /// <summary>
    /// Time which it takes to blend between the two cameras
    /// </summary>
    public float BlendTime { get; set; }
    
    /// <summary>
    /// Elapsed time since the blend started
    /// </summary>
    public float CurrentTime { get; set; }

    /// <summary>
    /// Returns true if the blend is still active
    /// </summary>
    public bool IsBlending => CurrentTime < BlendTime;
}