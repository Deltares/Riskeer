using System;
using Application.Ringtoets.Storage.Create;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class AssessmentSectionCreateExtensions
    {
        public static AssessmentSectionEntity Create(this AssessmentSection section, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new AssessmentSectionEntity
            {
                Name = section.Name,
                Composition = (short) section.Composition
            };

            CreatePipingFailureMechanism(section, entity, collector);
            CreateHydraulicDatabase(section, entity, collector);
            CreateReferenceLine(section, entity);
            CreateFailureMechanismPlaceHolders(section, entity, collector);

            collector.Add(entity, section);
            return entity;
        }

        private static void CreateFailureMechanismPlaceHolders(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            entity.FailureMechanismEntities.Add(section.MacrostabilityInwards.Create(FailureMechanismType.MacrostabilityInwards, collector));
            entity.FailureMechanismEntities.Add(section.Overtopping.Create(FailureMechanismType.StructureHeight, collector));
            entity.FailureMechanismEntities.Add(section.Closing.Create(FailureMechanismType.ReliabilityClosingOfStructure, collector));
            entity.FailureMechanismEntities.Add(section.FailingOfConstruction.Create(FailureMechanismType.StrengthAndStabilityPointConstruction, collector));
            entity.FailureMechanismEntities.Add(section.StoneRevetment.Create(FailureMechanismType.StabilityStoneRevetment, collector));
            entity.FailureMechanismEntities.Add(section.AsphaltRevetment.Create(FailureMechanismType.WaveImpactOnAsphaltRevetment, collector));
            entity.FailureMechanismEntities.Add(section.GrassRevetment.Create(FailureMechanismType.GrassRevetmentErosionOutwards, collector));
            entity.FailureMechanismEntities.Add(section.DuneErosion.Create(FailureMechanismType.DuneErosion, collector));
        }

        private static void CreatePipingFailureMechanism(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            entity.FailureMechanismEntities.Add(section.PipingFailureMechanism.Create(collector));
        }

        private static void CreateReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
        {
            if (section.ReferenceLine != null)
            {
                var i = 0;
                foreach (var point2D in section.ReferenceLine.Points)
                {
                    entity.ReferenceLinePointEntities.Add(point2D.CreateReferenceLinePoint(i++));
                }
            }
        }

        private static void CreateHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                foreach (var hydraulicBoundaryLocation in section.HydraulicBoundaryDatabase.Locations)
                {
                    entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(collector));
                }
            }
        }
    }
}