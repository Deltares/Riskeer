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
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

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
        /// Gets the z value.
        /// </summary>
        double ZValue { get; }

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
        SlidingDualCircle SlidingCurveResult { get; }

        /// <summary>
        /// Gets the slip plane result.
        /// </summary>
        SlipPlaneUpliftVan SlipPlaneResult { get; }

        /// <summary>
        /// Gets the messages returned by the kernel during
        /// the calculation.
        /// </summary>
        IEnumerable<LogMessage> CalculationMessages { get; }

        /// <summary>
        /// Sets the soil model.
        /// </summary>
        void SetSoilModel(IList<Soil> soilModel);

        /// <summary>
        /// Sets the soil profile.
        /// </summary>
        void SetSoilProfile(SoilProfile2D soilProfile);

        /// <summary>
        /// Sets the location under daily circumstances.
        /// </summary>
        void SetWaternetDaily(WtiStabilityWaternet waternetDaily);

        /// <summary>
        /// Sets the location under extreme circumstances.
        /// </summary>
        void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme);

        /// <summary>
        /// Sets the move grid property.
        /// </summary>
        void SetMoveGrid(bool moveGrid);

        /// <summary>
        /// Sets the maximum slice width.
        /// [m]
        /// </summary>
        void SetMaximumSliceWidth(double maximumSliceWidth);

        /// <summary>
        /// Sets the slip plane Uplift Van.
        /// </summary>
        void SetSlipPlaneUpliftVan(SlipPlaneUpliftVan slipPlaneUpliftVan);

        /// <summary>
        /// Sets the surface line.
        /// </summary>
        void SetSurfaceLine(SurfaceLine2 surfaceLine);

        /// <summary>
        /// Sets the slip plane constraints.
        /// </summary>
        void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints);

        /// <summary>
        /// Sets whether the grid is automatically determined or not.
        /// </summary>
        void SetGridAutomaticDetermined(bool gridAutomaticDetermined);

        /// <summary>
        /// Sets whether the tangent lines are automatically determined or not.
        /// </summary>
        void SetTangentLinesAutomaticDetermined(bool slipPlaneTangentLinesAutomaticAtBoundaries);

        /// <summary>
        /// Performs the Uplift Van calculation.
        /// </summary>
        /// <exception cref="UpliftVanKernelWrapperException">Thrown when
        /// an error occurs when performing the calculation.</exception>
        void Calculate();

        /// <summary>
        /// Validates the input for the Uplift Van calculation.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ValidationResult"/> objects.</returns>
        /// <exception cref="UpliftVanKernelWrapperException">Thrown when 
        /// an error occurs when performing the validation.</exception>
        IEnumerable<IValidationResult> Validate();
    }
}