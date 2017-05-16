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

namespace Application.Ringtoets.Storage.Create.GrassCoverSlipOffOutwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverSlipOffOutwardsFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverSlipOffOutwardsFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/> based on the information of the <see cref="GrassCoverSlipOffOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/>.</returns>
        internal static GrassCoverSlipOffOutwardsSectionResultEntity Create(this GrassCoverSlipOffOutwardsFailureMechanismSectionResult result)
        {
            var sectionResultEntity = new GrassCoverSlipOffOutwardsSectionResultEntity
            {
                LayerOne = Convert.ToByte(result.AssessmentLayerOne),
                LayerTwoA = Convert.ToByte(result.AssessmentLayerTwoA),
                LayerThree = result.AssessmentLayerThree.ToNaNAsNull()
            };

            return sectionResultEntity;
        }
    }
}