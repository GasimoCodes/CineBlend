using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// Allows you to offset the position and rotation of the camera.
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineRecomposeModule : Script, ICameraModule
{

    public Vector3 PositionOffset;
    public Quaternion RotationOffset;


    public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {

    }

    public void Initialize(ICineCamera camera)
    {

    }

    public void PostProcessProperties(ref CameraProperties state, float deltaTime)
    {
        if(!this.Enabled)
            return;

        state.Rotation.CurrentValue = state.Rotation.CurrentValue * RotationOffset;
        state.Position.CurrentValue += PositionOffset;
    }
}
