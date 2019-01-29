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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class FailureMechanismSectionsViewInfoTest
    {
        private static ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            using (var plugin = new RingtoetsPlugin())
            {
                info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(FailureMechanismSectionsView));
            }
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionsContext), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<FailureMechanismSection>), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Vakindeling", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsFailureMechanismSections()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();
            var failureMechanismSectionsContext = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            object viewData = info.GetViewData(failureMechanismSectionsContext);

            // Assert
            Assert.AreSame(failureMechanism.Sections, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSectionsIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.SectionsIcon, image);
        }

        [TestFixture]
        public class ShouldCloseFailureMechanismSectionsViewForDataTester : ShouldCloseViewWithFailureMechanismTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                return info.CloseForData(view, o);
            }

            protected override IView GetView(IFailureMechanism failureMechanism)
            {
                return new FailureMechanismSectionsView(failureMechanism.Sections, failureMechanism);
            }

            protected override IFailureMechanism GetFailureMechanism()
            {
                return new TestFailureMechanism();
            }
        }
    }
}