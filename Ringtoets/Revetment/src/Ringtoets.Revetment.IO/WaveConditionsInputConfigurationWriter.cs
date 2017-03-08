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

using System.ComponentModel;
using System.Globalization;
using System.Xml;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Common.IO.Writers;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO
{
    /// <summary>
    /// Base implementation of a writer for calculations that contain <see cref="WaveConditionsInput"/> as input,
    /// to XML format.
    /// </summary>
    /// <typeparam name="T">The type of calculations that are written to file.</typeparam>
    public abstract class WaveConditionsInputConfigurationWriter<T> : CalculationConfigurationWriter<T> where T : class, ICalculation
    {
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

            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                input.HydraulicBoundaryLocation.Name);
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.UpperBoundaryRevetment,
                XmlConvert.ToString(input.UpperBoundaryRevetment));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.LowerBoundaryRevetment,
                XmlConvert.ToString(input.LowerBoundaryRevetment));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.UpperBoundaryWaterLevels,
                XmlConvert.ToString(input.UpperBoundaryWaterLevels));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.LowerBoundaryWaterLevels,
                XmlConvert.ToString(input.LowerBoundaryWaterLevels));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.StepSize,
                string.Format(CultureInfo.InvariantCulture, "{0:0.0}", input.StepSize.AsValue()));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.ForeshoreProfile,
                input.ForeshoreProfile.Name);
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.Orientation,
                XmlConvert.ToString(input.Orientation));

            WriteWaveReduction(input, writer);

            writer.WriteEndElement();
        }

        private static void WriteWaveReduction(WaveConditionsInput input, XmlWriter writer)
        {
            writer.WriteStartElement(WaveConditionsInputConfigurationSchemaIdentifiers.WaveReduction);

            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.UseBreakWater,
                XmlConvert.ToString(input.UseBreakWater));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.BreakWaterType,
                BreakWaterTypeAsXmlString(input.BreakWater.Type));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.BreakWaterHeight,
                XmlConvert.ToString(input.BreakWater.Height));
            writer.WriteElementString(
                WaveConditionsInputConfigurationSchemaIdentifiers.UseForeshore,
                XmlConvert.ToString(input.UseForeshore));

            writer.WriteEndElement();
        }

        private static string BreakWaterTypeAsXmlString(BreakWaterType type)
        {
            switch (type)
            {
                case BreakWaterType.Caisson:
                    return "caisson";
                case BreakWaterType.Dam:
                    return "havendam";
                case BreakWaterType.Wall:
                    return "verticalewand";
                default:
                    throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(BreakWaterType));
            }
        }
    }
}