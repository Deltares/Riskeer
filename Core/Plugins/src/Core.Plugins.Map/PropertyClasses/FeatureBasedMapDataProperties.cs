// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Core.Common.Util.Extensions;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Properties;
using Core.Plugins.Map.UITypeEditors;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of <see cref="FeatureBasedMapData"/> for properties panel.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="FeatureBasedMapData"/> to be shown in the properties.</typeparam>
    public abstract class FeatureBasedMapDataProperties<T> : ObjectProperties<T>, IHasMetaData where T : FeatureBasedMapData
    {
        private const int namePropertyIndex = 0;
        private const int typePropertyIndex = 1;
        private const int isVisiblePropertyIndex = 2;
        private const int showLabelsPropertyIndex = 3;
        private const int selectedMetaDataAttributePropertyIndex = 4;
        private const int styleTypePropertyIndex = 5;
        private readonly IEnumerable<MapDataCollection> parents;

        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapDataProperties{T}"/>.
        /// </summary>
        /// <param name="data">The <typeparamref name="T"/> to show the properties for.</param>
        /// <param name="parents">A collection containing all parent <see cref="MapDataCollection"/> of
        /// the <paramref name="data"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected FeatureBasedMapDataProperties(T data, IEnumerable<MapDataCollection> parents)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (parents == null)
            {
                throw new ArgumentNullException(nameof(parents));
            }

            this.data = data;
            this.parents = parents;
        }

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Layer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapData_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(typePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Layer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FeatureBasedMapData_Type_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FeatureBasedMapData_Type_Description))]
        public abstract string Type { get; }

        [PropertyOrder(isVisiblePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Layer))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_IsVisible_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapData_IsVisible_Description))]
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

                parents.ForEachElementDo(collection => collection.NotifyObservers());
            }
        }

        [PropertyOrder(showLabelsPropertyIndex)]
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Label))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FeatureBasedMapData_ShowLabels_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FeatureBasedMapData_ShowLabels_Description))]
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

        [PropertyOrder(styleTypePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Styling))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FeatureBasedMapdata_StyleType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FeatureBasedMapdata_StyleType_Description))]
        public abstract string StyleType { get; }

        [PropertyOrder(selectedMetaDataAttributePropertyIndex)]
        [DynamicVisible]
        [DynamicReadOnly]
        [Editor(typeof(MetaDataAttributeEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_Label))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FeatureBasedMapdata_SelectedMetaDataAttribute_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.FeatureBasedMapdata_SelectedMetaDataAttribute_Description))]
        public SelectableMetaDataAttribute SelectedMetaDataAttribute
        {
            get
            {
                return new SelectableMetaDataAttribute(data.SelectedMetaDataAttribute ?? string.Empty);
            }
            set
            {
                data.SelectedMetaDataAttribute = value.MetaDataAttribute;
                data.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public virtual bool DynamicReadonlyValidator(string propertyName)
        {
            if (propertyName == nameof(ShowLabels)
                || propertyName == nameof(SelectedMetaDataAttribute))
            {
                return !data.MetaData.Any();
            }

            return false;
        }

        [DynamicVisibleValidationMethod]
        public virtual bool DynamicVisibleValidationMethod(string propertyName)
        {
            if (propertyName == nameof(SelectedMetaDataAttribute))
            {
                return data.ShowLabels;
            }

            return false;
        }

        public IEnumerable<SelectableMetaDataAttribute> GetAvailableMetaDataAttributes()
        {
            return data.MetaData.Select(md => new SelectableMetaDataAttribute(md)).ToArray();
        }
    }
}