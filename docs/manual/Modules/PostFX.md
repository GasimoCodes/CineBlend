# PostFXVolume Module

The **CinePostFXVolume** enables a virtual camera to override the post processing when its active. Cineblend transitions [PostFX Volumes](https://docs.flaxengine.com/manual/graphics/post-effects/post-fx-volumes.html) inbetween VirtualCamera transitions by modifying the weight property. If no PostFXVolume is assigned in the module, it defaults to the first [PostFX](https://docs.flaxengine.com/manual/graphics/post-effects/post-fx-volumes.html) Actor in the child actors of the Virtual Camera.

> This module does not override any camera property.