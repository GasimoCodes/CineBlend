using System;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace Gasimo.CineBlend.Modules
{
    /// <summary>
    /// Shakes this camera when an impulse is received.
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

        public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
        {
        }

        public void Initialize(ICineCamera camera)
        {
        }

        public void PostProcessProperties(ref CameraProperties state, float deltaTime)
        {
            if (!this.Enabled || currentTime >= targetTime)
                return;

            // Progress time (to-do: Smoothing?)
            currentTime += deltaTime;
            float normalizedTime = currentTime / targetTime;

            // Calculate shake intensity with a smooth fade-out
            float intensity = Mathf.Lerp(magnitude, 0, normalizedTime);

            // Generate random offsets with Perlin noise for smoothness
            Float2 perlin = new(Time.GameTime * frequency, 0);
            Float2 perlin2 = new(10f, Time.GameTime * frequency);
            Float2 perlin3 = new(0, Time.GameTime * frequency + 20f);

            float offsetX = Noise.PerlinNoise(perlin);
            float offsetY = Noise.PerlinNoise(perlin2);
            float offsetZ = Noise.PerlinNoise(perlin3);

            // Map from [0,1] to [-1,1] range
            Vector3 randomOffset = (new Vector3(offsetX, offsetY, offsetZ) * 2 - Vector3.One) * deviation;

            // Calculate rotational angles based on direction
            float pitchAngle = (float)-direction.Y * intensity; // (around X axis)
            float yawAngle = (float)-direction.X * intensity;   // (around Y axis)
            float rollAngle = (float)-direction.Z * intensity;  // (around Z axis)

            // Add random deviation to each axis
            pitchAngle += (float)randomOffset.X * intensity;
            yawAngle += (float)randomOffset.Y * intensity;
            rollAngle += (float)randomOffset.Z * intensity;

            // Create rotation from Euler angles
            currentShakeDelta = Quaternion.Euler(pitchAngle, yawAngle, rollAngle);

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
            // Default shake with no specific direction (will add random noise only)
            DoShake(magnitude, duration, 1f, Vector3.Zero);
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
        }
    }
}