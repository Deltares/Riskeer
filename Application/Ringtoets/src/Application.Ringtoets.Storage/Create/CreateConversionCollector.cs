using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// This class can be used to keep track of create operations on a database. When all operations have been performed, then 
    /// the collected information can be used, for example, to transfer the ids assigned to the created database instances
    /// back to the data model.
    /// </summary>
    public class CreateConversionCollector
    {
        private readonly Dictionary<ProjectEntity, Project> projects = new Dictionary<ProjectEntity, Project>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly Dictionary<AssessmentSectionEntity, AssessmentSection> assessmentSections = new Dictionary<AssessmentSectionEntity, AssessmentSection>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly Dictionary<FailureMechanismEntity, FailureMechanismBase> failureMechanisms = new Dictionary<FailureMechanismEntity, FailureMechanismBase>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicLocations = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SoilLayerEntity, PipingSoilLayer> soilLayers = new Dictionary<SoilLayerEntity, PipingSoilLayer>(new ReferenceEqualityComparer<SoilLayerEntity>());

        internal void Add(ProjectEntity entity, Project model)
        {
            Add(projects, entity, model);
        }

        internal void Add(AssessmentSectionEntity entity, AssessmentSection model)
        {
            Add(assessmentSections, entity, model);
        }

        internal void Add(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Add(hydraulicLocations, entity, model);
        }

        internal void Add(FailureMechanismEntity entity, FailureMechanismBase model)
        {
            Add(failureMechanisms, entity, model);
        }

        internal void Add(StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            Add(stochasticSoilModels, entity, model);
        }

        internal void Add(StochasticSoilProfileEntity entity, StochasticSoilProfile model)
        {
            Add(stochasticSoilProfiles, entity, model);
        }

        internal void Add(SoilProfileEntity entity, PipingSoilProfile model)
        {
            Add(soilProfiles, entity, model);
        }

        internal void Add(SoilLayerEntity entity, PipingSoilLayer model)
        {
            Add(soilLayers, entity, model);
        }

        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(soilProfiles, model);
        }

        internal SoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(soilProfiles, model);
        }

        internal void TransferIds()
        {
            foreach (var entity in projects.Keys)
            {
                projects[entity].StorageId = entity.ProjectEntityId;
            }

            foreach (var entity in failureMechanisms.Keys)
            {
                failureMechanisms[entity].StorageId = entity.FailureMechanismEntityId;
            }

            foreach (var entity in assessmentSections.Keys)
            {
                assessmentSections[entity].StorageId = entity.AssessmentSectionEntityId;
            }

            foreach (var entity in hydraulicLocations.Keys)
            {
                hydraulicLocations[entity].StorageId = entity.HydraulicLocationEntityId;
            }

            foreach (var entity in stochasticSoilModels.Keys)
            {
                stochasticSoilModels[entity].StorageId = entity.StochasticSoilModelEntityId;
            }

            foreach (var entity in stochasticSoilProfiles.Keys)
            {
                stochasticSoilProfiles[entity].StorageId = entity.StochasticSoilProfileEntityId;
            }

            foreach (var entity in soilProfiles.Keys)
            {
                soilProfiles[entity].StorageId = entity.SoilProfileEntityId;
            }

            foreach (var entity in soilLayers.Keys)
            {
                soilLayers[entity].StorageId = entity.SoilLayerEntityId;
            }
        }

        private bool ContainsValue<T,U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.ContainsValue(model);
        }

        private void Add<T, U>(Dictionary<T, U> collection, T entity, U model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            collection[entity] = model;
        }

        private T Get<T, U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.Keys.Single(k => ReferenceEquals(collection[k], model));
        }
    }
}