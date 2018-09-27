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
    /// Imports point shapefiles containing foreshore locations and text file containing the foreshore schematizations.
    /// </summary>
    public class ForeshoreProfilesImporter : ProfilesImporter<ForeshoreProfileCollection>
    {
        private readonly IForeshoreProfileUpdateDataStrategy foreshoreProfileUpdateStrategy;
        private IEnumerable<IObservable> affectedObjects;

        /// <summary>
        /// Creates a new instance of <see cref="ForeshoreProfilesImporter"/>.
        /// </summary>
        /// <param name="importTarget">The foreshore profiles to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="ForeshoreProfile"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="foreshoreProfileUpdateStrategy">The strategy to update the 
        /// foreshore profiles with the imported data.</param>
        /// <param name="messageProvider">The message provider to provide messages during the import.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ForeshoreProfilesImporter(ForeshoreProfileCollection importTarget,
                                         ReferenceLine referenceLine,
                                         string filePath,
                                         IForeshoreProfileUpdateDataStrategy foreshoreProfileUpdateStrategy,
                                         IImporterMessageProvider messageProvider)
            : base(referenceLine, filePath, importTarget, messageProvider, RingtoetsCommonDataResources.ForeshoreProfileCollection_TypeDescriptor)
        {
            if (foreshoreProfileUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfileUpdateStrategy));
            }

            this.foreshoreProfileUpdateStrategy = foreshoreProfileUpdateStrategy;
            affectedObjects = Enumerable.Empty<IObservable>();
        }

        protected override void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult,
                                               ReadResult<DikeProfileData> importDikeProfileDataResult)
        {
            IEnumerable<ForeshoreProfile> importedForeshoreProfiles =
                CreateForeshoreProfiles(importProfileLocationResult.Items, importDikeProfileDataResult.Items);

            affectedObjects = foreshoreProfileUpdateStrategy.UpdateForeshoreProfilesWithImportedData(importedForeshoreProfiles,
                                                                                                     FilePath);
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        protected override void LogImportCanceledMessage()
        {
            string logMessage =
                MessageProvider.GetCancelledLogMessageText(RingtoetsCommonDataResources.ForeshoreProfileCollection_TypeDescriptor);
            Log.Info(logMessage);
        }

        protected override bool DikeProfileDataIsValid(DikeProfileData data, string prflFilePath)
        {
            if (data.DamType != DamType.None || data.ForeshoreGeometry.Any())
            {
                return true;
            }

            Log.WarnFormat(Resources.ForeshoreProfilesImporter_No_dam_no_foreshore_geometry_file_0_skipped, prflFilePath);
            return false;
        }

        private static IEnumerable<ForeshoreProfile> CreateForeshoreProfiles(IEnumerable<ProfileLocation> dikeProfileLocationCollection,
                                                                             IEnumerable<DikeProfileData> dikeProfileDataCollection)
        {
            var foreshoreProfiles = new List<ForeshoreProfile>();
            foreach (ProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                DikeProfileData dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData == null)
                {
                    string message = string.Format(Resources.ForeshoreProfilesImporter_GetMatchingForeshoreProfileData_no_foreshoreprofiledata_for_location_0_, id);
                    throw new CriticalFileReadException(message);
                }

                ForeshoreProfile foreshoreProfile = CreateForeshoreProfile(dikeProfileLocation, dikeProfileData);
                foreshoreProfiles.Add(foreshoreProfile);
            }

            return foreshoreProfiles;
        }

        private static ForeshoreProfile CreateForeshoreProfile(ProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            var foreshoreProfile = new ForeshoreProfile(dikeProfileLocation.Point,
                                                        dikeProfileData.ForeshoreGeometry.Select(fg => fg.Point),
                                                        CreateBreakWater(dikeProfileData),
                                                        new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = dikeProfileData.Id,
                                                            Name = dikeProfileLocation.Name,
                                                            X0 = dikeProfileLocation.Offset,
                                                            Orientation = dikeProfileData.Orientation
                                                        });
            return foreshoreProfile;
        }
    }
}