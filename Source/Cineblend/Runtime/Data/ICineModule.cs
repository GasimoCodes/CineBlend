using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend;

/// <summary>
/// ICineModule interface. It defines a single module which contains a collection of properties that can be blended.
/// </summary>
public interface ICameraModule
{
    /// <summary>
    /// Called when the module is initialized
    /// </summary>
    void Initialize(ICineCamera camera);

    /// <summary>
    /// Blends between two module states
    /// </summary>
    void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t) { }

    /// <summary>
    /// Modifies the final camera properties before they are applied
    /// </summary>
    /// <param name="state">Camera State Data Reference.</param>
    /// <param name="deltaTime">Delta time for the effect to use. Compensation is added for inactive cameras which just became active.</param>
    void PostProcessProperties(ref CameraProperties state, float deltaTime);

}