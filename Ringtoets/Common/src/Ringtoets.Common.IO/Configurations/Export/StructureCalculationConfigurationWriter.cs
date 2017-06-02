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
using System.Xml;

namespace Ringtoets.Common.IO.Configurations.Export
{
    /// <summary>
    /// Writer for writing <see cref="StructuresCalculationConfiguration"/> in XML format to file.
    /// </summary>
    public abstract class StructureCalculationConfigurationWriter<T> : CalculationConfigurationWriter<T>
        where T : StructuresCalculationConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationWriter{T}"/>.
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

            WriteParameters(configuration, writer);

            WriteWaveReductionWhenAvailable(writer, configuration.WaveReduction);

            WriteStochasts(configuration, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes properties specific for a structure of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The instance of type <typeparamref name="T"/> for which
        /// to write the input.</param>
        /// <param name="writer">The writer that should be used to write the parameters.</param>
        protected virtual void WriteSpecificStructureParameters(T configuration, XmlWriter writer) {}

        /// <summary>        
        /// Writes stochasts definitions specific for a structure of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="configuration">The instance of type <typeparamref name="T"/> for which
        /// to write the stochasts.</param>
        /// <param name="writer">The writer that should be used to write the parameters.</param>
        protected abstract void WriteSpecificStochasts(T configuration, XmlWriter writer);

        private void WriteParameters(T configuration, XmlWriter writer)
        {
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement,
                                             configuration.HydraulicBoundaryLocationName);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.StructureElement,
                                             configuration.StructureId);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.Orientation,
                                             configuration.StructureNormalOrientation);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement,
                                             configuration.FailureProbabilityStructureWithErosion);
            WriteElementWhenContentAvailable(writer,
                                             ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement,
                                             configuration.ForeshoreProfileId);

            WriteSpecificStructureParameters(configuration, writer);
        }

        private void WriteStochasts(T configuration, XmlWriter writer)
        {
            writer.WriteStartElement(ConfigurationSchemaIdentifiers.StochastsElement);

            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName,
                                           configuration.FlowWidthAtBottomProtection);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName,
                                           configuration.WidthFlowApertures);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName,
                                           configuration.StorageStructureArea);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName,
                                           configuration.CriticalOvertoppingDischarge);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName,
                                           configuration.ModelFactorSuperCriticalFlow);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName,
                                           configuration.AllowedLevelIncreaseStorage);
            WriteDistributionWhenAvailable(writer,
                                           ConfigurationSchemaIdentifiers.StormDurationStochastName,
                                           configuration.StormDuration);

            WriteSpecificStochasts(configuration, writer);

            writer.WriteEndElement();
        }
    }
}