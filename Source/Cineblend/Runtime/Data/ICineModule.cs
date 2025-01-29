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
    void Initialize(VirtualCamera camera);

    /// <summary>
    /// Blends between two module states
    /// </summary>
    void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t);

    /// <summary>
    /// Modifies the final camera properties before they are applied
    /// </summary>
    void PostProcessProperties(ref CameraProperties state);

}
