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

using Core.Common.Base.Data;
using Ringtoets.Piping.KernelWrapper;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.InputParameterCalculation
{
    /// <summary>
    /// This class can be used to calculate input parameters for a piping calculation based on other input parameters.
    /// </summary>
    public static class InputParameterCalculationService
    {
        /// <summary>
        /// Calculates the thickness of the coverage layer based on the values of partial piping input.
        /// </summary>
        /// <param name="waterVolumetricWeight">The volumetric weight of water.</param>
        /// <param name="phreaticLevelExit">The design value of the phreatic level at the exit point.</param>
        /// <param name="exitPointL">The l-coordinate of the exit point.</param>
        /// <param name="surfaceLine">A surface line.</param>
        /// <param name="soilProfile">A soil profile.</param>
        /// <returns>The thickness of the coverage layer, or <see cref="double.NaN"/> if the thickness could not be calculated.</returns>
        public static double CalculateEffectiveThicknessCoverageLayer(double waterVolumetricWeight, RoundedDouble phreaticLevelExit, RoundedDouble exitPointL, PipingSurfaceLine surfaceLine, PipingSoilProfile soilProfile)
        {
            try
            {
                var calculatorInput = new PipingCalculatorInput(
                    new PipingCalculatorInput.ConstructionProperties
                    {
                        WaterVolumetricWeight = waterVolumetricWeight,
                        PhreaticLevelExit = phreaticLevelExit,
                        ExitPointXCoordinate = exitPointL,
                        SurfaceLine = surfaceLine,
                        SoilProfile = soilProfile
                    });
                return new PipingCalculator(calculatorInput, PipingSubCalculatorFactory.Instance).CalculateEffectiveThicknessCoverageLayer();
            }
            catch (PipingCalculatorException)
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Calculates the piezometric head at the exit point based on the values of partial piping input.
        /// </summary>
        /// <param name="assessmentLevel">The assessment level.</param>
        /// <param name="dampingFactorExit">The design value of the damping factor at exit point.</param>
        /// <param name="phreaticLevelExit">The design value of the phreatic level at exit point.</param>
        /// <returns>The piezometric head at the exit point.</returns>
        public static double CalculatePiezometricHeadAtExit(RoundedDouble assessmentLevel, RoundedDouble dampingFactorExit, RoundedDouble phreaticLevelExit)
        {
            var calculatorInput = new PipingCalculatorInput(
                new PipingCalculatorInput.ConstructionProperties
                {
                    AssessmentLevel = assessmentLevel,
                    DampingFactorExit = dampingFactorExit,
                    PhreaticLevelExit = phreaticLevelExit
                });
            return new PipingCalculator(calculatorInput, PipingSubCalculatorFactory.Instance).CalculatePiezometricHeadAtExit();
        }
    }
}