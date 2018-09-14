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

using System;
using System.Drawing;
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
    public class DuneLocationCalculationsViewInfoTest
    {
        private DuneErosionPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new DuneErosionPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(DuneLocationCalculationsView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void GetViewName_WithContext_ReturnsViewNameContainingCategoryBoundaryName()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string categoryBoundaryName = "A";
            var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                              new DuneErosionFailureMechanism(),
                                                              assessmentSection,
                                                              () => 0.01,
                                                              categoryBoundaryName);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual($"Hydraulische belastingen - Categoriegrens {categoryBoundaryName}", viewName);
            mocks.VerifyAll();
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IObservableEnumerable<DuneLocationCalculation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(DuneLocationCalculationsContext), dataType);
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
        public void GetViewData_Always_ReturnWrappedDataInContext()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                              new DuneErosionFailureMechanism(),
                                                              assessmentSection,
                                                              () => 0.01,
                                                              "A");

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(context.WrappedData, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationsEmpty_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                              new DuneErosionFailureMechanism(),
                                                              assessmentSection,
                                                              () => 0.01,
                                                              "A");

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_WithCalculations_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var duneLocationCalculations = new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new TestDuneLocation())
            };

            var context = new DuneLocationCalculationsContext(duneLocationCalculations,
                                                              new DuneErosionFailureMechanism(),
                                                              assessmentSection,
                                                              () => 0.01,
                                                              "A");

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var window = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(gs => gs.MainWindow).Return(window);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationCalculationsContext(new ObservableList<DuneLocationCalculation>(),
                                                              failureMechanism,
                                                              assessmentSection,
                                                              () => 0.01,
                                                              "A");

            plugin.Gui = gui;
            plugin.Activate();

            // Call
            using (var view = info.CreateInstance(context) as DuneLocationCalculationsView)
            {
                // Assert
                Assert.AreSame(assessmentSection, view.AssessmentSection);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsExpectedProperties()
        {
            // Setup
            Func<double> getNormFunc = () => 0.01;

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var window = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(gs => gs.MainWindow).Return(window);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var calculations = new ObservableList<DuneLocationCalculation>();

            const string categoryBoundaryName = "A";

            var data = new DuneLocationCalculationsContext(calculations,
                                                           failureMechanism,
                                                           assessmentSection,
                                                           getNormFunc,
                                                           categoryBoundaryName);

            plugin.Gui = gui;
            plugin.Activate();

            using (var view = new DuneLocationCalculationsView(calculations,
                                                               failureMechanism,
                                                               assessmentSection,
                                                               getNormFunc,
                                                               categoryBoundaryName))
            {
                // Call
                info.AfterCreate(view, data);

                // Assert
                Assert.IsInstanceOf<DuneLocationCalculationGuiService>(view.CalculationGuiService);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

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
            assessmentSectionA.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionA.Stub(a => a.Detach(null)).IgnoreArguments();
            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSectionA,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionB);

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
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            var duneErosionFailureMechanismContext = new DuneErosionFailureMechanismContext(
                failureMechanism,
                assessmentSection);

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, duneErosionFailureMechanismContext);

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
            assessmentSectionA.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionA.Stub(a => a.Detach(null)).IgnoreArguments();
            assessmentSectionB.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            mocks.ReplayAll();

            var duneErosionFailureMechanismContext = new DuneErosionFailureMechanismContext(
                new DuneErosionFailureMechanism(),
                assessmentSectionB);

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSectionA,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, duneErosionFailureMechanismContext);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
            {
                new DuneErosionFailureMechanism()
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();
            mocks.ReplayAll();

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSection,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ViewDataNull_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSection,
                                                               () => 0.01,
                                                               "A"))
            {
                // Call
                bool closeForData = info.CloseForData(view, null);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }
    }
}