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

using Core.Common.Base;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin.TestUtil;

namespace Riskeer.Common.Plugin.Test
{
    [TestFixture]
    public class RiskeerPluginHelperTest
    {
        [TestFixture]
        public class ShouldCloseViewWithCalculationDataTest : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return RiskeerPluginHelper.ShouldCloseViewWithCalculationData(view, o);
            }

            protected override IView GetView(ICalculation data)
            {
                return new TestView
                {
                    Data = data
                };
            }

            private class TestView : IView
            {
                public object Data { get; set; }

                public void Dispose()
                {
                    // Nothing to dispose
                }
            }
        }

        [TestFixture]
        public class ShouldCloseFailureMechanismViewForDataTester : ShouldCloseViewWithFailureMechanismTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return RiskeerPluginHelper.ShouldCloseForFailureMechanismView((CloseForFailurePathView)view, o);
            }

            protected override IView GetView(IFailureMechanism failureMechanism)
            {
                return new TestFailureMechanismView(failureMechanism);
            }

            private class TestFailureMechanismView : CloseForFailurePathView
            {
                public TestFailureMechanismView(IFailureMechanism failureMechanism)
                    : base(failureMechanism) {}
            }

            protected override IFailureMechanism GetFailureMechanism()
            {
                return new TestFailureMechanism();
            }
        }

        [TestFixture]
        public class ShouldCloseFailurePathViewForDataTester : ShouldCloseViewWithFailurePathTester
        {
            [Test]
            public void ShouldCloseMethod_ViewCorrespondingToRemovedAssessmentSectionAndFailureMechanism_ReturnsTrue()
            {
                // Setup
                IFailureMechanism failureMechanism = new TestFailureMechanism();

                var mocks = new MockRepository();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
                {
                    failureMechanism
                });
                assessmentSection.Stub(asm => asm.SpecificFailurePaths).Return(new ObservableList<IFailurePath>());
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

            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return RiskeerPluginHelper.ShouldCloseForFailurePathView((CloseForFailurePathView) view, o);
            }

            protected override IView GetView(IFailurePath failurePath)
            {
                return new TestFailurePathView(failurePath);
            }

            protected override IFailurePath GetFailurePath()
            {
                return new TestFailurePath();
            }

            private class TestFailurePathView : CloseForFailurePathView
            {
                public TestFailurePathView(IFailurePath failurePath)
                    : base(failurePath) {}
            }
        }
    }
}