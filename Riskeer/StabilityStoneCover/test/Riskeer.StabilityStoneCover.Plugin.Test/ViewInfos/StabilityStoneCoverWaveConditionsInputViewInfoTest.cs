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

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Views;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsInputViewInfoTest : ShouldCloseViewWithCalculationDataTester
    {
        private const int lowerBoundaryRevetmentChartDataIndex = 1;
        private const int upperBoundaryRevetmentChartDataIndex = 2;
        private const int designWaterLevelChartDataIndex = 5;
        private const int revetmentBaseChartDataIndex = 7;
        private const int revetmentChartDataIndex = 8;

        private ViewInfo info;
        private StabilityStoneCoverPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new StabilityStoneCoverPlugin();
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
            Assert.AreEqual(typeof(StabilityStoneCoverWaveConditionsInputContext), info.DataType);
            Assert.AreEqual(typeof(ICalculation<AssessmentSectionCategoryWaveConditionsInput>), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, info.Image);
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
        public void CreateInstance_StabilityStoneCoverWaveConditionsInputContext_ReturnViewWithStylingApplied()
        {
            // Setup
            var random = new Random(21);
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>()
                }
            };
            var context = new StabilityStoneCoverWaveConditionsInputContext(
                new AssessmentSectionCategoryWaveConditionsInput(),
                calculation,
                new AssessmentSectionStub(),
                new ForeshoreProfile[0]);

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
            typeof(AssessmentSectionTestHelper),
            nameof(AssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerAssessmentSectionCategoryType))]
        public void CreateInstance_WithContextThatHasInputWithSpecificCategoryType_ReturnViewWithCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            AssessmentSectionCategoryType categoryType,
            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation)
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = categoryType
                }
            };

            var context = new StabilityStoneCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new ForeshoreProfile[0]);

            // Call
            var view = (WaveConditionsInputView) info.CreateInstance(context);

            // Assert
            ChartDataCollection chartData = view.Chart.Data;
            var designWaterLevelChartData = (ChartLineData) chartData.Collection.ElementAt(designWaterLevelChartDataIndex);
            Assert.AreEqual(expectedHydraulicBoundaryLocationCalculation.Output.Result, designWaterLevelChartData.Points.First().Y);
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
                                               new StabilityStoneCoverWaveConditionsInputViewStyle());
        }

        protected override ICalculation GetCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculation();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculationContext(
                new StabilityStoneCoverWaveConditionsCalculation(),
                new CalculationGroup(),
                new StabilityStoneCoverFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        new StabilityStoneCoverWaveConditionsCalculation()
                    }
                },
                null,
                new StabilityStoneCoverFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new StabilityStoneCoverFailureMechanismContext(
                new StabilityStoneCoverFailureMechanism
                {
                    WaveConditionsCalculationGroup =
                    {
                        Children =
                        {
                            new StabilityStoneCoverWaveConditionsCalculation()
                        }
                    }
                }, new AssessmentSectionStub());
        }

        #endregion
    }
}