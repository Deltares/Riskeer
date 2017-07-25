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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;
using FormsResources = Ringtoets.MacroStabilityInwards.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class MacroStabilityInwardsInputViewInfoTest
    {
        private MockRepository mocks;
        private MacroStabilityInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new MacroStabilityInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(MacroStabilityInwardsInputView));
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
            Assert.AreEqual(typeof(MacroStabilityInwardsInputContext), info.DataType);
            Assert.AreEqual(typeof(MacroStabilityInwardsCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(FormsResources.MacroStabilityInwardsInputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculationScenario = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

                // Call
                string viewName = info.GetViewName(view, calculationScenario);

                // Assert
                Assert.AreEqual("Invoer", viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationInputContext = new MacroStabilityInwardsInputContext(input, calculation, Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                new MacroStabilityInwardsFailureMechanism(),
                                                                                assessmentSection);

            // Call
            object viewData = info.GetViewData(calculationInputContext);

            // Assert
            Assert.AreSame(calculation, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationScenarioContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationScenarioContext = new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                                 Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                                                 new MacroStabilityInwardsFailureMechanism(),
                                                                                                 assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationScenarioContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationScenarioContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationToRemove = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            var calculationScenarioContext = new MacroStabilityInwardsCalculationScenarioContext(calculationToRemove,
                                                                                                 Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                                                 new MacroStabilityInwardsFailureMechanism(),
                                                                                                 assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationScenarioContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                           Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<StochasticSoilModel>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);
            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                           Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<StochasticSoilModel>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);
            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(calculationGroup,
                                                                                           Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<StochasticSoilModel>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);
            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new MacroStabilityInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                           Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                           Enumerable.Empty<StochasticSoilModel>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);
            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingToRemovedFailureMechanismContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_NestedViewNotCorrespondingToRemovedFailureMechanismContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var failureMechanismContext = new MacroStabilityInwardsFailureMechanismContext(new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanismContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new MacroStabilityInwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, failureMechanism);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseForData_NestedViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new MacroStabilityInwardsFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            })
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
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
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
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }
    }
}