﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Utils.Extensions;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service for synchronizing grass cover erosion inwards data.
    /// </summary>
    public static class GrassCoverErosionInwardsDataSynchronizationService
    {
        /// <summary>
        /// Clears the output for all calculations in the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="GrassCoverErosionInwardsFailureMechanism"/> which contains the calculations.</param>
        public static void ClearAllCalculationOutput(GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            failureMechanism.Calculations
                            .Cast<GrassCoverErosionInwardsCalculation>()
                            .Where(c => c.HasOutput)
                            .ForEachElementDo(ClearCalculationOutput);
        }

        private static void ClearCalculationOutput(GrassCoverErosionInwardsCalculation calculation)
        {
            calculation.Output = null;
        }
    }
}