using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// CineTransformModule class holds camera transforms and can apply them.
/// This is a default module which is auto-included on built-in VirtualCameras.
/// </summary>
public class CineTransformModule : ICameraModule
{
    private ICineCamera cam;

    public void Initialize(ICineCamera camera)
    {
        cam = camera;
    }

    public void PostProcessProperties(ref CameraProperties state, float deltaTime)
    {
        if (cam == null)
        {
            return;
        }

        // string offset =  (cam == null) + " / " + ((cam == null) ? "NULL" : cam?.Name);

        state.Position.CurrentValue = cam.Actor.Position;
        state.Rotation.CurrentValue = cam.Actor.Orientation;

        // Debug.Log(state.Position.CurrentValue);
    }
}
