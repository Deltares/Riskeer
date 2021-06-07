using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Riskeer.API.ErrorHandling;
using Application.Riskeer.API.Interfaces;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.HydraRing;
using Riskeer.Common.IO.ReferenceLines;
using Riskeer.Common.Service;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.IO.Exporters;
using Riskeer.Integration.Service;

namespace Application.Riskeer.API.Implementation
{
    public class AssessmentSectionHandler : IAssessmentSectionHandler
    {
        public IAssessmentSectionApi FindAssessmentSection(IProjectApi project, string name)
        {
            var projectApi = project as ProjectApi;
            if (projectApi == null)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.NoProjectSpecified);
            }

            IAssessmentSectionApi assessmentSection = projectApi.AssessmentSectionsDictionary.Values.FirstOrDefault(p => p.Name == name);
            if (assessmentSection == null)
            {
                throw new RiskeerApiException(RiskeerApiExceptionType.AssessmentSectionNotFound);
            }

            return assessmentSection;
        }

        public void ImportReferenceLine(IAssessmentSectionApi assessmentSectionApi, string shapeFileLocation)
        {
            AssessmentSectionApi sectionApi = GetAssessmentSectionApi(assessmentSectionApi);

            ReadResult<ReferenceLine> readResult = ReadReferenceLine(shapeFileLocation);
            if (readResult.CriticalErrorOccurred)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            var originalReferenceLine = sectionApi.AssessmentSection.ReferenceLine;
            if (originalReferenceLine == null)
            {
                throw new ArgumentNullException(nameof(originalReferenceLine));
            }

            var newReferenceLine = readResult.Items.First();
            if (newReferenceLine == null)
            {
                throw new ArgumentNullException(nameof(newReferenceLine));
            }

            ClearResults results = RiskeerDataSynchronizationService.ClearReferenceLineDependentData(sectionApi.AssessmentSection);
            originalReferenceLine.SetGeometry(newReferenceLine.Points);
            foreach (IObservable observable in results.ChangedObjects)
            {
                observable.NotifyObservers();
            }
        }

        private static AssessmentSectionApi GetAssessmentSectionApi(IAssessmentSectionApi assessmentSectionApi)
        {
            var sectionApi = assessmentSectionApi as AssessmentSectionApi;
            if (sectionApi == null)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            return sectionApi;
        }

        private ReadResult<ReferenceLine> ReadReferenceLine(string filePath)
        {
            try
            {
                return new ReadResult<ReferenceLine>(false)
                {
                    Items = new[]
                    {
                        new ReferenceLineReader().ReadReferenceLine(filePath)
                    }
                };
            }
            catch (ArgumentException e)
            {
                //return HandleCriticalFileReadError(e);
            }
            catch (CriticalFileReadException e)
            {
                //TODO: return HandleCriticalFileReadError(e);
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
            throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
        }


        public void ExportReferenceLine(IAssessmentSectionApi assessmentSectionApi, string destinationFilePath)
        {
            AssessmentSectionApi sectionApi = GetAssessmentSectionApi(assessmentSectionApi);
            var exporter = new ReferenceLineExporter(sectionApi.AssessmentSection.ReferenceLine,sectionApi.AssessmentSection.Id,destinationFilePath);
            exporter.Export();
        }

        public void CoupleToHydraulicDatabase(IAssessmentSectionApi assessmentSectionApi, string databaseFileLocation)
        {
            var readHydraulicBoundaryDatabaseResult = ReadHydraulicBoundaryDatabaseFile(databaseFileLocation);
            if (readHydraulicBoundaryDatabaseResult.CriticalErrorOccurred)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = readHydraulicBoundaryDatabaseResult.Items.Single();

            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(databaseFileLocation), "hlcd.sqlite");

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase(
                hlcdFilePath, readHydraulicBoundaryDatabase.TrackId);

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = readHydraulicLocationConfigurationDatabaseResult.Items.Single();
            IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> hydraulicLocationConfigurationDatabaseSettings =
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings;
            if (hydraulicLocationConfigurationDatabaseSettings != null && hydraulicLocationConfigurationDatabaseSettings.Count() != 1)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            if (readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure
                && !File.Exists(HydraulicBoundaryDatabaseHelper.GetPreprocessorClosureFilePath(hlcdFilePath)))
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            ReadResult<IEnumerable<long>> readExcludedLocationsResult = ReadExcludedLocations(databaseFileLocation);

            if (readExcludedLocationsResult.CriticalErrorOccurred)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }

            //HydraulicBoundaryDatabaseImporter.AddHydraulicBoundaryDatabaseToDataModel
            throw new NotImplementedException();
        }

        /*private void AddHydraulicBoundaryDatabaseToDataModel(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                                             ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                             IEnumerable<long> excludedLocationIds)
        {
            changedObservables.AddRange(updateHandler.Update(ImportTarget, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                             excludedLocationIds, FilePath, GetHlcdFilePath()));
        }*/

        private ReadResult<IEnumerable<long>> ReadExcludedLocations(string filePath)
        {
            string settingsFilePath = HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(filePath);
            try
            {
                using (var reader = new HydraRingSettingsDatabaseReader(settingsFilePath))
                {
                    return ReadExcludedLocations(reader);
                }
            }
            catch (CriticalFileReadException e)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
        }

        private ReadResult<IEnumerable<long>> ReadExcludedLocations(HydraRingSettingsDatabaseReader reader)
        {
            try
            {
                return new ReadResult<IEnumerable<long>>(false)
                {
                    Items = new[]
                    {
                        reader.ReadExcludedLocations().ToArray()
                    }
                };
            }
            catch (CriticalFileReadException e)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(string hlcdFilePath, long trackId)
        {
            try
            {
                using (var reader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
                {
                    return ReadHydraulicLocationConfigurationDatabase(trackId, reader);
                }
            }
            catch (CriticalFileReadException)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(long trackId, HydraulicLocationConfigurationDatabaseReader reader)
        {
            try
            {
                return new ReadResult<ReadHydraulicLocationConfigurationDatabase>(false)
                {
                    Items = new[]
                    {
                        reader.Read(trackId)
                    }
                };
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
        }

        private static ReadResult<ReadHydraulicBoundaryDatabase> ReadHydraulicBoundaryDatabaseFile(string databaseFileLocation)
        {
            try
            {
                using (var reader = new HydraulicBoundaryDatabaseReader(databaseFileLocation))
                {
                    return new ReadResult<ReadHydraulicBoundaryDatabase>(false)
                    {
                        Items = new[]
                        {
                            reader.Read()
                        }
                    };
                }
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
                //return HandleCriticalFileReadError<ReadHydraulicBoundaryDatabase>(e);
            }
        }

        public void ExportAssemblyResults(IAssessmentSectionApi assessmentSectionApi, string filePath)
        {
            var section = (assessmentSectionApi as AssessmentSectionApi)?.AssessmentSection;
            if (section == null)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
            var exporter = new AssemblyExporter(section,filePath);
        }

        public void ExportHydraulicBoundaryConditions(IAssessmentSectionApi assessmentSectionApi, string filePath)
        {
            var section = (assessmentSectionApi as AssessmentSectionApi)?.AssessmentSection;
            if (section == null)
            {
                // TODO:
                throw new RiskeerApiException(RiskeerApiExceptionType.CouldNotConnectToFile);
            }
            var exporter = new HydraulicBoundaryLocationsExporter(section,filePath);
            exporter.Export();
        }
    }
}
