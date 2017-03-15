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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.IO.Exporters
{
    /// <summary>
    /// Exports a piping configuration and stores it as an XML file.
    /// </summary>
    public class PipingConfigurationExporter : ConfigurationExporter<PipingCalculationConfigurationWriter, PipingCalculation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingConfigurationExporter"/>.
        /// </summary>
        /// <param name="configuration">The configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public PipingConfigurationExporter(IEnumerable<ICalculationBase> configuration, string filePath) : base(configuration, filePath)
        {}
    }
}