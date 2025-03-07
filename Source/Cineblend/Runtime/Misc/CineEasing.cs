using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend;

/// <summary>
/// Easing enum.
/// </summary>
public enum Easing
{
    Linear,

    // Quadratic
    EaseIn,
    EaseOut,
    EaseInOut,

    // Cubic
    CubicIn,
    CubicOut,
    CubicInOut,

    // Quartic
    QuarticIn,
    QuarticOut,
    QuarticInOut,

    // Exponential
    ExpoIn,
    ExpoOut,
    ExpoInOut,

    // Sine
    SineIn,
    SineOut,
    SineInOut,

    // Circular
    CircIn,
    CircOut,
    CircInOut,

    // Elastic
    ElasticIn,
    ElasticOut,
    ElasticInOut,

    // Back
    BackIn,
    BackOut,
    BackInOut,

    // Bounce
    BounceIn,
    BounceOut,
    BounceInOut
}

public static class CineEasing
{

    public static float ApplyEasing(float t, Easing easingType)
    {
        // Ensure t is clamped between 0 and 1
        t = Mathf.Saturate(t);

        switch (easingType)
        {
            // Linear
            case Easing.Linear:
                return t;

            // Quadratic
            case Easing.EaseIn:
                return t * t; // Quadratic ease in
            case Easing.EaseOut:
                return t * (2 - t); // Quadratic ease out
            case Easing.EaseInOut:
                return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t; // Quadratic ease in-out

            // Cubic
            case Easing.CubicIn:
                return t * t * t;
            case Easing.CubicOut:
                return 1 - Mathf.Pow(1 - t, 3);
            case Easing.CubicInOut:
                return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;

            // Quartic
            case Easing.QuarticIn:
                return t * t * t * t;
            case Easing.QuarticOut:
                return 1 - Mathf.Pow(1 - t, 4);
            case Easing.QuarticInOut:
                return t < 0.5f ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;

            // Exponential
            case Easing.ExpoIn:
                return t == 0 ? 0 : Mathf.Pow(2, 10 * t - 10);
            case Easing.ExpoOut:
                return t == 1 ? 1 : 1 - Mathf.Pow(2, -10 * t);
            case Easing.ExpoInOut:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return t < 0.5f ?
                    Mathf.Pow(2, 20 * t - 10) / 2 :
                    (2 - Mathf.Pow(2, -20 * t + 10)) / 2;

            // Sine
            case Easing.SineIn:
                return 1 - Mathf.Cos(t * Mathf.Pi / 2);
            case Easing.SineOut:
                return Mathf.Sin(t * Mathf.Pi / 2);
            case Easing.SineInOut:
                return -(Mathf.Cos(Mathf.Pi * t) - 1) / 2;

            // Circular
            case Easing.CircIn:
                return 1 - Mathf.Sqrt(1 - t * t);
            case Easing.CircOut:
                return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
            case Easing.CircInOut:
                return t < 0.5f ?
                    (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2 :
                    (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;

            // Elastic
            case Easing.ElasticIn:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * (2 * Mathf.Pi) / 3);
            case Easing.ElasticOut:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * (2 * Mathf.Pi) / 3) + 1;
            case Easing.ElasticInOut:
                if (t == 0) return 0;
                if (t == 1) return 1;
                return t < 0.5f ?
                    -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.Pi) / 4.5f)) / 2 :
                    (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * (2 * Mathf.Pi) / 4.5f)) / 2 + 1;

            // Back
            case Easing.BackIn:
                float c1 = 1.70158f;
                return t * t * ((c1 + 1) * t - c1);
            case Easing.BackOut:
                float c2 = 1.70158f;
                return 1 + (--t) * t * ((c2 + 1) * t + c2);
            case Easing.BackInOut:
                float c3 = 1.70158f * 1.525f;
                return t < 0.5f ?
                    (Mathf.Pow(2 * t, 2) * ((c3 + 1) * 2 * t - c3)) / 2 :
                    (Mathf.Pow(2 * t - 2, 2) * ((c3 + 1) * (t * 2 - 2) + c3) + 2) / 2;

            // Bounce
            case Easing.BounceIn:
                return 1 - BounceEaseOut(1 - t);
            case Easing.BounceOut:
                return BounceEaseOut(t);
            case Easing.BounceInOut:
                return t < 0.5f ?
                    (1 - BounceEaseOut(1 - 2 * t)) / 2 :
                    (1 + BounceEaseOut(2 * t - 1)) / 2;

            default:
                return t;
        }
    }

    // Helper function for bounce easing
    public static float BounceEaseOut(float t)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (t < 1 / d1)
            return n1 * t * t;
        else if (t < 2 / d1)
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        else if (t < 2.5 / d1)
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        else
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
    }
}