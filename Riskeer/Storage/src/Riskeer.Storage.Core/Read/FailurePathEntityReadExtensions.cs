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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="IFailurePath"/> implementation based on the
    /// <see cref="IFailurePathEntity"/>
    /// </summary>
    internal static class FailurePathEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="IFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void ReadCommonFailureMechanismProperties(this FailureMechanismEntity entity,
                                                                  IFailureMechanism failureMechanism,
                                                                  ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            entity.ReadCommonFailurePathProperties(failureMechanism, collector);
            failureMechanism.CalculationsInputComments.Body = entity.CalculationsInputComments;
        }

        /// <summary>
        /// Reads the <see cref="IFailurePathEntity"/> and uses the information to create a <see cref="SpecificFailurePath"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SpecificFailurePathEntity"/> to create a <see cref="SpecificFailurePath"/> with.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        internal static SpecificFailurePath ReadSpecificFailurePath(this SpecificFailurePathEntity entity,
                                                                    ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var specificFailurePath = new SpecificFailurePath
            {
                Name = entity.Name,
                Code = entity.Code,
                GeneralInput =
                {
                    N = (RoundedDouble) entity.N,
                    ApplyLengthEffectInSection = Convert.ToBoolean(entity.ApplyLengthEffectInSection)
                }
            };
            entity.ReadCommonFailurePathProperties(specificFailurePath, collector);
            return specificFailurePath;
        }

        private static void ReadCommonFailurePathProperties<T>(this T entity, IFailurePath failurePath, ReadConversionCollector collector)
            where T : IFailurePathEntity
        {
            failurePath.InAssembly = Convert.ToBoolean(entity.InAssembly);
            failurePath.InAssemblyInputComments.Body = entity.InAssemblyInputComments;
            failurePath.InAssemblyOutputComments.Body = entity.InAssemblyOutputComments;
            failurePath.NotInAssemblyComments.Body = entity.NotInAssemblyComments;

            entity.ReadFailureMechanismSections(failurePath, collector);
            ReadAssemblyResult(entity, failurePath);
        }

        private static void ReadAssemblyResult(IFailurePathEntity entity, IFailurePath failurePath)
        {
            FailurePathAssemblyResult assemblyResult = failurePath.AssemblyResult;
            assemblyResult.ProbabilityResultType = (FailurePathAssemblyProbabilityResultType) entity.FailurePathAssemblyProbabilityResultType;
            if (entity.ManualFailurePathAssemblyProbability != null)
            {
                assemblyResult.ManualFailurePathAssemblyProbability = entity.ManualFailurePathAssemblyProbability.ToNullAsNaN();
            }
        }

        private static void ReadFailureMechanismSections(this IFailurePathEntity entity,
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
    }
}