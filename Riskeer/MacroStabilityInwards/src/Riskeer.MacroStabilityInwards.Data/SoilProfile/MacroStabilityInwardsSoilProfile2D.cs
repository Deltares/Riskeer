// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.Properties;

namespace Riskeer.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// This class represents a soil profile, which was imported for use in a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilProfile2D : IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer2D>
    {
        private MacroStabilityInwardsSoilLayer2D[] layers;
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfile2D"/>, with the given <paramref name="name"/>
        /// and <paramref name="layers"/>.
        /// A new collection is created for <paramref name="layers"/> and used in the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <param name="name">The name of the profile.</param>
        /// <param name="layers">The collection of layers that should be part of the profile.</param>
        /// <param name="preconsolidationStresses">The preconsolidation stresses that are part of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="layers"/> contains no layers.</exception>
        public MacroStabilityInwardsSoilProfile2D(string name,
                                                  IEnumerable<MacroStabilityInwardsSoilLayer2D> layers,
                                                  IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
        {
            if (preconsolidationStresses == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStresses));
            }

            Name = name;
            ValidateLayersCollection(layers);
            Layers = layers;
            PreconsolidationStresses = preconsolidationStresses.ToArray();
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsPreconsolidationStress"/>
        /// for the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsPreconsolidationStress> PreconsolidationStresses { get; }

        public IEnumerable<MacroStabilityInwardsSoilLayer2D> Layers
        {
            get
            {
                return layers;
            }
            private set
            {
                layers = value.ToArray();
            }
        }

        /// <summary>
        /// Gets the name of <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when the value is <c>null</c>.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                name = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MacroStabilityInwardsSoilProfile2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                return PreconsolidationStresses.Aggregate(hashCode, (currentHashCode, stress) => (currentHashCode * 397) ^ stress.GetHashCode());
            }
        }

        private bool Equals(MacroStabilityInwardsSoilProfile2D other)
        {
            return layers.SequenceEqual(other.layers)
                   && PreconsolidationStresses.SequenceEqual(other.PreconsolidationStresses)
                   && string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Validates the given <paramref name="collection"/>. A valid <paramref name="collection"/> has layers.
        /// </summary>
        /// <param name="collection">The collection of <see cref="MacroStabilityInwardsSoilLayer2D"/> to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="collection"/> contains no layers.</exception>
        private void ValidateLayersCollection(IEnumerable<MacroStabilityInwardsSoilLayer2D> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), string.Format(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers));
            }

            if (!collection.Any())
            {
                throw new ArgumentException(Resources.Error_Cannot_Construct_MacroStabilityInwardsSoilProfile_Without_Layers);
            }
        }
    }
}