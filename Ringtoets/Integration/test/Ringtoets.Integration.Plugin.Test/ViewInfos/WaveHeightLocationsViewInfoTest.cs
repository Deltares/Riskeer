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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaveHeightLocationsViewInfoTest
    {
        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightLocationsView));
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
            using (var view = new WaveHeightLocationsView())
            {
                // Call
                var viewName = info.GetViewName(view, Enumerable.Empty<HydraulicBoundaryLocation>());

                // Assert
                Assert.AreEqual("Golfhoogtes", viewName);
            }
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(WaveHeightLocationsContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            var image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void GetViewData_Always_ReturnsHydraulicBoundaryDatabase()
        {
            // Setup
            var mocks = new MockRepository();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();
            var context = new WaveHeightLocationsContext(assessmentSection);

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(hydraulicBoundaryDatabase.Locations, viewData);
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AfterCreate_WithGuiSet_SetsSpecificPropertiesToView()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            IGui guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiStub.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            guiStub.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            guiStub.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
            guiStub.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

            mocks.ReplayAll();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            var context = new WaveHeightLocationsContext(assessmentSection);

            using (var view = new WaveHeightLocationsView())
            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightLocationsView));
                ringtoetsPlugin.Gui = guiStub;
                ringtoetsPlugin.Activate();

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(view.CalculationGuiService);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var view = new WaveHeightLocationsView())
            {
                view.AssessmentSection = assessmentSection;

                // Call
                var closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForNonMatchingAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            var assessmentSectionB = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var view = new WaveHeightLocationsView())
            {
                view.AssessmentSection = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, assessmentSectionB);

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForOtherObjectType_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var view = new WaveHeightLocationsView())
            {
                view.Data = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ViewDataNull_ReturnsFalse()
        {
            // Setup
            using (var view = new WaveHeightLocationsView())
            {
                // Call
                var closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }
    }
}