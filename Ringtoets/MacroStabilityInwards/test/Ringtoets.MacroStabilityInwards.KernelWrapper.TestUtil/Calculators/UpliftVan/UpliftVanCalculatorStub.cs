﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan
{
    /// <summary>
    /// Uplift Van calculator stub for testing purposes.
    /// </summary>
    public class UpliftVanCalculatorStub : IUpliftVanCalculator
    {
        public UpliftVanCalculatorInput Input { get; set; }

        public UpliftVanCalculatorResult Output { get; private set; }

        public UpliftVanCalculatorResult Calculate()
        {
            return Output ?? (Output = CreateUpliftVanCalculatorResult());
        }

        public List<string> Validate()
        {
            return new List<string>();
        }

        private static UpliftVanCalculatorResult CreateUpliftVanCalculatorResult()
        {
            return new UpliftVanCalculatorResult(
                UpliftVanSlidingCurveResultTestFactory.Create(),
                new UpliftVanCalculationGridResult(
                    UpliftVanGridResultTestFactory.Create(),
                    UpliftVanGridResultTestFactory.Create(),
                    new[]
                    {
                        3,
                        2,
                        1.5
                    }),
                new UpliftVanCalculatorResult.ConstructionProperties
                {
                    FactorOfStability = 0.1,
                    ZValue = 0.2,
                    ForbiddenZonesXEntryMin = 0.3,
                    ForbiddenZonesXEntryMax = 0.4
                });
        }
    }
}