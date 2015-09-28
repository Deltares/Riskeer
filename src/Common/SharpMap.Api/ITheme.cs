// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Drawing;
using DelftTools.Utils.Collections.Generic;
using GeoAPI.Extensions.Feature;

namespace SharpMap.Api
{
	/// <summary>
	/// Interface for rendering a thematic layer
	/// 
    /// TODO: review ITheme, it should be easier to work with them, check implementation in ArgGIS, uDIG, QGIS...
	/// </summary>
	public interface ITheme : ICloneable
    {
        /// <summary>
        /// Returns the style based on a feature
        /// </summary>
        /// <param name="feature">Attribute to calculate color from</param>
        /// <returns>Color</returns>
        IStyle GetStyle(IFeature feature);

        /// <summary>
        /// Returns the style based on a value
        /// 
        /// <para>Added for access based on coverage values
        /// </summary>
        /// <param name="value">Value to determine style for</param>
        /// <returns>Color</returns>
        IStyle GetStyle<T>(T value) where T : IComparable<T>, IComparable;

        /// <summary>
        /// Returns all themeItems that are part of this theme
        /// </summary>
        /// <returns>Color</returns>
        IEventedList<IThemeItem> ThemeItems { get; set; }

        /// <summary>
        /// Returns color by value based on current configuration of theme.
        /// 
        /// ADDED ONLY FOR PERFORMANCE
        /// <para>Added for fast access to colors.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Color GetFillColor<T>(T value) where T : IComparable;

        /// <summary>
        /// Theme attribute is used to find the values on which to base the coloring
        /// </summary>
	    string AttributeName { get; }

        /// <summary>
        /// Rescales the Theme based on <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="min">Minimum value used for the visualisation of the theme</param>
        /// <param name="max">Maximum value used for the visualisation of the theme</param>
        /// <returns>Returns a new, rescaled theme based on this object.</returns>
        void ScaleTo(double min, double max );
	}
}
