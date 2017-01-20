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

using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonForms = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="BackgroundMapDataContext"/> for properties panel.
    /// </summary>
    public class BackgroundMapDataContextProperties : ObjectProperties<BackgroundMapDataContext>
    {
        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Name_Description))]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonForms), nameof(RingtoetsCommonForms.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Url_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContextProperties_Url_Description))]
        public string Url
        {
            get
            {
                return data.WrappedData.SourceCapabilitiesUrl;
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
                return data.WrappedData.Transparency;
            }
            set
            {
                data.WrappedData.Transparency = value.ToPrecision(data.WrappedData.Transparency.NumberOfDecimalPlaces);
                data.WrappedData.NotifyObservers();
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
                return data.WrappedData.IsVisible;
            }
            set
            {
                data.WrappedData.IsVisible = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            if (propertyName == TypeUtils.GetMemberName<BackgroundMapDataContextProperties>(p => p.Transparency))
            {
                return !data.WrappedData.IsConfigured;
            }
            if (propertyName == TypeUtils.GetMemberName<BackgroundMapDataContextProperties>(p => p.IsVisible))
            {
                return !data.WrappedData.IsConfigured;
            }

            return true;
        }
    }
}
