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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Definition of a height structure for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructure : StructureBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructure"/>.
        /// </summary>
        /// <param name="name">The name of the height structure.</param>
        /// <param name="id">The identifier of the height structure.</param>
        /// <param name="location">The location of the height structure.</param>
        /// <param name="orientationOfTheNormalOfTheStructure">The orientation of the height structure, relative to north.</param>
        /// <param name="levelOfCrestOfStructureMean">The mean crest level of the height structure.</param>
        /// <param name="levelOfCrestOfStructureStandardDeviation">The standard deviation of the crest level of the height structure.</param>
        /// <param name="flowWidthAtBottomProtectionMean">The mean flow width of the height structure at the bottom protection.</param>
        /// <param name="flowWidthAtBottomProtectionStandardDeviation">The standard deviation of the flow width of the height structure at the bottom protection.</param>
        /// <param name="criticalOvertoppingDischargeMean">The mean critical overtopping discharge of the height structure.</param>
        /// <param name="criticalOvertoppingDischargeStandardDeviation">The standard deviation of critical overtopping discharge of the height structure.</param>
        /// <param name="widthOfFlowAperturesMean">The mean flow apertures width of the height structure.</param>
        /// <param name="widthOfFlowAperturesStandardDeviation">The standard deviation of flow apertures width of the height structure.</param>
        /// <param name="failureProbabilityOfStructureGivenErosion">The failure probability of the height structure, given erosion.</param>
        /// <param name="storageStructureAreaMean">The mean storage area of the height structure.</param>
        /// <param name="storageStructureAreaStandardDeviation">The standard deviation of storage area of the height structure.</param>
        /// <param name="allowableIncreaseOfLevelForStorageMean">The mean allowable increase of level for storage of the height structure.</param>
        /// <param name="allowableIncreaseOfLevelForStorageStandardDeviation">The standard deviation of allowable increase of level for storage of the height structure.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> or <paramref name="id"/> is <c>null</c>
        /// , empty or consists of whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> is <c>null</c>.</exception>
        public HeightStructure(string name, string id, Point2D location,
                               double orientationOfTheNormalOfTheStructure,
                               double levelOfCrestOfStructureMean, double levelOfCrestOfStructureStandardDeviation,
                               double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                               double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeStandardDeviation,
                               double widthOfFlowAperturesMean, double widthOfFlowAperturesStandardDeviation,
                               double failureProbabilityOfStructureGivenErosion,
                               double storageStructureAreaMean, double storageStructureAreaStandardDeviation,
                               double allowableIncreaseOfLevelForStorageMean, double allowableIncreaseOfLevelForStorageStandardDeviation
            ) : base(name, id, location)
        {
            OrientationOfTheNormalOfTheStructure = new RoundedDouble(2, orientationOfTheNormalOfTheStructure);
            LevelOfCrestOfStructure = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, levelOfCrestOfStructureMean),
                StandardDeviation = new RoundedDouble(2, levelOfCrestOfStructureStandardDeviation)
            };
            FlowWidthAtBottomProtection = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, flowWidthAtBottomProtectionMean),
                StandardDeviation = new RoundedDouble(2, flowWidthAtBottomProtectionStandardDeviation)
            };
            CriticalOvertoppingDischarge = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, criticalOvertoppingDischargeMean),
                StandardDeviation = new RoundedDouble(2, criticalOvertoppingDischargeStandardDeviation)
            };
            WidthOfFlowApertures = new NormalDistribution(2)
            {
                Mean = new RoundedDouble(2, widthOfFlowAperturesMean),
                StandardDeviation = new RoundedDouble(2, widthOfFlowAperturesStandardDeviation)
            };
            FailureProbabilityOfStructureGivenErosion = failureProbabilityOfStructureGivenErosion;
            StorageStructureArea = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, storageStructureAreaMean),
                StandardDeviation = new RoundedDouble(2, storageStructureAreaStandardDeviation)
            };
            AllowableIncreaseOfLevelForStorage = new LogNormalDistribution(2)
            {
                Mean = new RoundedDouble(2, allowableIncreaseOfLevelForStorageMean),
                StandardDeviation = new RoundedDouble(2, allowableIncreaseOfLevelForStorageStandardDeviation)
            };
        }

        /// <summary>
        /// Gets the orientation of the height structure, relative to north.
        /// </summary>
        public RoundedDouble OrientationOfTheNormalOfTheStructure { get; private set; }

        /// <summary>
        /// Gets the crest level of the height structure.
        /// </summary>
        public NormalDistribution LevelOfCrestOfStructure { get; private set; }

        /// <summary>
        /// Gets the flow width of the height structure at the bottom protection.
        /// </summary>
        public LogNormalDistribution FlowWidthAtBottomProtection { get; private set; }

        /// <summary>
        /// Gets the critical overtopping discharge of the height structure.
        /// </summary>
        public LogNormalDistribution CriticalOvertoppingDischarge { get; private set; }

        /// <summary>
        /// Gets the flow apertures width of the height structure.
        /// </summary>
        public NormalDistribution WidthOfFlowApertures { get; private set; }

        /// <summary>
        /// Gets the failure probability of the height structure, given erosion.
        /// </summary>
        public double FailureProbabilityOfStructureGivenErosion { get; private set; }

        /// <summary>
        /// Gets the storage area of the height structure.
        /// </summary>
        public LogNormalDistribution StorageStructureArea { get; private set; }

        /// <summary>
        /// Gets the allowable increase of level for storage of the height structure.
        /// </summary>
        public LogNormalDistribution AllowableIncreaseOfLevelForStorage { get; private set; }
    }
}