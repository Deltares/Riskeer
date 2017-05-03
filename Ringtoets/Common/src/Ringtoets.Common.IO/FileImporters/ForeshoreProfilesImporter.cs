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
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.DikeProfiles;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Imports point shapefiles containing foreshore locations and text file containing the foreshore schematizations.
    /// </summary>
    public class ForeshoreProfilesImporter : ProfilesImporter<ObservableList<ForeshoreProfile>>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ForeshoreProfilesImporter"/>.
        /// </summary>
        /// <param name="importTarget">The foreshore profiles to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="ForeshoreProfile"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public ForeshoreProfilesImporter(ObservableList<ForeshoreProfile> importTarget, ReferenceLine referenceLine, string filePath)
            : base(referenceLine, filePath, importTarget, new ImportMessageProvider()) {}

        protected override void CreateProfiles(ReadResult<ProfileLocation> importProfileLocationResult,
                                               ReadResult<DikeProfileData> importDikeProfileDataResult)
        {
            IEnumerable<ForeshoreProfile> importedForeshoreProfiles =
                CreateForeshoreProfiles(importProfileLocationResult.Items, importDikeProfileDataResult.Items);

            foreach (ForeshoreProfile foreshoreProfile in importedForeshoreProfiles)
            {
                if (ImportTarget.Select(fp => fp.Id).Contains(foreshoreProfile.Id))
                {
                    Log.WarnFormat(Resources.ForeshoreProfilesImporter_CreateProfiles_ForeshoreProfile_0_already_imported, foreshoreProfile.Id);
                }
                else
                {
                    ImportTarget.Add(foreshoreProfile);
                }
            }
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.ForeshoreProfilesImporter_HandleUserCancelingImport_foreshoreprofile_import_aborted);
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

        private IEnumerable<ForeshoreProfile> CreateForeshoreProfiles(ICollection<ProfileLocation> dikeProfileLocationCollection,
                                                                      ICollection<DikeProfileData> dikeProfileDataCollection)
        {
            var foreshoreProfiles = new List<ForeshoreProfile>();
            foreach (ProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                DikeProfileData dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData == null)
                {
                    Log.ErrorFormat(Resources.ForeshoreProfilesImporter_GetMatchingForeshoreProfileData_no_foreshoreprofiledata_for_location_0_, id);
                }
                else
                {
                    ForeshoreProfile foreshoreProfile = CreateForeshoreProfile(dikeProfileLocation, dikeProfileData);
                    foreshoreProfiles.Add(foreshoreProfile);
                }
            }
            return foreshoreProfiles;
        }

        private static ForeshoreProfile CreateForeshoreProfile(ProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            var foreshoreProfile = new ForeshoreProfile(dikeProfileLocation.Point,
                                                        dikeProfileData.ForeshoreGeometry.Select(fg => fg.Point).ToArray(),
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