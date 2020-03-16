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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="Deltares.WTIPiping.HeaveCalculator"/>.
    /// </summary>
    public class HeaveCalculator : IHeaveCalculator
    {
        private readonly Deltares.WTIPiping.HeaveCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="HeaveCalculator"/>.
        /// </summary>
        public HeaveCalculator()
        {
            wrappedCalculator = new Deltares.WTIPiping.HeaveCalculator();
        }

        public double Gradient
        {
            get
            {
                return wrappedCalculator.Gradient;
            }
        }

        public double Zh
        {
            get
            {
                return wrappedCalculator.Zh;
            }
        }

        public double FoSh
        {
            get
            {
                return wrappedCalculator.FoSh;
            }
        }

        public void SetDTotal(double dTotal)
        {
            wrappedCalculator.DTotal = dTotal;
        }

        public void SetHExit(double hExit)
        {
            wrappedCalculator.HExit = hExit;
        }

        public void SetIch(double ich)
        {
            wrappedCalculator.Ich = ich;
        }

        public void SetPhiExit(double phiExit)
        {
            wrappedCalculator.PhiExit = phiExit;
        }

        public void SetPhiPolder(double phiPolder)
        {
            wrappedCalculator.PhiPolder = phiPolder;
        }

        public void SetRExit(double rExit)
        {
            wrappedCalculator.RExit = rExit;
        }

        public void Calculate()
        {
            wrappedCalculator.Calculate();
        }

        public List<string> Validate()
        {
            return wrappedCalculator.Validate();
        }
    }
}