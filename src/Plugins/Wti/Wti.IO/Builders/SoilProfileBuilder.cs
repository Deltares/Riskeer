using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wti.Data;
using Wti.IO.Properties;

namespace Wti.IO.Builders
{
    /// <summary>
    /// Constructs a 1d Soil Profile based on definitions of <see cref="2DSoilLayer"/>.
    /// </summary>
    public class SoilProfileBuilder
    {
        private readonly ICollection<PipingSoilLayer> layers = new Collection<PipingSoilLayer>();

        public SoilProfileBuilder(string profileName, double atX)
        {
            if (double.IsNaN(atX))
            {
                throw new ArgumentException(Resources.Error_SoilProfileBuilderCantDetermineIntersectAtDoubleNaN);
            }
            ProfileName = profileName;
            AtX = atX;
            Bottom = double.MaxValue;
        }

        public void Add(SoilLayer2D soilLayer)
        {
            double bottom;
            foreach (PipingSoilLayer layer in soilLayer.AsPipingSoilLayers(AtX, out bottom))
            {
                layers.Add(layer);
            }
            Bottom = Math.Min(Bottom, bottom);
        }

        public PipingSoilProfile Build()
        {
            return new PipingSoilProfile(ProfileName, Bottom, layers);
        }

        private double Bottom { get; set; }

        private double AtX { get; set; }

        private string ProfileName { get; set; }
    }
}