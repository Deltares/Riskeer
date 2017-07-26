// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilLayer2D
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="outerRing"></param>
        /// <param name="holes"></param>
        public MacroStabilityInwardsSoilLayer2D(Ring outerRing, Ring[] holes)
        {
            Properties = new SoilLayerProperties();
            OuterRing = outerRing;
            Holes = holes.ToArray();
        }

        /// <summary>
        /// Gets the properties of the <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        public SoilLayerProperties Properties { get; }

        /// <summary>
        /// Gets the outer ring of the polygon with holes describing the surface of the <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        public Ring OuterRing { get; }

        /// <summary>
        /// Gets the holes of the polygon with holes describing the surface of the <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        public Ring[] Holes { get; }

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
            return Equals((MacroStabilityInwardsSoilLayer2D) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode =  Properties.GetHashCode();
                hashCode = (hashCode * 397) ^ OuterRing.GetHashCode();
                foreach (Ring hole in Holes)
                {
                    hashCode = (hashCode * 397) ^ hole.GetHashCode();
                }
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayer2D other)
        {
            return Properties.Equals(other.Properties)
                && OuterRing.Equals(other.OuterRing)
                && Holes.SequenceEqual(other.Holes);
        }
    }
}