using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// CineAutoFrameModule Script that automatically frames target actors.
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineAutoFrameModule : Script, ICameraModule
{
    public Actor[] Target;

    public float Smoothing = 0;
    private Quaternion _currentRotation = Quaternion.Identity;
    private float _currentFOV = 60;


    [Range(0, 179)]
    public Float2 FOVRange = new(60, 170);

    private BoundingFrustum targetFrustum;

    public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {
    }

    public void Initialize(ICineCamera camera)
    {
        _currentRotation = camera.Actor.Transform.Orientation;
        _currentFOV = camera.Properties.FieldOfView.CurrentValue;
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        if (Target == null || Target.Length == 0 || Target[0] == null)
            return;

        
        // Smoothing
        float deltaTime = Time.UnscaledDeltaTime;
        float t = deltaTime / Mathf.Max(Smoothing, 0.0001f);


        // Calculations
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

            // DebugDraw.DrawWireBox(target.Box, Color.Red, 0, false);

            foreach (var corner in corners)
            {
                float dist = (float)Vector3.Distance(corner, cameraPos);
                nearDistance = float.Min(nearDistance, dist);
                farDistance = float.Max(farDistance, dist);
                center += corner;
                pointCount++;
            }
        }

        if (pointCount == 0) return;
        center = GetApproxCenterFromActors(Target, state);

        // Calculate look direction and rotation
        Vector3 dirToCenter = (center - cameraPos);
        var targetRotation = Quaternion.LookAt(Vector3.Forward, dirToCenter, Vector3.Up);
        Quaternion.Invert(ref targetRotation, out var finalRot);

        // Apply
        if (Smoothing <= 0)
        {
            _currentRotation = finalRot;
        }
        else
        {
            _currentRotation = Quaternion.Slerp(_currentRotation, finalRot, t);
        }
        state.Rotation.CurrentValue = _currentRotation;


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
                
                float horizontalAngle = float.Abs(float.Asin((float)Vector3.Dot(dirToCorner, cameraRight)));
                float verticalAngle = float.Abs(float.Asin((float)Vector3.Dot(dirToCorner, cameraUp)));

                maxHorizontalAngle = Math.Max(maxHorizontalAngle, horizontalAngle);
                maxVerticalAngle = Math.Max(maxVerticalAngle, verticalAngle);
            } 
        }

        // Calculate and set FOV
        float aspectRatio = Screen.Size.X / (float)Screen.Size.Y;
        float requiredHorizontalFOV = Mathf.RadiansToDegrees * maxHorizontalAngle * 2.1f / aspectRatio;
        float requiredVerticalFOV = Mathf.RadiansToDegrees * maxVerticalAngle * 2.1f;
        float requiredFOV = Math.Max(requiredHorizontalFOV, requiredVerticalFOV);
        
        float clampedFov = float.Clamp(requiredFOV, state.FieldOfView.CurrentValue, FOVRange.Y);

        if (Smoothing <= 0)
        {
            _currentFOV = clampedFov;
        } 
        else
        {
            _currentFOV = Mathf.Lerp(_currentFOV, clampedFov, t);
        }

        state.FieldOfView.CurrentValue = _currentFOV;


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

    /// <summary>
    /// Returns approximate point which camera needs to face to have all Actors visually centered in screen-space.
    /// </summary>
    /// <param name="actors"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    private Vector3 GetApproxCenterFromActors(Actor[] actors, CameraProperties state)
    {
        if (actors == null || actors.Length == 0)
            return Vector3.Zero;

        // Get centers actors
        List<Vector3> centers = new List<Vector3>();
        foreach (var actor in actors)
        {
            if (actor == null) continue;
            centers.Add((actor.Box.Minimum + actor.Box.Maximum) / 2);
        }

        // Get the direction from camera to center point (Normal vector)
        Vector3 averagePos = Vector3.Zero;

        Vector3 min = Vector3.Maximum;
        Vector3 max = Vector3.Minimum;
        foreach (var actor in actors)
        {
            if (actor == null) continue;
            foreach (Vector3 point in GetBoxCorners(actor.Box))
            {
                min = new Vector3(Math.Min(min.X, point.X), Math.Min(min.Y, point.Y), Math.Min(min.Z, point.Z));
                max = new Vector3(Math.Max(max.X, point.X), Math.Max(max.Y, point.Y), Math.Max(max.Z, point.Z));

                //DebugDraw.DrawWireSphere(new BoundingSphere(point, 10), Color.Blue);
            }
        }

        //DebugDraw.DrawWireBox(new BoundingBox(min, max), Color.Blue);

        averagePos = (min + max) / 2;

        //DebugDraw.DrawWireSphere(new BoundingSphere(averagePos, 10), Color.Yellow);

        Vector3 viewDir = Vector3.Normalize(averagePos - state.Position.CurrentValue);
        Real averageDistance = Vector3.Distance(state.Position.CurrentValue, averagePos);

        // Create basis vectors for the plane
        Vector3 up = Vector3.Up;
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, viewDir));
        up = Vector3.Cross(viewDir, right); // Recalculate up to ensure orthogonality

        // Project all points and find min/max bounds on the plane
        Real minX = float.MaxValue, maxX = float.MinValue;
        Real minY = float.MaxValue, maxY = float.MinValue;
        Vector3 planePoint = state.Position.CurrentValue + viewDir * averageDistance;

        foreach (var pos in centers)
        {
            // Project each point onto the plane
            Vector3 toPoint = pos - state.Position.CurrentValue;
            Real dot = Vector3.Dot(toPoint, viewDir);
            Vector3 projected = state.Position.CurrentValue  + toPoint * (averageDistance / dot);
            Vector3 projectedLocal = projected - planePoint;

            // Get coordinates in plane space
            Real x = Vector3.Dot(projectedLocal, right);
            Real y = Vector3.Dot(projectedLocal, up);

            minX = Real.Min(minX, x);
            maxX = Real.Max(maxX, x);
            minY = Real.Min(minY, y);
            maxY = Real.Max(maxY, y);

            
        }

        // Get center point using min/max bounds
        Real centerX = (minX + maxX) / 2;
        Real centerY = (minY + maxY) / 2;

        Vector3 worldSpaceCenterPoint = planePoint + right * centerX + up * centerY;

        // Draw basis vector
        DebugDraw.DrawLine(worldSpaceCenterPoint, worldSpaceCenterPoint + right * 100, Color.Red);
        DebugDraw.DrawLine(worldSpaceCenterPoint, worldSpaceCenterPoint + up * 100, Color.Green);

        // Convert back to world space
        return worldSpaceCenterPoint;
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