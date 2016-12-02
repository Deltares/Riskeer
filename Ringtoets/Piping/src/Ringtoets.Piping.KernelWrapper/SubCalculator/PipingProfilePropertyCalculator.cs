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

using System.Collections.Generic;
using Deltares.WTIPiping;

namespace Ringtoets.Piping.KernelWrapper.SubCalculator
{
    /// <summary>
    /// Class which wraps a <see cref="Deltares.WTIPiping.PipingProfilePropertyCalculator"/>.
    /// </summary>
    public class PipingProfilePropertyCalculator : IPipingProfilePropertyCalculator
    {
        private readonly Deltares.WTIPiping.PipingProfilePropertyCalculator wrappedCalculator;

        /// <summary>
        /// Creates a new instance of <see cref="PipingProfilePropertyCalculator"/>.
        /// </summary>
        public PipingProfilePropertyCalculator()
        {
            wrappedCalculator = new Deltares.WTIPiping.PipingProfilePropertyCalculator();
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

        public double ExitPointX
        {
            set
            {
                wrappedCalculator.ExitPointX = value;
            }
        }

        public double BottomAquitardLayerAboveExitPointZ
        {
            get
            {
                return wrappedCalculator.BottomLevelAquitardAboveExitPoint;
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