# Collider Offset Module
CineColliderModule postprocesses camera position to retain line-of-sight with the target. This is accompished by moving the camera position in front of the obstacle (with an offset) when the line of sight is broken.

> This module overwrites the Camera Position.

![Collider](../../images/Modules/Collider.mp4)

## Use Cases
- Third-person or orbit-style camera controlls where obstacles between camera and player are common.
- Top-down or side-scrolling cameras in 3D setting which could collide with geometry.