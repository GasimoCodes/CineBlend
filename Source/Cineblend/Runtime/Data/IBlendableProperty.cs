using System;
using System.Collections.Generic;
using FlaxEngine;
#if USE_LARGE_WORLDS
using Real = System.Double;
#else
using Real = System.Single;
#endif

namespace Gasimo.CineBlend;

/// <summary>
/// Interface for properties that can be blended
/// </summary>
public interface IBlendableProperty<T>
{
    /// <summary>
    /// Current value of the property
    /// </summary>
    T CurrentValue { get; set; }

    /// <summary>
    ///  Linearly interpolates between two values
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns>Interpolated value</returns>
    static abstract T Lerp(T start, T end, Real t);
}

