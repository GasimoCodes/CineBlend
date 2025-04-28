# Changelog
All notable changes to this package will be documented in this file.

## [0.1.3] - 28/04/2025
- Fixed incorrect gizmo previews in Prefab mode

## [0.1.2] - 12/03/2025
- Added OnRender event so CineBlend updates only before the camera renders after all the scripts.

- Fix prefab reload causing cameras not to deregister when GC Collected
- CineAuto frame now gets skipped when its disabled
- Fixed editor freezing when a camera has been deinitialized thru Finalizer (For instance on Script Reload)
- Fixed PostFX not being applied to cameras with no transitions (& Cuts)
- Minor Demo scene changes

## [0.1.1] - 09/03/2025
- Added an icon for the virtual camera actor.
- Modules deltaTime is now compensated when their camera becomes active to allow skipping damping on first frame.

- Breaking: Cached Virtual Camera properties are now accessible through 'FinalProperties' property instead of 'LastProperties'.
- Breaking: VirtualCameras now need to implement a ProcessProperties(float DeltaTime) method. The method should copy Properties, post process them and save the output to FinalProperties. Its essentially what calling FinalProperties did until now.
- Breaking: ICameraModule.PostProcessProperties now features a deltaTime argument. This is done to allow the Virtual Camera to compensate for the time a plugin has not been updated.

## [0.1.0] - 07/03/2025
- Added Transition easing options (Linear, SineInOut, ExpOut...), full list is in 'CineEasing.Easing' enum
- Added support for interrupting transitions with new transitions.

## [0.0.3] - 29/01/2025
- Added Collider Module
- Introduced caching into VirtualCamera properties when you want to access existing state and dont need to recalculate.


## [0.0.2] - 29/01/2025
**Changelog Init**
