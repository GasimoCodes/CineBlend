﻿using System;
using FlaxEngine;

namespace Gasimo.CineBlend;

/// <summary>
/// Collection of camera properties that can be blended
/// </summary>
public struct CameraProperties : ICloneable
{
    /// <summary>
    /// Position of the camera
    /// </summary>
    public BlendableVector3 Position = new();
    
    /// <summary>
    /// Orientation of the Camera
    /// </summary>
    public BlendableQuaternion Rotation = new();
    
    /// <summary>
    /// FOV Of the Camera
    /// </summary>
    public BlendableReal FieldOfView = new(60);
    
    /// <summary>
    /// Near Plane of the Camera
    /// </summary>
    public BlendableReal NearPlane = new(10);
    
    /// <summary>
    /// Far Plane of the Camera
    /// </summary>
    public BlendableReal FarPlane = new(10000);

    public CameraProperties()
    {
    }

    /// <summary>
    /// Utility class to blend between 2 camera properties and apply them to this property.
    /// </summary>
    /// <param name="start">Starting properties</param>
    /// <param name="end">Ending Properties</param>
    /// <param name="t">Lerp value</param>
    public void LerpAndSet(CameraProperties start, CameraProperties end, float t)
    {
        Position.CurrentValue = BlendableVector3.Lerp(start.Position.CurrentValue, end.Position.CurrentValue, t);
        Rotation.CurrentValue = BlendableQuaternion.Lerp(start.Rotation.CurrentValue, end.Rotation.CurrentValue, t);

        FieldOfView.CurrentValue = BlendableReal.Lerp(start.FieldOfView.CurrentValue, end.FieldOfView.CurrentValue, t);
        NearPlane.CurrentValue = BlendableReal.Lerp(start.NearPlane.CurrentValue, end.NearPlane.CurrentValue, t);
        FarPlane.CurrentValue = BlendableReal.Lerp(start.FarPlane.CurrentValue, end.FarPlane.CurrentValue, t);
    }

    /// <summary>
    /// Applies the camera properties to a real camera
    /// </summary>
    /// <param name="camera"></param>
    public void ApplyToCamera(Camera camera)
    {
        camera.Position = Position.CurrentValue;
        camera.Orientation = Rotation.CurrentValue;

        camera.FieldOfView = FieldOfView.CurrentValue;
        camera.NearPlane = NearPlane.CurrentValue;
        camera.FarPlane = FarPlane.CurrentValue;
    }

    /// <summary>
    /// Returns the ViewMatrix constructed from the camera properties
    /// </summary>
    /// <returns></returns>
    public Matrix GetViewMatrix()
    {
        return Matrix.LookAt(Position.CurrentValue, Position.CurrentValue + Vector3.Transform(Vector3.Forward, Rotation.CurrentValue), Vector3.Transform(Vector3.Up, Rotation.CurrentValue));
    }

    /// <summary>
    /// Returns the ProjectionMatrix constructed from the camera properties
    /// </summary>
    /// <returns></returns>
    public Matrix GetProjectionMatrix()
    {
        return Matrix.PerspectiveFov(FieldOfView.CurrentValue * Mathf.DegreesToRadians, (Screen.Size.X / Screen.Size.Y), NearPlane.CurrentValue, FarPlane.CurrentValue);

    }

    public object Clone()
    {
        return new CameraProperties
        {
            Position = new BlendableVector3(Position.CurrentValue),
            Rotation = new BlendableQuaternion(Rotation.CurrentValue),
            FieldOfView = new BlendableReal(FieldOfView.CurrentValue),
            NearPlane = new BlendableReal(NearPlane.CurrentValue),
            FarPlane = new BlendableReal(FarPlane.CurrentValue)
        };
    }
}

/// <summary>
/// Float Lerp utility class
/// </summary>
public class BlendableReal : IBlendableProperty<float>
{
    public Real CurrentValue { get; set; }
    public static Real Lerp(Real start, Real end, float t) => Real.Lerp(start, end, t);


    public BlendableReal()
    {
    }

    public BlendableReal(float value)
    {
        CurrentValue = value;
    }


}

/// <summary>
/// Vector3 Lerp utility class
/// </summary>
public class BlendableVector3 : IBlendableProperty<Vector3>
{
    public Vector3 CurrentValue { get; set; }
    public static Vector3 Lerp(Vector3 start, Vector3 end, float t) => Vector3.Lerp(start, end, t);

    public BlendableVector3()
    {
    }

    public BlendableVector3(Vector3 value)
    {
        CurrentValue = value;
    }

}


/// <summary>
/// Quaternion Lerp utility class
/// </summary>
public class BlendableQuaternion : IBlendableProperty<Quaternion>
{
    public Quaternion CurrentValue { get; set; }
    public static Quaternion Lerp(Quaternion start, Quaternion end, float t) => Quaternion.Slerp(start, end, t);

    public BlendableQuaternion()
    {
    }

    public BlendableQuaternion(Quaternion value)
    {
        CurrentValue = value;
    }

}
