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
    /// Class which wraps a <see cref="Deltares.WTIPiping.EffectiveThicknessCalculator"/>.
    /// </summary>
    public class EffectiveThicknessCalculator : IEffectiveThicknessCalculator
    {
        private readonly Deltares.WTIPiping.EffectiveThicknessCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="EffectiveThicknessCalculator"/>.
        /// </summary>
        public EffectiveThicknessCalculator()
        {
            wrappedCalculator = new Deltares.WTIPiping.EffectiveThicknessCalculator();
        }

        public double ExitPointXCoordinate
        {
            set
            {
                wrappedCalculator.ExitPointXCoordinate = value;
            }
        }

        public double PhreaticLevel
        {
            set
            {
                wrappedCalculator.PhreaticLevel = value;
            }
        }

        public double VolumicWeightOfWater
        {
            set
            {
                wrappedCalculator.VolumicWeightOfWater = value;
            }
        }

        public PipingProfile SoilProfile
        {
            set
            {
                wrappedCalculator.SoilProfile = value;
            }
        }

        public PipingSurfaceLine SurfaceLine
        {
            set
            {
                wrappedCalculator.SurfaceLine = value;
            }
        }

        public double EffectiveHeight
        {
            get
            {
                return wrappedCalculator.EffectiveHeight;
            }
        }

        public List<string> Validate()
        {
            return wrappedCalculator.Validate();
        }

        public void Calculate()
        {
            wrappedCalculator.Calculate();
        }
    }
}