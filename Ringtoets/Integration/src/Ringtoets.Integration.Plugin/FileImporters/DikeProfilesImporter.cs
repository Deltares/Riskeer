﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;
using Ringtoets.Integration.Plugin.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports point shapefiles containing dike profile locations and text files containing dike schematizations.
    /// </summary>
    public class DikeProfilesImporter : ProfilesImporter<ObservableList<DikeProfile>>
    {
        private readonly ObservableList<DikeProfile> importTarget;

        /// <summary>
        /// Creates a new instance of <see cref="DikeProfilesImporter"/>.
        /// </summary>
        /// <param name="importTarget">The dike profiles to import on.</param>
        /// <param name="referenceLine">The reference line used to check if the <see cref="DikeProfile"/>
        /// objects found in the file are intersecting it.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, 
        /// <paramref name="filePath"/> or <paramref name="importTarget"/> is <c>null</c>.</exception>
        public DikeProfilesImporter(ObservableList<DikeProfile> importTarget, ReferenceLine referenceLine, string filePath)
            : base(referenceLine, filePath, importTarget)
        {
            this.importTarget = importTarget;
        }

        protected override void CreateProfiles(ReadResult<DikeProfileLocation> importDikeProfilesResult,
                                               ReadResult<DikeProfileData> importDikeProfileDataResult)
        {
            IEnumerable<DikeProfile> importedDikeProfiles = CreateDikeProfiles(importDikeProfilesResult.ImportedItems,
                                                                               importDikeProfileDataResult.ImportedItems);

            foreach (DikeProfile dikeProfile in importedDikeProfiles)
            {
                importTarget.Add(dikeProfile);
            }
        }

        protected override void HandleUserCancellingImport()
        {
            log.Info(Resources.DikeProfilesImporter_HandleUserCancellingImport_dikeprofile_import_aborted);
            Canceled = false;
        }

        private IEnumerable<DikeProfile> CreateDikeProfiles(IEnumerable<DikeProfileLocation> dikeProfileLocationCollection,
                                                            ICollection<DikeProfileData> dikeProfileDataCollection)
        {
            var dikeProfiles = new List<DikeProfile>();
            foreach (DikeProfileLocation dikeProfileLocation in dikeProfileLocationCollection)
            {
                string id = dikeProfileLocation.Id;

                var dikeProfileData = GetMatchingDikeProfileData(dikeProfileDataCollection, id);
                if (dikeProfileData == null)
                {
                    log.ErrorFormat(Resources.DikeProfilesImporter_GetMatchingDikeProfileData_no_dikeprofiledata_for_location_0_, id);
                }
                else
                {
                    DikeProfile dikeProfile = CreateDikeProfile(dikeProfileLocation, dikeProfileData);
                    dikeProfiles.Add(dikeProfile);
                }
            }
            return dikeProfiles;
        }

        private static DikeProfile CreateDikeProfile(DikeProfileLocation dikeProfileLocation, DikeProfileData dikeProfileData)
        {
            var dikeProfile = new DikeProfile(dikeProfileLocation.Point, dikeProfileData.DikeGeometry,
                                              dikeProfileData.ForeshoreGeometry.Select(fg => fg.Point).ToArray(),
                                              CreateBreakWater(dikeProfileData),
                                              new DikeProfile.ConstructionProperties
                                              {
                                                  Name = dikeProfileData.Id,
                                                  X0 = dikeProfileLocation.Offset,
                                                  Orientation = dikeProfileData.Orientation,
                                                  DikeHeight = dikeProfileData.DikeHeight
                                              });

            return dikeProfile;
        }
    }
}