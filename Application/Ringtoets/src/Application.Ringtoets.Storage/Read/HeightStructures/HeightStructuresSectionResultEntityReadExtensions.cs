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
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Read.HeightStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="HeightStructuresFailureMechanismSectionResult"/> 
    /// based on the <see cref="HeightStructuresSectionResultEntity"/>.
    /// </summary>
    internal static class HeightStructuresSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HeightStructuresSectionResultEntity"/> and use the information 
        /// to construct a <see cref="HeightStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresSectionResultEntity"/> to 
        /// create <see cref="HeightStructuresFailureMechanismSectionResult"/> for.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="HeightStructuresFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this HeightStructuresSectionResultEntity entity, HeightStructuresFailureMechanismSectionResult sectionResult,
                                  ReadConversionCollector collector)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            sectionResult.AssessmentLayerOne = Convert.ToBoolean(entity.LayerOne);
            sectionResult.AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNullAsNaN();

            if (entity.HeightStructuresCalculationEntity != null)
            {
                sectionResult.Calculation = entity.HeightStructuresCalculationEntity.Read(collector);
            }
        }
    }
}