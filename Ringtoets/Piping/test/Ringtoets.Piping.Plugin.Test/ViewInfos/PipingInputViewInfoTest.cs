﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingInputViewInfoTest
    {
        private MockRepository mocks;
        private PipingPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingInputView));
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
            Assert.AreEqual(typeof(PipingInputContext), info.DataType);
            Assert.AreEqual(typeof(PipingCalculationScenario), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingInputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                var calculationScenario = new PipingCalculationScenario(new GeneralPipingInput());

                // Call
                string viewName = info.GetViewName(view, calculationScenario);

                // Assert
                Assert.AreEqual(RingtoetsCommonFormsResources.Calculation_Input, viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedCalculation()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationInputContext = new PipingInputContext(pipingInput, calculation, Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 new PipingFailureMechanism(),
                                                                 assessmentSection);

            // Call
            object viewData = info.GetViewData(calculationInputContext);

            // Assert
            Assert.AreEqual(calculation, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedPipingCalculationScenarioContext_ReturnsTrue()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput());
            var pipingCalculationScenarioContext = new PipingCalculationScenarioContext(pipingCalculation,
                                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                        Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationToRemove = new PipingCalculationScenario(new GeneralPipingInput());

            var pipingCalculationScenarioContext = new PipingCalculationScenarioContext(calculationToRemove,
                                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                        Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                            Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new PipingCalculationGroupContext(new CalculationGroup(),
                                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                            Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new PipingCalculationGroupContext(calculationGroup,
                                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                            Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new PipingCalculationGroupContext(new CalculationGroup(),
                                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                            Enumerable.Empty<StochasticSoilModel>(),
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
            {
                Data = new PipingCalculationScenario(new GeneralPipingInput())
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new PipingInputView
            {
                Data = new PipingCalculationScenario(new GeneralPipingInput())
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