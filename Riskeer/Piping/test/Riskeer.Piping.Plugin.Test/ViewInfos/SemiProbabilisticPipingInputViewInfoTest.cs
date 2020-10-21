﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;
using PipingFormsResources = Riskeer.Piping.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class SemiProbabilisticPipingInputViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.DataType == typeof(SemiProbabilisticPipingInputContext) && tni.ViewType == typeof(PipingInputView));
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
            Assert.AreEqual(typeof(SemiProbabilisticPipingInputContext), info.DataType);
            Assert.AreEqual(typeof(SemiProbabilisticPipingCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingInputIcon, info.Image);
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
        public void GetViewData_CorrectContext_ReturnsWrappedCalculation()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingInput = new SemiProbabilisticPipingInput();

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationInputContext = new SemiProbabilisticPipingInputContext(pipingInput, calculation, Enumerable.Empty<PipingSurfaceLine>(),
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
        public void CloseForData_ViewCorrespondingToRemovedPipingCalculationScenarioContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculation = new SemiProbabilisticPipingCalculationScenario();
            var pipingCalculationScenarioContext = new SemiProbabilisticPipingCalculationScenarioContext(pipingCalculation,
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

            var pipingCalculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationToRemove = new SemiProbabilisticPipingCalculationScenario();

            var pipingCalculationScenarioContext = new SemiProbabilisticPipingCalculationScenarioContext(calculationToRemove,
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new PipingInputView
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            using (var view = new PipingInputView
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

            using (var view = new PipingInputView
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

            using (var view = new PipingInputView
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var view = new PipingInputView
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new PipingFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_NestedViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            using (var view = new PipingInputView
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, new PipingFailureMechanism());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            var calculation = new SemiProbabilisticPipingCalculationScenario();
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
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
                Data = new SemiProbabilisticPipingCalculationScenario()
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
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
            var calculation = new SemiProbabilisticPipingCalculationScenario();
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
                Data = new SemiProbabilisticPipingCalculationScenario()
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