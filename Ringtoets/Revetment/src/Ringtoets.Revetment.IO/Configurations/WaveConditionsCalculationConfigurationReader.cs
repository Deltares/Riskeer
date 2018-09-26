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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Revetment.IO.Configurations.Converters;
using Ringtoets.Revetment.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Revetment.IO.Configurations
{
    /// <summary>
    /// This class reads a wave conditions calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="WaveConditionsCalculationConfiguration"/>.
    /// </summary>
    /// <typeparam name="T">The type of the calculation configuration.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationReader<T> : CalculationConfigurationReader<T>
        where T : WaveConditionsCalculationConfiguration
    {
        private const string hbLocationSchemaName = "HbLocatieSchema.xsd";
        private const string orientationSchemaName = "OrientatieSchema.xsd";
        private const string foreshoreProfileSchemaName = "VoorlandProfielSchema.xsd";
        private const string waveReductionSchemaName = "GolfReductieSchema.xsd";
        private const string revetmentBaseSchemaName = "BekledingenConfiguratieBasisSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationReader{T}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// <item><paramref name="mainSchemaDefinition"/> is invalid.</item>
        /// </list>
        /// </exception>
        protected WaveConditionsCalculationConfigurationReader(string xmlFilePath, string mainSchemaDefinition)
            : base(xmlFilePath, mainSchemaDefinition,
                   new Dictionary<string, string>
                   {
                       {
                           revetmentBaseSchemaName, Resources.BekledingenConfiguratieBasisSchema
                       },
                       {
                           hbLocationSchemaName, RingtoetsCommonIOResources.HbLocatieSchema
                       },
                       {
                           orientationSchemaName, RingtoetsCommonIOResources.OrientatieSchema
                       },
                       {
                           foreshoreProfileSchemaName, RingtoetsCommonIOResources.VoorlandProfielSchema
                       },
                       {
                           waveReductionSchemaName, RingtoetsCommonIOResources.GolfReductieSchema
                       }
                   }) {}

        protected abstract override T ParseCalculationElement(XElement calculationElement);

        protected void ParseCalculationElementData(XElement calculationElement, T configuration)
        {
            configuration.HydraulicBoundaryLocationName = calculationElement.GetHydraulicBoundaryLocationName();
            configuration.UpperBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryRevetment);
            configuration.LowerBoundaryRevetment = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryRevetment);
            configuration.UpperBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels);
            configuration.LowerBoundaryWaterLevels = calculationElement.GetDoubleValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels);
            configuration.StepSize = (ConfigurationWaveConditionsInputStepSize?) calculationElement.GetConvertedValueFromDescendantDoubleElement<ConfigurationWaveConditionsInputStepSizeConverter>(WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize);
            configuration.ForeshoreProfileId = calculationElement.GetStringValueFromDescendantElement(WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile);
            configuration.Orientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation);
            configuration.WaveReduction = calculationElement.GetWaveReductionParameters();
        }
    }
}