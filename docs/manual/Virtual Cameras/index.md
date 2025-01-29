# Virtual Cameras
Virtual Cameras exist in the scene but act as proxies for the physical camera. Each Virtual Camera stores its own settings, including position, rotation, field of view, and modular behaviors. However, they do not directly render the scene. Instead, they pass their settings to the active physical camera, which then applies them. Virtual Cameras are switched based on priority or controlled via API commands.

