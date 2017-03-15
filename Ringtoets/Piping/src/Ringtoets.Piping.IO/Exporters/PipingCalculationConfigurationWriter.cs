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

using System.Collections.Generic;
using System.Xml;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Common.IO.Writers;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Schema;

namespace Ringtoets.Piping.IO.Exporters
{
    /// <summary>
    /// Writer for writing a piping calculation configuration to XML.
    /// </summary>
    public class PipingCalculationConfigurationWriter : CalculationConfigurationWriter<PipingCalculation>
    {
        protected override void WriteCalculation(PipingCalculation calculation, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, calculation.Name);

            PipingInput calculationInputParameters = calculation.InputParameters;

            if (calculationInputParameters.UseAssessmentLevelManualInput)
            {
                writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement,
                                          XmlConvert.ToString(calculationInputParameters.AssessmentLevel));
            }
            else if (calculationInputParameters.HydraulicBoundaryLocation != null)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                          calculationInputParameters.HydraulicBoundaryLocation.Name);
            }

            if (calculationInputParameters.SurfaceLine != null)
            {
                writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.SurfaceLineElement,
                                          calculationInputParameters.SurfaceLine.Name);
                writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.EntryPointLElement,
                                          XmlConvert.ToString(calculationInputParameters.EntryPointL));
                writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.ExitPointLElement,
                                          XmlConvert.ToString(calculationInputParameters.ExitPointL));
            }

            if (calculationInputParameters.StochasticSoilModel != null)
            {
                writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement,
                                          calculationInputParameters.StochasticSoilModel.Name);

                if (calculationInputParameters.StochasticSoilProfile?.SoilProfile != null)
                {
                    writer.WriteElementString(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement,
                                              calculationInputParameters.StochasticSoilProfile.SoilProfile.Name);
                }
            }

            WriteDistributions(CreateInputDistributions(calculationInputParameters), writer);

            writer.WriteEndElement();
        }

        private static IDictionary<string, IDistribution> CreateInputDistributions(PipingInput calculationInputParameters)
        {
            return new Dictionary<string, IDistribution>
            {
                {
                    PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName,
                    calculationInputParameters.PhreaticLevelExit
                },
                {
                    PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName,
                    calculationInputParameters.DampingFactorExit
                }
            };
        }
    }
}