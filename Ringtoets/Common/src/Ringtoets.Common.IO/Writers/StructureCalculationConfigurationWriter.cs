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
    /// Writer for writing <see cref="StructuresCalculationConfiguration"/> in XML format to file.
    /// </summary>
    public abstract class StructureCalculationConfigurationWriter<T> : SchemaCalculationConfigurationWriter<T>
        where T : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="SchemaCalculationConfigurationWriter{T}"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to write to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <remarks>A valid path:
        /// <list type="bullet">
        /// <item>is not empty or <c>null</c>,</item>
        /// <item>does not consist out of only whitespace characters,</item>
        /// <item>does not contain an invalid character,</item>
        /// <item>does not end with a directory or path separator (empty file name).</item>
        /// </list></remarks>
        protected StructureCalculationConfigurationWriter(string filePath) : base(filePath) {}

        protected override void WriteCalculation(T configuration, XmlWriter writer)
        {
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

            WriteSpecificStructureParameters(configuration, writer);

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

            WriteSpecificStochasts(configuration, writer);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes properties specific for a structure of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The instance of type <typeparamref name="T"/> for which
        /// to write the input.</param>
        /// <param name="writer">The writer that should be used to write the parameters.</param>
        protected abstract void WriteSpecificStructureParameters(T configuration, XmlWriter writer);

        /// <summary>        
        /// Writes stochats definitions specific for a structure of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The instance of type <typeparamref name="T"/> for which
        /// to write the stochasts.</param>
        /// <param name="writer">The writer that should be used to write the parameters.</param>
        protected abstract void WriteSpecificStochasts(T configuration, XmlWriter writer);
    }
}