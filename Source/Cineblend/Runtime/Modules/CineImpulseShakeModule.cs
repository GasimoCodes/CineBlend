using System;
using FlaxEngine;
using FlaxEngine.Utilities;
using Game;

namespace Cineblend
{
    /// <summary>
    /// CineImpulseShakeModule Script.
    /// </summary>
    [RequireActor(typeof(VirtualCamera))]
    [Category("Cineblend")]
    public class CineImpulseShakeModule : Script, ICameraModule
    {
        private Quaternion currentShakeDelta = Quaternion.Identity;
        private float targetTime;
        private float currentTime;
        private float magnitude;
        private float frequency;

        private Vector3 direction;
        private float deviation;

        public void Blend(VirtualCamera fromSnapshot, VirtualCamera toSnapshot, float t)
        {
        }

        public void Initialize(VirtualCamera camera)
        {
        }

        public void PostProcessProperties(ref CameraProperties state)
        {
            if (!this.Enabled || currentTime >= targetTime)
                return;

            // Progress time
            currentTime += Time.DeltaTime;
            float normalizedTime = currentTime / targetTime;

            // Calculate shake intensity with a smooth fade-out
            float intensity = Mathf.Lerp(magnitude, 0, normalizedTime);

            // Generate random offsets with Perlin noise for smoothness
            Float2 perlin = new(Time.GameTime * frequency, 0);
            Float2 perlin2 = new(10f, Time.GameTime * frequency);
            Float2 perlin3 = new(perlin.X);
            float offsetX = Noise.PerlinNoise(perlin);
            float offsetY = Noise.PerlinNoise(perlin2);
            float offsetZ = Noise.PerlinNoise(perlin3);

            Vector3 randomOffset = (new Vector3(offsetX, offsetY, offsetZ) - 0.5f * 2) * deviation;

            // Compute final shake direction with deviation
            Vector3 shakeOffset = (direction + randomOffset) * intensity;

            // Apply shake to rotation as a quaternion
            currentShakeDelta = Quaternion.Euler(shakeOffset);

            // Modify camera's rotation
            state.Rotation.CurrentValue *= currentShakeDelta;
        }

        /// <summary>
        /// Does a camera shake for this camera.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake</param>
        /// <param name="duration">For how long the shake should play</param>
        public void DoShake(float magnitude = 1f, float duration = 3f)
        {
            DoShake(magnitude, duration, 1f, new Vector3(0, 0, 0));
        }

        /// <summary>
        /// Does a camera shake for this camera.
        /// </summary>
        /// <param name="magnitude">The intensity of the shake</param>
        /// <param name="duration">For how long the shake should play</param>
        /// <param name="deviation">Deviation from the direction vector. If the vector is up, this will ensure the camera can also deviate in other random directions.</param>
        /// <param name="direction">Direction in which the camera should shake. For instance the direction away from explosion and towards the camera.</param>
        public void DoShake(float magnitude, float duration, float deviation, Vector3 direction, float frequency = 2f)
        {
            currentTime = 0;
            targetTime = duration;

            this.direction = direction;
            this.magnitude = magnitude;
            this.deviation = deviation;
            this.frequency = frequency;

            Debug.Log("Shake");
        }
    }
}
