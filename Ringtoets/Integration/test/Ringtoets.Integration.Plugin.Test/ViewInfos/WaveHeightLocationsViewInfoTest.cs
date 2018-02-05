﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
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
            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(),
                                                          new ObservableTestAssessmentSectionStub(),
                                                          () => 0.01))
            {
                // Call
                string viewName = info.GetViewName(view, Enumerable.Empty<HydraulicBoundaryLocation>());

                // Assert
                Assert.AreEqual("Golfhoogtes", viewName);
            }
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(WaveHeightLocationsContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void GetViewData_Always_ReturnsHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            ObservableList<HydraulicBoundaryLocation> locations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            var context = new WaveHeightLocationsContext(locations,
                                                         assessmentSection,
                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                         "Category");

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(locations, viewData);
        }

        [Test]
        public void CreateInstance_Always_SetExpectedProperties()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var context = new WaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                         assessmentSection,
                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                         "Category");

            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightLocationsView));

                // Call
                var view = info.CreateInstance(context) as WaveHeightLocationsView;

                // Assert
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AfterCreate_WithGuiSet_SetsSpecificPropertiesToView()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
            gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

            mocks.ReplayAll();

            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var locations = new ObservableList<HydraulicBoundaryLocation>();
            var context = new WaveHeightLocationsContext(locations,
                                                         assessmentSection,
                                                         hbl => new HydraulicBoundaryLocationCalculation(),
                                                         "Category");

            using (var view = new WaveHeightLocationsView(locations,
                                                          hbl => new HydraulicBoundaryLocationCalculation(),
                                                          assessmentSection,
                                                          () => 0.01))
            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                info = ringtoetsPlugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightLocationsView));
                ringtoetsPlugin.Gui = gui;
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
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(),
                                                          assessmentSection,
                                                          () => 0.01))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseViewForData_ForNonMatchingAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSectionA = new ObservableTestAssessmentSectionStub();
            var assessmentSectionB = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(),
                                                          assessmentSectionA,
                                                          () => 0.01))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionB);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseViewForData_ForOtherObjectType_ReturnsFalse()
        {
            // Setup
            var assessmentSectionA = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(),
                                                          assessmentSectionA,
                                                          () => 0.01))
            {
                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }
    }
}