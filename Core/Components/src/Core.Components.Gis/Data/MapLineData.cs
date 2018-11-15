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
using System.Drawing;
using System.Linq;
using Core.Components.Gis.Features;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents features that are visible as a line.
    /// </summary>
    public class MapLineData : FeatureBasedMapData
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapLineData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapLineData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public MapLineData(string name) : this(name, CreateDefaultLineStyle()) {}

        /// <summary>
        /// Creates a new instance of <see cref="MapLineData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapLineData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public MapLineData(string name, LineStyle style) : base(name)
        {
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            Style = style;
        }

        /// <summary>
        /// Gets the style of the line.
        /// </summary>
        public LineStyle Style { get; }

        /// <summary>
        /// Gets or sets the map theme of the <see cref="MapLineData"/>.
        /// </summary>
        public MapTheme<LineCategoryTheme> Theme { get; set; }

        /// <summary>
        /// This method validates newly set features.
        /// </summary>
        /// <param name="featuresToValidate">The new features to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="featuresToValidate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any feature in <paramref name="featuresToValidate"/> 
        /// contains multiple point-collections.</exception>
        /// <seealso cref="Features"/>
        protected override void ValidateFeatures(IEnumerable<MapFeature> featuresToValidate)
        {
            base.ValidateFeatures(featuresToValidate);

            if (HasFeatureWithMultiplePointCollections(featuresToValidate))
            {
                throw new ArgumentException("MapLineData only accepts MapFeature instances whose MapGeometries contain a single point-collection.");
            }
        }

        private static LineStyle CreateDefaultLineStyle()
        {
            return new LineStyle
            {
                Color = Color.Black,
                Width = 2,
                DashStyle = LineDashStyle.Solid
            };
        }

        private static bool HasFeatureWithMultiplePointCollections(IEnumerable<MapFeature> lineFeatures)
        {
            return lineFeatures.SelectMany(feature => feature.MapGeometries)
                               .Any(geometry => geometry.PointCollections.Count() != 1);
        }
    }
}