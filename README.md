## Cineblend for FlaxEngine
Brings high-level virtual cameras workflow to the FlaxEngine. 
[Documentation](https://gasimo.dev/CineBlend/)
> This project is in early stages and its API is likely to change throughout development. This plugin was made for our own game production and will not receive priority support. PRs are welcome! If you wish this system to be included in the Flax ecosystem, make sure to share this repo!



https://github.com/user-attachments/assets/404e046d-217c-4ccc-a410-2f2ca095e6d7



### Features
- Priority based blending (Virtual Camera with highest priority will be active)
- Overriding priority by soloing Virtual Cameras.
- Camera blending previews work outside of Play Mode.
- Transition of all exposed Flax Camera values, including Far/Near plane, Orientation or Position.
- VirtualCamera Property PostProcessing, which allows you to non-destructively stack effects (such as CameraShake). These effects are not applied directly to the Virtual Camera transform, but are used while applying transform to the "real" Camera. See ICameraModule.
- Modular: All CineBlend cameras and effects work through interfaces to allow for easy implementation or extensions.
- Large World Support using Reals
- Customizable transitions with easing and length
- 
#### Modules (Camera Effects)
- Camera Shake
- Recompose (additive Reframe)
- Look At (Camera faces a world transform, with smoothing options)
- Auto-Framing (Zooms and Rotates the camera to ensure a list of Actors remains on-screen)


#### To-Do
- PostProcessing Volume blending
- Gizmo icons
- "Timeline/Sequencer" Integration

### How to Use  
- Assign CineblendMaster to your Camera Actor
- Create new Virtual Camera Actors (New/CineBlend/Virtual Camera) and set up desired values. To see live preview, SOLO the camera by pressing the SOLO button.
- Assign any Modules you want to the Virtual Camera

- To Transition between Virtual Cameras, use API / Override priorities / Solo an camera / Disable Virtual Camera Actor.

### Known Bugs

