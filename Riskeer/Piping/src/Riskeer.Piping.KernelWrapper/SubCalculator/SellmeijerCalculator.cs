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

namespace Riskeer.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="Sellmeijer2011Calculator"/>.
    /// </summary>
    public class SellmeijerCalculator : ISellmeijerCalculator
    {
        private readonly Sellmeijer2011Calculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="SellmeijerCalculator"/>.
        /// </summary>
        public SellmeijerCalculator()
        {
            wrappedCalculator = new Sellmeijer2011Calculator();
        }

        public double BeddingAngle
        {
            set
            {
                wrappedCalculator.BeddingAngle = value;
            }
        }

        public double D70
        {
            set
            {
                wrappedCalculator.D70 = value;
            }
        }

        public double D70Mean
        {
            set
            {
                wrappedCalculator.D70Mean = value;
            }
        }

        public double DAquifer
        {
            set
            {
                wrappedCalculator.DAquifer = value;
            }
        }

        public double DarcyPermeability
        {
            set
            {
                wrappedCalculator.DarcyPermeability = value;
            }
        }

        public double DTotal
        {
            set
            {
                wrappedCalculator.DTotal = value;
            }
        }

        public double GammaSubParticles
        {
            set
            {
                wrappedCalculator.GammaSubParticles = value;
            }
        }

        public double Gravity
        {
            set
            {
                wrappedCalculator.Gravity = value;
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

        public double KinematicViscosityWater
        {
            set
            {
                wrappedCalculator.KinematicViscosityWater = value;
            }
        }

        public double ModelFactorPiping
        {
            set
            {
                wrappedCalculator.ModelFactorPiping = value;
            }
        }

        public double Rc
        {
            set
            {
                wrappedCalculator.Rc = value;
            }
        }

        public double SeepageLength
        {
            set
            {
                wrappedCalculator.SeepageLength = value;
            }
        }

        public double VolumetricWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumetricWeightOfWater = value;
            }
        }

        public double WhitesDragCoefficient
        {
            set
            {
                wrappedCalculator.WhitesDragCoefficient = value;
            }
        }

        public double BottomLevelAquitardAboveExitPointZ
        {
            set
            {
                wrappedCalculator.BottomLevelAquitardAboveExitPointZ = value;
            }
        }

        public double CreepCoefficient
        {
            get
            {
                return wrappedCalculator.CCreep;
            }
        }

        public double CriticalFall
        {
            get
            {
                return wrappedCalculator.Hc;
            }
        }

        public double ReducedFall
        {
            get
            {
                return wrappedCalculator.ReducedFall;
            }
        }

        public double Zp
        {
            get
            {
                return wrappedCalculator.Zp;
            }
        }

        public double FoSp
        {
            get
            {
                return wrappedCalculator.FoSp;
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