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
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.SoilProfile.Schema;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents a stochastic soil model which consists out of a collection of <see cref="StochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class StochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">The name of the stochastic soil model.</param>
        /// <param name="failureMechanismType">The failure mechanism this stochastic soil model belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public StochasticSoilModel(string name, FailureMechanismType failureMechanismType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            FailureMechanismType = failureMechanismType;
            Geometry = new List<Point2D>();
            StochasticSoilProfiles = new List<StochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the name of the segment soil model.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the failure mechanism the soil model applies to.
        /// </summary>
        public FailureMechanismType FailureMechanismType { get; }

        /// <summary>
        /// Gets the list of geometry points.
        /// </summary>
        public List<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the list of <see cref="StochasticSoilProfile"/>.
        /// </summary>
        public List<StochasticSoilProfile> StochasticSoilProfiles { get; }
    }
}