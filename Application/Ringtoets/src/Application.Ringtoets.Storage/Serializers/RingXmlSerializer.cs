// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Application.Ringtoets.Storage.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="Ring"/> and an
    /// XML representation of that data.
    /// </summary>
    internal class RingXmlSerializer : DataCollectionSerializer<Ring, RingXmlSerializer.SerializableRing>
    {
        protected override SerializableRing[] ToSerializableData(IEnumerable<Ring> elements)
        {
            return elements.Select(r => new SerializableRing(r)).ToArray();
        }

        protected override Ring[] FromSerializableData(IEnumerable<SerializableRing> serializedElements)
        {
            return serializedElements.Select(sr => sr.ToRing()).ToArray();
        }

        [Serializable]
        internal class SerializableRing
        {
            private readonly IEnumerable<Point2DXmlSerializer.SerializablePoint2D> points;

            public SerializableRing(Ring ring)
            {
                points = ring.Points
                             .Select(p => new Point2DXmlSerializer.SerializablePoint2D(p))
                             .ToArray();
            }

            public Ring ToRing()
            {
                return new Ring(points.Select(p => p.ToPoint2D()));
            }
        }
    }
}