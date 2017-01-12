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
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class DuneErosionLocationsViewInfoTest
    {
        private DuneErosionPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new DuneErosionPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(DuneLocationsView));
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
            using (var view = new DuneLocationsView())
            {
                // Call
                var viewName = info.GetViewName(view, Enumerable.Empty<DuneLocation>());

                // Assert
                Assert.AreEqual("Hydraulische randvoorwaarden", viewName);
            }
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            var viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<DuneLocation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            var dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(DuneLocationsContext), dataType);
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
        public void GetViewData_Always_ReturnWrappedDataInContext()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            // Call
            var viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(failureMechanism.DuneLocations, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_LocationsEmpty_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            // Call
            var additionalDataCheck = info.AdditionalDataCheck(context);
            
            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_WithLocations_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.DuneLocations.Add(new TestDuneLocation());

            var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

            // Call
            var additionalDataCheck = info.AdditionalDataCheck(context);
            
            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void AfterCreate_Always_SetsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            IGui guiStub = mocks.Stub<IGui>();
            IMainWindow windowsStub = mocks.Stub<IMainWindow>();
            guiStub.Stub(gs => gs.MainWindow).Return(windowsStub);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var data = new DuneLocationsContext(
                new ObservableList<DuneLocation>(),
                failureMechanism,
                assessmentSectionStub);

            plugin.Gui = guiStub;
            plugin.Activate();

            using (var view = new DuneLocationsView())
            {
                // Call
                info.AfterCreate(view, data);

                // Assert
                Assert.AreSame(assessmentSectionStub, view.AssessmentSection);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.IsInstanceOf<DuneLocationCalculationGuiService>(view.CalculationGuiService);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new DuneLocationsView())
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

            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });

            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });

            mocks.ReplayAll();

            using (var view = new DuneLocationsView())
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
        public void CloseViewForData_ForMatchingFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            var duneErosionFailureMechanismContext = new DuneErosionFailureMechanismContext(
                new DuneErosionFailureMechanism(),
                assessmentSection);

            using (var view = new DuneLocationsView())
            {
                view.AssessmentSection = assessmentSection;

                // Call
                var closeForData = info.CloseForData(view, duneErosionFailureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForNonMatchingFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionA = mocks.Stub<IAssessmentSection>();
            var assessmentSectionB = mocks.Stub<IAssessmentSection>();

            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });

            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            var duneErosionFailureMechanismContext = new DuneErosionFailureMechanismContext(
                new DuneErosionFailureMechanism(),
                assessmentSectionB);

            using (var view = new DuneLocationsView())
            {
                view.AssessmentSection = assessmentSectionA;

                // Call
                var closeForData = info.CloseForData(view, duneErosionFailureMechanismContext);

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
            assessmentSectionA.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new DuneLocationsView())
            {
                view.AssessmentSection = assessmentSectionA;

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
            using (var view = new DuneLocationsView())
            {
                // Call
                var closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }
    }
}