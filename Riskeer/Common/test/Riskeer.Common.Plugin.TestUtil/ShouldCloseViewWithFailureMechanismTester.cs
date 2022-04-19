// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Plugin.TestUtil
{
    /// <summary>
    /// Class for testing <see cref="ShouldCloseMethod"/> for views related to a failure mechanism.
    /// </summary>
    [TestFixture]
    public abstract class ShouldCloseViewWithFailureMechanismTester
    {
        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(asm => asm.SpecificFailureMechanisms).Return(new ObservableList<SpecificFailureMechanism>());
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            using (IView view = GetView(failureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedAssessmentSectionAndFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(asm => asm.SpecificFailureMechanisms).Return(new ObservableList<SpecificFailureMechanism>());
            mocks.ReplayAll();

            using (IView view = GetView(failureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedAssessmentSectionAndFailureMechanism_ReturnsFalse()
        {
            // Setup
            var otherFailureMechanism = new TestCalculatableFailureMechanism();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(asm => asm.SpecificFailureMechanisms).Return(new ObservableList<SpecificFailureMechanism>());
            mocks.ReplayAll();

            using (IView view = GetView(otherFailureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();

            using (IView view = GetView(failureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var otherFailureMechanism = new TestCalculatableFailureMechanism();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            using (IView view = GetView(otherFailureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanism);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedAssessmentSectionAndSpecificFailureMechanism_ReturnsTrue()
        {
            // Setup
            var specificFailureMechanism = new SpecificFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(Enumerable.Empty<IFailureMechanism>());
            assessmentSection.Stub(asm => asm.SpecificFailureMechanisms).Return(new ObservableList<SpecificFailureMechanism>
            {
                specificFailureMechanism
            });
            mocks.ReplayAll();

            using (IView view = GetView(specificFailureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var otherFailureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanismContext = new TestFailureMechanismContext(otherFailureMechanism, assessmentSection);

            using (IView view = GetView(failureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            using (IView view = GetView(failureMechanism))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext);

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
        /// <param name="failureMechanism">The failure mechanism containing the data to set to the view.</param>
        /// <returns>A view object.</returns>
        protected abstract IView GetView(IFailureMechanism failureMechanism);

        private class TestFailureMechanismContext : IFailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent)
            {
                WrappedData = wrappedFailureMechanism;
                Parent = parent;
            }

            public IFailureMechanism WrappedData { get; }
            public IAssessmentSection Parent { get; }
        }
    }
}