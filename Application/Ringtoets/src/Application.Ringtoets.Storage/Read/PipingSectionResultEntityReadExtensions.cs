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

using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanismSectionResult"/> based on the
    /// <see cref="PipingSectionResultEntity"/>.
    /// </summary>
    internal static class PipingSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingSectionResultEntity"/> and use the information to construct a 
        /// <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> to create <see cref="PipingFailureMechanismSectionResult"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingSoilLayer"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static PipingFailureMechanismSectionResult Read(this PipingSectionResultEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var sectionResult = new PipingFailureMechanismSectionResult(collector.Get(entity.FailureMechanismSectionEntity))
            {
                StorageId = entity.PipingSectionResultEntityId,
                AssessmentLayerOne = Convert.ToBoolean(entity.LayerOne),
                AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNanableDouble()
            };
            return sectionResult;
        }
    }
}