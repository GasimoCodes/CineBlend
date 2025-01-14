using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor.States;
using FlaxEngine;

namespace Game
{
    [RequireActor(typeof(Camera))]
    [ExecuteInEditMode]
    public class CineblendMaster : Script, ICineCamera
    {
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


        private Camera camera;
        public float DefaultBlendTime { get; set; } = 1.0f;


        // Transition vars
        private ICineCamera lastVirtualCamera;
        public ICineCamera currentVirtualCamera { get; private set; }
        private ICineCamera soloCamera;
        private SortedDictionary<int, List<ICineCamera>> virtualCamerasByPriority = new(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        private class BlendState
        {
            public ICineCamera FromCamera;
            public ICineCamera ToCamera;
            public float BlendTime;
            public float CurrentTime;
            public bool IsActive => CurrentTime < BlendTime;

            public float NormalizedTime => Mathf.Saturate(CurrentTime / BlendTime);
        }

        private BlendState activeBlend = null;



        // Current camera setup        
        public CameraProperties Properties { get; private set; }
        public Dictionary<Type, ICameraModule> Modules { get; private set; } = new();
        public int Priority => int.MinValue;
        public string Name => this.Actor.Name;

        public CameraProperties FinalProperties => Properties;

        public override void OnStart()
        {
            camera = Actor as Camera;
            PullDefaultProperties();
        }


        public override void OnUpdate()
        {
            base.OnUpdate();

            UpdateActiveCamera();
            UpdateCameraBlending();
        }

        private void PullDefaultProperties()
        {
            Properties = new CameraProperties();
            Properties.FieldOfView.CurrentValue = camera.FieldOfView;
            Properties.NearPlane.CurrentValue = camera.NearPlane;
            Properties.FarPlane.CurrentValue = camera.FarPlane;

            Properties.Position.CurrentValue = camera.Transform.Translation;
            Properties.Rotation.CurrentValue = camera.Transform.Orientation;

            currentVirtualCamera = this;
        }


        private void UpdateActiveCamera()
        {
            ICineCamera highestPriorityCamera = GetHighestPriorityCamera();

            if (currentVirtualCamera != highestPriorityCamera)
            {
                lastVirtualCamera = currentVirtualCamera;
                currentVirtualCamera = highestPriorityCamera;

                Transition(lastVirtualCamera, currentVirtualCamera, DefaultBlendTime);
            }
        }


        private void UpdateCameraBlending()
        {
            if (activeBlend == null)
            {
                // Copy values from camera
                currentVirtualCamera.FinalProperties.Apply(camera);
                return;
            }

            // Lerp between the two cameras

            CameraProperties toProperties = currentVirtualCamera.FinalProperties;
            CameraProperties fromProperties = lastVirtualCamera?.FinalProperties ?? Properties;

            var blend = activeBlend;
            blend.CurrentTime += Time.UnscaledDeltaTime;
            float t = blend.NormalizedTime;

            // Perform the blend
            Properties.Lerp(fromProperties, toProperties, t);
            Properties.Apply(camera);

            // Check if blend is complete
            if (!blend.IsActive)
            {
                activeBlend = null;
                currentVirtualCamera = blend.ToCamera;
                lastVirtualCamera = blend.FromCamera;
            }
        }


        public void RegisterVirtualCamera(VirtualCamera virtualCamera)
        {
            if (!virtualCamerasByPriority.ContainsKey(virtualCamera.Priority))
            {
                virtualCamerasByPriority[virtualCamera.Priority] = new List<ICineCamera>();
            }
            virtualCamerasByPriority[virtualCamera.Priority].Add(virtualCamera);
            
            // Debug.Log("Registered camera " + virtualCamera.Name + " " + virtualCamera.Priority);
        }


        public void UnregisterVirtualCamera(VirtualCamera virtualCamera)
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

            return null;
        }


        /// <summary>
        /// Transitions to the selected camera
        /// </summary>
        /// <param name="toCamera"></param>
        public void Transition(ICineCamera toCamera, float blendTime = 1)
        {
            Transition(currentVirtualCamera, toCamera, blendTime);
        }

        /// <summary>
        /// Transitions from one camera to another
        /// </summary>
        /// <param name="fromCamera"></param>
        /// <param name="toCamera"></param>
        /// <param name="blendTime"></param>
        public void Transition(ICineCamera fromCamera, ICineCamera toCamera, float blendTime = 1)
        {

            // Validate parameters
            if (blendTime < 0)
                blendTime = 0;

            // Setup new blend
            activeBlend = new BlendState
            {
                FromCamera = fromCamera,
                ToCamera = toCamera,
                BlendTime = blendTime,
                CurrentTime = 0
            };

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