// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.CalculationsState;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class ProbabilisticPipingInputViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.DataType == typeof(ProbabilisticPipingInputContext) && tni.ViewType == typeof(PipingInputView));
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
        public void GetViewData_WithContext_ReturnsWrappedCalculation()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingInput = new ProbabilisticPipingInput();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationInputContext = new ProbabilisticPipingInputContext(pipingInput, calculation, Enumerable.Empty<PipingSurfaceLine>(),
                                                                              Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                              new PipingFailureMechanism(),
                                                                              assessmentSection);

            // Call
            object viewData = info.GetViewData(calculationInputContext);

            // Assert
            Assert.AreSame(calculation, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedPipingCalculation_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculation = new ProbabilisticPipingCalculationScenario();
            var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(pipingCalculation,
                                                                                                     new CalculationGroup(),
                                                                                                     Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                     new PipingFailureMechanism(),
                                                                                                     assessmentSection);

            using (var view = new PipingInputView
            {
                Data = pipingCalculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, pipingCalculationScenarioContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedPipingCalculationScenarioContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculation = new ProbabilisticPipingCalculationScenario();
            var calculationToRemove = new ProbabilisticPipingCalculationScenario();

            var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculationToRemove,
                                                                                                     new CalculationGroup(),
                                                                                                     Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                     new PipingFailureMechanism(),
                                                                                                     assessmentSection);

            using (var view = new PipingInputView
            {
                Data = pipingCalculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, pipingCalculationScenarioContext);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingWithRemovedPipingCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                            null,
                                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                            new PipingFailureMechanism(),
                                                                            assessmentSection);
            using (var view = new PipingInputView
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
        public void CloseForData_ViewNotCorrespondingWithRemovedPipingCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new PipingCalculationGroupContext(new CalculationGroup(),
                                                                            null,
                                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                            new PipingFailureMechanism(),
                                                                            assessmentSection);
            using (var view = new PipingInputView
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
        public void CloseForData_NestedViewCorrespondingWithRemovedParentPipingCalculationGroupContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                            null,
                                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                            new PipingFailureMechanism(),
                                                                            assessmentSection);
            using (var view = new PipingInputView
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
        public void CloseForData_NestedViewNotCorrespondingWithRemovedParentPipingCalculationGroupContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new ProbabilisticPipingCalculationScenario();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new PipingCalculationGroupContext(new CalculationGroup(),
                                                                            null,
                                                                            Enumerable.Empty<PipingSurfaceLine>(),
                                                                            Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                            new PipingFailureMechanism(),
                                                                            assessmentSection);
            using (var view = new PipingInputView
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

            var calculation = new ProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var context = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

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

            var calculation = new ProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var context = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

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

            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var context = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

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

            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var context = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, context);

                // Assert
                Assert.IsFalse(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
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
            var calculation = new ProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
            {
                Data = new ProbabilisticPipingCalculationScenario()
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
            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
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
            var calculation = new ProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
            {
                Data = new ProbabilisticPipingCalculationScenario()
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