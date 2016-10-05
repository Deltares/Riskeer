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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.ClosingStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ClosingStructuresContextBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var target = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var context = new SimpleClosingStructuresContext<IObservable>(target, failureMechanism, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(assessmentSectionMock, context.AssessmentSection);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var observableObject = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleClosingStructuresContext<IObservable>(observableObject, null, assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observableObject = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new SimpleClosingStructuresContext<IObservable>(observableObject, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        private class SimpleClosingStructuresContext<T> : ClosingStructuresContextBase<T> where T : IObservable
        {
            public SimpleClosingStructuresContext(T target, ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) { }
        }
    }
}
