// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.UITypeEditors;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of <see cref="FeatureBasedMapData"/> for properties panel.
    /// </summary>
    public abstract class FeatureBasedMapDataProperties<T> : ObjectProperties<T>, IHasMetaData where T : FeatureBasedMapData
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int isVisiblePropertyIndex = 2;
        private const int showLabelsPropertyIndex = 3;
        private const int selectedMetaDataAttributePropertyIndex = 4;

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Layer")]
        [ResourcesDisplayName(typeof(Resources), "MapData_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapData_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(typePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Layer")]
        [ResourcesDisplayName(typeof(Resources), "MapData_Type_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapData_Type_Description")]
        public abstract string Type { get; }

        [PropertyOrder(isVisiblePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Layer")]
        [ResourcesDisplayName(typeof(Resources), "MapData_IsVisible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapData_IsVisible_Description")]
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

        [PropertyOrder(showLabelsPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_Label")]
        [ResourcesDisplayName(typeof(Resources), "MapData_ShowLabels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "MapData_ShowLabels_Description")]
        public bool ShowLabels
        {
            get
            {
                return data.ShowLabels;
            }
            set
            {
                data.ShowLabels = value;
                data.NotifyObservers();
            }
        }

        [PropertyOrder(selectedMetaDataAttributePropertyIndex)]
        [DynamicVisible]
        [Editor(typeof(MetaDataAttributeEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_Label")]
        [ResourcesDisplayName(typeof(Resources), "Mapdata_SelectedMetaDataAttribute_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Mapdata_SelectedMetaDataAttribute_Description")]
        public string SelectedMetaDataAttribute
        {
            get
            {
                return data.SelectedMetaDataAttribute;
            }
            set
            {
                data.SelectedMetaDataAttribute = value;
                data.NotifyObservers();
            }
        }

        public IEnumerable<string> GetAvailableMetaDataAttributes()
        {
            return data.MetaData;
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadonlyValidator(string propertyName)
        {
            return !data.MetaData.Any();
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.ShowLabels;
        }
    }
}