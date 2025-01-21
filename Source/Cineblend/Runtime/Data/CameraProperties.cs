using System;
using System.Collections.Generic;
using System.Transactions;
using FlaxEngine;

namespace Game;

/// <summary>
/// Collection of camera properties that can be blended
/// </summary>
public struct CameraProperties
{
    public BlendableVector3 Position = new();
    public BlendableQuaternion Rotation = new();
    public BlendableFloat FieldOfView = new(60);
    public BlendableFloat NearPlane = new(10);
    public BlendableFloat FarPlane = new(1000);

    public CameraProperties()
    {
    }


    public void Lerp(CameraProperties start, CameraProperties end, float t)
    {
        Position.CurrentValue = Position.Lerp(start.Position.CurrentValue, end.Position.CurrentValue, t);
        Rotation.CurrentValue = Rotation.Lerp(start.Rotation.CurrentValue, end.Rotation.CurrentValue, t);

        FieldOfView.CurrentValue = FieldOfView.Lerp(start.FieldOfView.CurrentValue, end.FieldOfView.CurrentValue, t);
        NearPlane.CurrentValue = NearPlane.Lerp(start.NearPlane.CurrentValue, end.NearPlane.CurrentValue, t);
        FarPlane.CurrentValue = FarPlane.Lerp(start.FarPlane.CurrentValue, end.FarPlane.CurrentValue, t);
    }
    public void Apply(Camera camera)
    {
        camera.Position = Position.CurrentValue;
        camera.Orientation = Rotation.CurrentValue;

        camera.FieldOfView = FieldOfView.CurrentValue;
        camera.NearPlane = NearPlane.CurrentValue;
        camera.FarPlane = FarPlane.CurrentValue;
    }

    public Matrix GetViewMatrix()
    {
        return Matrix.LookAt(Position.CurrentValue, Position.CurrentValue + Vector3.Transform(Vector3.Forward, Rotation.CurrentValue), Vector3.Transform(Vector3.Up, Rotation.CurrentValue));
    }

    public Matrix GetProjectionMatrix()
    {
        return Matrix.PerspectiveFov(FieldOfView.CurrentValue * Mathf.DegreesToRadians, (16.0f / 9.0f), NearPlane.CurrentValue, FarPlane.CurrentValue);

    }
}


public class BlendableFloat : IBlendableProperty<float>
{
    public float CurrentValue { get; set; }
    public float Lerp(float start, float end, float t) => Mathf.Lerp(start, end, t);


    public BlendableFloat()
    {
    }

    public BlendableFloat(float value)
    {
        CurrentValue = value;
    }


}

public class BlendableVector3 : IBlendableProperty<Vector3>
{
    public Vector3 CurrentValue { get; set; }
    public Vector3 Lerp(Vector3 start, Vector3 end, float t) => Vector3.Lerp(start, end, t);

    public BlendableVector3()
    {
    }

    public BlendableVector3(Vector3 value)
    {
        CurrentValue = value;
    }

}

public class BlendableQuaternion : IBlendableProperty<Quaternion>
{
    public Quaternion CurrentValue { get; set; }
    public Quaternion Lerp(Quaternion start, Quaternion end, float t) => Quaternion.Slerp(start, end, t);

    public BlendableQuaternion()
    {
    }

    public BlendableQuaternion(Quaternion value)
    {
        CurrentValue = value;
    }

}
