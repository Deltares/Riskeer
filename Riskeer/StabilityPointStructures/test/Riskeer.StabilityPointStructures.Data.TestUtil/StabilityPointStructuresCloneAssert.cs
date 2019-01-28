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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityPointStructures.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class StabilityPointStructuresCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(StabilityPointStructuresInput original, StabilityPointStructuresInput clone)
        {
            CommonCloneAssert.AreClones(original, clone);

            CoreCloneAssert.AreObjectClones(original.InsideWaterLevelFailureConstruction, clone.InsideWaterLevelFailureConstruction, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.InsideWaterLevel, clone.InsideWaterLevel, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.DrainCoefficient, clone.DrainCoefficient, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.LevelCrestStructure, clone.LevelCrestStructure, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ThresholdHeightOpenWeir, clone.ThresholdHeightOpenWeir, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.AreaFlowApertures, clone.AreaFlowApertures, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ConstructiveStrengthLinearLoadModel, clone.ConstructiveStrengthLinearLoadModel, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ConstructiveStrengthQuadraticLoadModel, clone.ConstructiveStrengthQuadraticLoadModel, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.StabilityLinearLoadModel, clone.StabilityLinearLoadModel, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.StabilityQuadraticLoadModel, clone.StabilityQuadraticLoadModel, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.FailureCollisionEnergy, clone.FailureCollisionEnergy, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ShipMass, clone.ShipMass, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.ShipVelocity, clone.ShipVelocity, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.BankWidth, clone.BankWidth, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.FlowVelocityStructureClosable, clone.FlowVelocityStructureClosable, DistributionAssert.AreEqual);
            Assert.AreEqual(original.VolumicWeightWater, clone.VolumicWeightWater);
            Assert.AreEqual(original.FactorStormDurationOpenStructure, clone.FactorStormDurationOpenStructure);
            Assert.AreEqual(original.EvaluationLevel, clone.EvaluationLevel);
            Assert.AreEqual(original.VerticalDistance, clone.VerticalDistance);
            Assert.AreEqual(original.FailureProbabilityRepairClosure, clone.FailureProbabilityRepairClosure);
            Assert.AreEqual(original.ProbabilityCollisionSecondaryStructure, clone.ProbabilityCollisionSecondaryStructure);
            Assert.AreEqual(original.InflowModelType, clone.InflowModelType);
            Assert.AreEqual(original.LoadSchematizationType, clone.LoadSchematizationType);
            Assert.AreEqual(original.LevellingCount, clone.LevellingCount);
        }
    }
}