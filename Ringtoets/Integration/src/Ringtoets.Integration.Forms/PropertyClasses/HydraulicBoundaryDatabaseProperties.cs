﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryDatabaseProperties : ObjectProperties<HydraulicBoundaryDatabaseContext>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseContext">The context to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabaseContext"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseProperties(HydraulicBoundaryDatabaseContext hydraulicBoundaryDatabaseContext)
        {
            if (hydraulicBoundaryDatabaseContext == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabaseContext));
            }

            Data = hydraulicBoundaryDatabaseContext;
        }

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_FilePath_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_FilePath_Description))]
        public string FilePath
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryDatabase.IsCoupled() ? data.WrappedData.HydraulicBoundaryDatabase.FilePath : string.Empty;
            }
        }

        [PropertyOrder(2)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessor_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessor_Description))]
        public bool UsePreprocessor
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryDatabase.UsePreprocessor;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryDatabase.UsePreprocessor = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(3)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Description))]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string PreprocessorDirectory
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryDatabase.PreprocessorDirectory;
            }
            set
            {
                data.WrappedData.HydraulicBoundaryDatabase.PreprocessorDirectory = value;
            }
        }

        [PropertyOrder(4)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Description))]
        public string PreprocessorDirectoryReadOnly
        {
            get
            {
                return data.WrappedData.HydraulicBoundaryDatabase.PreprocessorDirectory;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            bool canUsePreprocessor = data.WrappedData.HydraulicBoundaryDatabase.CanUsePreprocessor;

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