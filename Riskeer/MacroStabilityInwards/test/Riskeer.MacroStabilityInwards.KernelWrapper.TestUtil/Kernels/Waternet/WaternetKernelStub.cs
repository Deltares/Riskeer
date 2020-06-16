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
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet
{
    /// <summary>
    /// Waternet kernel stub for testing purposes.
    /// </summary>
    public class WaternetKernelStub : IWaternetKernel
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> was called or not.
        /// </summary>
        public bool Validated { get; private set; }

        /// <summary>
        /// Gets or sets an indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether an exception must be thrown when performing the validation.
        /// </summary>
        public bool ThrowExceptionOnValidate { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether a validation result must be returned when performing the validation.
        /// </summary>
        public bool ReturnValidationResults { get; set; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Gets the soil profile.
        /// </summary>
        public SoilProfile2D SoilProfile { get; private set; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public SurfaceLine2 SurfaceLine { get; private set; }

        public WtiStabilityWaternet Waternet { get; set; }

        /// <summary>
        /// Sets the location.
        /// </summary>
        /// <param name="location">The <see cref="Location"/> to set.</param>
        public void SetLocation(Location location)
        {
            Location = location;
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            SoilProfile = soilProfile;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            SurfaceLine = surfaceLine;
        }

        public void SetSoilModel(IList<Soil> soilModel) {}

        public void Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new WaternetKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            Calculated = true;
        }

        public IEnumerable<IValidationResult> Validate()
        {
            if (ThrowExceptionOnValidate)
            {
                throw new WaternetKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            if (ReturnValidationResults)
            {
                yield return new ValidationResult(ValidationResultType.Warning, "Validation Warning");
                yield return new ValidationResult(ValidationResultType.Error, "Validation Error");
                yield return new ValidationResult(ValidationResultType.Info, "Validation Info");
                yield return new ValidationResult(ValidationResultType.Debug, "Validation Debug");
            }

            Validated = true;
        }
    }
}