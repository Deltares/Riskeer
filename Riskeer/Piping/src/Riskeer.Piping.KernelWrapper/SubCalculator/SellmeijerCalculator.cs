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

        public void SetBeddingAngle(double beddingAngle)
        {
            wrappedCalculator.BeddingAngle = beddingAngle;
        }

        public void SetD70(double d70)
        {
            wrappedCalculator.D70 = d70;
        }

        public void SetD70Mean(double d70Mean)
        {
            wrappedCalculator.D70Mean = d70Mean;
        }

        public void SetDAquifer(double dAquifer)
        {
            wrappedCalculator.DAquifer = dAquifer;
        }

        public void SetDarcyPermeability(double darcyPermeability)
        {
            wrappedCalculator.DarcyPermeability = darcyPermeability;
        }

        public void SetDTotal(double dTotal)
        {
            wrappedCalculator.DTotal = dTotal;
        }

        public void SetGammaSubParticles(double gammaSubParticles)
        {
            wrappedCalculator.GammaSubParticles = gammaSubParticles;
        }

        public void SetGravity(double gravity)
        {
            wrappedCalculator.Gravity = gravity;
        }

        public void SetHExit(double hExit)
        {
            wrappedCalculator.HExit = hExit;
        }

        public void SetHRiver(double hRiver)
        {
            wrappedCalculator.HRiver = hRiver;
        }

        public void SetKinematicViscosityWater(double kinematicViscosityWater)
        {
            wrappedCalculator.KinematicViscosityWater = kinematicViscosityWater;
        }

        public void SetModelFactorPiping(double modelFactorPiping)
        {
            wrappedCalculator.ModelFactorPiping = modelFactorPiping;
        }

        public void SetRc(double rc)
        {
            wrappedCalculator.Rc = rc;
        }

        public void SetSeepageLength(double seepageLength)
        {
            wrappedCalculator.SeepageLength = seepageLength;
        }

        public void SetVolumetricWeightOfWater(double volumetricWeightOfWater)
        {
            wrappedCalculator.VolumetricWeightOfWater = volumetricWeightOfWater;
        }

        public void SetWhitesDragCoefficient(double whitesDragCoefficient)
        {
            wrappedCalculator.WhitesDragCoefficient = whitesDragCoefficient;
        }

        public void SetBottomLevelAquitardAboveExitPointZ(double bottomLevelAquitardAboveExitPointZ)
        {
            wrappedCalculator.BottomLevelAquitardAboveExitPointZ = bottomLevelAquitardAboveExitPointZ;
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