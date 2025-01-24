using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Cineblend;

/// <summary>
/// Mover Script.
/// </summary>
public class Mover : Script
{
    Vector3 orbitcenter;

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
        float time = Time.GameTime;
        float speed = 0.5f;

        float x = Mathf.Sin(time * speed) * 100;

        Actor.Position = orbitcenter + new Vector3(x, 0, 0);

        // Rotate the object

        Actor.Orientation = Quaternion.RotationYawPitchRoll(time * speed, time * speed, time * speed);

    }
}
