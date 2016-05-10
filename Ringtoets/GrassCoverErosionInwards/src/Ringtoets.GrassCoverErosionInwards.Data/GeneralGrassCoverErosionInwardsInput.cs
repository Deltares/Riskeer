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
            CriticalOvertoppingModelFactor = 1.0;
            FbFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 4.75),
                StandardDeviation = new RoundedDouble(2, 0.5)
            };
            FnFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 2.6),
                StandardDeviation = new RoundedDouble(2, 0.35)
            };
            OvertoppingModelFactor = 1.0;
            FrunupModelFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 0.07)
            };
            FshallowModelFactor = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 0.92),
                StandardDeviation = new RoundedDouble(2, 0.24)
            };
        }

        #region Factors

        /// <summary>
        /// Gets the factor fb variable.
        /// </summary>
        public NormalDistribution FbFactor { get; private set; }

        /// <summary>
        /// Gets the factor fn variable.
        /// </summary>
        public NormalDistribution FnFactor { get; private set; }

        #endregion

        #region Model Factors

        /// <summary>
        /// Gets the model factor critical overtopping.
        /// </summary>
        public double CriticalOvertoppingModelFactor { get; private set; }

        /// <summary>
        /// Gets the model factor overtopping.
        /// </summary>
        public double OvertoppingModelFactor { get; private set; }

        /// <summary>
        /// Gets the Model factor frunup variable.
        /// </summary>
        public NormalDistribution FrunupModelFactor { get; private set; }

        /// <summary>
        /// Gets the Model factor fshallow variable.
        /// </summary>
        public NormalDistribution FshallowModelFactor { get; private set; }

        #endregion
    }
}