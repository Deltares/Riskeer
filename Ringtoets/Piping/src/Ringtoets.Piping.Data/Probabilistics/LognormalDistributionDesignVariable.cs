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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;

namespace Ringtoets.Piping.Data.Probabilistics
{
    /// <summary>
    /// This class defines a design variable for a lognormal distribution.
    /// </summary>
    public class LognormalDistributionDesignVariable : DesignVariable<LognormalDistribution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LognormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A lognormal distribution.</param>
        public LognormalDistributionDesignVariable(LognormalDistribution distribution) : base(distribution) {}

        public override double GetDesignValue()
        {
            var normalSpaceDesignValue = DetermineDesignValueInNormalDistributionSpace();
            return ProjectFromNormalToLognormalSpace(normalSpaceDesignValue);
        }

        /// <summary>
        /// Projects <see cref="DesignVariable{DistributionType}.Distribution"/> into 'normal
        /// distribution' space and calculates the design value for that value space.
        /// </summary>
        /// <returns>The design value in 'normal distribution' space.</returns>
        private double DetermineDesignValueInNormalDistributionSpace()
        {
            // Determine normal distribution parameters from log-normal parameters, as
            // design value can only be determined in 'normal distribution' space.
            // Below formula's come from Tu-Delft College dictaat "b3 Probabilistisch Ontwerpen"
            // by ir. A.C.W.M. Vrouwenvelder and ir.J.K. Vrijling 5th reprint 1987.
            double sigmaLogOverMuLog = Distribution.StandardDeviation / Distribution.Mean;
            double sigmaNormal = Math.Sqrt(Math.Log(sigmaLogOverMuLog * sigmaLogOverMuLog + 1.0));
            double muNormal = Math.Log(Distribution.Mean) - 0.5 * sigmaNormal * sigmaNormal;
            return DetermineDesignValue(muNormal, sigmaNormal);
        }

        private static double ProjectFromNormalToLognormalSpace(double normalSpaceDesignValue)
        {
            return Math.Exp(normalSpaceDesignValue);
        }
    }
}