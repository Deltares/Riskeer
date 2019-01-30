// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Xml.Serialization;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model.DataTypes
{
    /// <summary>
    /// Class describing a serializable measure.
    /// </summary>
    public class SerializableMeasure
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableMeasure"/>.
        /// </summary>
        public SerializableMeasure()
        {
            Value = double.NaN;
            UnitOfMeasure = Resources.SerializableMeasure_Meter;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableMeasure"/>.
        /// </summary>
        /// <param name="value">The value of the measure in meters.</param>
        public SerializableMeasure(double value) : this()
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the unit of measure.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.UnitOfMeasure)]
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the value of the measure.
        /// [m]
        /// </summary>
        [XmlText]
        public double Value { get; set; }
    }
}