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
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="PipingSectionResultEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingSectionResultEntity"/> based on the information of the <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="PipingSectionResultEntity"/>.</returns>
        internal static PipingSectionResultEntity Create(this PipingFailureMechanismSectionResult result)
        {
            var pipingSectionResultEntity = new PipingSectionResultEntity
            {
                LayerOne = Convert.ToByte(result.AssessmentLayerOne),
                LayerThree = result.AssessmentLayerThree.Value.ToNaNAsNull()
            };

            return pipingSectionResultEntity;
        }
    }
}