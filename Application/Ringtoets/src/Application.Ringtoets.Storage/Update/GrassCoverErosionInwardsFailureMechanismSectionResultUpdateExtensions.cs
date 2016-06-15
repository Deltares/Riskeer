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

using System;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> related to updating a 
    /// <see cref="GrassCoverErosionInwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsFailureMechanismSectionResultUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="GrassCoverErosionInwardsSectionResultEntity"/> in the database based on the information of the 
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="result"/>
        /// does not have a corresponding entity in <paramref name="context"/>.</exception>
        internal static void Update(this GrassCoverErosionInwardsFailureMechanismSectionResult result, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            GrassCoverErosionInwardsSectionResultEntity entity = GetCorrespondingGrassCoverErosionInwardsSectionResult(result, context);

            entity.LayerOne = Convert.ToByte(result.AssessmentLayerOne);
            entity.LayerThree = Convert.ToDecimal(result.AssessmentLayerThree);

            registry.Register(entity, result);
        }

        private static GrassCoverErosionInwardsSectionResultEntity GetCorrespondingGrassCoverErosionInwardsSectionResult(GrassCoverErosionInwardsFailureMechanismSectionResult result, IRingtoetsEntities context)
        {
            try
            {
                return context.GrassCoverErosionInwardsSectionResultEntities.Single(sle => sle.GrassCoverErosionInwardsSectionResultEntityId == result.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(GrassCoverErosionInwardsSectionResultEntity).Name, result.StorageId), exception);
            }
        }
    }
}