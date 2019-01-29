// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows.Forms.Integration;
using Core.Common.Util.Reflection;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms;
using Core.Components.GraphSharp.Forms.Layout;
using WPFExtensions.Controls;

namespace Core.Components.GraphSharp.TestUtil
{
    /// <summary>
    /// A helper for fetching the underlying properties of <see cref="PointedTreeGraphControl"/>.
    /// </summary>
    public static class PointedTreeGraphControlHelper
    {
        /// <summary>
        /// Gets the underlying <see cref="PointedTreeGraph"/> from the <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to get the underlying <see cref="PointedTreeGraph"/>.</param>
        /// <returns>The found <see cref="PointedTreeGraph"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="control"/> is <c>null</c>.</exception>
        public static PointedTreeGraph GetPointedTreeGraph(PointedTreeGraphControl control)
        {
            ZoomControl zoomControl = GetZoomControl(control);
            var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
            return graphLayout.Graph;
        }

        /// <summary>
        /// Gets the underlying <see cref="ZoomControl"/> from the <paramref name="control"/>.
        /// </summary>
        /// <param name="control">The control to get the underlying <see cref="ZoomControl"/>.</param>
        /// <returns>The found <see cref="ZoomControl"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="control"/> is <c>null</c>.</exception>
        public static ZoomControl GetZoomControl(PointedTreeGraphControl control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            var elementHost = TypeUtils.GetField<ElementHost>(control, "wpfElementHost");
            return (ZoomControl) elementHost.Child;
        }
    }
}