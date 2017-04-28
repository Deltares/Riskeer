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

using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Create.MacrostabilityOutwards
{
    /// <summary>
    /// Extension methods for <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="MacrostabilityOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class MacrostabilityOutwardsFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacrostabilityOutwardsSectionResultEntity"/> based on the information of the <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="MacrostabilityOutwardsSectionResultEntity"/>.</returns>
        internal static MacrostabilityOutwardsSectionResultEntity Create(this MacrostabilityOutwardsFailureMechanismSectionResult result)
        {
            var sectionResultEntity = new MacrostabilityOutwardsSectionResultEntity
            {
                LayerOne = Convert.ToByte(result.AssessmentLayerOne),
                LayerTwoA = result.AssessmentLayerTwoA.ToNaNAsNull(),
                LayerThree = result.AssessmentLayerThree.ToNaNAsNull()
            };

            return sectionResultEntity;
        }
    }
}