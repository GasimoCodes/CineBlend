using System;
using FlaxEngine;
using FlaxEngine.Utilities;

namespace Gasimo.CineBlend.Modules
{
    /// <summary>
    /// Adds realistic multi-frequency camera noise effects to the game.
    /// </summary>
    [RequireActor(typeof(VirtualCamera))]
    [Category("Cineblend")]
    public class CineShakeModule : Script, ICameraModule
    {
        // Customizable noise properties
        public Float3 PositionAmplitude = new Float3(0.1f, 0.1f, 0.1f); // Amplitude of position noise
        public Float3 RotationAmplitude = new Float3(1.0f, 1.0f, 1.0f); // Amplitude of rotation noise (in degrees)

        // Frequency layers for noise
        public Float3 PositionFrequency = new Float3(1.0f, 1.0f, 1.0f); // Frequency for position noise
        public Float3 RotationFrequency = new Float3(1.0f, 1.0f, 1.0f); // Frequency for rotation noise

        public void Blend(ICineCamera fromSnapshot, ICineCamera toSnapshot, float t)
        {
            // Not implemented for this example

        }

        public void Initialize(ICineCamera camera)
        {
            // Not implemented for this example

        }

        public void PostProcessProperties(ref CameraProperties state, float deltaTime)
        {
            if (!this.Enabled)
                return;

            float time = Time.UnscaledGameTime;

            // Generate Perlin noise for position
            Vector3 positionNoise = CalculatePerlinNoise(PositionFrequency, PositionAmplitude, time);

            // Generate Perlin noise for rotation
            Vector3 rotationNoise = CalculatePerlinNoise(RotationFrequency, RotationAmplitude, time);

            // Debug.Log("PosNoise " + positionNoise);

            // Apply noise to the position and rotation of the camera
            state.Position.CurrentValue += positionNoise;
            Quaternion rotationOffset = Quaternion.Euler(rotationNoise);
            state.Rotation.CurrentValue = state.Rotation.CurrentValue * rotationOffset;
        }

        private Vector3 CalculatePerlinNoise(Float3 frequencies, Float3 amplitude, float time)
        {
            Float2 TileFactor = new Float2(200.0f, 200.0f);

            return new Vector3(
                amplitude.X * (Noise.PerlinNoise(new Float2(time * frequencies.X, 0), TileFactor) -0.5f),
                amplitude.Y * (Noise.PerlinNoise(new Float2(100, time * frequencies.Y), TileFactor) - 0.5f),
                amplitude.Z * (Noise.PerlinNoise(new Float2(time * frequencies.Z, 100), TileFactor) - 0.5f)
            );
        }
    }
}
