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

using OxyPlot;

namespace Core.Components.OxyPlot
{
    /// <summary>
    /// This class represents a controller for which certain interactions can be toggled.
    /// </summary>
    internal sealed class DynamicPlotController : ControllerBase, IPlotController
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DynamicPlotController"/>.
        /// </summary>
        public DynamicPlotController()
        {
            ResetDefaultInteraction();
            TogglePanning();
        }

        /// <summary>
        /// Gets a value indicating whether or not panning is enabled for the <see cref="DynamicPlotController"/>.
        /// </summary>
        public bool IsPanningEnabled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not zooming by rectangle is enabled for the <see cref="DynamicPlotController"/>.
        /// </summary>
        public bool IsRectangleZoomingEnabled { get; private set; }

        /// <summary>
        /// Toggles panning by click and holding the left mouse button while moving.
        /// </summary>
        public void TogglePanning()
        {
            if (!IsPanningEnabled)
            {
                ResetDefaultInteraction();
                EnablePanning();
            }
        }

        /// <summary>
        /// Toggles zooming by drawing a rectangle with the mouse.
        /// </summary>
        public void ToggleRectangleZooming()
        {
            if (!IsRectangleZoomingEnabled)
            {
                ResetDefaultInteraction();
                EnableRectangleZoom();
            }
        }

        /// <summary>
        /// Resets all the toggleable interaction with the <see cref="DynamicPlotController"/>.
        /// </summary>
        private void ResetDefaultInteraction()
        {
            UnbindAll();
            IsPanningEnabled = false;
            IsRectangleZoomingEnabled = false;

            EnableScrollWheelZooming();
        }

        /// <summary>
        /// Enables zooming in and out by using the scroll wheel.
        /// </summary>
        private void EnableScrollWheelZooming()
        {
            this.BindMouseWheel(PlotCommands.ZoomWheel);
        }

        /// <summary>
        /// Enables panning of the <see cref="DynamicPlotController"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        private void EnablePanning()
        {
            this.BindMouseDown(OxyMouseButton.Left, PlotCommands.PanAt);
            this.BindMouseDown(OxyMouseButton.Middle, PlotCommands.PanAt);
            IsPanningEnabled = true;
        }

        /// <summary>
        /// Enables zooming by rectangle of the <see cref="DynamicPlotController"/>. Zooming by rectangle is invoked by clicking the left mouse-button.
        /// </summary>
        private void EnableRectangleZoom()
        {
            this.BindMouseDown(OxyMouseButton.Left, PlotCommands.ZoomRectangle);
            IsRectangleZoomingEnabled = true;
        }
    }
}