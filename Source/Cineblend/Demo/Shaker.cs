using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Cineblend;

/// <summary>
/// Shaker Script.
/// </summary>
public class Shaker : Script
{

    public CineImpulseShakeModule ShakeModule;
    float time = 5;
    /// <inheritdoc/>
    public override void OnStart()
    {
        // Here you can add code that needs to be called when script is created, just before the first game update
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
        // Randomly shake the camera every 5 seconds
        time -= Time.DeltaTime;

        if (time <= 0)
        {
            ShakeModule.DoShake();
            time = 5;
        }
    }
}
