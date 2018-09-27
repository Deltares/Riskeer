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
using Deltares.WTIPiping;
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real uplift sub calculator of piping.
    /// </summary>
    public class UpliftCalculatorStub : IUpliftCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> is called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> is called or not.
        /// </summary>
        public bool Validated { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        public double EffectiveStress { get; set; }
        public double HExit { get; set; }
        public double HRiver { get; set; }
        public double ModelFactorUplift { get; set; }
        public double PhiExit { get; set; }
        public double PhiPolder { get; set; }
        public double RExit { get; set; }
        public double VolumetricWeightOfWater { get; set; }
        public double Zu { get; private set; }
        public double FoSu { get; private set; }

        public void Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new WTIUpliftCalculatorException($"Message 1{Environment.NewLine}Message 2");
            }

            Calculated = true;
        }

        public List<string> Validate()
        {
            Validated = true;
            return new List<string>();
        }
    }
}