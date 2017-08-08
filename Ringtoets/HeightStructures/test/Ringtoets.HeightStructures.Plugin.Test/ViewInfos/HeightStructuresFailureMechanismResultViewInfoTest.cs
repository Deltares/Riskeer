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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class HeightStructuresFailureMechanismResultViewInfoTest
    {
        private HeightStructuresPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new HeightStructuresPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(HeightStructuresFailureMechanismResultView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<HeightStructuresFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsSectionResults()
        {
            // Setup
            var sectionResults = new[]
            {
                new HeightStructuresFailureMechanismSectionResult(CreateSection())
            };

            var context = new FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(sectionResults, new HeightStructuresFailureMechanism());

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(sectionResults, viewData);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismSectionResultIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var otherFailureMechanism = mocks.Stub<FailureMechanismBase>("N", "C");

            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                otherFailureMechanism
            });

            mocks.ReplayAll();

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new HeightStructuresFailureMechanism();

            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };
            assessmentSection.Stub(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new HeightStructuresFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            // Call
            bool closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            bool closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            bool closeForData = info.CloseForData(view, new HeightStructuresFailureMechanism());

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var view = new HeightStructuresFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            var failureMechanismContext = new HeightStructuresFailureMechanismContext(new HeightStructuresFailureMechanism(), assessmentSection);

            // Call
            bool closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism);

            var mocks = new MockRepository();
            var view = mocks.StrictMock<HeightStructuresFailureMechanismResultView>();
            view.Expect(v => v.FailureMechanism = failureMechanism);
            mocks.ReplayAll();

            // Call
            info.AfterCreate(view, context);

            // Assert
            mocks.VerifyAll();
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
        }
    }
}