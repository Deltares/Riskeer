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

using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data.SoilProfile;

namespace Ringtoets.Piping.Primitives
{
    /// <summary>
    /// This class represents a piping specific stochastic soil model which consists out of a 
    /// collection of <see cref="PipingStochasticSoilProfile"/>. 
    /// A stochastic soil model contains a segment for which the model applies.
    /// </summary>
    public class PipingStochasticSoilModel : Observable, IMechanismStochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModel"/>.
        /// </summary>
        /// <param name="name">Name of the segment soil model.</param>
        public PipingStochasticSoilModel(string name)
        {
            Name = name;
            Geometry = new List<Point2D>();
            StochasticSoilProfiles = new List<PipingStochasticSoilProfile>();
        }

        /// <summary>
        /// Gets the name of the soil model.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the list of geometry points.
        /// </summary>
        public List<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the list of <see cref="PipingStochasticSoilProfile"/>.
        /// </summary>
        public List<PipingStochasticSoilProfile> StochasticSoilProfiles { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}