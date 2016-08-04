﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;

namespace Ringtoets.Common.Data.Probabilistics
{
    /// <summary>
    /// This class defines a design variable for a log-normal distribution.
    /// </summary>
    public class LogNormalDistributionDesignVariable : DesignVariable<LogNormalDistribution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogNormalDistributionDesignVariable"/> class.
        /// </summary>
        /// <param name="distribution">A log-normal distribution.</param>
        public LogNormalDistributionDesignVariable(LogNormalDistribution distribution) : base(distribution) {}

        public override RoundedDouble GetDesignValue()
        {
            double normalSpaceDesignValue = DetermineDesignValueInNormalDistributionSpace();
            return ProjectFromNormalToLogNormalSpace(normalSpaceDesignValue) + Distribution.Shift;
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
            double sigmaLogOverMuLog = Distribution.StandardDeviation/(Distribution.Mean - Distribution.Shift);
            double sigmaNormal = Math.Sqrt(Math.Log(sigmaLogOverMuLog*sigmaLogOverMuLog + 1.0));
            double muNormal = Math.Log(Distribution.Mean - Distribution.Shift) - 0.5*sigmaNormal*sigmaNormal;
            return DetermineDesignValue(muNormal, sigmaNormal);
        }

        private RoundedDouble ProjectFromNormalToLogNormalSpace(double normalSpaceDesignValue)
        {
            return new RoundedDouble(Distribution.Mean.NumberOfDecimalPlaces, Math.Exp(normalSpaceDesignValue));
        }
    }
}