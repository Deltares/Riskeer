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
using System.Runtime.Serialization;
using Core.Common.Base.Data;

namespace Ringtoets.Storage.Core.Serializers
{
    /// <summary>
    /// Converter class that converts between tangent lines and an XML representation of that data.
    /// </summary>
    internal class TangentLineCollectionXmlSerializer : DataCollectionSerializer<RoundedDouble, TangentLineCollectionXmlSerializer.SerializableTangentLine>
    {
        protected override SerializableTangentLine[] ToSerializableData(IEnumerable<RoundedDouble> elements)
        {
            return elements.Select(e => new SerializableTangentLine(e)).ToArray();
        }

        protected override RoundedDouble[] FromSerializableData(IEnumerable<SerializableTangentLine> serializedElements)
        {
            return serializedElements.Select(se => se.ToTangentLine()).ToArray();
        }

        [Serializable]
        [DataContract(Name = nameof(SerializableTangentLine), Namespace = "")]
        internal class SerializableTangentLine
        {
            [DataMember]
            private readonly double tangentLine;

            /// <summary>
            /// Creates a new instance of <see cref="SerializableTangentLine"/>.
            /// </summary>
            /// <param name="tangentLine">The <see cref="double"/> to base the 
            /// <see cref="SerializableTangentLine"/> on.</param>
            public SerializableTangentLine(double tangentLine)
            {
                this.tangentLine = tangentLine;
            }

            public RoundedDouble ToTangentLine()
            {
                return (RoundedDouble) tangentLine;
            }
        }
    }
}