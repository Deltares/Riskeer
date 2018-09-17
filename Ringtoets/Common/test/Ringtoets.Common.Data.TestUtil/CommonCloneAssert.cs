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

using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Structures;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class that defines methods for asserting whether two objects are clones.
    /// </summary>
    public static class CommonCloneAssert
    {
        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(BreakWater original, BreakWater clone)
        {
            Assert.AreEqual(original.Type, clone.Type);
            Assert.AreEqual(original.Height, clone.Height);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(Comment original, Comment clone)
        {
            Assert.AreEqual(original.Body, clone.Body);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(WindDirection original, WindDirection clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Angle, clone.Angle);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(Stochast original, Stochast clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Alpha, clone.Alpha);
            Assert.AreEqual(original.Duration, clone.Duration);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(SubMechanismIllustrationPointStochast original, SubMechanismIllustrationPointStochast clone)
        {
            AreClones((Stochast) original, clone);
            Assert.AreEqual(original.Realization, clone.Realization);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(TopLevelIllustrationPointBase original, TopLevelIllustrationPointBase clone)
        {
            CoreCloneAssert.AreObjectClones(original.WindDirection, clone.WindDirection, AreClones);
            Assert.AreEqual(original.ClosingSituation, clone.ClosingSituation);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(TopLevelSubMechanismIllustrationPoint original, TopLevelSubMechanismIllustrationPoint clone)
        {
            AreClones((TopLevelIllustrationPointBase) original, clone);
            CoreCloneAssert.AreObjectClones(original.SubMechanismIllustrationPoint, clone.SubMechanismIllustrationPoint, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(TopLevelFaultTreeIllustrationPoint original, TopLevelFaultTreeIllustrationPoint clone)
        {
            AreClones((TopLevelIllustrationPointBase) original, clone);
            CoreCloneAssert.AreObjectClones(original.FaultTreeNodeRoot, clone.FaultTreeNodeRoot, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(IllustrationPointResult original, IllustrationPointResult clone)
        {
            Assert.AreEqual(original.Description, clone.Description);
            Assert.AreEqual(original.Value, clone.Value);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(IllustrationPointBase original, IllustrationPointBase clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
            Assert.AreEqual(original.Beta, clone.Beta);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(SubMechanismIllustrationPoint original, SubMechanismIllustrationPoint clone)
        {
            AreClones((IllustrationPointBase) original, clone);
            CoreCloneAssert.AreEnumerationClones(original.Stochasts, clone.Stochasts, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.IllustrationPointResults, clone.IllustrationPointResults, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(FaultTreeIllustrationPoint original, FaultTreeIllustrationPoint clone)
        {
            AreClones((IllustrationPointBase) original, clone);
            CoreCloneAssert.AreEnumerationClones(original.Stochasts, clone.Stochasts, AreClones);
            Assert.AreEqual(original.CombinationType, clone.CombinationType);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(IllustrationPointNode original, IllustrationPointNode clone)
        {
            CoreCloneAssert.AreObjectClones(original.Data, clone.Data, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.Children, clone.Children, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GeneralResult<TopLevelIllustrationPointBase> original, GeneralResult<TopLevelIllustrationPointBase> clone)
        {
            CoreCloneAssert.AreObjectClones(original.GoverningWindDirection, clone.GoverningWindDirection, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.Stochasts, clone.Stochasts, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.TopLevelIllustrationPoints, clone.TopLevelIllustrationPoints, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(GeneralResult<TopLevelFaultTreeIllustrationPoint> original, GeneralResult<TopLevelFaultTreeIllustrationPoint> clone)
        {
            CoreCloneAssert.AreObjectClones(original.GoverningWindDirection, clone.GoverningWindDirection, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.Stochasts, clone.Stochasts, AreClones);
            CoreCloneAssert.AreEnumerationClones(original.TopLevelIllustrationPoints, clone.TopLevelIllustrationPoints, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <typeparam name="T">The type of the structure contained by the structures input.</typeparam>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones<T>(StructuresInputBase<T> original, StructuresInputBase<T> clone) where T : StructureBase
        {
            CoreCloneAssert.AreObjectClones(original.AllowedLevelIncreaseStorage, clone.AllowedLevelIncreaseStorage, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.StorageStructureArea, clone.StorageStructureArea, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.FlowWidthAtBottomProtection, clone.FlowWidthAtBottomProtection, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.CriticalOvertoppingDischarge, clone.CriticalOvertoppingDischarge, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.WidthFlowApertures, clone.WidthFlowApertures, DistributionAssert.AreEqual);
            CoreCloneAssert.AreObjectClones(original.StormDuration, clone.StormDuration, DistributionAssert.AreEqual);
            Assert.AreSame(original.Structure, clone.Structure);
            Assert.AreEqual(original.StructureNormalOrientation, clone.StructureNormalOrientation);
            Assert.AreEqual(original.FailureProbabilityStructureWithErosion, clone.FailureProbabilityStructureWithErosion);
            Assert.AreSame(original.ForeshoreProfile, clone.ForeshoreProfile);
            Assert.AreEqual(original.ShouldIllustrationPointsBeCalculated, clone.ShouldIllustrationPointsBeCalculated);
            Assert.AreSame(original.HydraulicBoundaryLocation, clone.HydraulicBoundaryLocation);
            Assert.AreEqual(original.UseBreakWater, clone.UseBreakWater);
            CoreCloneAssert.AreObjectClones(original.BreakWater, clone.BreakWater, AreClones);
            Assert.AreEqual(original.UseForeshore, clone.UseForeshore);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(StructuresOutput original, StructuresOutput clone)
        {
            Assert.AreEqual(original.Reliability, clone.Reliability);
            CoreCloneAssert.AreObjectClones(original.GeneralResult, clone.GeneralResult, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <typeparam name="TInput">The type of structures input.</typeparam>
        /// <typeparam name="TStructure">The type of the structure contained by the structures input.</typeparam>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones<TInput, TStructure>(StructuresCalculation<TInput> original, StructuresCalculation<TInput> clone)
            where TInput : StructuresInputBase<TStructure>, new()
            where TStructure : StructureBase, new()
        {
            Assert.AreEqual(original.Name, clone.Name);
            CoreCloneAssert.AreObjectClones(original.Comments, clone.Comments, AreClones);
            CoreCloneAssert.AreObjectClones(original.InputParameters, clone.InputParameters, AreClones);
            CoreCloneAssert.AreObjectClones(original.Output, clone.Output, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(CalculationGroup original, CalculationGroup clone)
        {
            AreClones((ICalculationBase) original, clone);

            CoreCloneAssert.AreEnumerationClones(original.Children, clone.Children, AreClones);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        private static void AreClones(ICalculationBase original, ICalculationBase clone)
        {
            Assert.AreEqual(original.Name, clone.Name);
        }

        /// <summary>
        /// Method that asserts whether <paramref name="original"/> and <paramref name="clone"/>
        /// are clones.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="clone">The cloned object.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="original"/> and
        /// <paramref name="clone"/> are not clones.</exception>
        public static void AreClones(HydraulicBoundaryLocationCalculationInput original, HydraulicBoundaryLocationCalculationInput clone)
        {
            Assert.AreEqual(original.ShouldIllustrationPointsBeCalculated, clone.ShouldIllustrationPointsBeCalculated);
        }
    }
}