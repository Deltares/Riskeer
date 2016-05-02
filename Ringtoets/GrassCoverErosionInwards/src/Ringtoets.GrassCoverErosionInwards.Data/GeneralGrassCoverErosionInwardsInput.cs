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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Class that holds all the static grass cover erosion inwards calculation input parameters.
    /// </summary>
    public class GeneralGrassCoverErosionInwardsInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralGrassCoverErosionInwardsInput"/> class.
        /// </summary>
        public GeneralGrassCoverErosionInwardsInput()
        {
            Fb = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 4.75), StandardDeviation = new RoundedDouble(2, 0.5)
            };
            Fn = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 2.6), StandardDeviation = new RoundedDouble(2, 0.35)
            };
            Fshallow = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 0.92), StandardDeviation = new RoundedDouble(2, 0.24)
            };
            Mz2 = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1), StandardDeviation = new RoundedDouble(2, 0.07)
            };
            Mqc = 1.0;
            Mqo = 1.0;
        }

        /// <summary>
        /// Gets the factor fb variable.
        /// </summary>
        public NormalDistribution Fb { get; private set; }

        /// <summary>
        /// Gets the factor fn variable.
        /// </summary>
        public NormalDistribution Fn { get; private set; }

        /// <summary>
        /// Gets the factor fshallow variable.
        /// </summary>
        public NormalDistribution Fshallow { get; private set; }

        /// <summary>
        /// Gets the factor mz2 (or frunup) variable.
        /// </summary>
        public NormalDistribution Mz2 { get; private set; }

        /// <summary>
        /// Gets the model factor critical overtopping.
        /// </summary>
        public double Mqc { get; private set; }

        /// <summary>
        /// Gets the model factor overtopping.
        /// </summary>
        public double Mqo { get; private set; }
    }
}