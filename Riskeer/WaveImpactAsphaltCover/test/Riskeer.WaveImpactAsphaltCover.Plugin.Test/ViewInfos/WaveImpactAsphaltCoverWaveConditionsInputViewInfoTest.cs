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
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.Views;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsInputViewInfoTest : ShouldCloseViewWithCalculationDataTester
    {
        private const int lowerBoundaryRevetmentChartDataIndex = 1;
        private const int upperBoundaryRevetmentChartDataIndex = 2;
        private const int designWaterLevelChartDataIndex = 5;
        private const int revetmentBaseChartDataIndex = 7;
        private const int revetmentChartDataIndex = 8;

        private ViewInfo info;
        private WaveImpactAsphaltCoverPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new WaveImpactAsphaltCoverPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveConditionsInputView));
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
            Assert.AreEqual(typeof(WaveImpactAsphaltCoverWaveConditionsInputContext), info.DataType);
            Assert.AreEqual(typeof(ICalculation<WaveConditionsInput>), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Invoer", viewName);
        }

        [Test]
        public void CreateInstance_WaveImpactAsphaltCoverWaveConditionsInputContext_ReturnViewWithStylingApplied()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                new WaveConditionsInput(),
                calculation,
                new AssessmentSectionStub(),
                Array.Empty<ForeshoreProfile>());

            // Call 
            var view = (WaveConditionsInputView) info.CreateInstance(context);

            // Assert
            ChartDataCollection chartData = view.Chart.Data;

            var lowerBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
            var upperBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
            var revetmentBaseChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentBaseChartDataIndex);
            var revetmentChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentChartDataIndex);

            Color revetmentLineColor = Color.Gray;
            Assert.AreEqual(revetmentLineColor, lowerBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, upperBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(Color.FromArgb(120, revetmentLineColor), revetmentBaseChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, revetmentChartData.Style.Color);
        }

        [Test]
        [TestCaseSource(
            typeof(WaveConditionsInputTestHelper),
            nameof(WaveConditionsInputTestHelper.GetAssessmentLevelConfigurationPerWaterLevelType))]
        public void CreateInstance_WithContextThatHasInputWithSpecificWaterLevelType_ReturnViewWithCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            Action<WaveConditionsInput> configureInputAction,
            RoundedDouble expectedAssessmentLevel)
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            configureInputAction(calculation.InputParameters);

            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                Array.Empty<ForeshoreProfile>());

            // Call
            var view = (WaveConditionsInputView) info.CreateInstance(context);

            // Assert
            ChartDataCollection chartData = view.Chart.Data;
            var designWaterLevelChartData = (ChartLineData) chartData.Collection.ElementAt(designWaterLevelChartDataIndex);

            if (expectedAssessmentLevel != RoundedDouble.NaN)
            {
                Assert.AreEqual(expectedAssessmentLevel, designWaterLevelChartData.Points.First().Y);
            }
            else
            {
                Assert.IsEmpty(designWaterLevelChartData.Points);
            }
        }

        #region ShouldCloseViewWithCalculationDataTester

        protected override bool ShouldCloseMethod(IView view, object o)
        {
            return info.CloseForData(view, o);
        }

        protected override IView GetView(ICalculation data)
        {
            return new WaveConditionsInputView((ICalculation<WaveConditionsInput>) data,
                                               () => new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                                               new WaveImpactAsphaltCoverWaveConditionsInputViewStyle());
        }

        protected override ICalculation GetCalculation()
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculation();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculationContext(
                new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                new CalculationGroup(),
                new WaveImpactAsphaltCoverFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        new WaveImpactAsphaltCoverWaveConditionsCalculation()
                    }
                },
                null,
                new WaveImpactAsphaltCoverFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override IFailurePathContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverHydraulicLoadsContext(
                new WaveImpactAsphaltCoverFailureMechanism
                {
                    WaveConditionsCalculationGroup =
                    {
                        Children =
                        {
                            new WaveImpactAsphaltCoverWaveConditionsCalculation()
                        }
                    }
                }, new AssessmentSectionStub());
        }

        #endregion
        
                #region CloseForData

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationContext = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                             new CalculationGroup(),
                                                                                             new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                             assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationToRemove = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            var calculationContext = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculationToRemove,
                                                                                             new CalculationGroup(),
                                                                                             new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                             assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup,
                                                                                                       null,
                                                                                                       new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                       assessmentSection);
            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationGroupContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingWithRemovedCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                                       null,
                                                                                                       new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                       assessmentSection);
            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingWithRemovedParentCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(calculationGroup,
                                                                                                       null,
                                                                                                       new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                       assessmentSection);
            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationGroupContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewNotCorrespondingWithRemovedParentCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(new CalculationGroup(),
                                                                                                       null,
                                                                                                       new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                                       assessmentSection);
            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationGroupContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                  new CalculationGroup(),
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                  new CalculationGroup(),
                                                                                  new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                  assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                  calculationGroup,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);

            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                                                                                  calculationGroup,
                                                                                  new WaveImpactAsphaltCoverFailureMechanism(),
                                                                                  assessmentSection);

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = GetView(new WaveImpactAsphaltCoverWaveConditionsCalculation()))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = GetView(calculation))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewNotCorrespondingToRemovedAssessmentSection_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = GetView(new WaveImpactAsphaltCoverWaveConditionsCalculation()))
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}