using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// Allows you to offset the position of the camera closer to the target to avoid obstacles and keep Line of Sight
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineColliderModule : Script, ICameraModule
{    
    public Actor Target;

    public LayersMask Colliders = LayersMask.Default;

    [Limit(0, 100)]
    [ShowInEditor, Serialize]
    private float sphereRadius = 1f;

    [ShowInEditor, Serialize]
    private float offsetFromObstacle = 10;


    public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {

    }

    public void Initialize(ICineCamera camera)
    {

    }

    public void PostProcessProperties(ref CameraProperties state, float deltaTime)
    {
        if(!this.Enabled || Target == null)
            return;

        // spherecast from this to target position

        Vector3 from = Target.Position;
        Vector3 to = state.Position.CurrentValue;

        Vector3 direction = to - from;

        Real distance = direction.Length;
        Vector3 normalizedDirection = direction / distance;

        if (Physics.SphereCast(from, sphereRadius, normalizedDirection, out var hitInfo, (float)distance, Colliders))
        {
            // Calculate how far along the ray we've traveled to the hit point
            float hitDistance = hitInfo.Distance;

            // Calculate the sphere center position along the original ray
            Vector3 sphereCenter = from + (normalizedDirection * hitDistance);

            //BoundingSphere sphere = new BoundingSphere(sphereCenter, sphereRadius);
            //DebugDraw.DrawWireSphere(sphere, Color.Red, 0.1f);

            // Position the camera along the original direction, offset from sphere center
            state.Position.CurrentValue = sphereCenter + (normalizedDirection * offsetFromObstacle);
        }

    }
}
