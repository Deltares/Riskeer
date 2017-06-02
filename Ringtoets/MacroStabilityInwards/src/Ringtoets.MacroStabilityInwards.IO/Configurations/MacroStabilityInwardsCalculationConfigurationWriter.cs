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

using System.Xml;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Writer for writing a macro stability inwards calculation configuration to XML.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfigurationWriter : CalculationConfigurationWriter<MacroStabilityInwardsCalculation>
    {
        protected override void WriteCalculation(MacroStabilityInwardsCalculation calculation, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, calculation.Name);

            MacroStabilityInwardsInput calculationInputParameters = calculation.InputParameters;

            if (calculationInputParameters.UseAssessmentLevelManualInput)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement,
                                          XmlConvert.ToString(calculationInputParameters.AssessmentLevel));
            }
            else if (calculationInputParameters.HydraulicBoundaryLocation != null)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                          calculationInputParameters.HydraulicBoundaryLocation.Name);
            }

            if (calculationInputParameters.SurfaceLine != null)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.SurfaceLineElement,
                                          calculationInputParameters.SurfaceLine.Name);
            }

            if (calculationInputParameters.StochasticSoilModel != null)
            {
                writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement,
                                          calculationInputParameters.StochasticSoilModel.Name);

                if (calculationInputParameters.StochasticSoilProfile?.SoilProfile != null)
                {
                    writer.WriteElementString(MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement,
                                              calculationInputParameters.StochasticSoilProfile.SoilProfile.Name);
                }
            }

            writer.WriteEndElement();
        }
    }
}