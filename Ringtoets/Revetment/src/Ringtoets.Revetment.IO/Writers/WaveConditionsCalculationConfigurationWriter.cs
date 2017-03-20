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

using System.Xml;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Common.IO.Writers;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Writers
{
    /// <summary>
    /// Base implementation of a writer for calculations that contain <see cref="WaveConditionsInput"/> as input,
    /// to XML format.
    /// </summary>
    /// <typeparam name="T">The type of calculations that are written to file.</typeparam>
    public abstract class WaveConditionsCalculationConfigurationWriter<T> : CalculationConfigurationWriter<T> where T : class, ICalculation
    {
        private readonly WaveConditionsInputStepSizeConverter waveConditionsInputStepSizeConverter;

        /// <summary>
        /// Created a new instance of <see cref="WaveConditionsCalculationConfigurationWriter{T}"/>.
        /// </summary>
        protected WaveConditionsCalculationConfigurationWriter()
        {
            waveConditionsInputStepSizeConverter = new WaveConditionsInputStepSizeConverter();
        }

        /// <summary>
        /// Writes a single calculation with its <paramref name="input"/> in XML format to file.
        /// </summary>
        /// <param name="name">The name of the calculation to write.</param>
        /// <param name="input">The input of the calculation to write.</param>
        /// <param name="writer">The writer to use for writing.</param>
        protected void WriteCalculation(string name, WaveConditionsInput input, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, name);

            WriteHydraulicBoundaryLocation(input.HydraulicBoundaryLocation, writer);

            writer.WriteElementString(
                WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryRevetment,
                XmlConvert.ToString(input.UpperBoundaryRevetment));
            writer.WriteElementString(
                WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryRevetment,
                XmlConvert.ToString(input.LowerBoundaryRevetment));
            writer.WriteElementString(
                WaveConditionsCalculationConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels,
                XmlConvert.ToString(input.UpperBoundaryWaterLevels));
            writer.WriteElementString(
                WaveConditionsCalculationConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels,
                XmlConvert.ToString(input.LowerBoundaryWaterLevels));
            writer.WriteElementString(
                WaveConditionsCalculationConfigurationSchemaIdentifiers.StepSize,
                waveConditionsInputStepSizeConverter.ConvertToInvariantString(input.StepSize));

            WriteForeshoreProfile(input.ForeshoreProfile, writer);

            writer.WriteElementString(
                ConfigurationSchemaIdentifiers.Orientation,
                XmlConvert.ToString(input.Orientation));

            WriteWaveReduction(input, writer);

            writer.WriteEndElement();
        }

        private static void WriteHydraulicBoundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation, XmlWriter writer)
        {
            if (hydraulicBoundaryLocation != null)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                    hydraulicBoundaryLocation.Name);
            }
        }

        private static void WriteForeshoreProfile(ForeshoreProfile foreshoreProfile, XmlWriter writer)
        {
            if (foreshoreProfile != null)
            {
                writer.WriteElementString(
                    WaveConditionsCalculationConfigurationSchemaIdentifiers.ForeshoreProfile,
                    foreshoreProfile.Name);
            }
        }

        private static void WriteWaveReduction(WaveConditionsInput input, XmlWriter writer)
        {
            if (input.ForeshoreProfile != null)
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
}