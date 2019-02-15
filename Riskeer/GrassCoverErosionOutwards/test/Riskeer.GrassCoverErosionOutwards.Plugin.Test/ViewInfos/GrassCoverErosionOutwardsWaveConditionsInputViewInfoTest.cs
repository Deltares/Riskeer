// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.Views;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsInputViewInfoTest : ShouldCloseViewWithCalculationDataTester
    {
        private const int lowerBoundaryRevetmentChartDataIndex = 1;
        private const int upperBoundaryRevetmentChartDataIndex = 2;
        private const int designWaterLevelChartDataIndex = 5;
        private const int revetmentBaseChartDataIndex = 7;
        private const int revetmentChartDataIndex = 8;

        private ViewInfo info;
        private GrassCoverErosionOutwardsPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            plugin = new GrassCoverErosionOutwardsPlugin();
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsInputContext), info.DataType);
            Assert.AreEqual(typeof(ICalculation<FailureMechanismCategoryWaveConditionsInput>), info.ViewDataType);
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
        public void CreateInstance_GrassCoverErosionOutwardsWaveConditionsInputContext_ReturnViewWithStylingApplied()
        {
            // Setup
            var random = new Random(21);
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    CategoryType = random.NextEnumValue<FailureMechanismCategoryType>()
                }
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                new GrassCoverErosionOutwardsWaveConditionsInput(),
                calculation,
                new AssessmentSectionStub(),
                new GrassCoverErosionOutwardsFailureMechanism());

            // Call 
            var view = (WaveConditionsInputView) info.CreateInstance(context);

            // Assert
            ChartDataCollection chartData = view.Chart.Data;

            var lowerBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
            var upperBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
            var revetmentBaseChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentBaseChartDataIndex);
            var revetmentChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentChartDataIndex);

            Color revetmentLineColor = Color.Green;
            Assert.AreEqual(revetmentLineColor, lowerBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, upperBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(Color.FromArgb(120, revetmentLineColor), revetmentBaseChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, revetmentChartData.Style.Color);
        }

        [Test]
        [TestCaseSource(
            typeof(GrassCoverErosionOutwardsAssessmentSectionTestHelper),
            nameof(GrassCoverErosionOutwardsAssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerFailureMechanismCategoryType))]
        public void CreateInstance_GrassCoverErosionOutwardsWaveConditionsInputContext_ReturnViewWithCorrespondingAssessmentLevel(
            AssessmentSectionStub assessmentSection,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            FailureMechanismCategoryType categoryType,
            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = categoryType
                }
            };

            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                failureMechanism);

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
                                               new GrassCoverErosionOutwardsWaveConditionsInputViewStyle());
        }

        protected override ICalculation GetCalculation()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculationContext(
                new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                new CalculationGroup(),
                new GrassCoverErosionOutwardsFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        new GrassCoverErosionOutwardsWaveConditionsCalculation()
                    }
                },
                null,
                new GrassCoverErosionOutwardsFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new GrassCoverErosionOutwardsFailureMechanismContext(
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    WaveConditionsCalculationGroup =
                    {
                        Children =
                        {
                            new GrassCoverErosionOutwardsWaveConditionsCalculation()
                        }
                    }
                }, new AssessmentSectionStub());
        }

        #endregion
    }
}