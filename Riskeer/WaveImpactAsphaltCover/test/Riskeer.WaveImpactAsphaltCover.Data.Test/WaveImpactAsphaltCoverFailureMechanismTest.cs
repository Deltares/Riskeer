﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.WaveImpactAsphaltCover.Data.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase<NonAdoptableFailureMechanismSectionResult>>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.AreEqual("Golfklappen op asfaltbekleding", failureMechanism.Name);
            Assert.AreEqual("AGK", failureMechanism.Code);
            Assert.IsNotNull(failureMechanism.GeneralInput);

            Assert.AreEqual("Hydraulische belastingen", failureMechanism.CalculationsGroup.Name);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.Calculations);

            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);

            Assert.IsNotNull(failureMechanism.CalculationsInputComments);
        }

        [Test]
        public void Calculations_MultipleChildrenAdded_ReturnWaveImpactAsphaltCoverCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                CalculationsGroup =
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