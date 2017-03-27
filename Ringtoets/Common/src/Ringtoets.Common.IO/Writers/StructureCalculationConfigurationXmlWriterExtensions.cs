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
using System.Xml;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Schema;

namespace Ringtoets.Common.IO.Writers
{
    /// <summary>
    /// Extension methods for an <see cref="XmlWriter"/>, for writing <see cref="StructureCalculationConfiguration"/>
    /// in XML format to file.
    /// </summary>
    public static class StructureCalculationConfigurationXmlWriterExtensions
    {
        /// <summary>
        /// Action which is performed for writing parameters specific to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="StructureCalculationConfiguration"/> of
        /// which parameters are written.</typeparam>
        /// <param name="configuration">The instance of type <typeparamref name="T"/> for which
        /// to write the parameters.</param>
        /// <param name="writer">The writer that should be used to write the parameters.</param>
        public delegate void WriteSpecificStructureParameters<in T>(T configuration, XmlWriter writer);

        /// <summary>
        /// Writes the properties of structure configurations to file.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="StructureCalculationConfiguration"/> of
        /// which parameters are written.</typeparam>
        /// <param name="writer">The writer to use for writing out the configuration</param>
        /// <param name="configuration">The structure configuration properties to write.</param>
        /// <param name="writeProperties">Action which writes properties specific for a structure of type <typeparamref name="T"/>.</param>
        /// <param name="writeStochasts">Action which writes stochats definitions specific for a structure of type <typeparamref name="T"/>.</param>
        public static void WriteStructure<T>(
            this XmlWriter writer, 
            T configuration, 
            WriteSpecificStructureParameters<T> writeProperties,
            WriteSpecificStructureParameters<T> writeStochasts)
            where T : StructureCalculationConfiguration
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (writeProperties == null)
            {
                throw new ArgumentNullException(nameof(writeProperties));
            }
            if (writeStochasts == null)
            {
                throw new ArgumentNullException(nameof(writeStochasts));
            }

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.CalculationElement);
            writer.WriteAttributeString(ConfigurationSchemaIdentifiers.NameAttribute, configuration.Name);

            if (configuration.HydraulicBoundaryLocationName != null)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                    configuration.HydraulicBoundaryLocationName);
            }

            if (configuration.StructureName != null)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.StructureElement,
                    configuration.StructureName);
            }

            if (configuration.StructureNormalOrientation.HasValue)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.Orientation,
                    XmlConvert.ToString(configuration.StructureNormalOrientation.Value));
            }

            if (configuration.FailureProbabilityStructureWithErosion.HasValue)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement,
                    XmlConvert.ToString(configuration.FailureProbabilityStructureWithErosion.Value));
            }

            if (configuration.ForeshoreProfileName != null)
            {
                writer.WriteElementString(
                    ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement,
                    configuration.ForeshoreProfileName);
            }

            if (configuration.WaveReduction != null)
            {
                writer.WriteWaveReduction(configuration.WaveReduction);
            }

            writeProperties(configuration, writer);

            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

            if (configuration.FlowWidthAtBottomProtection != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName, configuration.FlowWidthAtBottomProtection);
            }
            if (configuration.WidthFlowApertures != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName, configuration.WidthFlowApertures);
            }
            if (configuration.StorageStructureArea != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName, configuration.StorageStructureArea);
            }
            if (configuration.CriticalOvertoppingDischarge != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName, configuration.CriticalOvertoppingDischarge);
            }
            if (configuration.ModelFactorSuperCriticalFlow != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName, configuration.ModelFactorSuperCriticalFlow);
            }
            if (configuration.AllowedLevelIncreaseStorage != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName, configuration.AllowedLevelIncreaseStorage);
            }
            if (configuration.StormDuration != null)
            {
                writer.WriteDistribution(ConfigurationSchemaIdentifiers.StormDurationStochastName, configuration.StormDuration);
            }

            writeStochasts(configuration, writer);

            writer.WriteEndElement();
        }
    }
}