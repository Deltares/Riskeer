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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// Extension methods for <see cref="IHydraulicLocationOutputEntity"/> related to creating 
    /// a <see cref="HydraulicBoundaryLocationOutput"/>.
    /// </summary>
    internal static class IHydraulicLocationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="IHydraulicLocationOutputEntity"/> and use the information to construct a <see cref="HydraulicBoundaryLocationOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="IHydraulicLocationOutputEntity"/> to create <see cref="HydraulicBoundaryLocationOutput"/> for.</param>
        /// <returns>A new <see cref="HydraulicBoundaryLocationOutput"/>.</returns>
        internal static HydraulicBoundaryLocationOutput Read(this IHydraulicLocationOutputEntity entity)
        {
            return new HydraulicBoundaryLocationOutput(entity.Result.ToNullAsNaN(),
                                                       entity.TargetProbability.ToNullAsNaN(),
                                                       entity.TargetReliability.ToNullAsNaN(),
                                                       entity.CalculatedProbability.ToNullAsNaN(),
                                                       entity.CalculatedReliability.ToNullAsNaN(),
                                                       (CalculationConvergence) entity.CalculationConvergence);
        }
    }
}