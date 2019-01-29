// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Probabilistics;
using BaseConstructionProperties = Ringtoets.Common.Data.StructureBase.ConstructionProperties;

namespace Riskeer.ClosingStructures.Data
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
            WidthFlowApertures = new NormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                StandardDeviation = constructionProperties.WidthFlowApertures.StandardDeviation
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
            ProbabilityOpenStructureBeforeFlooding = constructionProperties.ProbabilityOpenStructureBeforeFlooding;
            FailureProbabilityOpenStructure = constructionProperties.FailureProbabilityOpenStructure;
            IdenticalApertures = constructionProperties.IdenticalApertures;
            FailureProbabilityReparation = constructionProperties.FailureProbabilityReparation;
            InflowModelType = constructionProperties.InflowModelType;
        }

        /// <summary>
        /// Gets the storage area of the closing structure.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the closing structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; }

        /// <summary>
        /// Gets the width of the flow apertures of the closing structure.
        /// [m]
        /// </summary>
        public NormalDistribution WidthFlowApertures { get; }

        /// <summary>
        /// Gets the crest level of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructureNotClosing { get; }

        /// <summary>
        /// Gets the interior water level of the closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; }

        /// <summary>
        /// Gets the threshold height of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; }

        /// <summary>
        /// Gets the area of the flow aperture of the closing structure.
        /// [m^2]
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; }

        /// <summary>
        /// Gets the critical overtopping discharge per meter of the closing structure.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

        /// <summary>
        /// Gets the flow width of the closing structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; }

        /// <summary>
        /// Gets the probability of the closing structure being open before flooding.
        /// [1/year]
        /// </summary>
        public double ProbabilityOpenStructureBeforeFlooding { get; private set; }

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
        /// Copies the property values of the <paramref name="fromStructure"/> to the 
        /// <see cref="ClosingStructure"/>.
        /// </summary>
        /// <param name="fromStructure">The <see cref="ClosingStructure"/> to get the property 
        /// values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromStructure"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(ClosingStructure fromStructure)
        {
            base.CopyProperties(fromStructure);

            AllowedLevelIncreaseStorage.Mean = fromStructure.AllowedLevelIncreaseStorage.Mean;
            AllowedLevelIncreaseStorage.StandardDeviation = fromStructure.AllowedLevelIncreaseStorage.StandardDeviation;

            AreaFlowApertures.Mean = fromStructure.AreaFlowApertures.Mean;
            AreaFlowApertures.StandardDeviation = fromStructure.AreaFlowApertures.StandardDeviation;

            CriticalOvertoppingDischarge.Mean = fromStructure.CriticalOvertoppingDischarge.Mean;
            CriticalOvertoppingDischarge.CoefficientOfVariation = fromStructure.CriticalOvertoppingDischarge.CoefficientOfVariation;

            FailureProbabilityOpenStructure = fromStructure.FailureProbabilityOpenStructure;

            FailureProbabilityReparation = fromStructure.FailureProbabilityReparation;

            IdenticalApertures = fromStructure.IdenticalApertures;

            FlowWidthAtBottomProtection.Mean = fromStructure.FlowWidthAtBottomProtection.Mean;
            FlowWidthAtBottomProtection.StandardDeviation = fromStructure.FlowWidthAtBottomProtection.StandardDeviation;

            InflowModelType = fromStructure.InflowModelType;

            InsideWaterLevel.Mean = fromStructure.InsideWaterLevel.Mean;
            InsideWaterLevel.StandardDeviation = fromStructure.InsideWaterLevel.StandardDeviation;

            LevelCrestStructureNotClosing.Mean = fromStructure.LevelCrestStructureNotClosing.Mean;
            LevelCrestStructureNotClosing.StandardDeviation = fromStructure.LevelCrestStructureNotClosing.StandardDeviation;

            ProbabilityOpenStructureBeforeFlooding = fromStructure.ProbabilityOpenStructureBeforeFlooding;

            StorageStructureArea.Mean = fromStructure.StorageStructureArea.Mean;
            StorageStructureArea.CoefficientOfVariation = fromStructure.StorageStructureArea.CoefficientOfVariation;

            ThresholdHeightOpenWeir.Mean = fromStructure.ThresholdHeightOpenWeir.Mean;
            ThresholdHeightOpenWeir.StandardDeviation = fromStructure.ThresholdHeightOpenWeir.StandardDeviation;

            WidthFlowApertures.Mean = fromStructure.WidthFlowApertures.Mean;
            WidthFlowApertures.StandardDeviation = fromStructure.WidthFlowApertures.StandardDeviation;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && Equals((ClosingStructure) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowedLevelIncreaseStorage.GetHashCode();
                hashCode = (hashCode * 397) ^ AreaFlowApertures.GetHashCode();
                hashCode = (hashCode * 397) ^ CriticalOvertoppingDischarge.GetHashCode();
                hashCode = (hashCode * 397) ^ FlowWidthAtBottomProtection.GetHashCode();
                hashCode = (hashCode * 397) ^ InsideWaterLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ LevelCrestStructureNotClosing.GetHashCode();
                hashCode = (hashCode * 397) ^ StorageStructureArea.GetHashCode();
                hashCode = (hashCode * 397) ^ ThresholdHeightOpenWeir.GetHashCode();
                hashCode = (hashCode * 397) ^ WidthFlowApertures.GetHashCode();

                return hashCode;
            }
        }

        private bool Equals(ClosingStructure other)
        {
            return AllowedLevelIncreaseStorage.Equals(other.AllowedLevelIncreaseStorage)
                   && AreaFlowApertures.Equals(other.AreaFlowApertures)
                   && CriticalOvertoppingDischarge.Equals(other.CriticalOvertoppingDischarge)
                   && FailureProbabilityOpenStructure.Equals(other.FailureProbabilityOpenStructure)
                   && FailureProbabilityReparation.Equals(other.FailureProbabilityReparation)
                   && IdenticalApertures.Equals(other.IdenticalApertures)
                   && FlowWidthAtBottomProtection.Equals(other.FlowWidthAtBottomProtection)
                   && InflowModelType.Equals(other.InflowModelType)
                   && InsideWaterLevel.Equals(other.InsideWaterLevel)
                   && LevelCrestStructureNotClosing.Equals(other.LevelCrestStructureNotClosing)
                   && ProbabilityOpenStructureBeforeFlooding.Equals(other.ProbabilityOpenStructureBeforeFlooding)
                   && StorageStructureArea.Equals(other.StorageStructureArea)
                   && ThresholdHeightOpenWeir.Equals(other.ThresholdHeightOpenWeir)
                   && WidthFlowApertures.Equals(other.WidthFlowApertures);
        }

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
                StorageStructureArea = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.1
                };
                AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                WidthFlowApertures = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.2
                };
                LevelCrestStructureNotClosing = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };
                InsideWaterLevel = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                ThresholdHeightOpenWeir = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.1
                };
                AreaFlowApertures = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.01
                };
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.15
                };
                FlowWidthAtBottomProtection = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };

                ProbabilityOpenStructureBeforeFlooding = 1;
                FailureProbabilityOpenStructure = 1;
                IdenticalApertures = 1;
                FailureProbabilityReparation = 1;
                InflowModelType = ClosingStructureInflowModelType.VerticalWall;
            }

            /// <summary>
            /// Gets the storage area of the closing structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the closing structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; }

            /// <summary>
            /// Gets the width of the flow apertures of the closing structure.
            /// [m]
            /// </summary>
            public NormalDistribution WidthFlowApertures { get; }

            /// <summary>
            /// Gets the crest level of the opened closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructureNotClosing { get; }

            /// <summary>
            /// Gets the interior water level of the closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution InsideWaterLevel { get; }

            /// <summary>
            /// Gets the threshold height of the opened closing structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution ThresholdHeightOpenWeir { get; }

            /// <summary>
            /// Gets the area of the flow aperture of the closing structure.
            /// [m^2]
            /// </summary>
            public LogNormalDistribution AreaFlowApertures { get; }

            /// <summary>
            /// Gets the critical overtopping discharge per meter of the closing structure.
            /// [m^3/s/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

            /// <summary>
            /// Gets the flow width of the closing structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; }

            /// <summary>
            /// Gets or sets the probability of the closing structure being open before flooding.
            /// [1/year]
            /// </summary>
            public double ProbabilityOpenStructureBeforeFlooding { get; set; }

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