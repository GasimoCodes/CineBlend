using System;
using System.Collections.Generic;
using FlaxEngine;
using Game;

namespace Cineblend;

/// <summary>
/// CineImpulseShakeModule Script.
/// </summary>
public class CineImpulseShakeModule : Script, ICameraModule
{

    // Temp Data from shake
    Quaternion currentShakeDelta;
    float targetTime;
    float currentTime;
    float magnitude;

    Vector3 direction;
    float deviation;

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {
    }

    public void Initialize(VirtualCamera camera)
    {
    }

    public void PostProcessProperties(ref CameraProperties state)
    {
        currentTime += Time.DeltaTime;

        // Calculate current frame and lerp with perlin and add to current camera motion/rotation

    }

    /// <summary>
    /// Does a camera shake for this camera.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake</param>
    /// <param name="duration">For how long the shake should play</param>
    public void DoShake(float magnitude = 10f, float duration = 1f)
    {
        DoShake(magnitude, duration,  10f, new Vector3(0, 0, 0));
    }

    /// <summary>
    /// Does a camera shake for this camera.
    /// </summary>
    /// <param name="magnitude">The intensity of the shake</param>
    /// <param name="duration">For how long the shake should play</param>
    /// <param name="deviation">Deviation from the direction vector. If the vector is up, this will ensure the camera can also deviate in other random directions.</param>
    /// <param name="direction">Direction in which the camera should shake. For instance the direction away from explosion and towards the camera.</param>
    public void DoShake(float magnitude, float duration, float deviation, Vector3 direction)
    {
        currentTime = 0;
        targetTime = duration;

        this.direction = direction;
        this.magnitude = magnitude;
        this.deviation = deviation;

    }

}
