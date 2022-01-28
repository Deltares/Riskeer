// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Plugin.FileImporter;

namespace Riskeer.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class PipingFailureMechanismSectionUpdateStrategyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var updateStrategy = new PipingFailureMechanismSectionUpdateStrategy(
                new PipingFailureMechanism(), new AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionUpdateStrategy<AdoptableWithProfileProbabilityFailureMechanismSectionResult>>(updateStrategy);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithValidData_UpdatesDataAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismSectionUpdateStrategy = new PipingFailureMechanismSectionUpdateStrategy(
                failureMechanism, new AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());
            string sourcePath = TestHelper.GetScratchPadPath();
            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            };

            // Call
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, failureMechanism.FailureMechanismSectionSourcePath);
            Assert.AreEqual(sections.Single(), failureMechanism.Sections.Single());
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResults,
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
            }, affectedObjects);
        }

        [Test]
        public void GivenFailureMechanismWithSections_WhenUpdateSectionsWithImportedData_ThenDataUpdatedAndReturnsAffectedObjects()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismSectionUpdateStrategy = new PipingFailureMechanismSectionUpdateStrategy(
                failureMechanism, new AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            FailureMechanismSection[] sections =
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 0),
                    new Point2D(3, 0)
                })
            };
            string sourcePath = TestHelper.GetScratchPadPath();

            failureMechanism.SetSections(sections, sourcePath);
            failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.First().ScenarioConfigurationType = PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic;

            // When
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Then
            CollectionAssert.AreEqual(new[]
            {
                PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic,
                PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
            }, failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.Select(sc => sc.ScenarioConfigurationType));
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResults,
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
            }, affectedObjects);
        }

        [Test]
        public void DoPostUpdateActions_Always_ClearsAllProbabilisticOutputAndReturnsAffectedObjects()
        {
            // Setup
            var calculation1 = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };
            var calculation2 = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
            };
            var calculation3 = new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(double.NaN, double.NaN, double.NaN)
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new ICalculation[]
            {
                calculation1,
                calculation2,
                calculation3,
                new ProbabilisticPipingCalculationScenario()
            });

            var replaceStrategy = new PipingFailureMechanismSectionUpdateStrategy(failureMechanism, new AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy());

            // Call
            IEnumerable<IObservable> affectedObjects = replaceStrategy.DoPostUpdateActions();

            // Assert
            Assert.IsFalse(calculation1.HasOutput);
            Assert.IsFalse(calculation2.HasOutput);
            Assert.IsTrue(calculation3.HasOutput);
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedObjects);
        }
    }
}