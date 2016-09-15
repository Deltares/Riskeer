﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;

namespace Application.Ringtoets.Storage.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="RoughnessPoint"/>
    /// and an XML representation of that data.
    /// </summary>
    internal class RoughnessPointXmlSerializer : DataCollectionSerializer<RoughnessPoint, RoughnessPointXmlSerializer.SerializableRoughnessPoint>
    {
        protected override SerializableRoughnessPoint[] ToSerializableData(IEnumerable<RoughnessPoint> elements)
        {
            return elements.Select(p => new SerializableRoughnessPoint(p)).ToArray();
        }

        protected override RoughnessPoint[] FromSerializableData(IEnumerable<SerializableRoughnessPoint> serializedElements)
        {
            return serializedElements.Select(sp => sp.ToRoughnessPoint()).ToArray();
        }

        [Serializable]
        internal class SerializableRoughnessPoint
        {
            private readonly double x;
            private readonly double y;
            private readonly double roughness;

            public SerializableRoughnessPoint(RoughnessPoint point)
            {
                x = point.Point.X;
                y = point.Point.Y;
                roughness = point.Roughness;
            }

            public RoughnessPoint ToRoughnessPoint()
            {
                return new RoughnessPoint(new Point2D(x, y), roughness);
            }
        }
    }
}