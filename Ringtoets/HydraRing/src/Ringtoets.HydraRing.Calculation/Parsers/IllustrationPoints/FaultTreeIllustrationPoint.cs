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

namespace Ringtoets.HydraRing.Calculation.Parsers.IllustrationPoints
{
    /// <summary>
    /// An illustration points which uses the results of two sub illustration points
    /// to obtain a result.
    /// </summary>
    public class FaultTreeIllustrationPoint : IIllustrationPoint
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the combined stochasts of its children.
        /// </summary>
        public ICollection<Stochast> Stochasts { get; } = new List<Stochast>();

        /// <summary>
        /// Gets the combined beta values of its children.
        /// </summary>
        public double Beta { get; set; } = double.NaN;
        
        /// <summary>
        /// The way in which the sub illustartion points are combined to
        /// obtain a result for the fault tree illustration point.
        /// </summary>
        public CombinationType Combine { get; set; }
    }
}