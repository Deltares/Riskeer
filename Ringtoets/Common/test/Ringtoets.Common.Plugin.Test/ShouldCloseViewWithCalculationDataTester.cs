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

using System.Linq;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Service.Test
{
    public abstract class ShouldCloseViewWithCalculationDataTester
    {
        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue()
        {
            // Setup
            ICalculationContext<ICalculation, IFailureMechanism> calculationContext = GetCalculationContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = calculationContext.WrappedData;

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, calculationContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationContext_ReturnsFalse()
        {
            // Setup
            ICalculationContext<ICalculation, IFailureMechanism> calculationContext = GetCalculationContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = GetCalculation();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, calculationContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext = GetCalculationGroupContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = calculationGroupContext.WrappedData.GetCalculations().First();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, calculationGroupContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingWithRemovedCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            ICalculationContext<CalculationGroup, IFailureMechanism> calculationGroupContext = GetCalculationGroupContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = GetCalculation();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, calculationGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = failureMechanismContext.WrappedData.Calculations.First();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, failureMechanismContext.WrappedData);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = GetCalculation();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, failureMechanismContext.WrappedData);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = failureMechanismContext.WrappedData.Calculations.First();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingWithRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            IFailureMechanismContext<IFailureMechanism> failureMechanismContext = GetFailureMechanismContextWithCalculation();

            using (IView view = GetView())
            {
                view.Data = GetCalculation();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            IFailureMechanism failureMechanism = GetFailureMechanismContextWithCalculation().WrappedData;

            assessmentSectionStub.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (IView view = GetView())
            {
                view.Data = failureMechanism.Calculations.First();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, assessmentSectionStub);

                // Assert
                Assert.IsTrue(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            IFailureMechanism failureMechanism = GetFailureMechanismContextWithCalculation().WrappedData;

            assessmentSectionStub.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (IView view = GetView())
            {
                view.Data = GetCalculation();

                // Call
                bool closeForData = PerformShouldCloseViewWithCalculationDataMethod(view, assessmentSectionStub);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        protected abstract bool PerformShouldCloseViewWithCalculationDataMethod(IView view, object o);

        protected abstract IView GetView();

        protected abstract ICalculation GetCalculation();

        protected abstract ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation();

        protected abstract ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation();

        protected abstract IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation();
    }
}