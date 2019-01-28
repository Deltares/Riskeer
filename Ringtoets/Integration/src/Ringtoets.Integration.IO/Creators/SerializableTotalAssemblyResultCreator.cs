// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableTotalAssemblyResult"/>
    /// </summary>
    public static class SerializableTotalAssemblyResultCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableTotalAssemblyResult"/>
        /// based on its input parameters
        /// </summary>
        /// <param name="idGenerator">The generator to generate an id for the <see cref="SerializableTotalAssemblyResult"/>.</param>
        /// <param name="assessmentProcess">The assessment process this result belongs to.</param>
        /// <param name="failureMechanismAssemblyResultWithProbability">The assembly result for failure mechanisms with probability.</param>
        /// <param name="failureMechanismAssemblyResultWithoutProbability">The assembly result for failure mechanisms without probability.</param>
        /// <param name="assessmentSectionAssemblyResult">The assembly result for the assessment section.</param>
        /// <returns>A <see cref="SerializableTotalAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableTotalAssemblyResult Create(IdentifierGenerator idGenerator,
                                                             SerializableAssessmentProcess assessmentProcess,
                                                             SerializableFailureMechanismAssemblyResult failureMechanismAssemblyResultWithProbability,
                                                             SerializableFailureMechanismAssemblyResult failureMechanismAssemblyResultWithoutProbability,
                                                             SerializableAssessmentSectionAssemblyResult assessmentSectionAssemblyResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            return new SerializableTotalAssemblyResult(idGenerator.GetNewId(Resources.SerializableTotalAssemblyResult_IdPrefix),
                                                       assessmentProcess,
                                                       failureMechanismAssemblyResultWithoutProbability,
                                                       failureMechanismAssemblyResultWithProbability,
                                                       assessmentSectionAssemblyResult);
        }
    }
}