# Custom Camera Modules

## Overview
All Cineblend Modules must implement the [`ICameraModule`](/api/Gasimo.CineBlend.ICameraModule.html) interface. For best integration with the editor, your modules should inherit from Flax's `Script` type, allowing them to be assigned to Virtual Cameras through the editor interface.

## Core Interface Methods
The `ICameraModule` interface lets you integrate with the Cineblend pipeline through three main methods:

### 1. Initialize
```csharp
void Initialize(ICineCamera camera)
```
Called when a Virtual Camera is created and registered with the system. Use this method to cache any required references for your module.

### 2. PostProcessProperties
```csharp
void PostProcessProperties(ref CameraProperties state, float deltaTime)
```
This is the primary method for modifying camera behavior. It's called each frame when the camera is:
- Active (Soloed/Highest Priority)
- In preview mode
- Blending with another camera
- Otherwise needed

The method receives a reference to the current [camera state](/api/Gasimo.CineBlend.CameraProperties.html), which you can modify as needed. Important notes:
- The state has already been processed by any previous modules on the camera
- Changes you make will be passed to subsequent modules
- Always include an early exit check for the `Enabled` property to support user control via the editor
- deltaTime may be large as it is compensated by cineblend for the time the camera has been offline.

### 3. Blend
```csharp
void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
```
Used for implementing custom logic when blending between cameras. Only implement this if you need to blend properties not available in the standard [CameraProperties](/api/Gasimo.CineBlend.CameraProperties.html) struct. Note that this method is only called during active camera blends.

## Example Implementation
Here's a complete example of a camera module that adds position and rotation offsets to reframe the camera:

```csharp
/// <summary>
/// Allows you to offset the position and rotation of the camera.
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CineRecomposeModule : Script, ICameraModule
{
    public Vector3 PositionOffset;
    public Quaternion RotationOffset;

    public void Initialize(ICineCamera camera)
    {
    }

    public void PostProcessProperties(ref CameraProperties state, float deltaTime)
    {
        if(!this.Enabled)
            return;
            
        state.Rotation.CurrentValue = state.Rotation.CurrentValue * RotationOffset;
        state.Position.CurrentValue += PositionOffset;
    }

    public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
    {
    }
}
```