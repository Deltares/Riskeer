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

using System;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;
using BaseConstructionProperties = Ringtoets.Common.Data.StructureBase.ConstructionProperties;

namespace Ringtoets.HeightStructures.Data
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
        /// or <see cref="ConstructionProperties.Id"/> is <c>null</c> , empty or consists of whitespace.</exception>
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
            WidthFlowApertures = new VariationCoefficientNormalDistribution(2)
            {
                Mean = constructionProperties.WidthFlowApertures.Mean,
                CoefficientOfVariation = constructionProperties.WidthFlowApertures.CoefficientOfVariation
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
        public NormalDistribution LevelCrestStructure { get; private set; }

        /// <summary>
        /// Gets the flow width of the height structure at the bottom protection.
        /// [m]
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge of the height structure.
        /// [m^2/s]
        /// </summary>
        public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow apertures width of the height structure.
        /// [m]
        /// </summary>
        public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

        /// <summary>
        /// Gets the failure probability of the height structure, given erosion.
        /// [1/year]
        /// </summary>
        public double FailureProbabilityStructureWithErosion { get; private set; }

        /// <summary>
        /// Gets the storage area of the height structure.
        /// [m^2]
        /// </summary>
        public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the height structure.
        /// [m]
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

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
                LevelCrestStructure = new NormalDistribution(2);
                FlowWidthAtBottomProtection = new LogNormalDistribution(2);
                CriticalOvertoppingDischarge = new VariationCoefficientLogNormalDistribution(2);
                WidthFlowApertures = new VariationCoefficientNormalDistribution(2);
                StorageStructureArea = new VariationCoefficientLogNormalDistribution(2);
                AllowedLevelIncreaseStorage = new LogNormalDistribution(2);
            }

            /// <summary>
            /// Gets the crest level of the height structure.
            /// [m+NAP]
            /// </summary>
            public NormalDistribution LevelCrestStructure { get; private set; }

            /// <summary>
            /// Gets the flow width of the height structure at the bottom protection.
            /// [m]
            /// </summary>
            public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

            /// <summary>
            /// Gets the critical overtopping discharge of the height structure.
            /// [m^2/s]
            /// </summary>
            public VariationCoefficientLogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

            /// <summary>
            /// Gets the flow apertures width of the height structure.
            /// [m]
            /// </summary>
            public VariationCoefficientNormalDistribution WidthFlowApertures { get; private set; }

            /// <summary>
            /// Gets the failure probability of the height structure, given erosion.
            /// [1/year]
            /// </summary>
            public double FailureProbabilityStructureWithErosion { get; set; }

            /// <summary>
            /// Gets the storage area of the height structure.
            /// [m^2]
            /// </summary>
            public VariationCoefficientLogNormalDistribution StorageStructureArea { get; private set; }

            /// <summary>
            /// Gets the allowed increase of level for storage of the height structure.
            /// [m]
            /// </summary>
            public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }
        }
    }
}