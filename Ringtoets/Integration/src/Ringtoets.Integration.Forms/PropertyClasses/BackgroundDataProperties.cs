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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.Integration.Forms.Properties;
using Ringtoets.Common.Data.AssessmentSection;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GisFormsResources = Core.Components.Gis.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="BackgroundData"/> for properties panel.
    /// </summary>
    public class BackgroundDataProperties : ObjectProperties<BackgroundData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundDataProperties"/>.
        /// </summary>
        /// <param name="backgroundData">The data for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundData"/>
        /// is <c>null</c>.</exception>
        public BackgroundDataProperties(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            Data = backgroundData;
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundDataProperties_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_Name_Description))]
        public string Name
        {
            get
            {
                var configuration = data.Configuration as WmtsBackgroundDataConfiguration;

                return configuration != null
                           ? (configuration.IsConfigured
                                  ? data.Name
                                  : string.Empty)
                           : data.Name;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundDataProperties_Transparency_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_Transparency_Description))]
        public RoundedDouble Transparency
        {
            get
            {
                return data.Transparency;
            }
            set
            {
                data.Transparency = value;
                data.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundDataProperties_IsVisible_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_IsVisible_Description))]
        public bool IsVisible
        {
            get
            {
                return data.IsVisible;
            }
            set
            {
                data.IsVisible = value;
                data.NotifyObservers();
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            var configuration = data.Configuration as WmtsBackgroundDataConfiguration;

            return configuration != null
                   && configuration.IsConfigured;
        }

        #region Wmts MapData

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_Category))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_Url_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_Url_Description))]
        public string SourceCapabilitiesUrl
        {
            get
            {
                return GetBackgroundDataProperty(dataConfiguration => dataConfiguration.SourceCapabilitiesUrl);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_Category))]
        [ResourcesDisplayName(typeof(GisFormsResources), nameof(GisFormsResources.WmtsCapability_MapLayer_Id))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_SelectedCapabilityIdentifier_Description))]
        public string SelectedCapabilityIdentifier
        {
            get
            {
                return GetBackgroundDataProperty(dataConfiguration => dataConfiguration.SelectedCapabilityIdentifier);
            }
        }

        [DynamicVisible]
        [ResourcesCategory(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_Category))]
        [ResourcesDisplayName(typeof(GisFormsResources), nameof(GisFormsResources.WmtsCapability_MapLayer_Format))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundDataProperties_Wmts_PreferredFormat_Description))]
        public string PreferredFormat
        {
            get
            {
                return GetBackgroundDataProperty(dataConfiguration => dataConfiguration.PreferredFormat);
            }
        }

        private string GetBackgroundDataProperty(Func<WmtsBackgroundDataConfiguration, string> getProperty)
        {
            var configuration = data.Configuration as WmtsBackgroundDataConfiguration;

            return configuration != null && configuration.IsConfigured
                       ? getProperty(configuration)
                       : string.Empty;
        }

        #endregion
    }
}