using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game
{
    /// <summary>
    /// Virtual Camera Actor that can be blended between different states
    /// </summary>
    [ActorContextMenu("New/CineBlend/Virtual Camera")]
    public class VirtualCamera : Actor, ICineCamera
    {
        private int priority;
        private CameraProperties properties = new();

        [ShowInEditor, ReadOnly]
        private bool isActive => (CineblendMaster.Instance?.currentVirtualCamera == (ICineCamera)this);

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
        
        /// <summary>
        /// Gets postprocessed properties
        /// </summary>
        public CameraProperties FinalProperties
        {
            get
            {
                CameraProperties state = properties;
                
                foreach (var module in Modules)
                {
                    module.Value.PostProcessProperties(ref state);
                }

                // Debug.Log(state.Position.CurrentValue);

                return state;
            }
        }

        /// <summary>
        /// Camera priority. Higher priority cameras will override lower priority cameras.
        /// </summary>
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
        [Header("Camera")]
        public float FieldOfView
        {
            get => properties.FieldOfView.CurrentValue;
            set => properties.FieldOfView.CurrentValue = value;
        }

        public float NearPlane
        {
            get => properties.NearPlane.CurrentValue;
            set => properties.NearPlane.CurrentValue = value;
        }

        public float FarPlane
        {
            get => properties.FarPlane.CurrentValue;
            set => properties.FarPlane.CurrentValue = value;
        }




        /// <summary>
        /// Initialize the virtual camera
        /// </summary>
        public override void OnBeginPlay()
        {
            base.OnBeginPlay();

            // Search for modules on this object
            foreach (var module in GetScripts<ICameraModule>())
            {
                Modules[ module.GetType()] = module;
            }

            // Initialize modules
            foreach (var module in Modules)
            {
                module.Value.Initialize(this);
            }
        }

        /// <summary>
        /// Register with CineblendMaster when enabled
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            CineblendMaster.Instance?.RegisterVirtualCamera(this);
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

        public override void OnDebugDraw()
        {
            base.OnDebugDraw();

            // Draw frustrum same way as camera does. Ignore modules when inactive.
            var state = properties;

            Matrix matrix = Matrix.Scaling(state.FieldOfView.CurrentValue, state.FieldOfView.CurrentValue, state.FarPlane.CurrentValue) *
                           Matrix.Translation(this.Position) *
                           Matrix.RotationQuaternion(this.Orientation);

            BoundingFrustum frustum = new BoundingFrustum(matrix);

            DebugDraw.DrawWireFrustum (frustum, Color.Red, 0.0f, false);

        }


    }
}