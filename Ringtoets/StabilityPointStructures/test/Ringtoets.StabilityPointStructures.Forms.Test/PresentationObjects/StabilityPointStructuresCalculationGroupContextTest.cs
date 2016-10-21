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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityPointStructuresCalculationGroupContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            var groupContext = new StabilityPointStructuresCalculationGroupContext(calculationGroup, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<StructuresContextBase<CalculationGroup, StabilityPointStructuresFailureMechanism>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, StabilityPointStructuresFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, groupContext.AssessmentSection);
            mockRepository.VerifyAll();
        }
    }
}