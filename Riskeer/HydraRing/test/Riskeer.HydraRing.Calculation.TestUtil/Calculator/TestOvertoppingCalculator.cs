﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// Implementation of <see cref="IOvertoppingCalculator"/> for testing purposes.
    /// </summary>
    public class TestOvertoppingCalculator : TestHydraRingCalculator<OvertoppingCalculationInput>, IOvertoppingCalculator
    {
        public double ExceedanceProbabilityBeta { get; set; }
        public double WaveHeight { get; set; }
        public bool IsOvertoppingDominant { get; set; }
        public string OutputDirectory { get; set; }
        public string LastErrorFileContent { get; set; }
        public string IllustrationPointsParserErrorMessage { get; set; }
    }
}