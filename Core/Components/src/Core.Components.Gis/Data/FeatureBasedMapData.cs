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
using System.Linq;
using Core.Components.Gis.Features;
using Core.Components.Gis.Theme;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Base class for <see cref="MapData"/> which is based on an array of features.
    /// The features are defined in the RD-new coordinate system.
    /// </summary>
    public abstract class FeatureBasedMapData : MapData
    {
        private IEnumerable<MapFeature> features;

        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected FeatureBasedMapData(string name) : base(name)
        {
            features = new MapFeature[0];
            ShowLabels = false;
        }

        /// <summary>
        /// Gets or sets an array of features defined in the RD-new coordinate system.
        /// </summary>
        /// <remarks>Calls <see cref="ValidateFeatures"/> when setting a new array and so
        /// can throw all corresponding exceptions. This collection will not contain <c>null</c>
        /// elements.</remarks>
        public IEnumerable<MapFeature> Features
        {
            get
            {
                return features;
            }
            set
            {
                ValidateFeatures(value);

                features = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the labels of the <see cref="FeatureBasedMapData"/> should be shown.
        /// </summary>
        public bool ShowLabels { get; set; }

        /// <summary>
        /// Gets or sets the selected attribute of the meta data to show as label.
        /// </summary>
        public string SelectedMetaDataAttribute { get; set; }

        /// <summary>
        /// Gets the meta data associated with the map data.
        /// </summary>
        public IEnumerable<string> MetaData
        {
            get
            {
                return features.SelectMany(f => f.MetaData)
                               .Select(md => md.Key)
                               .Distinct();
            }
        }

        /// <summary>
        /// This method validates newly set features.
        /// </summary>
        /// <param name="featuresToValidate">The new features to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="featuresToValidate"/>
        /// is <c>null</c> or contains <c>null</c>.</exception>
        /// <seealso cref="Features"/>
        protected virtual void ValidateFeatures(IEnumerable<MapFeature> featuresToValidate)
        {
            if (featuresToValidate == null || featuresToValidate.Any(e => e == null))
            {
                throw new ArgumentNullException(nameof(featuresToValidate), @"The array of features cannot be null or contain null.");
            }
        }
    }

    /// <summary>
    /// Base class for <see cref="MapData"/> which is based on an array of features
    /// and has categorical theming. The features are defined in the RD-new coordinate system.
    /// </summary>
    /// <typeparam name="TCategoryTheme">The type of category theme.</typeparam>
    public abstract class FeatureBasedMapData<TCategoryTheme> : FeatureBasedMapData
        where TCategoryTheme : CategoryTheme
    {
        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapData{T}"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected FeatureBasedMapData(string name) : this(name, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="FeatureBasedMapData{T}"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>.</param>
        /// <param name="theme">The <see cref="MapTheme{TCategoryTheme}"/>
        /// belonging to the <see cref="FeatureBasedMapData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected FeatureBasedMapData(string name, MapTheme<TCategoryTheme> theme) : base(name)
        {
            Theme = theme;
        }

        /// <summary>
        /// Gets the <see cref="MapTheme{TCategoryTheme}"/> that belongs to the map data.
        /// </summary>
        public MapTheme<TCategoryTheme> Theme { get; }
    }
}