## Cineblend for FlaxEngine
Brings high-level virtual cameras workflow to the FlaxEngine. 
> This project is in early stages and its API is likely to change throughout development. This plugin was made for our own game production and will not receive priority support. PRs are welcome! If you wish this system to be included in the Flax ecosystem, make sure to share this repo!

### Features
- Priority based blending (Virtual Camera with highest priority will be active)
- Overriding priority by soloing Virtual Cameras.
- Camera blending previews work outside of Play Mode.
- Transition of all exposed Flax Camera values, including Far/Near plane, Orientation or Position.
- VirtualCamera Property PostProcessing, which allows you to non-destructively stack effects (such as CameraShake). These effects are not applied directly to the Virtual Camera transform, but are used while applying transform to the "real" Camera. See ICameraModule.
- Modular: All CineBlend cameras and effects work through interfaces to allow for easy implementation or extensions.
#### Modules (Camera Effects)
- Camera Shake
- Recompose (additive Reframe)
- Look At (Camera faces a world transform, with smoothing options)
- Auto-Framing (Zooms and Rotates the camera to ensure a list of Actors remains on-screen)




https://github.com/user-attachments/assets/1ac69c26-fbf7-4813-8722-a710b52ca834



https://github.com/user-attachments/assets/0bdb62a8-e1b0-4d9b-8f91-e05fa6ec7278



#### To-Do
- PostProcessing Volume blending
- More Gizmos
- "Timeline/Sequencer" Integration

### How to Use  
- Assign CineblendMaster to your Camera Actor
- Create new Virtual Camera Actors (New/CineBlend/Virtual Camera) and set up desired values. To see live preview, SOLO the camera by pressing the SOLO button.
- Assign any Modules you want to the Virtual Camera

- To Transition between Virtual Cameras, use API / Override priorities / Solo an camera / Disable Virtual Camera Actor.

### Known Bugs
- Perlin Noise function for Camera Shake is clamped. This is a FlaxEngine bug. 
