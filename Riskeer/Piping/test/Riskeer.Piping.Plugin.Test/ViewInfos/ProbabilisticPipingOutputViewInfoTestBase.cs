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

using System.Linq;
using System.Threading;
using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    /// <summary>
    /// Base class for probabilistic piping output view into tests.
    /// </summary>
    /// <typeparam name="TView">The type of view.</typeparam>
    /// <typeparam name="TOutputContext">The type of output context.</typeparam>
    /// <typeparam name="TTopLevelIllustrationPoint">The type of the top level illustration point.</typeparam>
    [Apartment(ApartmentState.STA)]
    public abstract class ProbabilisticPipingOutputViewInfoTestBase<TView, TOutputContext, TTopLevelIllustrationPoint> : ShouldCloseViewWithCalculationDataTester
        where TView : IView where TTopLevelIllustrationPoint : TopLevelIllustrationPointBase
    {
        private PipingPlugin plugin;
        private ViewInfo info;

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        protected abstract string ViewName { get; }

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(TView));
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
            Assert.AreEqual(typeof(TOutputContext), info.DataType);
            Assert.AreEqual(typeof(ProbabilisticPipingCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual(ViewName, viewName);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedCalculationScenario()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationScenario = new ProbabilisticPipingCalculationScenario();
            TOutputContext context = GetContext(calculationScenario, assessmentSection);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculationScenario, viewData);
            mocks.VerifyAll();
        }
        
        [Test]
        public void AdditionalDataCheck_CalculationWithoutOutput_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TOutputContext context = GetContext(new ProbabilisticPipingCalculationScenario(), assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithOutputWithoutIllustrationPoints_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationScenario = new ProbabilisticPipingCalculationScenario
            {
                Output = GetOutputWithCorrectIllustrationPoints(null)
            };
            TOutputContext context = GetContext(calculationScenario, assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithOutputWithIncorrectIllustrationPointsType_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationScenario = new ProbabilisticPipingCalculationScenario
            {
                Output = GetOutputWithIncorrectIllustrationPoints()
            };
            TOutputContext context = GetContext(calculationScenario, assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithOutputWithCorrectIllustrationPointsType_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationScenario = new ProbabilisticPipingCalculationScenario
            {
                Output = GetOutputWithCorrectIllustrationPoints(new GeneralResult<TTopLevelIllustrationPoint>(
                                                                    new WindDirection("test", 0),
                                                                    new Stochast[0],
                                                                    new TTopLevelIllustrationPoint[0]))
            };
            TOutputContext context = GetContext(calculationScenario, assessmentSection);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsTrue(additionalDataCheck);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            TOutputContext context = GetContext(new ProbabilisticPipingCalculationScenario(), assessmentSection);

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<TView>(view);
            mocks.VerifyAll();
        }

        protected abstract TOutputContext GetContext(ProbabilisticPipingCalculationScenario calculationScenario,
                                                     IAssessmentSection assessmentSection);

        protected abstract ProbabilisticPipingOutput GetOutputWithCorrectIllustrationPoints(GeneralResult<TTopLevelIllustrationPoint> generalResult);

        protected abstract ProbabilisticPipingOutput GetOutputWithIncorrectIllustrationPoints();

        protected override bool ShouldCloseMethod(IView view, object o)
        {
            return info.CloseForData(view, o);
        }

        protected override ICalculation GetCalculation()
        {
            return new ProbabilisticPipingCalculationScenario();
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new ProbabilisticPipingCalculationScenarioContext(
                new ProbabilisticPipingCalculationScenario(),
                new CalculationGroup(),
                Enumerable.Empty<PipingSurfaceLine>(),
                Enumerable.Empty<PipingStochasticSoilModel>(),
                new PipingFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new PipingCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        new ProbabilisticPipingCalculationScenario()
                    }
                },
                null,
                Enumerable.Empty<PipingSurfaceLine>(),
                Enumerable.Empty<PipingStochasticSoilModel>(),
                new PipingFailureMechanism(),
                new AssessmentSectionStub());
        }

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new PipingFailureMechanismContext(
                new PipingFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new ProbabilisticPipingCalculationScenario()
                        }
                    }
                }, new AssessmentSectionStub());
        }
    }
}