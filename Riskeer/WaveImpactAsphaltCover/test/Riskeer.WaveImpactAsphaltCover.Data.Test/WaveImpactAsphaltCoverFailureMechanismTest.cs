﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismTest
    {
        [Test]
        public void Constructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<WaveImpactAsphaltCoverFailureMechanismSectionResultOld>>(failureMechanism);
            Assert.AreEqual("Golfklappen op asfaltbekleding", failureMechanism.Name);
            Assert.AreEqual("AGK", failureMechanism.Code);
            Assert.AreEqual(3, failureMechanism.Group);
            Assert.IsNotNull(failureMechanism.GeneralWaveImpactAsphaltCoverInput);
            Assert.IsNotNull(failureMechanism.GeneralInput);

            Assert.AreEqual("Hydraulische belastingen", failureMechanism.WaveConditionsCalculationGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);
        }

        [Test]
        public void SetSections_WithSection_SetsSectionResults()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section
            });

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResultsOld.Count());
            Assert.AreSame(section, failureMechanism.SectionResultsOld.First().Section);
        }

        [Test]
        public void ClearAllSections_WithSectionResults_SectionResultsCleared()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

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
            Assert.AreEqual(2, failureMechanism.SectionResultsOld.Count());

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.SectionResultsOld);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnWaveImpactAsphaltCoverCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                WaveConditionsCalculationGroup =
                {
                    Children =
                    {
                        new CalculationGroup(),
                        new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                        mocks.StrictMock<ICalculation>(),
                        new WaveImpactAsphaltCoverWaveConditionsCalculation()
                    }
                }
            };

            mocks.ReplayAll();

            // Call
            List<ICalculation> calculations = failureMechanism.Calculations.ToList();

            // Assert
            Assert.AreEqual(2, calculations.Count);
            Assert.IsTrue(calculations.All(c => c is WaveImpactAsphaltCoverWaveConditionsCalculation));
            mocks.VerifyAll();
        }
    }
}