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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class ClosingStructuresCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(ClosingStructuresInput original, ClosingStructuresInput clone)
        {
            CommonCloneAssert.AreClones(original, clone);

            CoreCloneAssert.AreObjectClones(original.ThresholdHeightOpenWeir, clone.ThresholdHeightOpenWeir, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ModelFactorSuperCriticalFlow, clone.ModelFactorSuperCriticalFlow, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.DrainCoefficient, clone.DrainCoefficient, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.AreaFlowApertures, clone.AreaFlowApertures, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.LevelCrestStructureNotClosing, clone.LevelCrestStructureNotClosing, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.InsideWaterLevel, clone.InsideWaterLevel, DistributionAssert.AreEqual);
            Assert.AreEqual(original.FactorStormDurationOpenStructure, clone.FactorStormDurationOpenStructure);
            Assert.AreEqual(original.FailureProbabilityOpenStructure, clone.FailureProbabilityOpenStructure);
            Assert.AreEqual(original.FailureProbabilityReparation, clone.FailureProbabilityReparation);
            Assert.AreEqual(original.ProbabilityOpenStructureBeforeFlooding, clone.ProbabilityOpenStructureBeforeFlooding);
            Assert.AreEqual(original.DeviationWaveDirection, clone.DeviationWaveDirection);
            Assert.AreEqual(original.InflowModelType, clone.InflowModelType);
            Assert.AreEqual(original.IdenticalApertures, clone.IdenticalApertures);
        }
    }
}