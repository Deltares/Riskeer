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

namespace Ringtoets.HydraRing.Calculation.Services
{
    public interface IHydraRingCalculationService
    {
        /// <summary>
        /// This method performs a type II calculation via Hydra-Ring:
        /// Iterate towards a target probability, provided as reliability index.
        /// </summary>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="uncertaintiesType">The <see cref="HydraRingUncertaintiesType"/> to use while executing the calculation.</param>
        /// <param name="hydraRingCalculationInput">The input of the calculation to perform.</param>
        /// <param name="parsers">Parsers that will be invoked after the Hydra-Ring calculation has ran.</param>
        void PerformCalculation(
            string hlcdDirectory,
            string ringId,
            HydraRingUncertaintiesType uncertaintiesType,
            HydraRingCalculationInput hydraRingCalculationInput,
                                       IEnumerable<IHydraRingFileParser> parsers);
    }
}