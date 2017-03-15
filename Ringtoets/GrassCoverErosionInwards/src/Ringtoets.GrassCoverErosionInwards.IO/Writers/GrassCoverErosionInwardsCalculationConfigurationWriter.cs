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

using System.Collections.Generic;
using System.Xml;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Common.IO.Writers;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.IO.Writers
{
    /// <summary>
    /// Writer for writing a grass cover erosion inwards configuration to XML.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfigurationWriter : CalculationConfigurationWriter<GrassCoverErosionInwardsCalculation>
    {
        protected override void WriteCalculation(GrassCoverErosionInwardsCalculation calculation, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, calculation.Name);

            GrassCoverErosionInwardsInput input = calculation.InputParameters;

            if (input.HydraulicBoundaryLocation != null)
            {
                writer.WriteElementString(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                          input.HydraulicBoundaryLocation.Name);
            }
            if (input.DikeProfile != null)
            {
                writer.WriteElementString(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                          input.DikeProfile.Name);
            }

            writer.WriteElementString(
                ConfigurationSchemaIdentifiers.Orientation,
                XmlConvert.ToString(input.Orientation));

            WriteWaveReduction(input, writer);

            WriteDistributions(CreateInputDistributions(input), writer);

            writer.WriteEndElement();
        }

        private static IDictionary<string, IDistribution> CreateInputDistributions(GrassCoverErosionInwardsInput calculationInputParameters)
        {
            return new Dictionary<string, IDistribution>
            {
                {
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName,
                    calculationInputParameters.CriticalFlowRate
                }
            };
        }

        private static void WriteWaveReduction(GrassCoverErosionInwardsInput input, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.WaveReduction);

            writer.WriteElementString(
                ConfigurationSchemaIdentifiers.UseBreakWater,
                XmlConvert.ToString(input.UseBreakWater));

            WriteBreakWaterProperties(input.BreakWater, writer);

            writer.WriteElementString(
                ConfigurationSchemaIdentifiers.UseForeshore,
                XmlConvert.ToString(input.UseForeshore));

            writer.WriteEndElement();
        }
    }
}