// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Runtime.Serialization;
using Core.Common.Base.Geometry;

namespace Riskeer.Storage.Core.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="Point2D"/> and an
    /// XML representation of that data.
    /// </summary>
    internal class Point2DCollectionXmlSerializer : DataCollectionSerializer<Point2D, Point2DCollectionXmlSerializer.SerializablePoint2D>
    {
        protected override SerializablePoint2D[] ToSerializableData(IEnumerable<Point2D> points)
        {
            return points.Select(p => new SerializablePoint2D(p)).ToArray();
        }

        protected override Point2D[] FromSerializableData(IEnumerable<SerializablePoint2D> pointData)
        {
            return pointData.Select(pd => pd.ToPoint2D()).ToArray();
        }

        [Serializable]
        [DataContract(Name = nameof(SerializablePoint2D), Namespace = "")]
        internal class SerializablePoint2D
        {
            [DataMember]
            private readonly double x;

            [DataMember]
            private readonly double y;

            public SerializablePoint2D(Point2D point2D)
            {
                x = point2D.X;
                y = point2D.Y;
            }

            public Point2D ToPoint2D()
            {
                return new Point2D(x, y);
            }
        }
    }
}