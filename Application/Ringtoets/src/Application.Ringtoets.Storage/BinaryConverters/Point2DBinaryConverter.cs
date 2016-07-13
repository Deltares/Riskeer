// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.BinaryConverters
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="Point2D"/> and a
    /// binary representation of that data.
    /// </summary>
    public class Point2DBinaryConverter
    {
        /// <summary>
        /// Converts the collection of <see cref="Point2D"/> to binary data.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The binary data.</returns>
        public byte[] ToBytes(IEnumerable<Point2D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                SerializablePoint2D[] serializableData = GetSerializableData(points);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, serializableData);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Converts the binary data to a collection of <see cref="Point2D"/>.
        /// </summary>
        /// <param name="serializedData">The binary data.</param>
        /// <returns>The collection of <see cref="Point2D"/>.</returns>
        public Point2D[] ToData(byte[] serializedData)
        {
            if (serializedData == null)
            {
                throw new ArgumentNullException("serializedData");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(serializedData, 0, serializedData.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                return ((SerializablePoint2D[])formatter.Deserialize(memoryStream))
                    .Select(sp => sp.ToPoint2D())
                    .ToArray();
            }
        }

        private static SerializablePoint2D[] GetSerializableData(IEnumerable<Point2D> points)
        {
            return points.Select(p => new SerializablePoint2D(p)).ToArray();
        }

        [Serializable]
        private class SerializablePoint2D
        {
            private readonly double x;
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