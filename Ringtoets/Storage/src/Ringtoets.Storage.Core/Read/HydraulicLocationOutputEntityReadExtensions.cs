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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.IllustrationPoints;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicLocationOutputEntity"/> related to creating 
    /// a <see cref="HydraulicBoundaryLocationCalculationOutput"/>.
    /// </summary>
    internal static class HydraulicLocationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="HydraulicLocationOutputEntity"/> and use the information to construct a <see cref="HydraulicBoundaryLocationCalculationOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationOutputEntity"/> to create <see cref="HydraulicBoundaryLocationCalculationOutput"/> for.</param>
        /// <returns>A new <see cref="HydraulicBoundaryLocationCalculationOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal static HydraulicBoundaryLocationCalculationOutput Read(this HydraulicLocationOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new HydraulicBoundaryLocationCalculationOutput(entity.Result.ToNullAsNaN(),
                                                                  entity.TargetProbability.ToNullAsNaN(),
                                                                  entity.TargetReliability.ToNullAsNaN(),
                                                                  entity.CalculatedProbability.ToNullAsNaN(),
                                                                  entity.CalculatedReliability.ToNullAsNaN(),
                                                                  (CalculationConvergence) entity.CalculationConvergence,
                                                                  entity.GeneralResultSubMechanismIllustrationPointEntity?.Read());
        }
    }
}