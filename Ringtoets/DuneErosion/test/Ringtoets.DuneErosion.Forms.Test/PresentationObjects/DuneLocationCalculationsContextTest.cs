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

using System;
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;

namespace Ringtoets.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationCalculationsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>();
            Func<double> getNormFunc = () => 0.01;
            const string categoryBoundaryName = "Name";

            // Call
            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              failureMechanism,
                                                              assessmentSection,
                                                              getNormFunc,
                                                              categoryBoundaryName);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservableEnumerable<DuneLocationCalculation>>>(context);
            Assert.AreSame(duneLocationCalculations, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            Assert.AreSame(getNormFunc, context.GetNormFunc);
            Assert.AreSame(categoryBoundaryName, context.CategoryBoundaryName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                                          null,
                                                                          assessmentSection,
                                                                          () => 0.01,
                                                                          "Name");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                                          new DuneErosionFailureMechanism(),
                                                                          null,
                                                                          () => 0.01,
                                                                          "Name");

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                                          new DuneErosionFailureMechanism(),
                                                                          assessmentSection,
                                                                          null,
                                                                          "Name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getNormFunc", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_CategoryBoundaryNameInvalid_ThrowsArgumentException(string categoryBoundaryName)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                                          new DuneErosionFailureMechanism(),
                                                                          assessmentSection,
                                                                          () => 0.01,
                                                                          categoryBoundaryName);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("'categoryBoundaryName' must have a value.", exception.Message);
        }
    }
}