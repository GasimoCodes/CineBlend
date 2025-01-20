using System;
using System.Collections.Generic;
using FlaxEngine;
using Game;

namespace Cineblend;

/// <summary>
/// CineLookAtModule Script.
/// </summary>
public class CineLookAtModule : Script, ICameraModule
{

    public Actor Target;
    public float Smoothing = 0;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {

    }

    public void Initialize(VirtualCamera camera)
    {

    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if (Target == null)
            return;

        // Align this position to forward vector the desired object
        Vector3 localPos = state.Position.CurrentValue;

        Quaternion result = new();
        // Quaternion.GetRotationFromTo(localPos, Target.Position, result);


    }
}
