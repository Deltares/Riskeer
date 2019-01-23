// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Data;
using Riskeer.Storage.Core.Create.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="OvertoppingRateOutput"/> related to creating a
    /// <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>.
    /// </summary>
    internal static class OvertoppingRateOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>
        /// based on the information of the <see cref="OvertoppingRateOutput"/>.
        /// </summary>
        /// <param name="output">The output to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsOvertoppingRateOutputEntity Create(this OvertoppingRateOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return new GrassCoverErosionInwardsOvertoppingRateOutputEntity
            {
                OvertoppingRate = output.OvertoppingRate.ToNaNAsNull(),
                TargetProbability = output.TargetProbability.ToNaNAsNull(),
                TargetReliability = output.TargetReliability.ToNaNAsNull(),
                CalculatedProbability = output.CalculatedProbability.ToNaNAsNull(),
                CalculatedReliability = output.CalculatedReliability.ToNaNAsNull(),
                CalculationConvergence = Convert.ToByte(output.CalculationConvergence),
                GeneralResultFaultTreeIllustrationPointEntity = output.GeneralResult?.CreateGeneralResultFaultTreeIllustrationPointEntity()
            };
        }
    }
}