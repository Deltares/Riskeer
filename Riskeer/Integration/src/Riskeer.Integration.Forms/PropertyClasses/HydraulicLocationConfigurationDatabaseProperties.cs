// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.Util.Attributes;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="HydraulicLocationConfigurationDatabase"/> for properties panel.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseProperties : ObjectProperties<HydraulicLocationConfigurationDatabase>
    {
        private const int scenarioNamePropertyIndex = 0;
        private const int yearPropertyIndex = 1;
        private const int scopePropertyIndex = 2;
        private const int seaLevelPropertyIndex = 3;
        private const int riverDischargePropertyIndex = 4;
        private const int lakeLevelPropertyIndex = 5;
        private const int windDirectionPropertyIndex = 6;
        private const int windSpeedPropertyIndex = 7;
        private const int commentPropertyIndex = 8;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseProperties"/>.
        /// </summary>
        /// <param name="hydraulicLocationConfigurationDatabase">The hydraulic location configuration database to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicLocationConfigurationDatabase"/> is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseProperties(HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase)
        {
            if (hydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicLocationConfigurationDatabase));
            }

            Data = hydraulicLocationConfigurationDatabase;
        }

        [PropertyOrder(scenarioNamePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_ScenarioName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_ScenarioName_Description))]
        public string ScenarioName
        {
            get
            {
                return data.ScenarioName ?? string.Empty;
            }
        }

        [PropertyOrder(yearPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Year_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Year_Description))]
        public string Year
        {
            get
            {
                return data.Year.ToString();
            }
        }

        [PropertyOrder(scopePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Scope_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Scope_Description))]
        public string Scope
        {
            get
            {
                return data.Scope ?? string.Empty;
            }
        }

        [PropertyOrder(seaLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_SeaLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_SeaLevel_Description))]
        public string SeaLevel
        {
            get
            {
                return data.SeaLevel ?? string.Empty;
            }
        }

        [PropertyOrder(riverDischargePropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_RiverDischarge_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_RiverDischarge_Description))]
        public string RiverDischarge
        {
            get
            {
                return data.RiverDischarge ?? string.Empty;
            }
        }

        [PropertyOrder(lakeLevelPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_LakeLevel_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_LakeLevel_Description))]
        public string LakeLevel
        {
            get
            {
                return data.LakeLevel ?? string.Empty;
            }
        }

        [PropertyOrder(windDirectionPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_WindDirection_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_WindDirection_Description))]
        public string WindDirection
        {
            get
            {
                return data.WindDirection ?? string.Empty;
            }
        }

        [PropertyOrder(windSpeedPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_WindSpeed_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_WindSpeed_Description))]
        public string WindSpeed
        {
            get
            {
                return data.WindSpeed ?? string.Empty;
            }
        }

        [PropertyOrder(commentPropertyIndex)]
        [ResourcesCategory(typeof(RiskeerCommonFormsResources), nameof(RiskeerCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Comment_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicLocationConfigurationDatabase_Comment_Description))]
        public string Comment
        {
            get
            {
                return data.Comment ?? string.Empty;
            }
        }
    }
}