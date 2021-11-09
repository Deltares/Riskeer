// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.SpecificFailurePaths
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="SpecificFailurePath"/>
    /// based on the <see cref="SpecificFailurePathEntity"/>.
    /// </summary>
    internal static class SpecificFailurePathEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="SpecificFailurePathEntity"/> and use the information to create a 
        /// <see cref="SpecificFailurePath"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SpecificFailurePathEntity"/> to create <see cref="SpecificFailurePath"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="SpecificFailurePath"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static SpecificFailurePath Read(this SpecificFailurePathEntity entity,
                                                 ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var specificFailurePath = new SpecificFailurePath
            {
                Name = entity.Name,
                InAssembly = Convert.ToBoolean(entity.InAssembly),
                InAssemblyInputComments =
                {
                    Body = entity.InputComments
                },
                InAssemblyOutputComments =
                {
                    Body = entity.OutputComments
                },
                NotInAssemblyComments =
                {
                    Body = entity.NotInAssemblyComments
                }
            };

            entity.ReadFailureMechanismSections(specificFailurePath, collector);
            entity.ReadSpecificFailurePathInput(specificFailurePath);

            return specificFailurePath;
        }

        private static void ReadFailureMechanismSections(this SpecificFailurePathEntity entity,
                                                         IFailurePath specificFailurePath,
                                                         ReadConversionCollector collector)
        {
            FailureMechanismSection[] readFailureMechanismSections = entity.FailureMechanismSectionEntities
                                                                           .Select(failureMechanismSectionEntity =>
                                                                                       failureMechanismSectionEntity.Read(collector))
                                                                           .ToArray();
            if (readFailureMechanismSections.Any())
            {
                specificFailurePath.SetSections(readFailureMechanismSections, entity.FailureMechanismSectionCollectionSourcePath);
            }
        }

        private static void ReadSpecificFailurePathInput(this SpecificFailurePathEntity entity,
                                                         SpecificFailurePath failurePath)
        {
            failurePath.Input.N = (RoundedDouble) entity.N;
        }
    }
}