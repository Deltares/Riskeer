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

using System.Collections;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.ViewInfos
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
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters,
                                                                                  calculation,
                                                                                  new AssessmentSectionStub(),
                                                                                  new GrassCoverErosionOutwardsFailureMechanism());

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
        }

        [Test]
        public void CreateInstance_GrassCoverErosionOutwardsWaveConditionsInputContext_ReturnViewWithStylingApplied()
        {
            // Setup
            var context = new GrassCoverErosionOutwardsWaveConditionsInputContext(
                new FailureMechanismCategoryWaveConditionsInput(),
                new GrassCoverErosionOutwardsWaveConditionsCalculation(),
                new AssessmentSectionStub(),
                new GrassCoverErosionOutwardsFailureMechanism());

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

            Color revetmentLineColor = Color.Green;
            Assert.AreEqual(revetmentLineColor, lowerBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, upperBoundaryRevetmentChartData.Style.Color);
            Assert.AreEqual("Waterstand bij doorsnede-eis", designWaterLevelChartData.Name);
            Assert.AreEqual(Color.FromArgb(120, revetmentLineColor), revetmentBaseChartData.Style.Color);
            Assert.AreEqual(revetmentLineColor, revetmentChartData.Style.Color);
        }

        [Test]
        [TestCaseSource(nameof(DifferentCategoryTypes))]
        public void CreateInstance_GrassCoverErosionOutwardsWaveConditionsInputContext_ReturnViewWithCorrespondingAssessmentLevel(
            AssessmentSectionStub assessmentSection,
            GrassCoverErosionOutwardsFailureMechanism failureMechanism,
            HydraulicBoundaryLocation hydraulicBoundaryLocation,
            FailureMechanismCategoryType categoryType,
            RoundedDouble expectedAssessmentLevel)
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism,
                assessmentSection,
                new[]
                {
                    hydraulicBoundaryLocation
                }, true);

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificFactorizedSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificSignalingNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("MechanismSpecificLowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.LowerLimitNorm,
                assessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(0).Output.Result
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                assessmentSection,
                failureMechanism,
                hydraulicBoundaryLocation,
                FailureMechanismCategoryType.FactorizedLowerLimitNorm,
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
            return new WaveConditionsInputView(new GrassCoverErosionOutwardsWaveConditionsInputViewStyle(),
                                               () => (RoundedDouble) 1.1)
            {
                Data = data
            };
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