// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Deltares.WTIStability;
using Deltares.WTIStability.Calculation.Wrapper;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;
using Deltares.WTIStability.IO;
using WtiStabilityWaternet = Deltares.WTIStability.Data.Geo.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing a Waternet calculation.
    /// </summary>
    internal abstract class WaternetKernelWrapper : IWaternetKernel
    {
        private readonly StabilityModel stabilityModel;

        /// <summary>
        /// Creates a new instance of <see cref="WaternetKernelWrapper"/>.
        /// </summary>
        protected WaternetKernelWrapper()
        {
            stabilityModel = new StabilityModel();
        }

        public abstract StabilityLocation Location { set; }

        public SoilModel SoilModel
        {
            set
            {
                StabilityModel.SoilModel = value;
            }
        }

        public SoilProfile2D SoilProfile
        {
            set
            {
                StabilityModel.SoilProfile = value;
            }
        }

        public SurfaceLine2 SurfaceLine
        {
            set
            {
                StabilityModel.SurfaceLine2 = value;
            }
        }

        public WtiStabilityWaternet Waternet { get; private set; }

        public void Calculate()
        {
            try
            {
                var waternetCalculation = new WTIStabilityCalculation();
                waternetCalculation.InitializeForDeterministic(WTISerializer.Serialize(StabilityModel));

                string waternetXmlResult = CreateWaternetXmlResult(waternetCalculation);
                ReadValidationResult(waternetXmlResult);
                Waternet = ReadResult(waternetXmlResult);
            }
            catch (Exception e) when (!(e is WaternetKernelWrapperException))
            {
                throw new WaternetKernelWrapperException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the stability model of the kernel.
        /// </summary>
        protected StabilityModel StabilityModel
        {
            get
            {
                return stabilityModel;
            }
        }

        protected abstract string CreateWaternetXmlResult(WTIStabilityCalculation waternetCalculation);

        protected abstract WtiStabilityWaternet ReadResult(string waternetXmlResult);

        /// <summary>
        /// Reads the validation results of the calculation.
        /// </summary>
        /// <param name="waternetXmlResult">The result to read.</param>
        /// <exception cref="WaternetKernelWrapperException">Thrown when there
        /// are validation results of the type <see cref="ValidationResultType.Error"/>.</exception>
        private static void ReadValidationResult(string waternetXmlResult)
        {
            List<ValidationResult> validationResults = WTIDeserializer.DeserializeValidationMessagesForWaternet(waternetXmlResult);
            ValidationResult[] errorMessages = validationResults.Where(vr => vr.MessageType == ValidationResultType.Error).ToArray();

            if (errorMessages.Any())
            {
                string message = errorMessages.Aggregate(string.Empty,
                                                         (current, validationResult) => current + $"{validationResult.Text}{Environment.NewLine}")
                                              .Trim();

                throw new WaternetKernelWrapperException(message);
            }
        }
    }
}