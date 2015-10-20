using System.Collections.Generic;

namespace Wti.Data
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a piping calculation.
    /// </summary>
    public class PipingSoilProfile
    {
        public PipingSoilProfile(long id, string name)
        {
            Id = id;
            Name = name;
            Layers = new List<PipingSoilLayer>();
        }

        public string Name { get; private set; }

        private ICollection<PipingSoilLayer> Layers { get; set; }

        public long Id { get; private set; }

        public void Add(PipingSoilLayer pipingSoilLayer)
        {
            Layers.Add(pipingSoilLayer);
        }
    }
}