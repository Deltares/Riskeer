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

using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Plugin.TestUtil
{
    [TestFixture]
    public abstract class ShouldCloseViewWithFailurePathTester
    {
        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(asm => asm.SpecificFailurePaths).Return(new ObservableList<IFailurePath>());
            mocks.ReplayAll();

            IFailurePath failurePath = GetFailurePath();

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedAssessmentSectionAndFailurePath_ReturnsTrue()
        {
            // Setup
            IFailurePath failurePath = GetFailurePath();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(asm => asm.SpecificFailurePaths).Return(new ObservableList<IFailurePath>
            {
                failurePath
            });
            mocks.ReplayAll();

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedFailurePath_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var otherFailurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            IFailurePath failurePath = GetFailurePath();

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, otherFailurePath);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedFailurePath_ReturnsTrue()
        {
            // Setup
            IFailurePath failurePath = GetFailurePath();

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failurePath);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedFailurePathContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var otherFailurePath = mocks.Stub<IFailurePath>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            IFailurePath failurePath = GetFailurePath();
            var failurePathContext = new TestFailurePathContext(otherFailurePath, assessmentSection);

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failurePathContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedFailurePathContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            IFailurePath failurePath = GetFailurePath();
            var failurePathContext = new TestFailurePathContext(failurePath, assessmentSection);

            using (IView view = GetView(failurePath))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failurePathContext);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Performs the method that must be tested.
        /// </summary>
        /// <param name="view">The failure mechanism sections view involved.</param>
        /// <param name="o">The object involved.</param>
        /// <returns>Whether the view should close or not.</returns>
        protected abstract bool ShouldCloseMethod(IView view, object o);

        /// <summary>
        /// Gets a view for testing purposes.
        /// </summary>
        /// <param name="failurePath">The failure path containing the data to set to the view.</param>
        /// <returns>A view object.</returns>
        protected abstract IView GetView(IFailurePath failurePath);

        /// <summary>
        /// Gets a failure path for testing purposes.
        /// </summary>
        /// <returns>An <see cref="IFailurePath"/>.</returns>
        protected abstract IFailurePath GetFailurePath();

        private class TestFailurePathContext : IFailurePathContext<IFailurePath>
        {
            public TestFailurePathContext(IFailurePath wrappedFailurePath, IAssessmentSection parent)
            {
                WrappedData = wrappedFailurePath;
                Parent = parent;
            }

            public IFailurePath WrappedData { get; }
            public IAssessmentSection Parent { get; }
        }
    }
}