// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HrdFile"/> for properties panel.
    /// </summary>
    public class HrdFileProperties : ObjectProperties<HrdFile>
    {
        private const int filePathPropertyIndex = 0;
        private const int usePreprocessorClosurePropertyIndex = 1;
        private const int usePreprocessorPropertyIndex = 2;
        private const int preprocessorDirectoryPropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="HrdFileProperties"/>.
        /// </summary>
        /// <param name="hrdFile">The HRD file to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hrdFile"/> is <c>null</c>.</exception>
        public HrdFileProperties(HrdFile hrdFile)
        {
            if (hrdFile == null)
            {
                throw new ArgumentNullException(nameof(hrdFile));
            }

            Data = hrdFile;
        }

        [PropertyOrder(filePathPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HrdFile_FilePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HrdFile_FilePath_Description))]
        public string FilePath
        {
            get
            {
                return data.FilePath;
            }
        }

        [PropertyOrder(usePreprocessorClosurePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_UsePreprocessorClosure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationSettings_UsePreprocessorClosure_Description))]
        public bool UsePreprocessorClosure
        {
            get
            {
                return data.UsePreprocessorClosure;
            }
        }

        [PropertyOrder(usePreprocessorPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
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
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
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
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
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

            return true;
        }
    }
}