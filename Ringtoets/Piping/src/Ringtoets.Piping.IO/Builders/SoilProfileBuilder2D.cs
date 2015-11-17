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
    internal class SoilProfileBuilder2D
    {
        private readonly ICollection<PipingSoilLayer> layers = new Collection<PipingSoilLayer>();
        private readonly double atX;
        private readonly string profileName;

        private double bottom;

        /// <summary>
        /// Creates a new instance of <see cref="SoilProfileBuilder2D"/> with the supposed name for the new <see cref="PipingSoilProfile"/>
        /// and the point at which a 1D profile should be obtained from the 2D profile.
        /// </summary>
        /// <param name="profileName">The name for the <see cref="PipingSoilProfile"/> constructed by the <see cref="SoilProfileBuilder2D"/>.</param>
        /// <param name="atX">The x position from which to obtain a 1D profile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="atX"/> can not be used to determine intersections with
        /// (is <see cref="double.NaN"/>).</exception>
        internal SoilProfileBuilder2D(string profileName, double atX)
        {
            if (double.IsNaN(atX))
            {
                var message = string.Format(Resources.Error_SoilProfileBuilder_cant_determine_intersect_at_double_NaN , profileName);
                throw new ArgumentException(message);
            }
            this.profileName = profileName;
            this.atX = atX;
            bottom = double.MaxValue;
        }

        /// <summary>
        /// Creates a new instances of the <see cref="PipingSoilProfile"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when trying to build a 
        /// <see cref="PipingSoilProfile"/> and not having added any layers using <see cref="Add"/>.
        /// </exception>
        internal PipingSoilProfile Build()
        {
            try
            {
                return new PipingSoilProfile(profileName, bottom, layers);
            }
            catch (ArgumentException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }
        }

        /// <summary>
        /// Adds a new <see cref="SoilLayer2D"/> to the profile.
        /// </summary>
        /// <param name="soilLayer">The <see cref="SoilLayer2D"/> to add to the profile.</param>
        /// <returns>The <see cref="SoilProfileBuilder2D"/>.</returns>
        /// <exception cref="SoilProfileBuilderException">Thrown when the <paramref name="soilLayer"/>'s geometry
        /// contains vertical segments the X-coordinate given for the construction of the 
        /// <see cref="SoilProfileBuilder2D(string,double)"/>.</exception>
        internal SoilProfileBuilder2D Add(SoilLayer2D soilLayer)
        {
            double newBottom;

            try
            {
                var pipingSoilLayers = soilLayer.AsPipingSoilLayers(atX, out newBottom);
                foreach (PipingSoilLayer layer in pipingSoilLayers)
                {
                    layers.Add(layer);
                }
            }
            catch (SoilLayer2DConversionException e)
            {
                throw new SoilProfileBuilderException(e.Message, e);
            }

            bottom = Math.Min(bottom, newBottom);
            return this;
        }
    }
}