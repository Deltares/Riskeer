﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class MacroStabilityInwardsOutputViewInfoTest
    {
        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsOutputView));
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
            Assert.AreEqual(typeof(MacroStabilityInwardsOutputContext), info.DataType);
            Assert.AreEqual(typeof(MacroStabilityInwardsCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(Resources.GeneralOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsOutputResourceName()
        {
            // Call
            string viewName = info.GetViewName(null, null);

            // Assert
            Assert.AreEqual("Resultaat", viewName);
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var scenario = new MacroStabilityInwardsCalculationScenario();
            var context = new MacroStabilityInwardsOutputContext(scenario, failureMechanism, assessmentSection);

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(scenario, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateInstance_WithContext_SetsDataCorrectly()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var calculationOutputContext = new MacroStabilityInwardsOutputContext(calculation,
                                                                                  new MacroStabilityInwardsFailureMechanism(),
                                                                                  assessmentSection);

            // Call
            IView view = info.CreateInstance(calculationOutputContext);

            // Assert
            Assert.AreSame(calculation, view.Data);
            mocks.VerifyAll();
        }

        [TestFixture]
        public class ShouldCloseMacroStabilityInwardsOutputViewTester : ShouldCloseViewWithCalculationDataTester
        {
            protected override bool ShouldCloseMethod(IView view, object o)
            {
                using (var plugin = new MacroStabilityInwardsPlugin())
                {
                    return plugin.GetViewInfos()
                                 .First(tni => tni.ViewType == typeof(MacroStabilityInwardsOutputView))
                                 .CloseForData(view, o);
                }
            }

            protected override IView GetView(ICalculation data)
            {
                return new MacroStabilityInwardsOutputView((MacroStabilityInwardsCalculationScenario) data, new GeneralMacroStabilityInwardsInput(), () => (RoundedDouble) 1.1);
            }

            protected override ICalculation GetCalculation()
            {
                return new MacroStabilityInwardsCalculationScenario();
            }

            protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
            {
                return new MacroStabilityInwardsCalculationScenarioContext(
                    new MacroStabilityInwardsCalculationScenario(),
                    new CalculationGroup(),
                    Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                    Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                    new MacroStabilityInwardsFailureMechanism(),
                    new AssessmentSectionStub());
            }

            protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
            {
                return new MacroStabilityInwardsCalculationGroupContext(
                    new CalculationGroup
                    {
                        Children =
                        {
                            new MacroStabilityInwardsCalculationScenario()
                        }
                    },
                    null,
                    Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                    Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>(),
                    new MacroStabilityInwardsFailureMechanism(),
                    new AssessmentSectionStub());
            }

            protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
            {
                return new MacroStabilityInwardsFailureMechanismContext(
                    new MacroStabilityInwardsFailureMechanism
                    {
                        CalculationsGroup =
                        {
                            Children =
                            {
                                new MacroStabilityInwardsCalculationScenario()
                            }
                        }
                    }, new AssessmentSectionStub());
            }
        }
    }
}