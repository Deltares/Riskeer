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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil
{
    /// <summary>
    /// Class to help asserting the <see cref="MacroStabilityInwardsTestDataGenerator"/>.
    /// </summary>
    public static class MacroStabilityInwardsTestDataGeneratorHelper
    {
        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains all possible calculation configurations 
        /// for the parent and nested calculations with output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have all expected calculation configurations.</exception>
        public static void AssertHasAllPossibleCalculationConfigurationsWithOutputs(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertCalculationGroupWithOutput(calculationRoot.OfType<MacroStabilityInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertCalculationGroupWithOutput(nestedCalculations.Children.OfType<MacroStabilityInwardsCalculation>());
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains all possible calculation configurations 
        /// for the parent and nested calculations without output.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have all expected calculation configurations.</exception>
        public static void AssertHasAllPossibleCalculationConfigurationsWithoutOutputs(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<ICalculationBase> calculationRoot = failureMechanism.CalculationsGroup.Children;
            AssertCalculationGroupWithoutOutput(calculationRoot.OfType<MacroStabilityInwardsCalculation>());

            CalculationGroup nestedCalculations = calculationRoot.OfType<CalculationGroup>().First();
            AssertCalculationGroupWithoutOutput(nestedCalculations.Children.OfType<MacroStabilityInwardsCalculation>());
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="MacroStabilityInwardsStochasticSoilModel"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have stochastic soil models.</exception>
        public static void AssertHasStochasticSoilModels(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.StochasticSoilModels);
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="MacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have anysurface lines.</exception>
        public static void AssertHasSurfaceLines(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            CollectionAssert.IsNotEmpty(failureMechanism.SurfaceLines);
        }

        /// <summary>
        /// Asserts that the <paramref name="failureMechanism"/> contains <see cref="FailureMechanismSection"/> and
        /// <see cref="MacroStabilityInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to assert.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="failureMechanism"/>
        /// doesn't have the expected amount of sections.</exception>
        public static void AssertHasFailureMechanismSections(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            IEnumerable<FailureMechanismSection> sections = failureMechanism.Sections;
            int sectionsCount = sections.Count();
            Assert.AreEqual(2, sectionsCount);
            Assert.AreEqual(sectionsCount, failureMechanism.SectionResults.Count());
            Assert.AreEqual("path/to/sections", failureMechanism.FailureMechanismSectionSourcePath);
        }

        private static void AssertCalculationGroupWithOutput(IEnumerable<MacroStabilityInwardsCalculation> children)
        {
            AssertCalculationConfig(children, true, true);
        }

        private static void AssertCalculationGroupWithoutOutput(IEnumerable<MacroStabilityInwardsCalculation> children)
        {
            AssertCalculationConfig(children, false, false);
            AssertCalculationConfig(children, true, false);
        }

        private static void AssertCalculationConfig(
            IEnumerable<MacroStabilityInwardsCalculation> children, bool hasHydraulicBoundaryLocation, bool hasOutput)
        {
            Assert.NotNull(children.FirstOrDefault(calc => calc.InputParameters.HydraulicBoundaryLocation != null == hasHydraulicBoundaryLocation
                                                           && calc.HasOutput == hasOutput));
        }
    }
}