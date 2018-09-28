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

using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="WTIUpliftCalculator"/>.
    /// </summary>
    public class UpliftCalculator : IUpliftCalculator
    {
        private readonly WTIUpliftCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftCalculator"/>.
        /// </summary>
        public UpliftCalculator()
        {
            wrappedCalculator = new WTIUpliftCalculator();
        }

        public double EffectiveStress
        {
            get
            {
                return wrappedCalculator.EffectiveStress;
            }
            set
            {
                wrappedCalculator.EffectiveStress = value;
            }
        }

        public double HExit
        {
            set
            {
                wrappedCalculator.HExit = value;
            }
        }

        public double HRiver
        {
            set
            {
                wrappedCalculator.HRiver = value;
            }
        }

        public double ModelFactorUplift
        {
            set
            {
                wrappedCalculator.ModelFactorUplift = value;
            }
        }

        public double PhiExit
        {
            set
            {
                wrappedCalculator.PhiExit = value;
            }
        }

        public double PhiPolder
        {
            set
            {
                wrappedCalculator.PhiPolder = value;
            }
        }

        public double RExit
        {
            set
            {
                wrappedCalculator.RExit = value;
            }
        }

        public double VolumetricWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumetricWeightOfWater = value;
            }
        }

        public double Zu
        {
            get
            {
                return wrappedCalculator.Zu;
            }
        }

        public double FoSu
        {
            get
            {
                return wrappedCalculator.FoSu;
            }
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