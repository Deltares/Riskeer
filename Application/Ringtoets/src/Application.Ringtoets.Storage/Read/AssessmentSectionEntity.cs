using System;
using System.Linq;
using Application.Ringtoets.Storage.Read;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class AssessmentSectionEntity
    {
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

                assessmentSection.PipingFailureMechanism.StochasticSoilModels.AddRange(failureMechanism.StochasticSoilModels);
                assessmentSection.PipingFailureMechanism.IsRelevant = failureMechanism.IsRelevant;
                assessmentSection.PipingFailureMechanism.StorageId = failureMechanism.StorageId;
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
                failureMechanismPlaceholder.StorageId = failureMechanismEntity.FailureMechanismEntityId;
                failureMechanismPlaceholder.IsRelevant = Convert.ToBoolean(failureMechanismEntity.IsRelevant);
            }
        }
    }
}