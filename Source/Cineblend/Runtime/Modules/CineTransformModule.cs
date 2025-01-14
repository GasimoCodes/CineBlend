using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// CineTransformModule class holds camera transforms and can apply them
/// </summary>
public class CineTransformModule : ICameraModule
{
    private VirtualCamera cam;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {
        cam.Properties.Position.CurrentValue = Vector3.Lerp(fromSnapshot.Properties.Position.CurrentValue, toSnapshot.Properties.Position.CurrentValue, t);
        cam.Properties.Rotation.CurrentValue = Quaternion.Slerp(fromSnapshot.Properties.Rotation.CurrentValue, toSnapshot.Properties.Rotation.CurrentValue, t);
    }

    public void Initialize(VirtualCamera camera)
    {
        cam = camera;
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        state.Position.CurrentValue = cam.Position;
        state.Rotation.CurrentValue = cam.Orientation;

        // Debug.Log(state.Position.CurrentValue);
    }
}
