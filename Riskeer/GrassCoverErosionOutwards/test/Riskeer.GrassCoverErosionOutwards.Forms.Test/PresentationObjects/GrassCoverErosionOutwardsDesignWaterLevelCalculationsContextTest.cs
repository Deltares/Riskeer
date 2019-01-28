// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculations = mocks.Stub<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(calculations, null, assessmentSection,
                                                                                                       () => 0.1, "Test name");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculations = mockRepository.Stub<IObservableEnumerable<HydraulicBoundaryLocationCalculation>>();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            Func<double> getNormFunc = () => 0.01;
            const string categoryBoundaryName = "Test name";

            // Call
            var context = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(calculations, failureMechanism, assessmentSection,
                                                                                           getNormFunc, categoryBoundaryName);

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationsContext>(context);
            Assert.AreSame(calculations, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(getNormFunc, context.GetNormFunc);
            Assert.AreEqual(categoryBoundaryName, context.CategoryBoundaryName);
            mockRepository.VerifyAll();
        }
    }
}