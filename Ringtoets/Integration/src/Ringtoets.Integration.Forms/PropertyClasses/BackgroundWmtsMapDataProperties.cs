// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Attributes;
using Core.Components.Gis.Data;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonForms = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="WmtsMapData"/> for properties panel.
    /// </summary>
    public class BackgroundWmtsMapDataProperties : ObjectProperties<WmtsMapData>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundWmtsMapDataProperties"/>.
        /// </summary>
        /// <param name="data">The data for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public BackgroundWmtsMapDataProperties(WmtsMapData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Data = data;
        }

        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Name_Description))]
        public string Name
        {
            get
            {
                return data.IsConfigured ? data.Name : string.Empty;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Url_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Url_Description))]
        public string Url
        {
            get
            {
                return data.SourceCapabilitiesUrl;
            }
        }

        [DynamicReadOnly]
        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Transparency_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Transparency_Description))]
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

        [DynamicReadOnly]
        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_IsVisible_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_IsVisible_Description))]
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

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == nameof(Transparency))
            {
                return !data.IsConfigured;
            }
            if (propertyName == nameof(IsVisible))
            {
                return !data.IsConfigured;
            }

            return false;
        }
    }
}