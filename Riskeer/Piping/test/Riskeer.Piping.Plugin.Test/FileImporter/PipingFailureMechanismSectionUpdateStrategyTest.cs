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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
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
                new PipingFailureMechanism(), new PipingFailureMechanismSectionResultUpdateStrategy());

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionUpdateStrategy<PipingFailureMechanismSectionResult>>(updateStrategy);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithValidData_UpdatesDataAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var failureMechanismSectionUpdateStrategy = new PipingFailureMechanismSectionUpdateStrategy(
                failureMechanism, new PipingFailureMechanismSectionResultUpdateStrategy());
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
        public void DoPostUpdateActions_Always_ClearsOutputAndReturnsAffectedObjects()
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

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculation1,
                calculation2,
                new ProbabilisticPipingCalculationScenario()
            });

            var replaceStrategy = new PipingFailureMechanismSectionUpdateStrategy(failureMechanism, new PipingFailureMechanismSectionResultUpdateStrategy());

            // Call
            IEnumerable<IObservable> affectedObjects = replaceStrategy.DoPostUpdateActions();

            // Assert
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedObjects);
        }
    }
}