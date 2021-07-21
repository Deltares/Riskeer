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

using System;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsScenariosContextTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            void Call() => new MacroStabilityInwardsScenariosContext(calculationGroup, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculationGroup = new CalculationGroup();

            // Call
            var context = new MacroStabilityInwardsScenariosContext(calculationGroup, failureMechanism);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<CalculationGroup>>(context);
            Assert.AreSame(calculationGroup, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
        }
    }
}