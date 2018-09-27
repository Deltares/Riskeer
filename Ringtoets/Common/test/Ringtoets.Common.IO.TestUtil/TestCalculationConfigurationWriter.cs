// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Xml;
using Ringtoets.Common.IO.Configurations.Export;

namespace Ringtoets.Common.IO.TestUtil
{
    /// <summary>
    /// Simple <see cref="CalculationConfigurationWriter{T}"/> that can be used for testing purposes.
    /// </summary>
    public class TestCalculationConfigurationWriter : CalculationConfigurationWriter<TestConfigurationItem>
    {
        private const string calculationElementTag = "calculation";

        /// <summary>
        /// Creates a new instance of <see cref="TestCalculationConfigurationWriter"/>
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public TestCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(TestConfigurationItem calculation, XmlWriter writer)
        {
            writer.WriteElementString(calculationElementTag, calculation.Name);
        }
    }
}