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
using Ringtoets.Piping.KernelWrapper.SubCalculator;

namespace Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator
{
    /// <summary>
    /// Stub for the real piping profile property calculator.
    /// </summary>
    public class PipingProfilePropertyCalculatorStub : IPipingProfilePropertyCalculator
    {
        public PipingProfile SoilProfile { get; set; }
        public PipingSurfaceLine SurfaceLine { get; set; }
        public double ExitPointX { get; set; }
        
        public double BottomAquitardLayerAboveExitPointZ { get; set; }

        public bool Calculated { get; private set; }
        public bool Validated { get; private set; }

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