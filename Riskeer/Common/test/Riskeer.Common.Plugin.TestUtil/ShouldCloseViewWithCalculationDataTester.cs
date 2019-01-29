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

using System.Linq;
using System.Threading;
using Core.Common.Base;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Plugin.TestUtil
{
    /// <summary>
    /// Class for testing <see cref="ShouldCloseMethod"/> for views that contain calculation data.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public abstract class ShouldCloseViewWithCalculationDataTester
    {
        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue()
        {
            // Setup
            ICalculationContext<ICalculation, IFailureMechanism> calculationContext = GetCalculationContextWithCalculation();

            using (IView view = GetView(calculationContext.WrappedData))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, calculationContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedCalculationContext_ReturnsFalse()
        {
            // Setup
            ICalculationContext<ICalculation, IFailureMechanism> calculationContext = GetCalculationContextWithCalculation();

            using (IView view = GetView(GetCalculation()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, calculationContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext = GetCalculationGroupContextWithCalculation();

            using (IView view = GetView(calculationGroupContext.WrappedData.GetCalculations().First()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, calculationGroupContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingWithRemovedCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext = GetCalculationGroupContextWithCalculation();

            using (IView view = GetView(GetCalculation()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, calculationGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingWithRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView(failureMechanismContext.WrappedData.Calculations.First()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext.WrappedData);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingWithRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView(GetCalculation()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext.WrappedData);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingWithRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView(failureMechanismContext.WrappedData.Calculations.First()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingWithRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView(GetCalculation()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void ShouldCloseMethod_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            IFailureMechanism failureMechanism = GetFailureMechanismContextWithCalculation().WrappedData;

            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (IView view = GetView(failureMechanism.Calculations.First()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ShouldCloseMethod_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            IFailureMechanism failureMechanism = GetFailureMechanismContextWithCalculation().WrappedData;

            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (IView view = GetView(GetCalculation()))
            {
                // Call
                bool closeForData = ShouldCloseMethod(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Performs the method that must be tested.
        /// </summary>
        /// <param name="view">The view involved.</param>
        /// <param name="o">The object involved.</param>
        /// <returns>Whether the view should close or not.</returns>
        protected abstract bool ShouldCloseMethod(IView view, object o);

        /// <summary>
        /// Gets a view for testing purposes.
        /// </summary>
        /// <param name="data">The calculation to set as data of the view.</param>
        /// <returns>A view object.</returns>
        protected abstract IView GetView(ICalculation data);

        /// <summary>
        /// Gets a calculation for testing purposes.
        /// </summary>
        /// <returns>A calculation object.</returns>
        /// <remarks>A default implementation is added for creating a <see cref="TestCalculation"/>.</remarks>
        protected virtual ICalculation GetCalculation()
        {
            return new TestCalculation("Calculation");
        }

        /// <summary>
        /// Gets a calculation context for testing purposes.
        /// </summary>
        /// <returns>A calculation context object.</returns>
        /// <remarks>A default implementation is added for creating a <see cref="TestCalculationContext"/>.</remarks>
        protected virtual ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new TestCalculationContext();
        }

        /// <summary>
        /// Gets a calculation group context for testing purposes.
        /// </summary>
        /// <returns>A calculation group context object.</returns>
        /// <remarks>A default implementation is added for creating a <see cref="TestCalculationGroupContext"/>.</remarks>
        protected virtual ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new TestCalculationGroupContext();
        }

        /// <summary>
        /// Gets a failure mechanism context for testing purposes.
        /// </summary>
        /// <returns>A failure mechanism context object.</returns>
        /// <remarks>A default implementation is added for creating a <see cref="TestFailureMechanismContext"/>.</remarks>
        protected virtual IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new TestFailureMechanismContext();
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, TestFailureMechanism>
        {
            public TestCalculationContext()
            {
                WrappedData = new TestCalculation("Calculation");
                Parent = new CalculationGroup();
                FailureMechanism = new TestFailureMechanism();
            }

            public TestCalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public TestFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, TestFailureMechanism>
        {
            public TestCalculationGroupContext()
            {
                WrappedData = new CalculationGroup
                {
                    Children =
                    {
                        new TestCalculation("Calculation")
                    }
                };
                Parent = new CalculationGroup();
                FailureMechanism = new TestFailureMechanism();
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public TestFailureMechanism FailureMechanism { get; }
        }

        private class TestFailureMechanismContext : Observable, IFailureMechanismContext<TestFailureMechanism>
        {
            public TestFailureMechanismContext()
            {
                WrappedData = new TestFailureMechanism(new[]
                {
                    new TestCalculation("Calculation")
                });
            }

            public TestFailureMechanism WrappedData { get; }

            public IAssessmentSection Parent { get; }
        }
    }
}