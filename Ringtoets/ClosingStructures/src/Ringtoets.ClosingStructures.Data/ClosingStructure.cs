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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using BaseConstructionProperties = Ringtoets.Common.Data.StructureBase.ConstructionProperties;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// Definition of a closing structure for the <see cref="ClosingStructuresFailureMechanism"/>
    /// </summary>
    public class ClosingStructure : StructureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingStructure"/> class.
        /// </summary>
        /// <param name="constructionProperties">The construction properties.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="ConstructionProperties.Name"/>
        /// or <see cref="ConstructionProperties.Id"/> is <c>null</c>, empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="ConstructionProperties.Location"/> is <c>null</c>.</exception>
        public ClosingStructure(ConstructionProperties constructionProperties) : base(constructionProperties)
        {
            StorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.StorageStructureArea.Mean,
                CoefficientOfVariation = constructionProperties.StorageStructureArea.CoefficientOfVariation
            };
            AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.AllowedLevelIncreaseStorage.Mean,
                StandardDeviation = constructionProperties.AllowedLevelIncreaseStorage.StandardDeviation
            };
            WidthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                CoefficientOfVariation = constructionProperties.WidthFlowApertures.CoefficientOfVariation
            };
            LevelCrestStructureNotClosing = new NormalDistribution(2)
            {
                Mean = constructionProperties.LevelCrestStructureNotClosing.Mean,
                StandardDeviation = constructionProperties.LevelCrestStructureNotClosing.StandardDeviation
            };
            InsideWaterLevel = new NormalDistribution(2)
            {
                Mean = constructionProperties.InsideWaterLevel.Mean,
                StandardDeviation = constructionProperties.InsideWaterLevel.StandardDeviation
            };
            ThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = constructionProperties.ThresholdHeightOpenWeir.Mean,
                StandardDeviation = constructionProperties.ThresholdHeightOpenWeir.StandardDeviation
            };
            AreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.AreaFlowApertures.Mean,
                StandardDeviation = constructionProperties.AreaFlowApertures.StandardDeviation
            };
            CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.CriticalOvertoppingDischarge.Mean,
                CoefficientOfVariation = constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.FlowWidthAtBottomProtection.Mean,
                StandardDeviation = constructionProperties.FlowWidthAtBottomProtection.StandardDeviation
            };
            ProbabilityOrFrequencyOpenStructureBeforeFlooding = constructionProperties.ProbabilityOrFrequencyOpenStructureBeforeFlooding;
            FailureProbabilityOpenStructure = constructionProperties.FailureProbabilityOpenStructure;
            IdenticalApertures = constructionProperties.IdenticalApertures;
            FailureProbabilityReparation = constructionProperties.FailureProbabilityReparation;
            InflowModelType = constructionProperties.InflowModelType;
        }

        /// <summary>
        /// Gets the storage area of the closing structure.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the closing structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

        /// <summary>
        /// Gets the width of the flow apertures of the closing structure.
        /// [m]
        /// </summary>
        public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

        /// <summary>
        /// Gets the crest level of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructureNotClosing { get; private set; }

        /// <summary>
        /// Gets the interior water level of the closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; private set; }

        /// <summary>
        /// Gets the threshold height of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

        /// <summary>
        /// Gets the area of the flow aperture of the closing structure.
        /// [m^2]
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge per meter of the closing structure.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow width of the closing structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the probability or frequency of the closing structure being open before flooding.
        /// [1/year]
        /// </summary>
        /// <remarks>Because this property can also be used to denote a frequency, there
        /// is no guarantee that this property returns a value in the range [0.0, 1.0]
        /// nor that formal rules of probability apply.</remarks>
        public double ProbabilityOrFrequencyOpenStructureBeforeFlooding { get; private set; }

        /// <summary>
        /// Gets the probability of failing to close the closing structure.
        /// [1/year]
        /// </summary>
        public double FailureProbabilityOpenStructure { get; private set; }

        /// <summary>
        /// Gets the number of identical apertures of the closing structure.
        /// </summary>
        public int IdenticalApertures { get; private set; }

        /// <summary>
        /// Gets the probability of failing to repair a failed closure of the closing structure.
        /// [1/year]
        /// </summary>
        public double FailureProbabilityReparation { get; private set; }

        /// <summary>
        /// Gets the type of closing structure inflow model.
        /// </summary>
        public ClosingStructureInflowModelType InflowModelType { get; private set; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ClosingStructure"/>.
        /// </summary>
        public new class ConstructionProperties : BaseConstructionProperties
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructionProperties"/> class.
            /// </summary>
            public ConstructionProperties()
            {
                StorageStructureArea = new VariationCoefficientLogNormalDistribution(2);
                AllowedLevelIncreaseStorage = new LogNormalDistribution(2);
                WidthFlowApertures = new VariationCoefficientNormalDistribution(2);
                LevelCrestStructureNotClosing = new NormalDistribution(2);
                InsideWaterLevel = new NormalDistribution(2);
                ThresholdHeightOpenWeir = new NormalDistribution(2);
                AreaFlowApertures = new LogNormalDistribution(2);
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2);
                FlowWidthAtBottomProtection = new LogNormalDistribution(2);
            }

            /// <summary>
            /// Gets the storage area of the closing structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the closing structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

            /// <summary>
            /// Gets the width of the flow apertures of the closing structure.
            /// [m]
            /// </summary>
            public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

            /// <summary>
            /// Gets the crest level of the opened closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructureNotClosing { get; private set; }

            /// <summary>
            /// Gets the interior water level of the closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevel { get; private set; }

            /// <summary>
            /// Gets the threshold height of the opened closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

            /// <summary>
            /// Gets the area of the flow aperture of the closing structure.
            /// [m^2]
            /// </summary>
            public LogNormalDistribution AreaFlowApertures { get; private set; }

            /// <summary>
            /// Gets the critical overtopping discharge per meter of the closing structure.
            /// [m^3/s/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

            /// <summary>
            /// Gets the flow width of the closing structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

            /// <summary>
            /// Gets the probability or frequency of the closing structure being open before flooding.
            /// [1/year]
            /// </summary>
            /// <remarks>Because this property can also be used to denote a frequency, there
            /// is no guarantee that this property returns a value in the range [0.0, 1.0]
            /// nor that formal rules of probability apply.</remarks>
            public double ProbabilityOrFrequencyOpenStructureBeforeFlooding { get; set; }

            /// <summary>
            /// Gets or sets the probability of failing to close the closing structure.
            /// [1/year]
            /// </summary>
            public double FailureProbabilityOpenStructure { get; set; }

            /// <summary>
            /// Gets or sets the number of identical apertures of the closing structure.
            /// </summary>
            public int IdenticalApertures { get; set; }

            /// <summary>
            /// Gets or sets the probability of failing to repair a failed closure of the closing structure.
            /// [1/year]
            /// </summary>
            public double FailureProbabilityReparation { get; set; }

            /// <summary>
            /// Gets or sets the type of closing structure inflow model.
            /// </summary>
            public ClosingStructureInflowModelType InflowModelType { get; set; }
        }
    }
}