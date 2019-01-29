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

namespace Riskeer.HeightStructures.Data
{
    /// <summary>
    /// Definition of a height structure for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructure : StructureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeightStructure"/> class.
        /// </summary>
        /// <param name="constructionProperties">The construction properties.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="ConstructionProperties.Name"/>
        /// or <see cref="ConstructionProperties.Id"/> is <c>null</c>, empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="ConstructionProperties.Location"/> is <c>null</c>.</exception>
        public HeightStructure(ConstructionProperties constructionProperties) : base(constructionProperties)
        {
            LevelCrestStructure = new NormalDistribution(2)
            {
                Mean = constructionProperties.LevelCrestStructure.Mean,
                StandardDeviation = constructionProperties.LevelCrestStructure.StandardDeviation
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = constructionProperties.FlowWidthAtBottomProtection.Mean,
                StandardDeviation = constructionProperties.FlowWidthAtBottomProtection.StandardDeviation
            };
            CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = constructionProperties.CriticalOvertoppingDischarge.Mean,
                CoefficientOfVariation = constructionProperties.CriticalOvertoppingDischarge.CoefficientOfVariation
            };
            WidthFlowApertures = new NormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                StandardDeviation = constructionProperties.WidthFlowApertures.StandardDeviation
            };
            FailureProbabilityStructureWithErosion = constructionProperties.FailureProbabilityStructureWithErosion;
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
        }

        /// <summary>
        /// Gets the crest level of the height structure.
        /// [m+NAP]
        /// </summary>
        public NormalDistribution LevelCrestStructure { get; }

        /// <summary>
        /// Gets the flow width of the height structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; }

        /// <summary>
        /// Gets the critical overtopping discharge per meter of the height structure.
        /// [m^3/s/m]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

        /// <summary>
        /// Gets the flow apertures width of the height structure.
        /// [m]
        /// </summary>
        public NormalDistribution WidthFlowApertures { get; }

        /// <summary>
        /// Gets the failure probability of the height structure, given erosion.
        /// [1/year]
        /// </summary>
        public double FailureProbabilityStructureWithErosion { get; private set; }

        /// <summary>
        /// Gets the storage area of the height structure.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the height structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; }

        /// <summary>
        /// Copies the property values of the <paramref name="fromStructure"/> to the 
        /// <see cref="HeightStructure"/>.
        /// </summary>
        /// <param name="fromStructure">The <see cref="HeightStructure"/> to get the property 
        /// values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromStructure"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(HeightStructure fromStructure)
        {
            base.CopyProperties(fromStructure);

            AllowedLevelIncreaseStorage.Mean = fromStructure.AllowedLevelIncreaseStorage.Mean;
            AllowedLevelIncreaseStorage.StandardDeviation = fromStructure.AllowedLevelIncreaseStorage.StandardDeviation;

            CriticalOvertoppingDischarge.Mean = fromStructure.CriticalOvertoppingDischarge.Mean;
            CriticalOvertoppingDischarge.CoefficientOfVariation = fromStructure.CriticalOvertoppingDischarge.CoefficientOfVariation;

            FailureProbabilityStructureWithErosion = fromStructure.FailureProbabilityStructureWithErosion;

            FlowWidthAtBottomProtection.Mean = fromStructure.FlowWidthAtBottomProtection.Mean;
            FlowWidthAtBottomProtection.StandardDeviation = fromStructure.FlowWidthAtBottomProtection.StandardDeviation;

            LevelCrestStructure.Mean = fromStructure.LevelCrestStructure.Mean;
            LevelCrestStructure.StandardDeviation = fromStructure.LevelCrestStructure.StandardDeviation;

            StorageStructureArea.Mean = fromStructure.StorageStructureArea.Mean;
            StorageStructureArea.CoefficientOfVariation = fromStructure.StorageStructureArea.CoefficientOfVariation;

            WidthFlowApertures.Mean = fromStructure.WidthFlowApertures.Mean;
            WidthFlowApertures.StandardDeviation = fromStructure.WidthFlowApertures.StandardDeviation;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) && Equals((HeightStructure) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ AllowedLevelIncreaseStorage.GetHashCode();
                hashCode = (hashCode * 397) ^ CriticalOvertoppingDischarge.GetHashCode();
                hashCode = (hashCode * 397) ^ FlowWidthAtBottomProtection.GetHashCode();
                hashCode = (hashCode * 397) ^ LevelCrestStructure.GetHashCode();
                hashCode = (hashCode * 397) ^ StorageStructureArea.GetHashCode();
                hashCode = (hashCode * 397) ^ WidthFlowApertures.GetHashCode();

                return hashCode;
            }
        }

        private bool Equals(HeightStructure other)
        {
            return AllowedLevelIncreaseStorage.Equals(other.AllowedLevelIncreaseStorage)
                   && CriticalOvertoppingDischarge.Equals(other.CriticalOvertoppingDischarge)
                   && FailureProbabilityStructureWithErosion.Equals(other.FailureProbabilityStructureWithErosion)
                   && FlowWidthAtBottomProtection.Equals(other.FlowWidthAtBottomProtection)
                   && LevelCrestStructure.Equals(other.LevelCrestStructure)
                   && StorageStructureArea.Equals(other.StorageStructureArea)
                   && WidthFlowApertures.Equals(other.WidthFlowApertures);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="HeightStructure"/>.
        /// </summary>
        public new class ConstructionProperties : BaseConstructionProperties
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstructionProperties"/> class.
            /// </summary>
            public ConstructionProperties()
            {
                LevelCrestStructure = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };
                FlowWidthAtBottomProtection = new LogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.05
                };
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = (RoundedDouble) 0.15
                };
                WidthFlowApertures = new NormalDistribution(2)
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = (RoundedDouble) 0.2
                };
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

                FailureProbabilityStructureWithErosion = 1.0;
            }

            /// <summary>
            /// Gets the crest level of the height structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructure { get; }

            /// <summary>
            /// Gets the flow width of the height structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; }

            /// <summary>
            /// Gets the critical overtopping discharge per meter of the height structure.
            /// [m^3/s/m]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; }

            /// <summary>
            /// Gets the flow apertures width of the height structure.
            /// [m]
            /// </summary>
            public NormalDistribution WidthFlowApertures { get; }

            /// <summary>
            /// Gets or sets the failure probability of the height structure, given erosion.
            /// [1/year]
            /// </summary>
            public double FailureProbabilityStructureWithErosion { get; set; }

            /// <summary>
            /// Gets the storage area of the height structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the height structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; }
        }
    }
}