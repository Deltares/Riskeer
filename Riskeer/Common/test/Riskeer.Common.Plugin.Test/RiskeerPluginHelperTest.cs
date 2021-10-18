﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Core.Common.Controls.Views;
using NUnit.Framework;
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
        public class ShouldCloseFailureMechanismSectionsViewForDataTester : ShouldCloseViewWithFailureMechanismTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return RiskeerPluginHelper.ShouldCloseForFailureMechanismView((FailureMechanismSectionsView) view, o);
            }

            protected override IView GetView(IFailureMechanism failureMechanism)
            {
                return new TestFailureMechanismSectionsView(failureMechanism);
            }

            private class TestFailureMechanismSectionsView : FailureMechanismSectionsView
            {
                public TestFailureMechanismSectionsView(IFailurePath failurePath)
                    : base(failurePath.Sections, failurePath) {}
            }

            protected override IFailureMechanism GetFailureMechanism()
            {
                return new TestFailureMechanism();
            }
        }
    }
}