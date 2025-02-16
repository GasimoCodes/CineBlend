using FlaxEngine;
#if USE_LARGE_WORLDS
using Real = System.Double;
#else
using Real = System.Single;
#endif

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// CineTransformModule class holds camera transforms and can apply them.
/// This is a default module which is auto-included on built-in VirtualCameras.
/// </summary>
public class CineTransformModule : ICameraModule
{
    private VirtualCamera cam;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, Real t)
    {
        cam.Properties.Position.CurrentValue = Vector3.Lerp(fromSnapshot.Properties.Position.CurrentValue, toSnapshot.Properties.Position.CurrentValue, (float)t);
        cam.Properties.Rotation.CurrentValue = Quaternion.Slerp(fromSnapshot.Properties.Rotation.CurrentValue, toSnapshot.Properties.Rotation.CurrentValue, (float)t);
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
