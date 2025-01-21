using System;
using System.Collections.Generic;
using FlaxEngine;
using Game;

namespace Cineblend;

/// <summary>
/// CineLookAtModule Script.
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineRecomposeModule : Script, ICameraModule
{

    public Vector3 PositionOffset;
    public Quaternion RotationOffset;


    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {

    }

    public void Initialize(VirtualCamera camera)
    {

    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if(!this.Enabled)
            return;

        state.Rotation.CurrentValue = state.Rotation.CurrentValue * RotationOffset;
        state.Position.CurrentValue += PositionOffset;
    }
}
