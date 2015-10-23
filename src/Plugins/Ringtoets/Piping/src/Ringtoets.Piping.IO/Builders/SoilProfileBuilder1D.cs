using System.Collections.ObjectModel;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// Helps in the creation of a <see cref="PipingSoilProfile"/>.
    /// </summary>
    public class SoilProfileBuilder1D : ISoilProfileBuilder
    {
        internal SoilProfileBuilder1D(string name, double bottom)
        {
            Name = name;
            Bottom = bottom;
            Layers = new Collection<PipingSoilLayer>();
        }

        private Collection<PipingSoilLayer> Layers { get; set; }

        private string Name { get; set; }

        private double Bottom { get; set; }

        public PipingSoilProfile Build()
        {
            return new PipingSoilProfile(Name, Bottom, Layers);
        }

        /// <summary>
        /// Adds a new <see cref="PipingSoilLayer"/>, which will be added to the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="soilLayer"></param>
        internal void Add(PipingSoilLayer soilLayer)
        {
            Layers.Add(soilLayer);
        }
    }
}