# Look-At Module

The **CinePostFXVolume** enables a virtual camera to override the post processing when its active. Cineblend transitions PostFX Volumes inbetween VirtualCamera transitions by modifying the weight property. If no PostFXVolume is assigned in the module, it defaults to the first PostFX Actor in the child actors the camera.

> This module does not override any camera property.