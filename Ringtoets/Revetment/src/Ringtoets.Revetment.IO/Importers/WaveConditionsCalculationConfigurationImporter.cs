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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Readers;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Revetment.IO.Importers
{
    /// <summary>
    /// Imports a wave conditions calculation configuration from an XML file and stores it on a
    /// <see cref="CalculationGroup"/>.
    /// </summary>
    /// <typeparam name="T">The type of the calculation to import.</typeparam>
    public class WaveConditionsCalculationConfigurationImporter<T>
        : CalculationConfigurationImporter<WaveConditionsCalculationConfigurationReader, ReadWaveConditionsCalculation>
        where T : IWaveConditionsCalculation, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationImporter{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The path to the XML file to import from.</param>
        /// <param name="importTarget">The calculation group to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is
        /// <c>null</c>.</exception>
        public WaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                              CalculationGroup importTarget)
            : base(xmlFilePath, importTarget) {}

        protected override WaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(string xmlFilePath)
        {
            return new WaveConditionsCalculationConfigurationReader(xmlFilePath);
        }

        protected override ICalculation ParseReadCalculation(ReadWaveConditionsCalculation readCalculation)
        {
            var calculation = new T
            {
                Name = readCalculation.Name
            };

            return calculation;
        }
    }
}