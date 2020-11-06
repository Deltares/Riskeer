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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ProbabilisticPipingSectionSpecificOutputViewInfoTest
    {
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(ProbabilisticPipingSectionSpecificOutputView));
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
            Assert.AreEqual(typeof(ProbabilisticPipingSectionSpecificOutputContext), info.DataType);
            Assert.AreEqual(typeof(ProbabilisticPipingCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedCalculationScenario()
        {
            // Setup
            var calculationScenario = new ProbabilisticPipingCalculationScenario();
            var context = new ProbabilisticPipingSectionSpecificOutputContext(calculationScenario);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculationScenario, viewData);
        }

        [Test]
        public void GetViewName_Always_ReturnsCorrectViewName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Sterkte berekening vak", viewName);
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var context = new ProbabilisticPipingSectionSpecificOutputContext(new ProbabilisticPipingCalculationScenario());

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ProbabilisticPipingSectionSpecificOutputView>(view);
        }

        [TestFixture]
        public class ProbabilisticPipingSectionSpecificOutputViewTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new PipingPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(ProbabilisticPipingSectionSpecificOutputView))
                                 .CloseForData(view, o);
                }
            }

            protected override IView GetView(ICalculation data)
            {
                return new ProbabilisticPipingSectionSpecificOutputView(() => new TestGeneralResultFaultTreeIllustrationPoint())
                {
                    Data = data
                };
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
}