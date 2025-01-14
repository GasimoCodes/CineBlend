using System;
using FlaxEngine;

namespace Cineblend
{
    /// <summary>
    /// The sample game plugin.
    /// </summary>
    /// <seealso cref="FlaxEngine.GamePlugin" />
    public class Cineblend : GamePlugin
    {
        /// <inheritdoc />
        public Cineblend()
        {
            _description = new PluginDescription
            {
                Name = "Cineblend",
                Category = "Camera",
                Author = "Gasimo",
                AuthorUrl = "https://gasimo.dev/",
                HomepageUrl = null,
                RepositoryUrl = "https://github.com/GasimoCodes/CineBlend",
                Description = "Flax plugin which introduces virtual cameras and compositor effects to the engine.",
                Version = new Version(0,1),
                IsAlpha = true,
                IsBeta = false,
            };
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <inheritdoc />
        public override void Deinitialize()
        {
            // Use it to cleanup data
            base.Deinitialize();
        }
    }
}
