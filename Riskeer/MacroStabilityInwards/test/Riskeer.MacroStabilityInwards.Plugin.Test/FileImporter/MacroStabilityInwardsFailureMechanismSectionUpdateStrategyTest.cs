// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.FileImporters;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Plugin.FileImporter;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionUpdateStrategyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var updateStrategy = new MacroStabilityInwardsFailureMechanismSectionUpdateStrategy(
                new MacroStabilityInwardsFailureMechanism(), new AdoptableFailureMechanismSectionResultUpdateStrategy());

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionUpdateStrategy<AdoptableFailureMechanismSectionResult>>(updateStrategy);
        }

        [Test]
        public void UpdateSectionsWithImportedData_WithValidData_UpdatesDataAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var failureMechanismSectionUpdateStrategy = new MacroStabilityInwardsFailureMechanismSectionUpdateStrategy(
                failureMechanism, new AdoptableFailureMechanismSectionResultUpdateStrategy());
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
                failureMechanism.SectionConfigurations
            }, affectedObjects);
        }

        [Test]
        public void GivenFailureMechanismWithSections_WhenUpdateSectionsWithImportedData_ThenDataUpdatedAndReturnsAffectedObjects()
        {
            // Given
            var random = new Random(21);
            RoundedDouble firstSectionA = random.NextRoundedDouble();
            RoundedDouble secondSectionA = random.NextRoundedDouble();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var failureMechanismSectionUpdateStrategy = new MacroStabilityInwardsFailureMechanismSectionUpdateStrategy(
                failureMechanism, new AdoptableFailureMechanismSectionResultUpdateStrategy());

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
            failureMechanism.SectionConfigurations.First().A = firstSectionA;
            failureMechanism.SectionConfigurations.ElementAt(1).A = secondSectionA;

            // When
            IEnumerable<IObservable> affectedObjects = failureMechanismSectionUpdateStrategy.UpdateSectionsWithImportedData(sections, sourcePath);

            // Then
            FailureMechanismSectionConfiguration firstSectionConfiguration = failureMechanism.SectionConfigurations.ElementAt(0);
            Assert.AreEqual(firstSectionA, firstSectionConfiguration.A, firstSectionConfiguration.A.GetAccuracy());

            FailureMechanismSectionConfiguration secondSectionConfiguration = failureMechanism.SectionConfigurations.ElementAt(1);
            Assert.AreEqual(secondSectionA, secondSectionConfiguration.A, secondSectionConfiguration.A.GetAccuracy());

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism,
                failureMechanism.SectionResults,
                failureMechanism.SectionConfigurations
            }, affectedObjects);
        }
    }
}