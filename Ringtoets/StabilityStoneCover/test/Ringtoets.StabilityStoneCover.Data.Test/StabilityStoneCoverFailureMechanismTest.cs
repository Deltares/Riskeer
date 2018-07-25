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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<StabilityStoneCoverFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Stabiliteit steenzetting", failureMechanism.Name);
            Assert.AreEqual("ZST", failureMechanism.Code);
            Assert.AreEqual(3, failureMechanism.Group);
            Assert.IsInstanceOf<GeneralStabilityStoneCoverWaveConditionsInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Hydraulische randvoorwaarden", failureMechanism.WaveConditionsCalculationGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResult()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.AreSame(section, failureMechanism.SectionResults.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionResults_SectionResultsCleared()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(2, 1)
                })
            });

            // Precondition
            Assert.AreEqual(2, failureMechanism.SectionResults.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnStabilityStoneCoverWaveConditionsCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new StabilityStoneCoverWaveConditionsCalculation(),
                        mocks.StrictMock<ICalculation>(),
                        new StabilityStoneCoverWaveConditionsCalculation()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is StabilityStoneCoverWaveConditionsCalculation));
            mocks.VerifyAll();
        }
    }
}