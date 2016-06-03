﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StandAloneFailureMechanismContextPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup

            // Call
            var properties = new StandAloneFailureMechanismContextProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<FailureMechanismContext<IFailureMechanism>>>(properties);
        }

        [Test]
        public void Data_SetNewSimpleFailureMechanismContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var properties = new StandAloneFailureMechanismContextProperties();

            // Call
            properties.Data = new FailureMechanismContext<IFailureMechanism>(failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(failureMechanism.Name, properties.Name);
            Assert.AreEqual(failureMechanism.Code, properties.Code);
            mockRepository.VerifyAll();
        } 
    }
}