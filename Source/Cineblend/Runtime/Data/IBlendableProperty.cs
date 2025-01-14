using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// Interface for properties that can be blended
/// </summary>
public interface IBlendableProperty<T>
{
    T CurrentValue { get; set; }
    T Lerp(T start, T end, float t);
}

