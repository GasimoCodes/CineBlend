#if USE_LARGE_WORLDS
global using Real = System.Double;
#else
global using Real = System.Single;
#endif


using System;
using FlaxEngine;

namespace Gasimo.CineBlend
{
    /// <summary>
    /// CineFlax Plugin
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
                Version = new Version(0,1,1),
                IsAlpha = false,
                IsBeta = true,
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
