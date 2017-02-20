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
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Components.Gis;
using Ringtoets.Integration.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of the <see cref="BackgroundMapDataContainer"/> for properties panel.
    /// </summary>
    public class BackgroundMapDataContainerProperties : ObjectProperties<BackgroundMapDataContainer>
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundMapDataContainer"/>.
        /// </summary>
        /// <param name="container">The data for which the properties are shown.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/>
        /// is <c>null</c>.</exception>
        public BackgroundMapDataContainerProperties(BackgroundMapDataContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            Data = container;
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_Name_Description))]
        public string Name
        {
            get
            {
                return HasConfiguredMapData() ? data.MapData.Name : string.Empty;
            }
        }

        [ResourcesCategory(typeof(RingtoetsCommonFormsResources), nameof(RingtoetsCommonFormsResources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_Transparency_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_Transparency_Description))]
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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_IsVisible_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BackgroundMapDataContainerProperties_IsVisible_Description))]
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

        private bool HasConfiguredMapData()
        {
            return data.MapData != null && data.MapData.IsConfigured;
        }
    }
}