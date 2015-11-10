using System;
using System.Collections.ObjectModel;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Helps in the creation of a <see cref="PipingSoilProfile"/>.
    /// </summary>
    public class SoilProfileBuilder1D
    {
        private readonly Collection<PipingSoilLayer> layers;
        private readonly string name;
        private readonly double bottom;

        internal SoilProfileBuilder1D(string name, double bottom)
        {
            this.name = name;
            this.bottom = bottom;
            layers = new Collection<PipingSoilLayer>();
        }

        /// <summary>
        /// Creates a new instances of the <see cref="PipingSoilProfile"/> based on the layer definitions.
        /// </summary>
        /// <returns>A new <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when no layers have been added through <see cref="Add"/>.</exception>
        public PipingSoilProfile Build()
        {
            return new PipingSoilProfile(name, bottom, layers);
        }

        /// <summary>
        /// Adds a new <see cref="PipingSoilLayer"/>, which will be added to the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="soilLayer">The <see cref="PipingSoilLayer"/> to add.</param>
        internal void Add(PipingSoilLayer soilLayer)
        {
            layers.Add(soilLayer);
        }
    }
}