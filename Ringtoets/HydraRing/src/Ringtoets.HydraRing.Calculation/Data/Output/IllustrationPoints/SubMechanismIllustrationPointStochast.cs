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

namespace Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints
{
    /// <summary>
    /// Container of alpha value definitions read from a Hydra-Ring output database with
    /// a realization of the described stochast.
    /// </summary>
    public class SubMechanismIllustrationPointStochast : Stochast
    {
        /// <summary>
        /// Creates an new instance of <see cref="SubMechanismIllustrationPointStochast"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="realization">The realization.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public SubMechanismIllustrationPointStochast(string name, double duration, double alpha, double realization)
            : base(name, duration, alpha)
        {
            Realization = realization;
        }

        /// <summary>
        /// Gets the realization.
        /// </summary>
        public double Realization { get; }
    }
}