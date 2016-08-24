// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverHydraulicBoundariesCalculationGroupContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculationGroup = new CalculationGroup();

            // Call
            var context = new StabilityStoneCoverHydraulicBoundariesCalculationGroupContext(calculationGroup, failureMechanism);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<CalculationGroup>>(context);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, StabilityStoneCoverFailureMechanism>>(context);

            Assert.AreSame(calculationGroup, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
        }

        [Test]
        public void Constructor_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            TestDelegate call = () => new StabilityStoneCoverHydraulicBoundariesCalculationGroupContext(calculationGroup, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }
    }
}