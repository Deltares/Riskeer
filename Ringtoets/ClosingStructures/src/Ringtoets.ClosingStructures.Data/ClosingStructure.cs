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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.ClosingStructures.Data
{
    /// <summary>
    /// Definition of a closing structure for the <see cref="ClosingStructuresFailureMechanism"/>
    /// </summary>
    public class ClosingStructure : StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructure"/>.
        /// </summary>
        /// <param name="name">The name of the closing structure.</param>
        /// <param name="id">The identifier of the closing structure.</param>
        /// <param name="location">The location of the closing structure.</param>
        /// <param name="storageStructureAreaMean">The mean of the storage area of the closing structure.</param>
        /// <param name="storageStructureAreaStandardDeviation">The standard deviation of the storage area of the closing structure.</param>
        /// <param name="allowedLevelIncreaseStorageMean">The mean allowed increase of level for storage of the closing structure.</param>
        /// <param name="allowedLevelIncreaseStorageStandardDeviation">The standard deviation of allowed increase of level for storage of the closing structure.</param>
        /// <param name="structureNormalOrientation">The orientation of the closing structure, relative to north.</param>
        /// <param name="widthFlowAperturesMean">The mean of the width of the flow apertures of the closing structure.</param>
        /// <param name="widthFlowAperturesStandardDeviation">The standard deviation of the width of the flow apertures of the closing structure.</param>
        /// <param name="levelCrestStructureNotClosingMean">The mean crest level of the opened closing structure.</param>
        /// <param name="levelCrestStructureNotClosingStandardDeviation">The standard deviation of the crest level of the opened closing structure.</param>
        /// <param name="insideWaterLevelMean">The mean interior water level of the closing structure.</param>
        /// <param name="insideWaterLevelStandardDeviation">The standard deviation of the interior water level of the closing structure.</param>
        /// <param name="thresholdHeightOpenWeirMean">The mean threshold height of the opened closure structure.</param>
        /// <param name="thresholdHeightOpenWeirStandardDeviation">The standard deviation of the threshold height of the opened closure structure.</param>
        /// <param name="areaFlowAperturesMean">The mean area of the flow aperture of the closing structure.</param>
        /// <param name="areaFlowAperturesStandardDeviation">The standard deviation of the area of the flow aperture of the closing structure.</param>
        /// <param name="criticalOvertoppingDischargeMean">The mean critical overtopping discharge of the closing structure.</param>
        /// <param name="criticalOvertoppingDischargeStandardDeviation">The standard deviation of critical overtopping discharge of the closing structure.</param>
        /// <param name="flowWidthAtBottomProtectionMean">The mean flow width of the closing structure at the bottom protection.</param>
        /// <param name="flowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width of the closing structure at the bottom protection.</param>
        /// <param name="probabilityOpenStructureBeforeFlooding">The probability of the closing structure being open before flooding.</param>
        /// <param name="failureProbablityOpenStructure">The probability of failing to close the closing structure.</param>
        /// <param name="numberOfIdenticalApertures">The number of identical apertures of the closing structure.</param>
        /// <param name="failureProbabilityReparation">The probability of failing to repair a failed closure of the closing structure.</param>
        /// <param name="inflowModel">The type of closing structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="id"/> is <c>null</c>
        /// , empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any parameter is out of range.</exception>
        public ClosingStructure(string name, string id, Point2D location,
                                double storageStructureAreaMean, double storageStructureAreaStandardDeviation,
                                double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                double structureNormalOrientation,
                                double widthFlowAperturesMean, double widthFlowAperturesStandardDeviation,
                                double levelCrestStructureNotClosingMean, double levelCrestStructureNotClosingStandardDeviation,
                                double insideWaterLevelMean, double insideWaterLevelStandardDeviation,
                                double thresholdHeightOpenWeirMean, double thresholdHeightOpenWeirStandardDeviation,
                                double areaFlowAperturesMean, double areaFlowAperturesStandardDeviation,
                                double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeStandardDeviation,
                                double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                double probabilityOpenStructureBeforeFlooding,
                                double failureProbablityOpenStructure,
                                int numberOfIdenticalApertures,
                                double failureProbabilityReparation,
                                int inflowModel
            )
            : base(name, id, location)
        {
            StorageStructureArea = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, storageStructureAreaMean),
                StandardDeviation = new RoundedDouble(2, storageStructureAreaStandardDeviation)
            };
            AllowedLevelIncreaseStorage = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, allowedLevelIncreaseStorageMean),
                StandardDeviation = new RoundedDouble(2, allowedLevelIncreaseStorageStandardDeviation)
            };
            StructureNormalOrientation = new RoundedDouble(2, structureNormalOrientation);
            WidthFlowApertures = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, widthFlowAperturesMean),
                StandardDeviation = new RoundedDouble(2, widthFlowAperturesStandardDeviation)
            };
            LevelCrestStructureNotClosing = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, levelCrestStructureNotClosingMean),
                StandardDeviation = new RoundedDouble(2, levelCrestStructureNotClosingStandardDeviation)
            };
            InsideWaterLevel = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, insideWaterLevelMean),
                StandardDeviation = new RoundedDouble(2, insideWaterLevelStandardDeviation)
            };
            ThresholdHeightOpenWeir = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, thresholdHeightOpenWeirMean),
                StandardDeviation = new RoundedDouble(2, thresholdHeightOpenWeirStandardDeviation)
            };
            AreaFlowApertures = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, areaFlowAperturesMean),
                StandardDeviation = new RoundedDouble(2, areaFlowAperturesStandardDeviation)
            };
            CriticalOvertoppingDischarge = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, criticalOvertoppingDischargeMean),
                StandardDeviation = new RoundedDouble(2, criticalOvertoppingDischargeStandardDeviation)
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, flowWidthAtBottomProtectionMean),
                StandardDeviation = new RoundedDouble(2, flowWidthAtBottomProtectionStandardDeviation)
            };
            ProbabilityOpenStructureBeforeFlooding = new RoundedDouble(2, probabilityOpenStructureBeforeFlooding);
            FailureProbablityOpenStructure = new RoundedDouble(2, failureProbablityOpenStructure);
            NumberOfIdenticalApertures = numberOfIdenticalApertures;
            FailureProbabilityReparation = new RoundedDouble(2, failureProbabilityReparation);
            InflowModel = inflowModel;
        }

        /// <summary>
        /// Gets the storage area of the closing structure.
        /// </summary>
        public LogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowed increase of level for storage of the closing structure.
        /// </summary>
        public LogNormalDistribution AllowedLevelIncreaseStorage { get; private set; }

        /// <summary>
        /// Gets the orientation of the closing structure, relative to north.
        /// </summary>
        public RoundedDouble StructureNormalOrientation { get; private set; }

        /// <summary>
        /// Gets the width of the flow apertures of the closing structure.
        /// </summary>
        public NormalDistribution WidthFlowApertures { get; private set; }

        /// <summary>
        /// Gets the crest level of the opened closing structure.
        /// </summary>
        public NormalDistribution LevelCrestStructureNotClosing { get; private set; }

        /// <summary>
        /// Gets the interior water level of the closing structure.
        /// </summary>
        public NormalDistribution InsideWaterLevel { get; private set; }

        /// <summary>
        /// Gets the threshold height of the opened closure structure.
        /// </summary>
        public NormalDistribution ThresholdHeightOpenWeir { get; private set; }

        /// <summary>
        /// Gets the area of the flow aperture of the closing structure.
        /// </summary>
        public LogNormalDistribution AreaFlowApertures { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge of the closing structure.
        /// </summary>
        public LogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow width of the closing structure at the bottom protection.
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the probability of the closing structure being open before flooding.
        /// </summary>
        public RoundedDouble ProbabilityOpenStructureBeforeFlooding { get; private set; }

        /// <summary>
        /// Gets the probability of failing to close the closing structure.
        /// </summary>
        public RoundedDouble FailureProbablityOpenStructure { get; private set; }

        /// <summary>
        /// Gets the number of identical apertures of the closing structure.
        /// </summary>
        public int NumberOfIdenticalApertures { get; private set; }

        /// <summary>
        /// Gets the probability of failing to repair a failed closure of the closing structure.
        /// </summary>
        public RoundedDouble FailureProbabilityReparation { get; private set; }

        /// <summary>
        /// Gets the type of closing structure.
        /// </summary>
        public int InflowModel { get; private set; }
    }
}