using System;
using System.Collections.Generic;
using System.Linq;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a piping calculation.
    /// </summary>
    public class PipingSoilProfile
    {
        private IEnumerable<PipingSoilLayer> layers;

        /// <summary>
        /// Creates a new instance ofL <see cref="PipingSoilProfile"/>, with the given <paramref name="name"/>, <paramref name="bottom"/> and <paramref name="layers"/>.
        /// A new collection is created for <paramref name="layers"/> and used in the <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="bottom">The bottom level of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        public PipingSoilProfile(string name, double bottom, IEnumerable<PipingSoilLayer> layers)
        {
            Name = name;
            Bottom = bottom;
            Layers = layers;
        }

        /// <summary>
        /// Gets the bottom level of the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public double Bottom { get; private set; }

        /// <summary>
        /// Gets the name of <see cref="PipingSoilProfile"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets an ordered (by <see cref="PipingSoilLayer.Top"/>, descending) <see cref="IEnumerable{T}"/> of 
        /// <see cref="PipingSoilLayer"/> for the <see cref="PipingSoilProfile"/>.
        /// </summary>
        public IEnumerable<PipingSoilLayer> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                if (value == null || !value.Any())
                {
                    throw new ArgumentException(string.Format(Resources.Error_Cannot_Construct_PipingSoilProfile_Without_Layers));
                }
                layers = value.OrderByDescending(l => l.Top).ToArray();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}