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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// This class represents profiles that were imported from D-Soil Model and will later on be used to create the
    /// necessary input for executing a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsSoilLayer
    {
        private string materialName;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayer"/>, where the top is set to <paramref name="top"/>.
        /// </summary>
        /// <param name="top"></param>
        public MacroStabilityInwardsSoilLayer(double top)
        {
            Top = top;
            MaterialName = string.Empty;
        }

        /// <summary>
        /// Gets the top level of the <see cref="MacroStabilityInwardsSoilLayer"/>.
        /// </summary>
        public double Top { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the <see cref="MacroStabilityInwardsSoilLayer"/> is an aquifer.
        /// </summary>
        public bool IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the name of the material that was assigned to the <see cref="MacroStabilityInwardsSoilLayer"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string MaterialName
        {
            get
            {
                return materialName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                materialName = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Color"/> that was used to represent the <see cref="MacroStabilityInwardsSoilLayer"/>.
        /// </summary>
        public Color Color { get; set; }

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
            return Equals((MacroStabilityInwardsSoilLayer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = materialName?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ IsAquifer.GetHashCode();
                hashCode = (hashCode * 397) ^ Color.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsSoilLayer other)
        {
            return string.Equals(materialName, other.materialName)
                   && Top.Equals(other.Top)
                   && IsAquifer == other.IsAquifer
                   && Color.Equals(other.Color);
        }
    }
}