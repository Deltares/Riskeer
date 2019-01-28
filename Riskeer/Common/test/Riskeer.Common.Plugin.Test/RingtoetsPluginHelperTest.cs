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
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Plugin;
using Ringtoets.Common.Plugin.TestUtil;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsPluginHelperTest
    {
        [Test]
        public void FormatCategoryBoundaryName_CategoryBoundaryNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsPluginHelper.FormatCategoryBoundaryName(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("categoryBoundaryName", paramName);
        }

        [Test]
        public void FormatCategoryBoundaryName_WithCategoryBoundaryName_ReturnsExpectedValue()
        {
            // Setup
            const string categoryBoundaryName = "A";

            // Call
            string formattedCategoryBoundaryName = RingtoetsPluginHelper.FormatCategoryBoundaryName(categoryBoundaryName);

            // Assert
            Assert.AreEqual("Categoriegrens " + categoryBoundaryName, formattedCategoryBoundaryName);
        }

        [TestFixture]
        public class ShouldCloseViewWithCalculationDataTest : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return RingtoetsPluginHelper.ShouldCloseViewWithCalculationData(view, o);
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

                public string Text { get; set; }

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
                return RingtoetsPluginHelper.ShouldCloseForFailureMechanismView((FailureMechanismSectionsView) view, o);
            }

            protected override IView GetView(IFailureMechanism failureMechanism)
            {
                return new TestFailureMechanismSectionsView(failureMechanism);
            }

            private class TestFailureMechanismSectionsView : FailureMechanismSectionsView
            {
                public TestFailureMechanismSectionsView(IFailureMechanism failureMechanism)
                    : base(failureMechanism.Sections, failureMechanism) {}
            }

            protected override IFailureMechanism GetFailureMechanism()
            {
                return new TestFailureMechanism();
            }
        }
    }
}