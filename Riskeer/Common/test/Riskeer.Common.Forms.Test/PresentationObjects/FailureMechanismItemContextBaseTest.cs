// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismItemContextBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var observable = mockRepository.Stub<IObservable>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var context = new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observable, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<IObservable>>(context);
            Assert.AreSame(observable, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var observable = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observable, null, assessmentSection);

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
            var observable = mockRepository.Stub<IObservable>();
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleFailureMechanismItemContext<IObservable, IFailureMechanism>(observable, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
            mockRepository.VerifyAll();
        }

        private class SimpleFailureMechanismItemContext<TData, TFailureMechanism> : FailureMechanismItemContextBase<TData, TFailureMechanism>
            where TData : IObservable
            where TFailureMechanism : IFailureMechanism
        {
            public SimpleFailureMechanismItemContext(TData target, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
                : base(target, failureMechanism, assessmentSection) {}
        }
    }
}