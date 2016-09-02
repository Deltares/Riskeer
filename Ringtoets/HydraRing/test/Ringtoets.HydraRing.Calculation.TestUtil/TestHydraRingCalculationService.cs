// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// This class allows for retrieving the input parameters used 
    /// in <see cref="IHydraRingCalculationService.PerformCalculation"/>, 
    /// so that tests can be performed upon them.
    /// </summary>
    public class TestHydraRingCalculationService : IHydraRingCalculationService
    {
        /// <summary>
        /// Gets the parsers used in <see cref="IHydraRingCalculationService.PerformCalculation"/>.
        /// </summary>
        public IEnumerable<IHydraRingFileParser> Parsers { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="IHydraRingCalculationService.PerformCalculation"/>.
        /// </summary>
        public HydraRingCalculationInput HydraRingCalculationInput { get; private set; }

        /// <summary>
        /// Gets the uncertainties type used in <see cref="IHydraRingCalculationService.PerformCalculation"/>.
        /// </summary>
        public HydraRingUncertaintiesType UncertaintiesType { get; private set; }

        /// <summary>
        /// Gets the ring id used in <see cref="IHydraRingCalculationService.PerformCalculation"/>.
        /// </summary>
        public string RingId { get; private set; }

        /// <summary>
        /// Gets the HLCD directory used in <see cref="IHydraRingCalculationService.PerformCalculation"/>.
        /// </summary>
        public string HlcdDirectory { get; private set; }

        public void PerformCalculation(string hlcdDirectory,
                                       string ringId,
                                       HydraRingUncertaintiesType uncertaintiesType,
                                       HydraRingCalculationInput hydraRingCalculationInput,
                                       IEnumerable<IHydraRingFileParser> parsers)
        {
            HlcdDirectory = hlcdDirectory;
            RingId = ringId;
            UncertaintiesType = uncertaintiesType;
            HydraRingCalculationInput = hydraRingCalculationInput;
            Parsers = parsers;
        }
    }
}