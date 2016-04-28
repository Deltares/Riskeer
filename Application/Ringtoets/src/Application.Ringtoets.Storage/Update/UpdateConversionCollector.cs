using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils;

namespace Application.Ringtoets.Storage.Update
{
    public class UpdateConversionCollector : CreateConversionCollector
    {
        private readonly HashSet<ProjectEntity> projects = new HashSet<ProjectEntity>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly HashSet<AssessmentSectionEntity> assessmentSections = new HashSet<AssessmentSectionEntity>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly HashSet<FailureMechanismEntity> failureMechanisms = new HashSet<FailureMechanismEntity>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly HashSet<HydraulicLocationEntity> hydraulicLocations = new HashSet<HydraulicLocationEntity>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly HashSet<StochasticSoilModelEntity> stochasticSoilModels = new HashSet<StochasticSoilModelEntity>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly HashSet<StochasticSoilProfileEntity> stochasticSoilProfiles = new HashSet<StochasticSoilProfileEntity>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly HashSet<SoilProfileEntity> soilProfiles = new HashSet<SoilProfileEntity>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly HashSet<SoilLayerEntity> soilLayers = new HashSet<SoilLayerEntity>(new ReferenceEqualityComparer<SoilLayerEntity>());

        internal void Update(ProjectEntity entity)
        {
            projects.Add(entity);
        }

        internal void Update(AssessmentSectionEntity entity)
        {
            assessmentSections.Add(entity);
        }

        internal void Update(FailureMechanismEntity entity)
        {
            failureMechanisms.Add(entity);
        }

        internal void Update(HydraulicLocationEntity entity)
        {
            hydraulicLocations.Add(entity);
        }

        internal void Update(StochasticSoilModelEntity entity)
        {
            stochasticSoilModels.Add(entity);
        }

        internal void Update(StochasticSoilProfileEntity entity)
        {
            stochasticSoilProfiles.Add(entity);
        }

        internal void Update(SoilProfileEntity entity)
        {
            soilProfiles.Add(entity);
        }

        internal void Update(SoilLayerEntity entity)
        {
            soilLayers.Add(entity);
        }

        public void RemoveUntouched(IRingtoetsEntities dbContext)
        {
            var projectEntities = dbContext.ProjectEntities;
            projectEntities.RemoveRange(projectEntities.Local.Except(projects));

            var assessmentSectionEntities = dbContext.AssessmentSectionEntities;
            assessmentSectionEntities.RemoveRange(assessmentSectionEntities.Local.Except(assessmentSections));

            var failureMechanismEntities = dbContext.FailureMechanismEntities;
            failureMechanismEntities.RemoveRange(failureMechanismEntities.Local.Except(failureMechanisms));

            var hydraulicLocationEntities = dbContext.HydraulicLocationEntities;
            hydraulicLocationEntities.RemoveRange(hydraulicLocationEntities.Local.Except(hydraulicLocations));

            var stochasticSoilModelEntities = dbContext.StochasticSoilModelEntities;
            stochasticSoilModelEntities.RemoveRange(stochasticSoilModelEntities.Local.Except(stochasticSoilModels));

            var stochasticSoilProfileEntities = dbContext.StochasticSoilProfileEntities;
            stochasticSoilProfileEntities.RemoveRange(stochasticSoilProfileEntities.Local.Except(stochasticSoilProfiles));

            var soilProfileEntities = dbContext.SoilProfileEntities;
            soilProfileEntities.RemoveRange(soilProfileEntities.Local.Except(soilProfiles));

            var soilLayerEntities = dbContext.SoilLayerEntities;
            soilLayerEntities.RemoveRange(soilLayerEntities.Local.Except(soilLayers));
        }
    }
}