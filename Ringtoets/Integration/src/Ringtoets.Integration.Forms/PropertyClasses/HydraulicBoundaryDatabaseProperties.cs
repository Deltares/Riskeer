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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Forms.Editors;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryDatabaseProperties : ObjectProperties<HydraulicBoundaryDatabase>
    {
        private const int hrdFilePathPropertyIndex = 0;
        private const int hlcdFilePathPropertyIndex = 1;
        private const int scenarioNamePropertyIndex = 2;
        private const int yearPropertyIndex = 3;
        private const int scopePropertyIndex = 4;
        private const int seaLevelPropertyIndex = 5;
        private const int riverDischargePropertyIndex = 6;
        private const int lakeLevelPropertyIndex = 7;
        private const int windDirectionPropertyIndex = 8;
        private const int windSpeedPropertyIndex = 9;
        private const int commentPropertyIndex = 10;
        private const int usePreprocessorPropertyIndex = 11;
        private const int preprocessorDirectoryPropertyIndex = 12;

        private readonly IHydraulicLocationConfigurationDatabaseImportHandler hydraulicLocationConfigurationDatabaseImportHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to show the properties for.</param>
        /// <param name="hydraulicLocationConfigurationDatabaseImportHandler">The handler to update the hydraulic location configuration settings.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseProperties(HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                   IHydraulicLocationConfigurationDatabaseImportHandler hydraulicLocationConfigurationDatabaseImportHandler)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (hydraulicLocationConfigurationDatabaseImportHandler == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationDatabaseImportHandler));
            }

            this.hydraulicLocationConfigurationDatabaseImportHandler = hydraulicLocationConfigurationDatabaseImportHandler;
            Data = hydraulicBoundaryDatabase;
        }

        [PropertyOrder(hrdFilePathPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_FilePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_FilePath_Description))]
        public string HrdFilePath
        {
            get
            {
                return data.IsLinked() ? data.FilePath : string.Empty;
            }
        }

        [PropertyOrder(hlcdFilePathPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_FilePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_FilePath_Description))]
        [Editor(typeof(HlcdFileNameEditor), typeof(UITypeEditor))]
        public string HlcdFilePath
        {
            get
            {
                return data.IsLinked() ? data.HydraulicLocationConfigurationSettings.FilePath : string.Empty;
            }
            set
            {
                hydraulicLocationConfigurationDatabaseImportHandler.ImportHydraulicLocationConfigurationSettings(
                    data.HydraulicLocationConfigurationSettings, value);
            }
        }

        [PropertyOrder(hlcdFilePathPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_FilePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_FilePath_Description))]
        public string HlcdFilePathReadOnly
        {
            get
            {
                return data.IsLinked() ? data.HydraulicLocationConfigurationSettings.FilePath : string.Empty;
            }
        }

        [PropertyOrder(scenarioNamePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_ScenarioName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_ScenarioName_Description))]
        public string ScenarioName
        {
            get
            {
                return data.IsLinked() ? data.HydraulicLocationConfigurationSettings.ScenarioName : string.Empty;
            }
        }

        [PropertyOrder(yearPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Year_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Year_Description))]
        public int Year
        {
            get
            {
                return data.HydraulicLocationConfigurationSettings.Year;
            }
        }

        [PropertyOrder(scopePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Scope_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Scope_Description))]
        public string Scope
        {
            get
            {
                return data.IsLinked() ? data.HydraulicLocationConfigurationSettings.Scope : string.Empty;
            }
        }

        [PropertyOrder(seaLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_SeaLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_SeaLevel_Description))]
        public string SeaLevel
        {
            get
            {
                string seaLevel = data.HydraulicLocationConfigurationSettings.SeaLevel;
                return data.IsLinked() && seaLevel != null ? seaLevel : string.Empty;
            }
        }

        [PropertyOrder(riverDischargePropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_RiverDischarge_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_RiverDischarge_Description))]
        public string RiverDischarge
        {
            get
            {
                string riverDischarge = data.HydraulicLocationConfigurationSettings.RiverDischarge;
                return data.IsLinked() && riverDischarge != null ? riverDischarge : string.Empty;
            }
        }

        [PropertyOrder(lakeLevelPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_LakeLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_LakeLevel_Description))]
        public string LakeLevel
        {
            get
            {
                string lakeLevel = data.HydraulicLocationConfigurationSettings.LakeLevel;
                return data.IsLinked() && lakeLevel != null ? lakeLevel : string.Empty;
            }
        }

        [PropertyOrder(windDirectionPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                string windDirection = data.HydraulicLocationConfigurationSettings.WindDirection;
                return data.IsLinked() && windDirection != null ? windDirection : string.Empty;
            }
        }

        [PropertyOrder(windSpeedPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_WindSpeed_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_WindSpeed_Description))]
        public string WindSpeed
        {
            get
            {
                string windSpeed = data.HydraulicLocationConfigurationSettings.WindSpeed;
                return data.IsLinked() && windSpeed != null ? windSpeed : string.Empty;
            }
        }

        [PropertyOrder(commentPropertyIndex)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Comment_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_Comment_Description))]
        public string Comment
        {
            get
            {
                string comment = data.HydraulicLocationConfigurationSettings.Comment;
                return data.IsLinked() && comment != null ? comment : string.Empty;
            }
        }

        [PropertyOrder(usePreprocessorPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessor_Description))]
        public bool UsePreprocessor
        {
            get
            {
                return data.UsePreprocessor;
            }
            set
            {
                data.UsePreprocessor = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(preprocessorDirectoryPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Description))]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string PreprocessorDirectory
        {
            get
            {
                return data.PreprocessorDirectory;
            }
            set
            {
                data.PreprocessorDirectory = value;
            }
        }

        [PropertyOrder(preprocessorDirectoryPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Description))]
        public string PreprocessorDirectoryReadOnly
        {
            get
            {
                return data.PreprocessorDirectory;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            bool canUsePreprocessor = data.CanUsePreprocessor;

            if (propertyName.Equals(nameof(UsePreprocessor)) && !canUsePreprocessor)
            {
                return false;
            }

            if (propertyName.Equals(nameof(PreprocessorDirectory)) && (!canUsePreprocessor || !UsePreprocessor))
            {
                return false;
            }

            if (propertyName.Equals(nameof(PreprocessorDirectoryReadOnly)) && (!canUsePreprocessor || UsePreprocessor))
            {
                return false;
            }

            if (propertyName.Equals(nameof(HlcdFilePath)) && !data.IsLinked())
            {
                return false;
            }

            if (propertyName.Equals(nameof(HlcdFilePathReadOnly)) && data.IsLinked())
            {
                return false;
            }

            return true;
        }
    }
}