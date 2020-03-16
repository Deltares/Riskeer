// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Piping.KernelWrapper.SubCalculator;

namespace Riskeer.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real heave sub calculator of piping.
    /// </summary>
    public class HeaveCalculatorStub : IHeaveCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> was called or not.
        /// </summary>
        public bool Validated { get; private set; }

        public double DTotal { get; private set; }
        public double HExit { get; private set; }
        public double Ich { get; private set; }
        public double PhiExit { get; private set; }
        public double PhiPolder { get; private set; }
        public double RExit { get; private set; }

        public double Gradient { get; private set; }
        public double Zh { get; private set; }
        public double FoSh { get; private set; }

        public void SetDTotal(double dTotal)
        {
            DTotal = dTotal;
        }

        public void SetHExit(double hExit)
        {
            HExit = hExit;
        }

        public void SetIch(double ich)
        {
            Ich = ich;
        }

        public void SetPhiExit(double phiExit)
        {
            PhiExit = phiExit;
        }

        public void SetPhiPolder(double phiPolder)
        {
            PhiPolder = phiPolder;
        }

        public void SetRExit(double rExit)
        {
            RExit = rExit;
        }

        public void Calculate()
        {
            Calculated = true;
        }

        public List<string> Validate()
        {
            Validated = true;
            return new List<string>();
        }
    }
}