﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.IO;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryDataContextProperties : ObjectProperties<HydraulicLocationConfigurationDatabase>
    {
        private const int workingDirectoryPropertyIndex = 0;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public HydraulicBoundaryDataContextProperties(HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase)
        {
            if (hydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationDatabase));
            }

            Data = hydraulicLocationConfigurationDatabase;
        }

        [PropertyOrder(workingDirectoryPropertyIndex)]
        [DynamicVisible]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicDatabase_WorkingDirectory_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicDatabase_WorkingDirectory_Description))]
        public string WorkingDirectory
        {
            get
            {
                return Path.GetDirectoryName(data.FilePath) ?? string.Empty;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(string propertyName)
        {
            return !string.IsNullOrEmpty(data.FilePath);
        }
    }
}