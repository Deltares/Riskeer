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
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="ClosingStructureFailureMechanismSectionResult"/> based on the
    /// <see cref="ClosingStructureSectionResultEntity"/>.
    /// </summary>
    internal static class ClosingStructureSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ClosingStructureSectionResultEntity"/> and use the information to construct a 
        /// <see cref="ClosingStructureFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureSectionResultEntity"/> to create <see cref="ClosingStructureFailureMechanismSectionResult"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="ClosingStructureFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static ClosingStructureFailureMechanismSectionResult Read(this ClosingStructureSectionResultEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var sectionResult = new ClosingStructureFailureMechanismSectionResult(collector.Get(entity.FailureMechanismSectionEntity))
            {
                StorageId = entity.ClosingStructureSectionResultEntityId,
                AssessmentLayerOne = Convert.ToBoolean(entity.LayerOne),
                AssessmentLayerTwoA = (RoundedDouble)entity.LayerTwoA.ToNanableDouble(),
                AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNanableDouble()
            };
            return sectionResult;
        }
    }
}