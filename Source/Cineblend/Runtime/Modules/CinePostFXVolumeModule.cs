using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Gasimo.CineBlend.Modules;

/// <summary>
/// Allows you to blend postprocessing volumes
/// </summary>
[RequireActor(typeof(VirtualCamera))]
[Category("Cineblend")]
public class CinePostFXVolumeModule : Script, ICameraModule
{

    public PostFxVolume PostFxVolume;
    private ICineCamera cineCamera;

    public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
    {
        if (!this.Enabled)
            return;

        t = Mathf.Clamp(t, 0, 1);

        // if we are the target, lerp our weight to 1
        if (toSnapshot == cineCamera)
        {
            PostFxVolume.BlendWeight = Mathf.Lerp(0f, 1f, t);
        }
        else
        {
            // Else lerp to 0
            PostFxVolume.BlendWeight = Mathf.Lerp(1f, 0f, t);
        }
    }

    public void Initialize(ICineCamera camera)
    {
        cineCamera = camera;

        if (PostFxVolume == null)
            PostFxVolume = Actor.GetChild<PostFxVolume>();

        if (PostFxVolume == null)
        {
            Debug.LogError("CinePostFxModule requires a PostFxVolume child actor to work. The module will be disabled.");
            this.Enabled = false;
            return;
        }

        PostFxVolume.BlendRadius = 0;
        PostFxVolume.BlendWeight = 0;
        PostFxVolume.IsBounded = false;

    }

    public void PostProcessProperties(ref CameraProperties state, float deltaTime)
    {
        if (!this.Enabled)
            return;

    }
}
