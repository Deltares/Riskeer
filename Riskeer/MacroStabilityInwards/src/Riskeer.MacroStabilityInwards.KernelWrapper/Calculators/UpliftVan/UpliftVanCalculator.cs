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
using Deltares.MacroStability.Geometry;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan
{
    /// <summary>
    /// Class representing an Uplift Van calculator.
    /// </summary>
    public class UpliftVanCalculator : IUpliftVanCalculator
    {
        private readonly UpliftVanCalculatorInput input;
        private readonly IMacroStabilityInwardsKernelFactory factory;
        private WaternetDailyKernelWrapper waternetDailyKernelWrapper;
        private WaternetExtremeKernelWrapper waternetExtremeKernelWrapper;

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
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);

            IUpliftVanKernel upliftVanKernel = factory.CreateUpliftVanKernel();

            SurfaceLine2 surfaceLine2 = SurfaceLineCreator.Create(input.SurfaceLine);
            SoilProfile2D soilProfile2D = SoilProfileCreator.Create(input.SoilProfile.PreconsolidationStresses, layersWithSoil);

            upliftVanKernel.SetSlipPlaneUpliftVan(SlipPlaneUpliftVanCreator.Create(input.SlipPlane));
            upliftVanKernel.SetSlipPlaneConstraints(SlipPlaneConstraintsCreator.Create(input.SlipPlaneConstraints));
            upliftVanKernel.SetGridAutomaticDetermined(input.SlipPlane.GridAutomaticDetermined);
            upliftVanKernel.SetTangentLinesAutomaticDetermined(input.SlipPlane.TangentLinesAutomaticAtBoundaries);
            upliftVanKernel.SetMoveGrid(input.MoveGrid);
            upliftVanKernel.SetMaximumSliceWidth(input.MaximumSliceWidth);
            upliftVanKernel.SetSurfaceLine(surfaceLine2);
            upliftVanKernel.SetSoilModel(layersWithSoil.Select(lws => lws.Soil).ToArray());
            upliftVanKernel.SetSoilProfile(soilProfile2D);
            
            waternetDailyKernelWrapper = new WaternetDailyKernelWrapper();
            waternetDailyKernelWrapper.SetLocation(UpliftVanStabilityLocationCreator.CreateDaily(input));
            waternetDailyKernelWrapper.SetSoilProfile(soilProfile2D);
            waternetDailyKernelWrapper.SetSurfaceLine(surfaceLine2);
            waternetDailyKernelWrapper.Calculate();
            upliftVanKernel.SetWaternetDaily(waternetDailyKernelWrapper.Waternet);

            waternetExtremeKernelWrapper = new WaternetExtremeKernelWrapper();
            waternetExtremeKernelWrapper.SetLocation(UpliftVanStabilityLocationCreator.CreateExtreme(input));
            waternetExtremeKernelWrapper.SetSoilProfile(soilProfile2D);
            waternetExtremeKernelWrapper.SetSurfaceLine(surfaceLine2);
            waternetExtremeKernelWrapper.Calculate();
            upliftVanKernel.SetWaternetExtreme(waternetExtremeKernelWrapper.Waternet);
            
            return upliftVanKernel;
        }
    }
}