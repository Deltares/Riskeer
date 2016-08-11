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

namespace Application.Ringtoets.Storage.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="Point3D"/> and an
    /// XML representation of that data.
    /// </summary>
    internal class Point3DXmlSerializer : SimpleDataCollectionSerializer<Point3D, Point3DXmlSerializer.SerializablePoint3D>
    {
        protected override Point3D[] FromSerializableData(IEnumerable<SerializablePoint3D> serializedElements)
        {
            return serializedElements.Select(sp => sp.ToPoint3D()).ToArray();
        }

        protected override SerializablePoint3D[] ToSerializableData(IEnumerable<Point3D> elements)
        {
            return elements.Select(p => new SerializablePoint3D(p)).ToArray();
        }

        [Serializable]
        internal class SerializablePoint3D
        {
            private readonly double x;
            private readonly double y;
            private readonly double z;

            public SerializablePoint3D(Point3D point3D)
            {
                x = point3D.X;
                y = point3D.Y;
                z = point3D.Z;
            }

            public Point3D ToPoint3D()
            {
                return new Point3D(x, y, z);
            }
        }
    }
}