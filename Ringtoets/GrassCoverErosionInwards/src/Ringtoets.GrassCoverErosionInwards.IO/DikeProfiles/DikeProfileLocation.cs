// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;

namespace Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles
{
    /// <summary>
    /// Representation of a dike profile location as read from a shapefile.
    /// </summary>
    public class DikeProfileLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="DikeProfileLocation"/>.
        /// </summary>
        /// <param name="idValue">The identifier for this <see cref="DikeProfileLocation"/></param>
        /// <param name="nameValue">The name of this <see cref="DikeProfileLocation"/></param>
        /// <param name="x0Value">The coordinate offset for this <see cref="DikeProfileLocation"/></param>
        /// <param name="pointValue">The actual location.</param>
        public DikeProfileLocation(string idValue, string nameValue, double x0Value, Point2D pointValue)
        {
            if (idValue == null)
            {
                throw new ArgumentException(Resources.DikeProfileLocation_Constructor_Invalid_Id);
            }
            if (nameValue == null)
            {
                throw new ArgumentException(Resources.DikeProfileLocation_Constructor_Invalid_Name);
            }
            if (double.IsNaN(x0Value))
            {
                throw new ArgumentException(Resources.DikeProfileLocation_Constructor_Invalid_X0);
            }
            if (pointValue == null)
            {
                throw new ArgumentException(Resources.DikeProfileLocation_Constructor_Invalid_Point);
            }

            Id = idValue;
            Name = nameValue;
            X0 = x0Value;
            Point = pointValue;
        }

        /// <summary>
        /// Gets the identifier for this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the coordinate offset for this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public double X0 { get; private set; }

        /// <summary>
        /// Gets the actual location of this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public Point2D Point { get; private set; }
    }
}