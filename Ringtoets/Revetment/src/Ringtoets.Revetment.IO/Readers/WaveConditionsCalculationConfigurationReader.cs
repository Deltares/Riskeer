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
using System.Xml;
using System.Xml.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Revetment.IO.Properties;
using RingtoestCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Revetment.IO.Readers
{
    /// <summary>
    /// This class reads a wave conditions configuration from XML and creates a collection of corresponding
    /// <see cref="IReadConfigurationItem"/>, typically containing one or more <see cref="ReadWaveConditionsCalculation"/>.
    /// </summary>
    internal class WaveConditionsCalculationConfigurationReader : CalculationConfigurationReader<ReadWaveConditionsCalculation>
    {
        private const string hydraulicBoundaryLocationSchemaName = "HrLocatieSchema.xsd";
        private const string orientationSchemaName = "OrientatieSchema.xsd";
        private const string foreshoreProfileSchemaName = "VoorlandProfielSchema.xsd";
        private const string waveReductionSchemaName = "GolfReductieSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="WaveConditionsCalculationConfigurationReader"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        public WaveConditionsCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   Resources.BekledingenHrConfiguratieSchema,
                   new Dictionary<string, string>
                   {
                       {
                           hydraulicBoundaryLocationSchemaName, RingtoestCommonIOResources.HrLocatieSchema
                       },
                       {
                           orientationSchemaName, RingtoestCommonIOResources.OrientatieSchema
                       },
                       {
                           foreshoreProfileSchemaName, RingtoestCommonIOResources.VoorlandProfielSchema
                       },
                       {
                           waveReductionSchemaName, RingtoestCommonIOResources.GolfReductieSchema
                       }
                   }) {}

        protected override ReadWaveConditionsCalculation ParseCalculationElement(XElement calculationElement)
        {
            var constructionProperties = new ReadWaveConditionsCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                HydraulicBoundaryLocation = CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                                                     ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                UpperBoundaryRevetment = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                                  WaveConditionsConfigurationSchemaIdentifiers.UpperBoundaryRevetment),
                LowerBoundaryRevetment = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                                  WaveConditionsConfigurationSchemaIdentifiers.LowerBoundaryRevetment),
                UpperBoundaryWaterLevels = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                                    WaveConditionsConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels),
                LowerBoundaryWaterLevels = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                                    WaveConditionsConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels),
                StepSize = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                    WaveConditionsConfigurationSchemaIdentifiers.StepSize),
                ForeshoreProfile = CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                                            WaveConditionsConfigurationSchemaIdentifiers.ForeshoreProfile),
                Orientation = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                       ConfigurationSchemaIdentifiers.Orientation),
                UseBreakWater = GetBoolValueFromChildElement(calculationElement,
                                                             ConfigurationSchemaIdentifiers.UseBreakWater),
                BreakWaterType = GetBreakWaterType(calculationElement,
                                                   ConfigurationSchemaIdentifiers.BreakWaterType),
                BreakWaterHeight = CalculationConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                                            ConfigurationSchemaIdentifiers.BreakWaterHeight),
                UseForeshore = GetBoolValueFromChildElement(calculationElement,
                                                            ConfigurationSchemaIdentifiers.UseForeshore)
            };

            return new ReadWaveConditionsCalculation(constructionProperties);
        }

        private static ReadBreakWaterType GetBreakWaterType(XElement parentElement, string childElementName)
        {
            string element = CalculationConfigurationReaderHelper.GetStringValueFromDescendantElement(parentElement, childElementName);

            switch (element)
            {
                case ConfigurationSchemaIdentifiers.BreakWaterCaisson:
                    return ReadBreakWaterType.Caisson;
                case ConfigurationSchemaIdentifiers.BreakWaterDam:
                    return ReadBreakWaterType.HarborDam;
                case ConfigurationSchemaIdentifiers.BreakWaterWall:
                    return ReadBreakWaterType.Vertical;
                default:
                    return ReadBreakWaterType.None;
            }
        }

        private static bool? GetBoolValueFromChildElement(XElement parentElement, string childElementName)
        {
            XElement descendantElement = CalculationConfigurationReaderHelper.GetDescendantElement(parentElement, childElementName);

            return descendantElement != null
                       ? (bool?) XmlConvert.ToBoolean(descendantElement.Value)
                       : null;
        }
    }
}