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
using Deltares.WTIStability.Data.Geo;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan
{
    /// <summary>
    /// Class representing an Uplift Van calculator.
    /// </summary>
    public class UpliftVanCalculator : IUpliftVanCalculator
    {
        private readonly UpliftVanCalculatorInput input;
        private readonly IMacroStabilityInwardsKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing the Uplift Van calculation.</param>
        /// <param name="factory">The factory responsible for creating the Uplift Van kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> or <paramref name="factory"/> is <c>null</c>.</exception>
        public UpliftVanCalculator(UpliftVanCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.input = input;
            this.factory = factory;
        }

        public IEnumerable<UpliftVanKernelMessage> Validate()
        {
            try
            {
                IUpliftVanKernel upliftVanKernel = CreateUpliftVanKernel();
                return UpliftVanKernelMessagesCreator.CreateFromValidationResults(upliftVanKernel.Validate().ToArray());
            }
            catch (UpliftVanKernelWrapperException e)
            {
                throw new UpliftVanCalculatorException(e.Message, e);
            }
        }

        public UpliftVanCalculatorResult Calculate()
        {
            IUpliftVanKernel upliftVanKernel = CalculateUpliftVan();

            return new UpliftVanCalculatorResult(
                UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.SlipPlaneResult),
                UpliftVanKernelMessagesCreator.CreateFromLogMessages(upliftVanKernel.CalculationMessages),
                new UpliftVanCalculatorResult.ConstructionProperties
                {
                    FactorOfStability = upliftVanKernel.FactorOfStability,
                    ZValue = upliftVanKernel.ZValue,
                    ForbiddenZonesXEntryMin = upliftVanKernel.ForbiddenZonesXEntryMin,
                    ForbiddenZonesXEntryMax = upliftVanKernel.ForbiddenZonesXEntryMax
                });
        }

        private IUpliftVanKernel CalculateUpliftVan()
        {
            IUpliftVanKernel upliftVanKernel = CreateUpliftVanKernel();

            try
            {
                upliftVanKernel.Calculate();
            }
            catch (UpliftVanKernelWrapperException e)
            {
                throw new UpliftVanCalculatorException(e.Message, e);
            }

            return upliftVanKernel;
        }

        private IUpliftVanKernel CreateUpliftVanKernel()
        {
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile);

            IUpliftVanKernel upliftVanKernel = factory.CreateUpliftVanKernel();

            upliftVanKernel.MoveGrid = input.MoveGrid;
            upliftVanKernel.MaximumSliceWidth = input.MaximumSliceWidth;
            upliftVanKernel.SetSoilModel(SoilModelCreator.Create(layersWithSoil.Select(lws => lws.Soil).ToArray()));
            upliftVanKernel.SoilProfile = SoilProfileCreator.Create(input.SoilProfile.PreconsolidationStresses, layersWithSoil);
            upliftVanKernel.LocationExtreme = UpliftVanStabilityLocationCreator.CreateExtreme(input);
            upliftVanKernel.LocationDaily = UpliftVanStabilityLocationCreator.CreateDaily(input);
            upliftVanKernel.SurfaceLine = SurfaceLineCreator.Create(input.SurfaceLine, input.LandwardDirection);
            upliftVanKernel.SlipPlaneUpliftVan = SlipPlaneUpliftVanCreator.Create(input.SlipPlane);
            upliftVanKernel.SlipPlaneConstraints = SlipPlaneConstraintsCreator.Create(input.SlipPlaneConstraints);
            upliftVanKernel.GridAutomaticDetermined = input.SlipPlane.GridAutomaticDetermined;

            return upliftVanKernel;
        }
    }
}