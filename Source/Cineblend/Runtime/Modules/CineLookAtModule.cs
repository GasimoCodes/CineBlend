using System;
using System.Collections.Generic;
using FlaxEngine;
namespace Gasimo.CineBlend.Modules;

/// <summary>
/// CineLookAtModule Script with smooth follow functionality.
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineLookAtModule : Script, ICameraModule
{
    public Actor Target;
    public float Smoothing = 0;

    private Quaternion _currentRotation = Quaternion.Identity;

    public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {
    }

    public void Initialize(ICineCamera camera)
    {
        // Initialize the current rotation to match the camera's initial rotation
        _currentRotation = camera.Actor.Transform.Orientation;
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if (!this.Enabled || Target == null)
            return;

        Vector3 localPos = state.Position.CurrentValue;
        Vector3 targetPos = Target.Transform.Translation;

        // Get the rotation needed for the camera to look at the target
        Vector3 direction = targetPos - localPos;
        var targetRotation = Quaternion.LookAt(Vector3.Forward, direction, Vector3.Up);
        Quaternion.Invert(ref targetRotation, out targetRotation);

        if (Smoothing <= 0)
        {
            // Instant rotation if no smoothing
            _currentRotation = targetRotation;
        }
        else
        {
            // Calculate smooth rotation using Slerp

            float deltaTime = Time.UnscaledDeltaTime;
            float t = deltaTime / Mathf.Max(Smoothing, 0.0001f);
            _currentRotation = Quaternion.Slerp(_currentRotation, targetRotation, t);
        }

        // Apply the rotation
        state.Rotation.CurrentValue = _currentRotation;
    }

#if FLAX_EDITOR

    public override void OnDebugDrawSelected()
    {
        if (Target != null)
        {
            // Draw Bounding Box
            DebugDraw.DrawWireBox(Target.Box, Color.Blue);
        }
    }

#endif

}