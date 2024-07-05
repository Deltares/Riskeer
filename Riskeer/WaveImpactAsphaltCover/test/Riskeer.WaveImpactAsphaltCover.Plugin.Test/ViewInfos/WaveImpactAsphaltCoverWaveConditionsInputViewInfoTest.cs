﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Components.Chart.Data;
using Core.Gui.Plugin;
using NUnit.Framework;
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

        protected override ICalculationContext<ICalculation, ICalculatableFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculationContext(
                new WaveImpactAsphaltCoverWaveConditionsCalculation(),
                new CalculationGroup(),
                new WaveImpactAsphaltCoverFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override ICalculationContext<CalculationGroup, ICalculatableFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new WaveImpactAsphaltCoverCalculationGroupContext(
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

        protected override ICalculatableFailureMechanism GetFailureMechanismWithCalculation()
        {
            return new WaveImpactAsphaltCoverFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        new WaveImpactAsphaltCoverWaveConditionsCalculation()
                    }
                }
            };
        }

        #endregion
    }
}