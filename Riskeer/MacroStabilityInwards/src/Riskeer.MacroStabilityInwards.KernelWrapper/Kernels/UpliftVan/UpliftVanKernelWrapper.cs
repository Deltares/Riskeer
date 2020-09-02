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

using System;
using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps the <see cref="Deltares.MacroStability.CSharpWrapper"/> for performing an Uplift Van calculation.
    /// </summary>
    internal class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly ICalculator calculator;
        private readonly IValidator validator;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper(ICalculator calculator, IValidator validator)
        {
            this.calculator = calculator;
            this.validator = validator;

            FactorOfStability = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
        }

        public double FactorOfStability { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public DualSlidingCircleMinimumSafetyCurve SlidingCurveResult { get; private set; }

        public UpliftVanCalculationGrid UpliftVanCalculationGridResult { get; private set; }

        public IEnumerable<Message> CalculationMessages { get; private set; }

        public void Calculate()
        {
            try
            {
                MacroStabilityOutput output = calculator.Calculate();

                CalculationMessages = output.StabilityOutput.Messages ?? Enumerable.Empty<Message>();
                SetResults(output);
            }
            catch (Exception e) when (!(e is UpliftVanKernelWrapperException))
            {
                throw new UpliftVanKernelWrapperException(e.Message, e, CalculationMessages.Where(lm => lm.MessageType == MessageType.Error));
            }
        }

        public IEnumerable<Message> Validate()
        {
            try
            {
                ValidationOutput output = validator.Validate();
                return output.Messages;
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private void SetResults(MacroStabilityOutput output)
        {
            FactorOfStability = output.StabilityOutput.SafetyFactor;
            ForbiddenZonesXEntryMin = output.PreprocessingOutputBase.ForbiddenZone.XEntryMin;
            ForbiddenZonesXEntryMax = output.PreprocessingOutputBase.ForbiddenZone.XEntryMax;

            SlidingCurveResult = (DualSlidingCircleMinimumSafetyCurve) output.StabilityOutput.MinimumSafetyCurve;
            UpliftVanCalculationGridResult = ((UpliftVanPreprocessingOutput) output.PreprocessingOutputBase).UpliftVanCalculationGrid;
        }
    }
}