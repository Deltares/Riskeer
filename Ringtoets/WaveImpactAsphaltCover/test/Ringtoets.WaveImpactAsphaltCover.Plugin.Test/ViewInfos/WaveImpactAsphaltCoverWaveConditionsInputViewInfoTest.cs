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

using System.Collections;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
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
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.ViewInfos
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
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                               calculation,
                                                                               new AssessmentSectionStub(),
                                                                               new ForeshoreProfile[0]);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
        }

        [Test]
        public void CreateInstance_WaveImpactAsphaltCoverWaveConditionsInputContext_ReturnViewWithStylingApplied()
        {
            // Setup
            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                new AssessmentSectionCategoryWaveConditionsInput(),
                new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                new AssessmentSectionStub(),
                new ForeshoreProfile[0]);

            // Call 
            var view = (WaveConditionsInputView) info.CreateInstance(context);
            view.Data = context.Calculation;

            // Assert
            ChartDataCollection chartData = view.Chart.Data;

            var lowerBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(lowerBoundaryRevetmentChartDataIndex);
            var upperBoundaryRevetmentChartData = (ChartLineData) chartData.Collection.ElementAt(upperBoundaryRevetmentChartDataIndex);
            var designWaterLevelChartData = (ChartLineData) chartData.Collection.ElementAt(designWaterLevelChartDataIndex);
            var revetmentBaseChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentBaseChartDataIndex);
            var revetmentChartData = (ChartLineData) chartData.Collection.ElementAt(revetmentChartDataIndex);

            Color revetmentLineColor = Color.Gray;
            Assert.AreEqual(revetmentLineColor, lowerBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, upperBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual("Toetspeil", designWaterLevelChartData.Name);
            Assert.AreEqual(Color.FromArgb(120, revetmentLineColor), revetmentBaseChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, revetmentChartData.Style.Color);
        }

        [Test]
        [TestCaseSource(nameof(DifferentCategoryTypes))]
        public void CreateInstance_WithContextThatHasInputWithSpecificCategoryType_ReturnViewWithCorrespondingAssessmentLevel(
            IAssessmentSection assessmentSection,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            AssessmentSectionCategoryType categoryType,
            RoundedDouble expectedAssessmentLevel)
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = categoryType
                }
            };

            var context = new WaveImpactAsphaltCoverWaveConditionsInputContext(
                calculation.InputParameters,
                calculation,
                assessmentSection,
                new ForeshoreProfile[0]);

            // Call
            var view = (WaveConditionsInputView) info.CreateInstance(context);
            view.Data = context.Calculation;

            // Assert
            ChartDataCollection chartData = view.Chart.Data;
            var designWaterLevelChartData = (ChartLineData) chartData.Collection.ElementAt(designWaterLevelChartDataIndex);
            Assert.AreEqual(expectedAssessmentLevel, designWaterLevelChartData.Points.First().Y);
        }

        private static IEnumerable DifferentCategoryTypes()
        {
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                AssessmentSectionCategoryType.FactorizedSignalingNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(0).Output.Result
            ).SetName("FactorizedSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                AssessmentSectionCategoryType.SignalingNorm,
                assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(0).Output.Result
            ).SetName("SignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                AssessmentSectionCategoryType.LowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                hydraulicBoundaryLocation,
                AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("FactorizedLowerLimitNorm");
        }

        #region ShouldCloseViewWithCalculationDataTester

        protected override bool ShouldCloseMethod(IView view, object o)
        {
            return info.CloseForData(view, o);
        }

        protected override IView GetView(ICalculation data)
        {
            return new WaveConditionsInputView(new WaveImpactAsphaltCoverWaveConditionsInputViewStyle(),
                                               () => (RoundedDouble) 1.1)
            {
                Data = data
            };
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

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverFailureMechanismContext(
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
    }
}