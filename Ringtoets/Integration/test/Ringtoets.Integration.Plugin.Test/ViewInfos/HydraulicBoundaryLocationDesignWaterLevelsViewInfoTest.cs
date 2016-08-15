// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class HydraulicBoundaryLocationDesignWaterLevelsViewInfoTest
    {
        private MockRepository mocks;
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(HydraulicBoundaryLocationDesignWaterLevelsView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var view = new HydraulicBoundaryLocationDesignWaterLevelsView();

            // Call
            var viewName = info.GetViewName(view, hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual("Toetspeilen", viewName);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(HydraulicBoundaryDatabase), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(DesignWaterLevelLocationsContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GenericInputOutputIcon, image);
        }

        [Test]
        public void GetViewData_Always_ReturnsHydraulicBoundaryDatabase()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();
            var designWaterLevelContext = new DesignWaterLevelLocationsContext(assessmentSection);

            // Call
            var viewData = info.GetViewData(designWaterLevelContext);

            // Assert
            Assert.AreSame(hydraulicBoundaryDatabase, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView
            {
                Data = hydraulicBoundaryDatabase,
                AssessmentSection = assessmentSection
            })
            {
                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSectionButIncorrectHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView
            {
                Data = hydraulicBoundaryDatabase,
                AssessmentSection = assessmentSection
            })
            {
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var otherAssessmentSection = mocks.Stub<IAssessmentSection>();
            otherAssessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mocks.ReplayAll();

            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView
            {
                Data = hydraulicBoundaryDatabase,
                AssessmentSection = assessmentSection
            })
            {
                // Call
                var closeForData = info.CloseForData(view, otherAssessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewWithoutData_ReturnsFalse()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var view = new HydraulicBoundaryLocationDesignWaterLevelsView();

            // Call
            var closeForData = info.CloseForData(view, assessmentSection);

            // Assert
            Assert.IsFalse(closeForData);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_AssessmentSectionWithHydraulicBoundaryDatabase_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mocks.ReplayAll();
            var designWaterLevelContext = new DesignWaterLevelLocationsContext(assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(designWaterLevelContext);

            // Assert
            Assert.IsTrue(additionalDataCheck);
            mocks.VerifyAll();
        }
        
        [Test]
        public void AdditionalDataCheck_AssessmentSectionWithoutHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = null;
            mocks.ReplayAll();
            var designWaterLevelContext = new DesignWaterLevelLocationsContext(assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(designWaterLevelContext);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_WithGuiSet_SetsAssessmentSection()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            IGui guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());

            mocks.ReplayAll();

            var context = new DesignWaterLevelLocationsContext(assessmentSection);

            var view = new HydraulicBoundaryLocationDesignWaterLevelsView();

            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(HydraulicBoundaryLocationDesignWaterLevelsView));
                ringtoetsPlugin.Gui = guiStub;

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(view.AssessmentSection, assessmentSection);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsSpecificPropertiesToView()
        {
            // Setup
            var viewMock = mocks.StrictMock<HydraulicBoundaryLocationDesignWaterLevelsView>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var context = new DesignWaterLevelLocationsContext(assessmentSectionMock);

            viewMock.Expect(v => v.AssessmentSection = assessmentSectionMock);
            viewMock.Expect(v => v.ApplicationSelection = plugin.Gui);

            mocks.ReplayAll();

            // Call
            info.AfterCreate(viewMock, context);

            // Assert
            mocks.VerifyAll();
        }
    }
}