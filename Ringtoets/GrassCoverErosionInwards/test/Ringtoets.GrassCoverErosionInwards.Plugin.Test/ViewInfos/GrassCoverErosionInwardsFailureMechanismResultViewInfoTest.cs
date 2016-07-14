﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismResultViewInfoTest
    {
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsFailureMechanismResultView));
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
            Assert.AreEqual(typeof(FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>), info.DataType);
            Assert.AreEqual(typeof(IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult>), info.ViewDataType);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            var viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsSectionResults()
        {
            // Setup
            var sectionResults = new[]
            {
                new GrassCoverErosionInwardsFailureMechanismSectionResult(CreateSection()),
            };

            var context = new FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(
                sectionResults, new GrassCoverErosionInwardsFailureMechanism());

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(sectionResults, viewData);
        }

        [Test]
        public void Image_Always_ReturnsFailureMechanismSectionResultIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, image);
        }

        [Test]
        public void CloseForData_AssessmentSectionRemovedWithoutFailureMechanism_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[0]);

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new[]
            {
                 new GrassCoverErosionInwardsFailureMechanism()
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, assessmentSectionMock);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };
            assessmentSectionMock.Expect(asm => asm.GetFailureMechanisms()).Return(new IFailureMechanism[]
            {
                new GrassCoverErosionInwardsFailureMechanism(),
                failureMechanism
            });

            mocks.ReplayAll();

            // Call
            var closeForData = info.CloseForData(view, assessmentSectionMock);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            var closeForData = info.CloseForData(view, failureMechanism);

            // Assert
            Assert.IsTrue(closeForData);
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            var closeForData = info.CloseForData(view, new GrassCoverErosionInwardsFailureMechanism());

            // Assert
            Assert.IsFalse(closeForData);
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsTrue(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var view = new GrassCoverErosionInwardsFailureMechanismResultView
            {
                Data = failureMechanism.SectionResults
            };

            var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(),
                                                                                              assessmentSectionMock);

            // Call
            var closeForData = info.CloseForData(view, failureMechanismContext);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var context = new FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults,
                                                                                                                          failureMechanism);

            var mocks = new MockRepository();
            var viewMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanismResultView>();
            viewMock.Expect(v => v.FailureMechanism = failureMechanism);
            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

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
