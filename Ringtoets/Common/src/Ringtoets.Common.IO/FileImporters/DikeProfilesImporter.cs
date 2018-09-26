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
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.DikeProfiles;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Imports point shapefiles containing dike profile locations and text files containing dike schematizations.
    /// </summary>
    public class DikeProfilesImporter : ProfilesImporter<DikeProfileCollection>
    {
        private readonly IDikeProfileUpdateDataStrategy dikeProfileUpdateDataStrategy;
        private IEnumerable<IObservable> updatedInstances;

        /// <summary>
        /// Creates a new instance of <see cref="DikeProfilesImporter"/>.
        /// </summary>
        /// <param name="importTarget">The dike profiles to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="DikeProfile"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="dikeProfileUpdateStrategy">The strategy to update the dike profiles 
        /// with the imported data.</param>
        /// <param name="messageProvider">The message provider to provide the messages during the importer action.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public DikeProfilesImporter(DikeProfileCollection importTarget, ReferenceLine referenceLine,
                                    string filePath,
                                    IDikeProfileUpdateDataStrategy dikeProfileUpdateStrategy,
                                    IImporterMessageProvider messageProvider)
            : base(referenceLine, filePath, importTarget, messageProvider, RingtoetsCommonDataResources.DikeProfileCollection_TypeDescriptor)
        {
            if (dikeProfileUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(dikeProfileUpdateStrategy));
            }

            dikeProfileUpdateDataStrategy = dikeProfileUpdateStrategy;
            updatedInstances = Enumerable.Empty<IObservable>();
        }

        protected override void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult,
                                               ReadResult<DikeProfileData> importDikeProfileDataResult)
        {
            IEnumerable<DikeProfile> importedDikeProfiles = CreateDikeProfiles(importProfileLocationResult.Items,
                                                                               importDikeProfileDataResult.Items);

            updatedInstances = dikeProfileUpdateDataStrategy.UpdateDikeProfilesWithImportedData(importedDikeProfiles,
                                                                                                FilePath);
        }

        protected override void LogImportCanceledMessage()
        {
            string logMessage = MessageProvider.GetCancelledLogMessageText(
                RingtoetsCommonDataResources.DikeProfileCollection_TypeDescriptor);
            Log.Info(logMessage);
        }

        protected override bool DikeProfileDataIsValid(DikeProfileData data, string prflFilePath)
        {
            if (data.DikeGeometry.Any())
            {
                return true;
            }

            Log.WarnFormat(Resources.DikeProfilesImporter_No_dike_geometry_file_0_skipped, prflFilePath);
            return false;
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable updatedInstance in updatedInstances)
            {
                updatedInstance.NotifyObservers();
            }
        }

        private static IEnumerable<DikeProfile> CreateDikeProfiles(IEnumerable<ProfileLocation> dikeProfileLocationCollection,
                                                                   IEnumerable<DikeProfileData> dikeProfileDataCollection)
        {
            var dikeProfiles = new List<DikeProfile>();
            foreach (ProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                DikeProfileData dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData == null)
                {
                    string message = string.Format(Resources.DikeProfilesImporter_GetMatchingDikeProfileData_no_dikeprofiledata_for_location_0_, id);
                    throw new CriticalFileReadException(message);
                }

                DikeProfile dikeProfile = CreateDikeProfile(dikeProfileLocation, dikeProfileData);
                dikeProfiles.Add(dikeProfile);
            }

            return dikeProfiles;
        }

        private static DikeProfile CreateDikeProfile(ProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            return new DikeProfile(dikeProfileLocation.Point, dikeProfileData.DikeGeometry,
                                   dikeProfileData.ForeshoreGeometry.Select(fg => fg.Point),
                                   CreateBreakWater(dikeProfileData),
                                   new DikeProfile.ConstructionProperties
                                   {
                                       Id = dikeProfileData.Id,
                                       Name = dikeProfileLocation.Name,
                                       X0 = dikeProfileLocation.Offset,
                                       Orientation = dikeProfileData.Orientation,
                                       DikeHeight = dikeProfileData.DikeHeight
                                   });
        }
    }
}