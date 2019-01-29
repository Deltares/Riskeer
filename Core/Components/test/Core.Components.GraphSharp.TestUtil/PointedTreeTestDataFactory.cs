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
using System.Windows.Media;
using Core.Common.TestUtil;
using Core.Components.GraphSharp.Data;

namespace Core.Components.GraphSharp.TestUtil
{
    /// <summary>
    /// Factory for creating pointed tree related test data objects.
    /// </summary>
    public static class PointedTreeTestDataFactory
    {
        /// <summary>
        /// Creates a <see cref="PointedTreeElementVertex"/> object for testing purposes.
        /// </summary>
        /// <param name="isSelectable">Whether or not the vertex should be selectable.</param>
        /// <returns>A <see cref="PointedTreeElementVertex"/>.</returns>
        public static PointedTreeElementVertex CreatePointedTreeElementVertex(bool isSelectable = false)
        {
            var random = new Random(21);

            return new PointedTreeElementVertex("<text>test</text>",
                                                Color.FromArgb((byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255)),
                                                Color.FromArgb((byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255),
                                                               (byte) random.Next(0, 255)),
                                                random.Next(0, int.MaxValue),
                                                random.NextEnumValue<PointedTreeVertexType>(),
                                                isSelectable);
        }
    }
}