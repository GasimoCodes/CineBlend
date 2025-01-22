﻿using System;
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

    [Range(0, 179)]
    public Vector2 FOVRange = new(60, 170);

    private BoundingFrustum targetFrustum;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {
    }

    public void Initialize(VirtualCamera camera)
    {
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if (Target == null || Target.Length == 0 || Target[0] == null)
            return;

        Vector3 cameraPos = state.Position.CurrentValue;
        float nearDistance = float.MaxValue;
        float farDistance = float.MinValue;
        Vector3 center = Vector3.Zero;
        int pointCount = 0;

        // First pass: find near/far distances and center
        foreach (var target in Target)
        {
            if (target == null) continue;

            Vector3[] corners = GetBoxCorners(target.Box);
            foreach (var corner in corners)
            {
                float dist = Vector3.Distance(corner, cameraPos);
                nearDistance = Math.Min(nearDistance, dist);
                farDistance = Math.Max(farDistance, dist);
                center += corner;
                pointCount++;
            }
        }

        if (pointCount == 0) return;
        center = GetApproxCenterFromActors(Target, state);

        DebugDraw.DrawLine(center, cameraPos, Color.Green);

        // Calculate look direction and rotation
        Vector3 dirToCenter = (center - cameraPos);
        var targetRotation = Quaternion.LookAt(Vector3.Forward, dirToCenter, Vector3.Up);
        Quaternion.Invert(ref targetRotation, out var finalRot);
        state.Rotation.CurrentValue = finalRot;

        // Find maximum angles relative to view direction
        Vector3 cameraRight = Vector3.Cross(dirToCenter, Vector3.Up).Normalized;
        Vector3 cameraUp = Vector3.Cross(cameraRight, dirToCenter).Normalized;
        float maxHorizontalAngle = 0;
        float maxVerticalAngle = 0;

        // Second pass: find maximum angles
        foreach (var target in Target)
        {
            if (target == null) continue;

            foreach (var corner in GetBoxCorners(target.Box))
            {
                Vector3 dirToCorner = (corner - cameraPos).Normalized;
                float horizontalAngle = Math.Abs(Mathf.Asin(Vector3.Dot(dirToCorner, cameraRight)));
                float verticalAngle = Math.Abs(Mathf.Asin(Vector3.Dot(dirToCorner, cameraUp)));

                maxHorizontalAngle = Math.Max(maxHorizontalAngle, horizontalAngle);
                maxVerticalAngle = Math.Max(maxVerticalAngle, verticalAngle);
            }
        }

        // Calculate and set FOV
        float aspectRatio = Screen.Size.X / (float)Screen.Size.Y;
        float requiredHorizontalFOV = Mathf.RadiansToDegrees * maxHorizontalAngle * 2.1f / aspectRatio;
        float requiredVerticalFOV = Mathf.RadiansToDegrees * maxVerticalAngle * 2.1f;
        float requiredFOV = Math.Max(requiredHorizontalFOV, requiredVerticalFOV);
        state.FieldOfView.CurrentValue = Mathf.Clamp(requiredFOV, state.FieldOfView.CurrentValue, 179);

        // Create debug frustum
        targetFrustum = new BoundingFrustum(
            Matrix.LookAt(cameraPos, cameraPos + dirToCenter, Vector3.Up) *
            Matrix.PerspectiveFov(
                state.FieldOfView.CurrentValue * Mathf.DegreesToRadians,
                aspectRatio,
                nearDistance,
                farDistance
            )
        );
    }

    private Vector3[] GetBoxCorners(BoundingBox box)
    {
        return new Vector3[] {
            new Vector3(box.Minimum.X, box.Minimum.Y, box.Minimum.Z),
            new Vector3(box.Maximum.X, box.Minimum.Y, box.Minimum.Z),
            new Vector3(box.Minimum.X, box.Maximum.Y, box.Minimum.Z),
            new Vector3(box.Maximum.X, box.Maximum.Y, box.Minimum.Z),
            new Vector3(box.Minimum.X, box.Minimum.Y, box.Maximum.Z),
            new Vector3(box.Maximum.X, box.Minimum.Y, box.Maximum.Z),
            new Vector3(box.Minimum.X, box.Maximum.Y, box.Maximum.Z),
            new Vector3(box.Maximum.X, box.Maximum.Y, box.Maximum.Z)
        };
    }

    private Vector3 GetApproxCenterFromActors(Actor[] actors, CameraProperties state)
    {
        if (actors == null || actors.Length == 0)
            return Vector3.Zero;

        // Get centers of all actors
        List<Vector3> centers = new List<Vector3>();
        foreach (var actor in actors)
        {
            if (actor == null) continue;
            centers.Add((actor.Box.Minimum + actor.Box.Maximum) / 2);
        }

        // Get the direction from camera to average point (this will be our plane normal)
        Vector3 averagePos = Vector3.Zero;
        foreach (var pos in centers)
            averagePos += pos;
        averagePos /= centers.Count;

        Vector3 viewDir = Vector3.Normalize(averagePos - state.Position.CurrentValue);
        float averageDistance = Vector3.Distance(state.Position.CurrentValue, averagePos);

        // Create basis vectors for the plane
        Vector3 up = Vector3.Up;
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, viewDir));
        up = Vector3.Cross(viewDir, right); // Recalculate up to ensure orthogonality

        // Project all points and find min/max bounds on the plane
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        Vector3 planePoint = state.Position.CurrentValue + viewDir * averageDistance;

        foreach (var pos in centers)
        {
            // Project each point onto the plane
            Vector3 toPoint = pos - state.Position.CurrentValue;
            float dot = Vector3.Dot(toPoint, viewDir);
            Vector3 projected = state.Position.CurrentValue  + toPoint * (averageDistance / dot);
            Vector3 projectedLocal = projected - planePoint;

            // Get coordinates in plane space
            float x = Vector3.Dot(projectedLocal, right);
            float y = Vector3.Dot(projectedLocal, up);

            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
        }

        // Get center point using min/max bounds
        float centerX = (minX + maxX) / 2;
        float centerY = (minY + maxY) / 2;

        // Convert back to world space
        return planePoint + right * centerX + up * centerY;
    }



#if FLAX_EDITOR
    public override void OnDebugDrawSelected()
    {
        base.OnDebugDrawSelected();
        if (targetFrustum != null)
        {
            DebugDraw.DrawWireFrustum(targetFrustum, Color.Blue);
        }
    }
#endif


}