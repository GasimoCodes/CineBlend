using System;
using System.Collections.Generic;
using FlaxEngine;
#if USE_LARGE_WORLDS
using Real = System.Double;
#else
using Real = System.Single;
#endif

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


    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, Real t)
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
