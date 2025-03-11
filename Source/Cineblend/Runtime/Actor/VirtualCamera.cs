﻿using System;
using System.Collections.Generic;
using FlaxEngine;
using Gasimo.CineBlend.Modules;

#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.SceneGraph;
#endif

namespace Gasimo.CineBlend
{
    /// <summary>
    /// Virtual Camera Actor that can be blended between different states
    /// </summary>
    [ActorContextMenu("New/CineBlend/Virtual Camera")]
    [ActorToolbox("Visuals")]
    public class VirtualCamera : EmptyActor, ICineCamera
    {

        private int priority;
        private CameraProperties properties = new();

        [ShowInEditor, ReadOnly]
        [EditorDisplay("Virtual Camera Status")]
        [EditorOrder(0)]
        private bool isActive => (CineblendMaster.Instance?.currentVirtualCamera == (ICineCamera)this);

        [ReadOnly, ShowInEditor, NoSerialize]
        [EditorDisplay("Virtual Camera Status")]
        public string UpdateMode
        {
            get
            {
                if (CineblendMaster.Instance == null || CineblendMaster.Instance.UpdateModeOverride == CameraUpdateMode.Auto)
                {
                    return this.CameraUpdateMode.ToString();
                }

                return CineblendMaster.Instance.UpdateModeOverride.ToString() + " (From CineblendMaster)";
            }
        }

