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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.TestUtil
{
    /// <summary>
    /// Class to help asserting the <see cref="PipingTestDataGenerator"/>.
    /// </summary>
    public static class PipingTestDataGeneratorHelper
    {
        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains all possible calculation configurations 
        /// for the parent and nested calculations with output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have all expected calculation configurations.</exception>
        public static void AssertHasAllPossibleCalculationConfigurationsWithOutputs(PipingFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertPipingCalculationGroupWithOutput(calculationRoot.OfType<PipingCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertPipingCalculationGroupWithOutput(nestedCalculations.Children.OfType<PipingCalculation>());
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains all possible calculation configurations 
        /// for the parent and nested calculations without output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have all expected calculation configurations.</exception>
        public static void AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(PipingFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertPipingCalculationGroupWithoutOutput(calculationRoot.OfType<PipingCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertPipingCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<PipingCalculation>());
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have stochastic soil models.</exception>
        public static void AssertHasStochasticSoilModels(PipingFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.StochasticSoilModels);
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have any surface lines.</exception>
        public static void AssertHasSurfaceLines(PipingFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.SurfaceLines);
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="FailureMechanismSection"/> and
        /// <see cref="PipingFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have the expected amount of sections.</exception>
        public static void AssertHasFailureMechanismSections(PipingFailureMechanism failureMechanism)
        {
            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            int sectionsCount = sections.Count();
            Assert.AreEqual(2, sectionsCount);
            Assert.AreEqual(sectionsCount, failureMechanism.SectionResults.Count());
            Assert.AreEqual("path/to/sections", failureMechanism.FailureMechanismSectionSourcePath);
        }

        private static void AssertPipingCalculationGroupWithOutput(IEnumerable<PipingCalculation> children)
        {
            AssertCalculationConfig(children, true, true);
        }

        private static void AssertPipingCalculationGroupWithoutOutput(IEnumerable<PipingCalculation> children)
        {
            AssertCalculationConfig(children, false, false);
            AssertCalculationConfig(children, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<PipingCalculation> children, bool hasHydraulicBoundaryLocation, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.HasOutput == hasOutput));
        }
    }
}