// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class DuneLocationCalculationsViewInfoTest
    {
        [Test]
        [TestCase(0.025, 0.0025, 0.00025, "1/400")]
        [TestCase(0.0025, 0.0025, 0.025, "1/400 (1)")]
        [TestCase(0.0025, 0.0025, 0.0025, "1/400 (2)")]
        public void GetViewName_WithContext_ReturnsExpectedViewName(double userDefinedTargetProbability1,
                                                                    double userDefinedTargetProbability2,
                                                                    double userDefinedTargetProbability3,
                                                                    string expectedProbabilityText)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability2);
            var failureMechanism = new DuneErosionFailureMechanism
            {
                DuneLocationCalculationsForUserDefinedTargetProbabilities =
                {
                    new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability1),
                    new DuneLocationCalculationsForTargetProbability(userDefinedTargetProbability3),
                    calculationsForTargetProbability
                }
            };

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(calculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                string viewName = info.GetViewName(null, context);

                // Assert
                Assert.AreEqual($"Hydraulische belastingen - {expectedProbabilityText}", viewName);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                Type viewDataType = info.ViewDataType;

                // Assert
                Assert.AreEqual(typeof(IObservableEnumerable<DuneLocationCalculation>), viewDataType);
            }
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                Type dataType = info.DataType;

                // Assert
                Assert.AreEqual(typeof(DuneLocationCalculationsForUserDefinedTargetProbabilityContext), dataType);
            }
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsDuneLocationCalculations()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                             new DuneErosionFailureMechanism(),
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                object viewData = info.GetViewData(context);

                // Assert
                Assert.AreSame(context.WrappedData.DuneLocationCalculations, viewData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var window = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(window);
            gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                plugin.Gui = gui;
                plugin.Activate();

                // Call
                using (var view = info.CreateInstance(context) as DuneLocationCalculationsView)
                {
                    // Assert
                    Assert.AreSame(assessmentSection, view.AssessmentSection);
                    Assert.AreSame(failureMechanism, view.FailureMechanism);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsExpectedProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var window = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(window);
            gui.Stub(g => g.ViewHost).Return(mocks.Stub<IViewHost>());
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();
            var data = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(0.1),
                                                                                          failureMechanism,
                                                                                          assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                ViewInfo info = GetInfo(plugin);

                plugin.Gui = gui;
                plugin.Activate();

                using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                   failureMechanism,
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   () => "1/100"))
                {
                    // Call
                    info.AfterCreate(view, data);

                    // Assert
                    Assert.IsInstanceOf<DuneLocationCalculationGuiService>(view.CalculationGuiService);
                }
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

            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

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

            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSectionA,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionB);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingFailureMechanism_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();

            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               failureMechanism,
                                                               new AssessmentSectionStub(),
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseViewForData_ForNonMatchingFailureMechanism_ReturnsFalse()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               new AssessmentSectionStub(),
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, new DuneErosionFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
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

            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSection,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

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

            using (var plugin = new DuneErosionPlugin())
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSection,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                ViewInfo info = GetInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, null);

                // Assert
                Assert.IsFalse(closeForData);
            }

            mocks.VerifyAll();
        }

        private static ViewInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetViewInfos().First(vi => vi.ViewType == typeof(DuneLocationCalculationsView));
        }
    }
}