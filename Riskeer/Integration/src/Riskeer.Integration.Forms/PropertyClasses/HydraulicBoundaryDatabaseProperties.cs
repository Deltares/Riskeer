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
using System.Linq;
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicBoundaryDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicBoundaryDatabaseProperties : ObjectProperties<HydraulicBoundaryDatabase>
    {
        private const int usePreprocessorClosurePropertyIndex = 0;
        private const int versionPropertyIndex = 1;
        private const int locationsPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseProperties(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            Data = hydraulicBoundaryDatabase;
        }

        [PropertyOrder(usePreprocessorClosurePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessorClosure_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_UsePreprocessorClosure_Description))]
        public bool UsePreprocessorClosure
        {
            get
            {
                return data.UsePreprocessorClosure;
            }
        }

        [PropertyOrder(versionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicDatabase_Version_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicDatabase_Version_Description))]
        public string Version
        {
            get
            {
                return data.Version;
            }
        }

        [PropertyOrder(locationsPropertyIndex)]
        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Locations_Description))]

        public HydraulicBoundaryDatabaseLocationProperties[] Locations
        {
            get
            {
                return data.Locations.Select(x => new HydraulicBoundaryDatabaseLocationProperties(x)).ToArray();
            }
        }
    }
}