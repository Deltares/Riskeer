using System.Collections;
using System.Collections.Generic;

namespace Wti.Data
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a piping calculation.
    /// </summary>
    public class PipingSoilProfile : IEnumerable<PipingSoilLayer>
    {
        public PipingSoilProfile(string name, IEnumerable<PipingSoilLayer> layers)
        {
            Name = name;
            Layers = layers;
        }

        public string Name { get; private set; }

        public IEnumerator<PipingSoilLayer> GetEnumerator()
        {
            return Layers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<PipingSoilLayer> Layers { get; private set; }
    }
}