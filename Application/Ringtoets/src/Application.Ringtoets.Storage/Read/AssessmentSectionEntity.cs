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
using System.Linq;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for an <see cref="AssessmentSection"/> based on the
    /// <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    public partial class AssessmentSectionEntity
    {
        /// <summary>
        /// Read the <see cref="AssessmentSectionEntity"/> and use the information to construct a <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public AssessmentSection Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var assessmentSection = new AssessmentSection((AssessmentSectionComposition) Composition)
            {
                StorageId = AssessmentSectionEntityId,
                Name = Name
            };

            ReadPipingFailureMechanism(assessmentSection, collector);
            ReadHydraulicDatabase(assessmentSection);
            ReadReferenceLine(assessmentSection);
            ReadFailureMechanismPlaceholders(assessmentSection);

            return assessmentSection;
        }

        private void ReadReferenceLine(AssessmentSection assessmentSection)
        {
            if (ReferenceLinePointEntities.Any())
            {
                assessmentSection.ReferenceLine = new ReferenceLine();
                assessmentSection.ReferenceLine.SetGeometry(ReferenceLinePointEntities.OrderBy(rlpe => rlpe.Order).Select(rlpe => rlpe.Read()));
            }
        }

        private void ReadHydraulicDatabase(AssessmentSection assessmentSection)
        {
            if (HydraulicDatabaseLocation != null)
            {
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = HydraulicDatabaseLocation,
                    Version = HydraulicDatabaseVersion
                };

                foreach (var hydraulicLocationEntity in HydraulicLocationEntities)
                {
                    assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicLocationEntity.Read());
                }
            }
        }

        private void ReadPipingFailureMechanism(AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var pipingFailureMechanismEntity = FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.Piping);
            if (pipingFailureMechanismEntity != null)
            {
                var failureMechanism = pipingFailureMechanismEntity.ReadAsPipingFailureMechanism(collector);

                var pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
                pipingFailureMechanism.StochasticSoilModels.AddRange(failureMechanism.StochasticSoilModels);
                pipingFailureMechanism.IsRelevant = failureMechanism.IsRelevant;
                pipingFailureMechanism.StorageId = failureMechanism.StorageId;
                foreach (var failureMechanismSection in failureMechanism.Sections)
                {
                    pipingFailureMechanism.AddSection(failureMechanismSection);
                }
            }
        }

        private void ReadFailureMechanismPlaceholders(AssessmentSection assessmentSection)
        {
            ReadFailureMechanismPlaceholder(FailureMechanismType.MacrostabilityInwards, assessmentSection.MacrostabilityInwards);
            ReadFailureMechanismPlaceholder(FailureMechanismType.StructureHeight, assessmentSection.Overtopping);
            ReadFailureMechanismPlaceholder(FailureMechanismType.ReliabilityClosingOfStructure, assessmentSection.Closing);
            ReadFailureMechanismPlaceholder(FailureMechanismType.StrengthAndStabilityPointConstruction, assessmentSection.FailingOfConstruction);
            ReadFailureMechanismPlaceholder(FailureMechanismType.StabilityStoneRevetment, assessmentSection.StoneRevetment);
            ReadFailureMechanismPlaceholder(FailureMechanismType.WaveImpactOnAsphaltRevetment, assessmentSection.AsphaltRevetment);
            ReadFailureMechanismPlaceholder(FailureMechanismType.GrassRevetmentErosionOutwards, assessmentSection.GrassRevetment);
            ReadFailureMechanismPlaceholder(FailureMechanismType.DuneErosion, assessmentSection.DuneErosion);
        }

        private void ReadFailureMechanismPlaceholder(FailureMechanismType failureMechanismType, FailureMechanismPlaceholder failureMechanismPlaceholder)
        {
            var failureMechanismEntity = FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) failureMechanismType);
            if (failureMechanismEntity != null)
            {
                var failureMechanism = failureMechanismEntity.ReadAsFailureMechanismPlaceholder();

                failureMechanismPlaceholder.StorageId = failureMechanism.StorageId;
                failureMechanismPlaceholder.IsRelevant = failureMechanism.IsRelevant;
                foreach (var failureMechanismSection in failureMechanism.Sections)
                {
                    failureMechanismPlaceholder.AddSection(failureMechanismSection);
                }
            }
        }
    }
}