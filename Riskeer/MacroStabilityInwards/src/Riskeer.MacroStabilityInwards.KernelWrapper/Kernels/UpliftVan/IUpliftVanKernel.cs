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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using WtiStabilityWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Interface representing Uplift Van kernel input, methods and output.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item><see cref="UpliftVanCalculator"/> input into kernel input;</item>
    /// <item>kernel output into <see cref="UpliftVanCalculator"/> output.</item>
    /// </list>
    /// </remarks>
    public interface IUpliftVanKernel
    {
        /// <summary>
        /// Gets the factor of stability.
        /// </summary>
        double FactorOfStability { get; }

        /// <summary>
        /// Gets the forbidden zones x entry min.
        /// </summary>
        double ForbiddenZonesXEntryMin { get; }

        /// <summary>
        /// Gets the forbidden zones x entry max.
        /// </summary>
        double ForbiddenZonesXEntryMax { get; }

        /// <summary>
        /// Gets the sliding curve result.
        /// </summary>
        DualSlidingCircleMinimumSafetyCurve SlidingCurveResult { get; }

        /// <summary>
        /// Gets the slip plane result.
        /// </summary>
        UpliftVanCalculationGrid SlipPlaneResult { get; }

        /// <summary>
        /// Gets the messages returned by the kernel during
        /// the calculation.
        /// </summary>
        IEnumerable<Message> CalculationMessages { get; }

        /// <summary>
        /// Sets the slip plane Uplift Van.
        /// </summary>
        /// <param name="slipPlaneUpliftVan">The slip plane Uplift Van to set.</param>
        void SetSlipPlaneUpliftVan(UpliftVanCalculationGrid slipPlaneUpliftVan);

        /// <summary>
        /// Sets the slip plane constraints.
        /// </summary>
        /// <param name="slipPlaneConstraints">The slip plane constraints to set.</param>
        void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints);

        /// <summary>
        /// Sets the soil model.
        /// </summary>
        /// <param name="soilModel">The soil model to set.</param>
        void SetSoilModel(IEnumerable<Soil> soilModel);

        /// <summary>
        /// Sets the soil profile.
        /// </summary>
        /// <param name="soilProfile">The soil profile to set.</param>
        void SetSoilProfile(SoilProfile soilProfile);

        /// <summary>
        /// Sets the Waternet under daily circumstances.
        /// </summary>
        /// <param name="waternetDaily">The daily Waternet to set.</param>
        void SetWaternetDaily(WtiStabilityWaternet waternetDaily);

        /// <summary>
        /// Sets the Waternet under extreme circumstances.
        /// </summary>
        /// <param name="waternetExtreme">The extreme Waternet to set.</param>
        void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme);

        /// <summary>
        /// Sets the move grid property.
        /// </summary>
        /// <param name="moveGrid">The move grid value to set.</param>
        void SetMoveGrid(bool moveGrid);

        /// <summary>
        /// Sets the maximum slice width.
        /// [m]
        /// </summary>
        /// <param name="maximumSliceWidth">The maximum slice width to set.</param>
        void SetMaximumSliceWidth(double maximumSliceWidth);

        /// <summary>
        /// Sets the surface line.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set.</param>
        void SetSurfaceLine(SurfaceLine surfaceLine);

        /// <summary>
        /// Sets whether the grid is automatically determined or not.
        /// </summary>
        /// <param name="gridAutomaticDetermined">The grid automatic determined value to set.</param>
        void SetGridAutomaticDetermined(bool gridAutomaticDetermined);

        /// <summary>
        /// Sets whether the tangent lines are automatically determined or not.
        /// </summary>
        /// <param name="tangentLinesAutomaticDetermined">The tangent automatic lines determined value to set.</param>
        void SetTangentLinesAutomaticDetermined(bool tangentLinesAutomaticDetermined);

        /// <summary>
        /// Sets the fixed soil stresses.
        /// </summary>
        /// <param name="soilStresses">The soil stresses to set.</param>
        void SetFixedSoilStresses(IEnumerable<FixedSoilStress> soilStresses);

        /// <summary>
        /// Sets the preconsolidation stresses.
        /// </summary>
        /// <param name="preConsolidationStresses">The preconsolidation stresses to set.</param>
        void SetPreConsolidationStresses(IEnumerable<PreconsolidationStress> preconsolidationStresses);

        /// <summary>
        /// Sets whether the forbidden zones are automatically determined or not.
        /// </summary>
        /// <param name="automaticForbiddenZones">The automatic forbidden zones to set.</param>
        void SetAutomaticForbiddenZones(bool automaticForbiddenZones);

        /// <summary>
        /// Performs the Uplift Van calculation.
        /// </summary>
        /// <exception cref="UpliftVanKernelWrapperException">Thrown when
        /// an error occurs when performing the calculation.</exception>
        void Calculate();

        /// <summary>
        /// Validates the input for the Uplift Van calculation.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IValidationResult"/> objects.</returns>
        /// <exception cref="UpliftVanKernelWrapperException">Thrown when 
        /// an error occurs when performing the validation.</exception>
        IEnumerable<Message> Validate();
    }
}