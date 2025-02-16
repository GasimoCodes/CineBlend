using System;
using System.Collections.Generic;
using FlaxEngine;
#if USE_LARGE_WORLDS
using Real = System.Double;
#else
using Real = System.Single;
#endif

namespace Cineblend;

/// <summary>
/// Mover Script.
/// </summary>
public class Mover : Script
{
    Vector3 orbitcenter;
    public Vector3 RotateBy = new Vector3(0, 0.1f, 0);

    /// <inheritdoc/>
    public override void OnStart()
    {
        // Here you can add code that needs to be called when script is created, just before the first game update
        orbitcenter = Actor.Position;
    }
    
    /// <inheritdoc/>
    public override void OnEnable()
    {
        // Here you can add code that needs to be called when script is enabled (eg. register for events)
    }

    /// <inheritdoc/>
    public override void OnDisable()
    {
        // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
    }

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        // Make this object orbit around itself
        Real time = Time.GameTime;
        Real speed = 0.5f;

        Real x = Real.Sin(time * speed) * 100;

        Actor.Position = orbitcenter + new Vector3(x, 0, 0);

        // Rotate the object

        Actor.Orientation = Quaternion.RotationYawPitchRoll((float)(time * speed * RotateBy.X), (float)(time * speed * RotateBy.Y), (float)(time * speed * RotateBy.Z));

    }
}
