using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Constructs a 1d Soil Profile based on definitions of <see cref="SoilLayer2D"/>.
    /// </summary>
    internal class SoilProfileBuilder2D : ISoilProfileBuilder
    {
        private readonly ICollection<PipingSoilLayer> layers = new Collection<PipingSoilLayer>();

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfileBuilder2D"/> with the supposed name for the new <see cref="PipingSoilProfile"/>
        /// and the point at which a 1D profile should be obtained from the 2D profile.
        /// </summary>
        /// <param name="profileName">The name for the <see cref="PipingSoilProfile"/> constructed by the <see cref="SoilProfileBuilder2D"/>.</param>
        /// <param name="atX">The x position from which to obtain a 1D profile.</param>
        internal SoilProfileBuilder2D(string profileName, double atX)
        {
            if (double.IsNaN(atX))
            {
                throw new ArgumentException(Resources.Error_SoilProfileBuilderCantDetermineIntersectAtDoubleNaN);
            }
            ProfileName = profileName;
            AtX = atX;
            Bottom = double.MaxValue;
        }

        /// <summary>
        /// Adds a new <see cref="SoilLayer2D"/> to the profile.
        /// </summary>
        /// <param name="soilLayer">The <see cref="SoilLayer2D"/> to add to the profile.</param>
        /// <returns>The <see cref="SoilProfileBuilder2D"/>.</returns>
        internal SoilProfileBuilder2D Add(SoilLayer2D soilLayer)
        {
            double bottom;
            foreach (PipingSoilLayer layer in soilLayer.AsPipingSoilLayers(AtX, out bottom))
            {
                layers.Add(layer);
            }
            Bottom = Math.Min(Bottom, bottom);
            return this;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        public PipingSoilProfile Build()
        {
            return new PipingSoilProfile(ProfileName, Bottom, layers);
        }

        private double Bottom { get; set; }

        private double AtX { get; set; }

        private string ProfileName { get; set; }
    }
}