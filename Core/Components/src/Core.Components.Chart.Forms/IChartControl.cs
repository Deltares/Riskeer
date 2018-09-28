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
using Core.Components.Chart.Data;

namespace Core.Components.Chart.Forms
{
    /// <summary>
    /// Interface describing general chart interactions.
    /// </summary>
    public interface IChartControl
    {
        /// <summary>
        /// Gets a value indicating whether or not the chart can be panned with the left mouse button.
        /// </summary>
        bool IsPanningEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the chart can be zoomed by rectangle with the left mouse button.
        /// </summary>
        bool IsRectangleZoomingEnabled { get; }

        /// <summary>
        /// Gets or sets the data to show in the <see cref="IChartControl"/>.
        /// </summary>
        ChartDataCollection Data { get; set; }

        /// <summary>
        /// Gets or sets the title of the chart.
        /// </summary>
        string ChartTitle { get; set; }

        /// <summary>
        /// Gets or sets the title of the bottom axis in the view.
        /// </summary>
        string BottomAxisTitle { get; set; }

        /// <summary>
        /// Gets or sets the title of the left axis in the view.
        /// </summary>
        string LeftAxisTitle { get; set; }

        /// <summary>
        /// Toggles panning of the <see cref="IChartControl"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        void TogglePanning();

        /// <summary>
        /// Toggles rectangle zooming of the <see cref="IChartControl"/>. Rectangle zooming is invoked by clicking the left mouse-button.
        /// </summary>
        void ToggleRectangleZooming();

        /// <summary>
        /// Zooms to a level so that everything is in view.
        /// </summary>
        void ZoomToAllVisibleLayers();

        /// <summary>
        /// Zooms to a level such that the given chart data is in view.
        /// </summary>
        /// <param name="layerData">The data to zoom to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="layerData"/>
        /// is not part of <see cref="Data"/>.</exception>
        void ZoomToAllVisibleLayers(ChartData layerData);
    }
}