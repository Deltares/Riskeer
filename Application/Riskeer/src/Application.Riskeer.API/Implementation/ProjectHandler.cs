using Application.Riskeer.API.Interfaces;
using System;
using System.IO;
using System.Linq;
using Application.Riskeer.API.ErrorHandling;
using Core.Common.Base.Storage;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms;
using Riskeer.Storage.Core;

namespace Application.Riskeer.API.Implementation
{
    public class ProjectHandler : IProjectHandler
    {
        private readonly IStoreProject storage;

        public ProjectHandler()
        {
            storage = new StorageSqLite();
        }

        public ProjectApi OpenProject(string fileLocation)
        {
            // TODO: Check file location
            // TODO: Check version and migration etc.
            try
            {
                return new ProjectApi(storage.LoadProject(fileLocation) as RiskeerProject);
            }
            catch (ArgumentException e)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.InvalidFilePath, e);
            }
            catch (CouldNotConnectException e)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile, e);
            }
            catch (StorageValidationException e)
            {
                // bij uitlezen connectie, validatie Riskeer versie gegeven bestand
                throw new RiskeerApiException(RiskeerApiExceptionType.InvalidRiskeerFile, e);
            }
            catch (StorageException e)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.InvalidRiskeerFile, e);
            }
            catch
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.InvalidRiskeerFile);
            }
        }

        public void SaveProject(ProjectApi project, string filePath, bool overwrite = false)
        {
            var riskeerProject = project?.RiskeerProject;
            if (riskeerProject == null)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.EmptyRiskeerProject);
            }

            // TODO: Check destination location for right extension etc.

            if (File.Exists(filePath) && !overwrite)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.FileAlreadyExists);
            }

            storage.StageProject(riskeerProject);
            try
            {
                storage.SaveProjectAs(filePath);
            }
            catch (InvalidOperationException e)
            {
                // Staging failed
            }
            catch (StorageException e)
            {
                // Invalid file or error writing file
            }
        }

        public IAssessmentSectionApi AddAssessmentSection(ProjectApi project, string sectionId, bool useLowerBoundaryNorm)
        {
            // TODO: This introduces a reference to a Forms project. This is not necessary if RiskeerSettingsHelper is placed in another (data?) project
            var referenceLineMetas = new ReferenceLineMetaImporter(RiskeerSettingsHelper.GetCommonDocumentsRiskeerShapeFileDirectory())
                .GetReferenceLineMetas();

            var meta = referenceLineMetas.FirstOrDefault(r => r.AssessmentSectionId == sectionId);
            if (meta == null)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.ReferenceLineNotFound);
            }

            double metaLowerLimitValue = 1.0/meta.LowerLimitValue;
            double metaSignalingValue = 1.0 / meta.SignalingValue ?? metaLowerLimitValue;

            AssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSection(meta,
                                                                                                  metaLowerLimitValue,
                                                                                                  metaSignalingValue,
                                                                                                  useLowerBoundaryNorm ? NormType.LowerLimit : NormType.Signaling);
            project.RiskeerProject.AssessmentSections.Add(assessmentSection);
            project.RiskeerProject.NotifyObservers();

            return new AssessmentSectionApi(assessmentSection);
        }
    }
}
