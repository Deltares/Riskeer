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
using Ringtoets.Piping.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Configurations
{
    /// <summary>
    /// This class reads a piping calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="PipingCalculationConfiguration"/>.
    /// </summary>
    public class PipingCalculationConfigurationReader
        : CalculationConfigurationReader<PipingCalculationConfiguration>
    {
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";
        private const string scenarioSchemaName = "ScenarioSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationReader"/>.
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
        internal PipingCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   Resources.PipingConfiguratieSchema,
                   new Dictionary<string, string>
                   {
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       },
                       {
                           stochastStandaardafwijkingSchemaName, RingtoetsCommonIOResources.StochastStandaardafwijkingSchema
                       },
                       {
                           scenarioSchemaName, RingtoetsCommonIOResources.ScenarioSchema
                       }
                   }) {}

        protected override PipingCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            return new PipingCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                AssessmentLevel = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElementOld),
                SurfaceLineName = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.SurfaceLineElement),
                EntryPointL = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.EntryPointLElement),
                ExitPointL = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.ExitPointLElement),
                StochasticSoilModelName = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfileName = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement),
                PhreaticLevelExit = calculationElement.GetStochastConfiguration(PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName),
                DampingFactorExit = calculationElement.GetStochastConfiguration(PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName),
                Scenario = calculationElement.GetScenarioConfiguration()
            };
        }
    }
}