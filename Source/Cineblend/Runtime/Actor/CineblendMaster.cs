using System;
using System.Collections.Generic;
using System.Linq;
using Cineblend;
using FlaxEngine;

namespace Gasimo.CineBlend
{
    /// <summary>
    /// Camera Controller. Manages camera transitions and blending and applies them to the Main Camera this script is attached to.
    /// </summary>
    [RequireActor(typeof(Camera))]
    [ExecuteInEditMode]
    [Category("Cineblend")]
    public class CineblendMaster : Script, ICineCamera
    {
        #region singleton
        private static CineblendMaster instance;
        public static CineblendMaster Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Level.FindScript<CineblendMaster>();
                }
                return instance;
            }
        }

        #endregion


        private Camera camera;

        /// <summary>
        /// Default blend time for camera transitions
        /// </summary>
        [Tooltip("Default blend time for camera transitions")]
        public float DefaultBlendTime { get; set; } = 1.0f;

        [Tooltip("Default easing type for camera transitions")]
        public Easing DefaultEasingType = Easing.EaseInOut;


        /// <summary>
        /// Update mode of Cineblend. If set to Auto, cameras can override their update mode.
        /// </summary>
        public CameraUpdateMode UpdateModeOverride = CameraUpdateMode.Auto;

        private CameraUpdateMode currentUpdateMode;
        public CameraUpdateMode CameraUpdateMode => currentUpdateMode;



        // Transition vars
        private ICineCamera lastVirtualCamera;

        /// <summary>
        /// Current active Virtual Camera
        /// </summary>
        public ICineCamera currentVirtualCamera { get; private set; }
        private ICineCamera soloCamera;
        private SortedDictionary<int, List<ICineCamera>> virtualCamerasByPriority = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        private class BlendState
        {
            public ICineCamera FromCamera;
            public ICineCamera ToCamera;
            public Easing TransitionEasing;
            public float BlendTime;
            public float CurrentTime;
            public bool IsActive => CurrentTime < BlendTime;

            public float NormalizedTime => Mathf.Saturate(CurrentTime / BlendTime);
        }

        private BlendState activeBlend = null;



        /// <summary>
        /// This Camera's Properties.
        /// </summary>
        public CameraProperties Properties { get; private set; }
        /// <summary>
        /// This Camera's active modules.
        /// </summary>
        public Dictionary<Type, ICameraModule> Modules { get; private set; } = new();
        public int Priority => int.MinValue;
        public string Name => this.Actor.Name;

        public CameraProperties FinalProperties => throw new NotImplementedException();

        public void ProcessProperties(float deltaTime)
        { }


        public override void OnStart()
        {
            camera = Actor as Camera;
            PullDefaultProperties();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            MainRenderTask.Instance.PreRender += OnRender;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            // Check null in case engine is shutting down and renderer may not exist anymore.
            if (MainRenderTask.Instance != null)
                MainRenderTask.Instance.PreRender -= OnRender;
        }

        #region Update Methods

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateActiveCamera();

            if (currentUpdateMode == CameraUpdateMode.Update)
            {
                UpdateCameraBlending();
            }
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if (currentUpdateMode == CameraUpdateMode.LateUpdate)
            {
                UpdateCameraBlending();
            }
        }

        public override void OnLateFixedUpdate()
        {
            base.OnLateFixedUpdate();

            if (currentUpdateMode == CameraUpdateMode.LateFixedUpdate)
            {
                UpdateCameraBlending();
            }
        }

        public override void OnFixedUpdate()
        {

            base.OnFixedUpdate();

            if (currentUpdateMode == CameraUpdateMode.FixedUpdate)
            {
                UpdateCameraBlending();
            }
        }

        private void OnRender(GPUContext arg0, ref RenderContext arg1)
        {
            if (currentUpdateMode == CameraUpdateMode.OnRender)
            {
                UpdateCameraBlending();
            }
        }

        #endregion

        private void PullDefaultProperties()
        {
            Properties = new CameraProperties();
            Properties.FieldOfView.CurrentValue = camera.FieldOfView;
            Properties.NearPlane.CurrentValue = camera.NearPlane;
            Properties.FarPlane.CurrentValue = camera.FarPlane;

            Properties.Position.CurrentValue = camera.Transform.Translation;
            Properties.Rotation.CurrentValue = camera.Transform.Orientation;

            currentVirtualCamera = null;
            activeBlend = null;
            currentUpdateMode = UpdateModeOverride;



            RegisterVirtualCamera(this);
        }


        private void UpdateActiveCamera()
        {
            ICineCamera highestPriorityCamera = GetHighestPriorityCamera();

            if (currentVirtualCamera != highestPriorityCamera)
            {
                // Update mode
                if (UpdateModeOverride == CameraUpdateMode.Auto)
                {
                    currentUpdateMode = highestPriorityCamera.CameraUpdateMode;
                }

                if (currentVirtualCamera == null)
                {
                    // Dont perform transition when this is the first camera.
                    Transition(highestPriorityCamera, 0);
                }
                else
                {
                    // Perform default settings transition
                    Transition(highestPriorityCamera, DefaultBlendTime, DefaultEasingType);
                }

            }
        }


        public void UpdateCameraBlending()
        {
            if (activeBlend == null)
            {
                // Copy values from camera
                currentVirtualCamera.ProcessProperties(Time.UnscaledDeltaTime);
                CameraProperties finalProp = currentVirtualCamera.FinalProperties;
                finalProp.ApplyToCamera(camera);
                return;
            }

            var blend = activeBlend;
            blend.CurrentTime += Time.UnscaledDeltaTime;
            float t = blend.NormalizedTime;


            // Let modules know we are transitioning
            blend.FromCamera.OnBlend(blend.FromCamera, blend.ToCamera, t);
            blend.ToCamera.OnBlend(blend.FromCamera, blend.ToCamera, t);



            blend.FromCamera.ProcessProperties(Time.UnscaledDeltaTime);
            blend.ToCamera.ProcessProperties(Time.UnscaledDeltaTime);

            // Lerp between the two cameras
            CameraProperties toProperties = currentVirtualCamera.FinalProperties;
            CameraProperties fromProperties = lastVirtualCamera?.FinalProperties ?? Properties;

            // Apply easing
            float easedT = CineEasing.ApplyEasing(t, blend.TransitionEasing);

            // Perform the blend with the eased time value
            Properties.LerpAndSet(fromProperties, toProperties, easedT);
            Properties.ApplyToCamera(camera);


            // Check if blend is complete
            if (!blend.IsActive)
            {
                activeBlend = null;
                currentVirtualCamera = blend.ToCamera;
                lastVirtualCamera = blend.FromCamera;
            }
        }


        public void RegisterVirtualCamera(ICineCamera virtualCamera)
        {
            if (!virtualCamerasByPriority.ContainsKey(virtualCamera.Priority))
            {
                virtualCamerasByPriority[virtualCamera.Priority] = new List<ICineCamera>();
            }
            virtualCamerasByPriority[virtualCamera.Priority].Add(virtualCamera);

            // Debug.Log("Registered camera " + virtualCamera.Name + " " + virtualCamera.Priority);
        }


        public void UnregisterVirtualCamera(ICineCamera virtualCamera)
        {
            if (virtualCamerasByPriority.TryGetValue(virtualCamera.Priority, out var cameras))
            {
                cameras.Remove(virtualCamera);
                if (cameras.Count == 0)
                {
                    virtualCamerasByPriority.Remove(virtualCamera.Priority);
                }
            }

            if (soloCamera == virtualCamera)
            {
                soloCamera = null;
            }

            // If this was the last camera, take values from the MainCam. This way we prevent a null reference when a camera gets destroyed.
            if (currentVirtualCamera == (ICineCamera)virtualCamera)
            {
                lastVirtualCamera = this;
            }

        }


        public void UpdateVirtualCameraPriority(VirtualCamera virtualCamera, int oldPriority, int newPriority)
        {
            if (virtualCamerasByPriority.TryGetValue(oldPriority, out var oldList))
            {
                oldList.Remove(virtualCamera);
                if (oldList.Count == 0)
                {
                    virtualCamerasByPriority.Remove(oldPriority);
                }
            }

            if (!virtualCamerasByPriority.ContainsKey(newPriority))
            {
                virtualCamerasByPriority[newPriority] = new List<ICineCamera>();
            }
            virtualCamerasByPriority[newPriority].Add(virtualCamera);
        }



        private ICineCamera GetHighestPriorityCamera()
        {
            // If any camera is soloed, only consider soloed cameras
            if (soloCamera != null)
            {
                return soloCamera;
            }

            // Otherwise, get the highest priority camera
            foreach (var kvp in virtualCamerasByPriority)
            {
                if (kvp.Value.Count > 0)
                {
                    return kvp.Value[0];
                }
            }

            Debug.LogWarning("No cameras found");

            return null;
        }




        /// <summary>
        /// Transitions to the selected camera using global values
        /// </summary>
        /// <param name="toCamera"></param>
        public void Transition(ICineCamera toCamera)
        {
            Transition(toCamera, DefaultBlendTime);
        }

        /// <summary>
        /// Transitions to the selected camera
        /// </summary>
        /// <param name="toCamera"></param>
        public void Transition(ICineCamera toCamera, float blendTime)
        {
            Transition(toCamera, blendTime, DefaultEasingType);
        }

        /// <summary>
        /// Transitions to the selected camera
        /// </summary>
        /// <param name="toCamera">Camera to transition to</param>
        /// <param name="blendTime">Time to complete the transition</param>
        /// <param name="easing">Easing/Smoothing which should be applied to the transition progress</param>
        /// <param name="forceSnapshotCreation">When enabled, Cineblend will interpolate from a static snapshot of the last Virtual Camera, rather than from a live value.</param>
        public void Transition(ICineCamera toCamera, float blendTime, Easing easing, bool forceSnapshotCreation = false)
        {
            ICineCamera fromCamera = currentVirtualCamera;

            if (activeBlend != null && activeBlend.IsActive || (forceSnapshotCreation && lastVirtualCamera != null))
            {
                fromCamera = new StaticCameraProperties(Properties, lastVirtualCamera, currentVirtualCamera, lastVirtualCamera.Name + "[Transition Proxy]");
            }

            Transition(fromCamera, toCamera, blendTime, easing);

        }


        /// <summary>
        /// Transitions from one camera to another
        /// </summary>
        /// <param name="fromCamera"></param>
        /// <param name="toCamera"></param>
        /// <param name="blendTime"></param>
        /// <param name="easing"></param>
        public void Transition(ICineCamera fromCamera, ICineCamera toCamera, float blendTime = 1, Easing easing = Easing.Linear)
        {

            // Validate parameters
            if (blendTime < 0)
                blendTime = 0;


            if (fromCamera == null)
            {
                fromCamera = this;
            }

            // Setup new blend
            activeBlend = new BlendState
            {
                FromCamera = fromCamera,
                ToCamera = toCamera,
                BlendTime = blendTime,
                CurrentTime = 0,
                TransitionEasing = easing
            };

            // Cut transition
            if (blendTime == 0)
            {
                activeBlend = null;
                toCamera.OnBlend(fromCamera, toCamera, 1);
            }

            // Preprocess toCamera to ensure effects appear like they were active the whole time. For now this is 10 seconds.
            toCamera.ProcessProperties(10);

            // Update current camera immediately
            currentVirtualCamera = toCamera;
            lastVirtualCamera = fromCamera;
        }

        public void SetSolo(ICineCamera camera)
        {
            soloCamera = camera;
        }

        public void ClearSolo()
        {
            soloCamera = null;
        }


    }
}