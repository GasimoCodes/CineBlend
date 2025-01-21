using System;
using System.Collections.Generic;
using FlaxEngine;
using Game;
namespace Cineblend;

/// <summary>
/// CineAutoFrameModule Script that automatically frames target actors.
/// </summary>
public class CineAutoFrameModule : Script, ICameraModule
{
    public Actor[] Target;
    private BoundingBox targetBoundingBox;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {
    }

    public void Initialize(VirtualCamera camera)
    {
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if (Target == null || Target.Length == 0)
            return;

        // Calculate combined bounding box of all targets
        Vector3 boundsMin = Target[0].Transform.Translation;
        Vector3 boundsMax = boundsMin;

        foreach (var target in Target)
        {
            if (target == null) continue;

            // Get bounds in world space
            BoundingBox actorBounds = target.Box;
            boundsMin = Vector3.Min(boundsMin, actorBounds.Minimum);
            boundsMax = Vector3.Max(boundsMax, actorBounds.Maximum);
        }

        targetBoundingBox = new BoundingBox(boundsMin, boundsMax);

        Vector3 cameraPos = state.Position.CurrentValue;
        Vector3 dirToTarget = (targetBoundingBox.Center - cameraPos);

        // DebugDraw.DrawLine(cameraPos, targetBoundingBox.Center, Color.Red);

        // Get the rotation needed for the camera to look at the target
        var targetRotation = Quaternion.LookAt(Vector3.Forward, dirToTarget, Vector3.Up);
        Quaternion finalRot = new();
        Quaternion.Invert(ref targetRotation, out finalRot);
        state.Rotation.CurrentValue = finalRot;

        // Calculate vectors for the camera's view plane
        Vector3 cameraRight = Vector3.Cross(dirToTarget, Vector3.Up).Normalized;
        Vector3 cameraUp = Vector3.Cross(cameraRight, dirToTarget).Normalized;

        // Find the maximum angles needed to view the box corners
        float maxHorizontalAngle = 0;
        float maxVerticalAngle = 0;


        float aspectRatio = Screen.Size.X / (float)Screen.Size.Y;

        // Check each corner of the bounding box (yuck)
        Vector3[] corners = {
            new Vector3(boundsMin.X, boundsMin.Y, boundsMin.Z),
            new Vector3(boundsMax.X, boundsMin.Y, boundsMin.Z),
            new Vector3(boundsMin.X, boundsMax.Y, boundsMin.Z),
            new Vector3(boundsMax.X, boundsMax.Y, boundsMin.Z),
            new Vector3(boundsMin.X, boundsMin.Y, boundsMax.Z),
            new Vector3(boundsMax.X, boundsMin.Y, boundsMax.Z),
            new Vector3(boundsMin.X, boundsMax.Y, boundsMax.Z),
            new Vector3(boundsMax.X, boundsMax.Y, boundsMax.Z)
        };

        foreach (var corner in corners)
        {
            Vector3 dirToCorner = (corner - cameraPos).Normalized;

            // Get signed angles to determine which way we need to rotate
            float horizontalAngle = Mathf.Asin(Vector3.Dot(dirToCorner, cameraRight));
            float verticalAngle = Mathf.Asin(Vector3.Dot(dirToCorner, cameraUp));

            maxHorizontalAngle = Math.Max(maxHorizontalAngle, Math.Abs(horizontalAngle));
            maxVerticalAngle = Math.Max(maxVerticalAngle, Math.Abs(verticalAngle));
        }

        // Calculate required FOV to fit both horizontal and vertical angles
        // Adding 5% padding (1.05f) for slight margin
        float requiredHorizontalFOV = Mathf.RadiansToDegrees * maxHorizontalAngle * 2.0f * 1.05f / aspectRatio;
        float requiredVerticalFOV = Mathf.RadiansToDegrees * maxVerticalAngle * 2.0f * 1.05f;

        // Take the larger of the two angles to fit this
        float requiredFOV = Math.Max(requiredHorizontalFOV, requiredVerticalFOV);

        // Clamper, current FOV - 179
        requiredFOV = Mathf.Clamp(requiredFOV, state.FieldOfView.CurrentValue, 179);

        state.FieldOfView.CurrentValue = requiredFOV;
    }

#if FLAX_EDITOR
    public override void OnDebugDrawSelected()
    {
        base.OnDebugDrawSelected();
        // Draw bounding box
        DebugDraw.DrawWireBox(targetBoundingBox, Color.Blue);
        DebugDraw.DrawText("Auto-Frame", targetBoundingBox.Center, Color.Blue);
    }
#endif
}
