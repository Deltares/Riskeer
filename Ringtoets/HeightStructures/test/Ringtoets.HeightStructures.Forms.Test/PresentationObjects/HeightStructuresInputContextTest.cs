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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HeightStructuresInputContextTest
    {
        [Test]
        public void Constructor_CalcationIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            TestDelegate test = () => new HeightStructuresInputContext(null, failureMechanism, assessmentSectionMock);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidInputParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new HeightStructuresCalculation();
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var context = new HeightStructuresInputContext(calculation, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<StructuresContextBase<HeightStructuresInput, HeightStructuresFailureMechanism>>(context);
            Assert.AreSame(calculation.InputParameters, context.WrappedData);
            Assert.AreSame(calculation, context.Calculation);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSectionMock, context.AssessmentSection);
            mocks.VerifyAll();
        }
    }
}