        [ReadOnly, HideInEditor, NoSerialize]
        public CameraUpdateMode CameraUpdateMode
        {
            get
            {
                if (UpdateModeOverride != CameraUpdateMode.Auto)
                {
                    return UpdateModeOverride;
                }

                Actor ParentObj = Parent;

                // Check if this Camera is a child of any Rigidbody
                while (true)
                {
                    if (ParentObj is RigidBody)
                    {
                        return CameraUpdateMode.FixedUpdate;
                    }
                    if (ParentObj is Actor)
                    {
                        ParentObj = (ParentObj as Actor).Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                return CameraUpdateMode.Update;
            }
        }

        [EditorDisplay("Virtual Camera Status")]
        public CameraUpdateMode UpdateModeOverride = CameraUpdateMode.Auto;


        /// <summary>
        /// Modules on the camera. Init with CineTransform module.
        /// </summary>
        public Dictionary<Type, ICameraModule> Modules { get; private set; } = new() {
 { typeof(CineTransformModule), new CineTransformModule() }
        };

        /// <summary>
        /// Gets the camera's current properties
        /// </summary>
        [HideInEditor]
        public CameraProperties Properties
        {
            get => properties;
            set => properties = value;
        }


        public void ProcessProperties(float deltaTime)
        {
            CameraProperties state = (CameraProperties)properties.Clone();

            foreach (var module in Modules)
            {
                module.Value.PostProcessProperties(ref state, deltaTime);
            }

            FinalProperties = state;
        }

        /// <summary>
        /// Cached properties from the last time this camera was used as active. Use when you need to read the final properties for this camera while active to prevent 
        /// re-calculating or use Camera.Main instead.
        /// </summary>
        public CameraProperties FinalProperties { get; private set; }

        /// <summary>
        /// Camera priority. Higher priority cameras will override lower priority cameras.
        /// </summary>
        [EditorDisplay("Virtual Camera Status")]
        [EditorOrder(1)]
        public int Priority
        {
            get => priority;
            set
            {
                if (priority != value)
                {
                    int oldPriority = priority;
                    priority = value;
                    OnPriorityChanged(oldPriority, value);
                }
            }
        }


        // Expose properties via the blendable system

        [EditorDisplay("Virtual Camera Properties")]
        public float FieldOfView
        {
            get => properties.FieldOfView.CurrentValue;
            set => properties.FieldOfView.CurrentValue = value;
        }

        [EditorDisplay("Virtual Camera Properties")]
        public float NearPlane
        {
            get => properties.NearPlane.CurrentValue;
            set => properties.NearPlane.CurrentValue = value;
        }

        [EditorDisplay("Virtual Camera Properties")]
        public float FarPlane
        {
            get => properties.FarPlane.CurrentValue;
            set => properties.FarPlane.CurrentValue = value;
        }

        public Actor Actor => this;

       
        /// <summary>
        /// Initialize the virtual camera
        /// </summary>
        public override void OnBeginPlay()
        {
            base.OnBeginPlay();
            RefreshModules();
        }

        /// <summary>
        /// Register with CineblendMaster when enabled
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            RefreshModules();
            CineblendMaster.Instance?.RegisterVirtualCamera(this);

#if FLAX_EDITOR
            ViewportIconsRenderer.AddActor(this);
#endif

        }

        /// <summary>
        /// Unregister from CineblendMaster when disabled
        /// </summary>
        public override void OnDisable()
        {
            CineblendMaster.Instance?.UnregisterVirtualCamera(this);
            base.OnDisable();
        }

        private void OnPriorityChanged(int oldPriority, int newPriority)
        {
            CineblendMaster.Instance?.UpdateVirtualCameraPriority(this, oldPriority, newPriority);
        }


        /// <summary>
        /// Adds a new module to the camera
        /// </summary>
        public T AddModule<T>(T module) where T : ICameraModule
        {
            module.Initialize(this);
            Modules.Add(module.GetType(), module);
            return module;
        }

        /// <summary>
        /// Gets a module of the specified type
        /// </summary>
        public T GetModule<T>() where T : class, ICameraModule
        {
            Modules.TryGetValue(typeof(T), out var module);
            return module as T;
        }

        /// <summary>
        /// Removes a module of the specified type
        /// </summary>
        public void RemoveModule<T>() where T : ICameraModule
        {
            Modules.Remove(typeof(T));
        }

        [Button("Solo This")]
        public void Solo()
        {
            CineblendMaster.Instance?.SetSolo(this);
        }

        [Button("Clear Solo")]
        public void ClearSolo()
        {
            CineblendMaster.Instance?.ClearSolo();
        }

        private void RefreshModules()
        {
            // Clear modules to ensure the order of effects is cleared
            Modules.Clear();
            Modules.Add(typeof(CineTransformModule), new CineTransformModule());

            // Search for modules on this object
            foreach (var module in GetScripts<ICameraModule>())
            {
                Modules[module.GetType()] = module;
            }

            // Initialize modules
            foreach (var module in Modules)
            {
                // Debug.Log("-- Initiate module for " + this.Name + "/" + module.GetType());
                module.Value.Initialize(this);
            }
        }


#if FLAX_EDITOR
        public override void OnDebugDraw()
        {
            base.OnDebugDraw();

            // var prop = properties;

            // DebugDraw.DrawWireArrow(Position, prop.Rotation.CurrentValue, 0.5f, 1, FlaxEngine.Color.Red);
        }

        public override void OnDebugDrawSelected()
        {
            base.OnDebugDrawSelected();

            // Process properties for preview if not active
            if (!isActive)
                ProcessProperties(Time.UnscaledDeltaTime);

            var color = isActive ? FlaxEngine.Color.Green : FlaxEngine.Color.Orange;
            // Draw frustrum the same way Camera does
            BoundingFrustum frustrum = new BoundingFrustum((FinalProperties.GetViewMatrix()) * FinalProperties.GetProjectionMatrix());
            DebugDraw.DrawWireFrustum(frustrum, color);

        }


        static VirtualCamera()
        {
            ViewportIconsRenderer.AddCustomIcon(typeof(VirtualCamera), Content.LoadAsync<Texture>(System.IO.Path.Combine(Globals.ProjectFolder, "Plugins/CineBlend/Content/Gizmos/VCam.flax")));
            SceneGraphFactory.CustomNodesTypes.Add(typeof(VirtualCamera), typeof(VirtualCameraNode));
        }
#endif

        ~VirtualCamera()
        {
#if FLAX_EDITOR
            ViewportIconsRenderer.RemoveActor(this);
#endif

            CineblendMaster.Instance?.UnregisterVirtualCamera(this);
        }


    }

#if FLAX_EDITOR
    /// <summary>Custom actor node for Editor.</summary>
    [HideInEditor]
    public sealed class VirtualCameraNode : ActorNodeWithIcon
    {

        /// <inheritdoc />
        public VirtualCameraNode(Actor actor)
            : base(actor)
        {
        }
    }
#endif



}