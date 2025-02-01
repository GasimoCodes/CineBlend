using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// CineTransformModule class holds camera transforms and can apply them.
/// This is a default module which is auto-included on built-in VirtualCameras.
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
        if (cam == null)
        {
            return;
        }
        // string offset =  (cam == null) + " / " + ((cam == null) ? "NULL" : cam?.Name);

        state.Position.CurrentValue = cam.Position;
        state.Rotation.CurrentValue = cam.Orientation;

        // Debug.Log(state.Position.CurrentValue);
    }
}
