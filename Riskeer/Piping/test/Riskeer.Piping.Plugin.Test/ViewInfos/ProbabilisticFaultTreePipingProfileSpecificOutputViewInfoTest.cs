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

using System.Linq;
using System.Threading;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
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
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ProbabilisticFaultTreePipingProfileSpecificOutputViewInfoTest
    {
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.DataType == typeof(ProbabilisticPipingProfileSpecificOutputContext) &&
                                                      tni.ViewType == typeof(ProbabilisticFaultTreePipingOutputView));
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
            Assert.AreEqual(typeof(ProbabilisticPipingCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewData_WithContext_ReturnsWrappedCalculationScenario()
        {
            // Setup
            var calculationScenario = new ProbabilisticPipingCalculationScenario();
            var context = new ProbabilisticPipingProfileSpecificOutputContext(calculationScenario);

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
            Assert.AreEqual("Sterkte berekening doorsnede", viewName);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithoutOutput_ReturnsTrue()
        {
            // Setup
            var context = new ProbabilisticPipingProfileSpecificOutputContext(new ProbabilisticPipingCalculationScenario());

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithoutFaultTreeOutput_ReturnsFalse()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
            };

            var context = new ProbabilisticPipingProfileSpecificOutputContext(calculation);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsFalse(additionalDataCheck);
        }

        [Test]
        public void AdditionalDataCheck_CalculationWithFaultTreeOutput_ReturnsTrue()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = new ProbabilisticPipingOutput(
                    PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput(),
                    PipingTestDataGenerator.GetRandomPartialProbabilisticFaultTreePipingOutput())
            };

            var context = new ProbabilisticPipingProfileSpecificOutputContext(calculation);

            // Call
            bool additionalDataCheck = info.AdditionalDataCheck(context);

            // Assert
            Assert.IsTrue(additionalDataCheck);
        }

        [Test]
        public void CreateInstance_WithContext_ReturnsView()
        {
            // Setup
            var context = new ProbabilisticPipingProfileSpecificOutputContext(new ProbabilisticPipingCalculationScenario());

            // Call
            IView view = info.CreateInstance(context);

            // Assert
            Assert.IsInstanceOf<ProbabilisticFaultTreePipingOutputView>(view);
        }

        [TestFixture]
        public class ProbabilisticPipingProfileSpecificOutputViewTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new PipingPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(ProbabilisticFaultTreePipingOutputView))
                                 .CloseForData(view, o);
                }
            }

            protected override IView GetView(ICalculation data)
            {
                return new ProbabilisticFaultTreePipingOutputView(
                    (ProbabilisticPipingCalculationScenario) data,
                    () => new TestGeneralResultFaultTreeIllustrationPoint());
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

            protected override IFailureMechanism GetFailureMechanismWithCalculation()
            {
                return new PipingFailureMechanism
                {
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new ProbabilisticPipingCalculationScenario()
                        }
                    }
                };
            }
        }
    }
}