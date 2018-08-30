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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="AggregatedSerializableFailureMechanism"/>.
    /// </summary>
    public static class AggregatedSerializableFailureMechanismCreator
    {
        public static AggregatedSerializableFailureMechanism Create(UniqueIdentifierGenerator idGenerator,
                                                                    SerializableTotalAssemblyResult serializableTotalAssembly,
                                                                    ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> failureMechanism)
        {
            SerializableFailureMechanism serializableFailureMechanism = SerializableFailureMechanismCreator.Create(idGenerator, serializableTotalAssembly, failureMechanism);
            var serializableCollection = new SerializableFailureMechanismSectionCollection(idGenerator.GetNewId("Vi"), serializableFailureMechanism);

            var serializableFailureMechanismSectionAssemblyResults = new List<AggregatedSerializableFailureMechanismSectionAssembly>();
            foreach (ExportableAggregatedFailureMechanismSectionAssemblyResultBase sectionAssemblyResult in failureMechanism.SectionAssemblyResults)
            {
                serializableFailureMechanismSectionAssemblyResults.Add(CreateFailureMechanismSectionAssembly(idGenerator, 
                                                                                                             serializableFailureMechanism, 
                                                                                                             serializableCollection,
                                                                                                             sectionAssemblyResult));
            }

            return new AggregatedSerializableFailureMechanism(serializableFailureMechanism,
                                                              serializableCollection,
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSection),
                                                              serializableFailureMechanismSectionAssemblyResults.Select(fmr => fmr.FailureMechanismSectionAssembly));
        }

        private static AggregatedSerializableFailureMechanismSectionAssembly CreateFailureMechanismSectionAssembly(
            UniqueIdentifierGenerator idGenerator,
            SerializableFailureMechanism serializableFailureMechanism, 
            SerializableFailureMechanismSectionCollection serializableCollection,
            ExportableAggregatedFailureMechanismSectionAssemblyResultBase failureMechanismSectionAssemblyResult)
        {
            var resultWithProbability = failureMechanismSectionAssemblyResult as ExportableAggregatedFailureMechanismSectionAssemblyResultWithProbability;
            if (resultWithProbability != null)
            {
                return AggregatedSerializableFailureMechanismSectionAssemblyCreator.Create(idGenerator, serializableCollection, serializableFailureMechanism, resultWithProbability);
            }

            throw new NotSupportedException();
        }
    }
}