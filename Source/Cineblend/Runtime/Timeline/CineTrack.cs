#if FLAX_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlaxEditor;
using FlaxEditor.GUI.Timeline;
using FlaxEditor.GUI.Timeline.Tracks;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEditor.Utilities;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEditor.GUI.Timeline.Tracks;

namespace Gasimo.CineBlend.Timeline;

/// <summary>
/// CineTrack class.
/// </summary>
public class CineTrack : ActorTrack
{

    private Image _pilotCamera;

    //
    // Summary:
    //     Gets the camera object instance (it might be missing).
    public Camera Camera => base.Actor as Camera;

    private CameraCutMedia TrackMedia
    {
        get
        {
            CameraCutMedia cameraCutMedia;
            if (base.Media.Count == 0)
            {
                cameraCutMedia = new CameraCutMedia
                {
                    StartFrame = 0,
                    DurationFrames = ((base.Timeline != null) ? ((int)(base.Timeline.FramesPerSecond * 2f)) : 60)
                };
                AddMedia(cameraCutMedia);
            }
            else
            {
                cameraCutMedia = (CameraCutMedia)base.Media[0];
            }

            return cameraCutMedia;
        }
    }



    public CineTrack(ref TrackCreateOptions options, bool useProxyKeyframes = true) : base(ref options, useProxyKeyframes)
    {
        MinMediaCount = 1;
        base.Height = CameraCutThumbnailRenderer.Height + 8;
        EditorIcons icons = Editor.Instance.Icons;
        
        /*
        _pilotCamera = new Image
        {
            TooltipText = "Starts piloting camera (in scene edit window)",
            AutoFocus = true,
            AnchorPreset = AnchorPresets.MiddleRight,
            IsScrollable = false,
            Color = Style.Current.ForegroundGrey,
            Margin = new Margin(1f),
            Brush = new SpriteBrush(icons.CameraFill32),
            Offsets = new Margin(-20f + _selectActor.Offsets.Left, 18f, -9f, 18f),
            Parent = this
        };
        _pilotCamera.Clicked += OnClickedPilotCamera;
        */
    }







    //
    // Summary:
    //     Gets the archetype.
    //
    // Returns:
    //     The archetype.
    public new static TrackArchetype GetArchetype()
    {
        TrackArchetype result = default(TrackArchetype);
        result.TypeId = 16;
        result.Name = "Cine Cut (Virtual Cameras)";
        result.Create = (TrackCreateOptions options) => new CineTrack(ref options);
        result.Load = LoadTrack;
        result.Save = SaveTrack;
        return result;
    }

    private static void LoadTrack(int version, Track track, BinaryReader stream)
    {
        CineTrack cameraCutTrack = (CineTrack)track;
        //cameraCutTrack.ActorID = stream.ReadGuid();
        if (version <= 3)
        {
            CameraCutMedia trackMedia = cameraCutTrack.TrackMedia;
            trackMedia.StartFrame = stream.ReadInt32();
            trackMedia.DurationFrames = stream.ReadInt32();
            return;
        }

        int num = stream.ReadInt32();
        while (cameraCutTrack.Media.Count > num)
        {
            cameraCutTrack.RemoveMedia(cameraCutTrack.Media.Last());
        }

        while (cameraCutTrack.Media.Count < num)
        {
            cameraCutTrack.AddMedia(new CameraCutMedia());
        }

        for (int i = 0; i < num; i++)
        {
            CameraCutMedia cameraCutMedia = (CameraCutMedia)cameraCutTrack.Media[i];
            cameraCutMedia.StartFrame = stream.ReadInt32();
            cameraCutMedia.DurationFrames = stream.ReadInt32();
            cameraCutMedia.UpdateThumbnails();
        }
    }

    private static void SaveTrack(Track track, BinaryWriter stream)
    {
        CameraCutTrack cameraCutTrack = (CameraCutTrack)track;
        //stream.WriteGuid(ref cameraCutTrack.ActorID);
        int count = cameraCutTrack.Media.Count;
        stream.Write(count);
        for (int i = 0; i < count; i++)
        {
            Media media = cameraCutTrack.Media[i];
            stream.Write(media.StartFrame);
            stream.Write(media.DurationFrames);
        }
    }















    private void UpdateThumbnails()
    {
        foreach (CameraCutMedia medium in base.Media)
        {
            medium.UpdateThumbnails();
        }
    }

    protected override void OnObjectExistenceChanged(object obj)
    {
        base.OnObjectExistenceChanged(obj);
        UpdateThumbnails();
    }

    protected override bool IsActorValid(Actor actor)
    {
        if (base.IsActorValid(actor))
        {
            return actor is Camera && (actor.GetScript<CineblendMaster>() != null);
        }

        return false;
    }

    protected override void OnActorChanged()
    {
        base.OnActorChanged();
        UpdateThumbnails();
    }

    public override void OnSpawned()
    {
        CameraCutMedia trackMedia = TrackMedia;
        base.OnSpawned();
    }

    public override void OnDestroy()
    {
        _pilotCamera = null;
        base.OnDestroy();
    }
}
#endif