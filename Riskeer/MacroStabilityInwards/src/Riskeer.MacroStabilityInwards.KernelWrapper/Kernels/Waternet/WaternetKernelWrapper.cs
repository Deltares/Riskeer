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
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using Deltares.WTIStability.Calculation.Wrapper;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing a Waternet calculation.
    /// </summary>
    internal class WaternetKernelWrapper : IWaternetKernel
    {
        private SoilProfile2D soilProfile2D;
        private SurfaceLine2 surfaceLine2;
        private Location location;

        public WtiStabilityWaternet Waternet { get; protected set; }

        public void SetLocation(Location stabilityLocation)
        {
            location = stabilityLocation;
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            soilProfile2D = soilProfile;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            surfaceLine2 = surfaceLine;
        }

        public void Calculate()
        {
            try
            {
                const double unitWeightWater = 9.81; // Taken from kernel
                var waternetCreator = new WaternetCreator(unitWeightWater);
                location.Surfaceline = surfaceLine2;
                location.SoilProfile2D = soilProfile2D;
                waternetCreator.UpdateWaternet(Waternet, location);

                ReadLogMessages(waternetCreator.LogMessages);
            }
            catch (Exception e) when (!(e is WaternetKernelWrapperException))
            {
                throw new WaternetKernelWrapperException(e.Message, e);
            }
        }

        /// <summary>
        /// Reads the log messages of the calculation.
        /// </summary>
        /// <param name="receivedLogMessages">The messages to read.</param>
        /// <exception cref="WaternetKernelWrapperException">Thrown when there
        /// are log messages of the type <see cref="LogMessageType.FatalError"/> or <see cref="LogMessageType.Error"/>.</exception>
        private static void ReadLogMessages(IEnumerable<LogMessage> receivedLogMessages)
        {
            LogMessage[] errorMessages = receivedLogMessages.Where(lm => lm.MessageType == LogMessageType.FatalError
                                                                         || lm.MessageType == LogMessageType.Error).ToArray();

            if (errorMessages.Any())
            {
                string message = errorMessages.Aggregate(string.Empty,
                                                         (current, logMessage) => current + $"{logMessage.Message}{Environment.NewLine}")
                                              .Trim();

                throw new WaternetKernelWrapperException(message);
            }
        }
    }
}