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
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(GrassCoverErosionInwardsInputView));
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsInputContext), info.DataType);
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculation), info.ViewDataType);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var input = new GrassCoverErosionInwardsInput();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var context = new GrassCoverErosionInwardsInputContext(input,
                                                                   calculation,
                                                                   new GrassCoverErosionInwardsFailureMechanism(),
                                                                   assessmentSection);
            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculation, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationContext_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculationScenario();
            var calculationContext = new GrassCoverErosionInwardsCalculationScenarioContext(calculation,
                                                                                            new CalculationGroup(),
                                                                                            new GrassCoverErosionInwardsFailureMechanism(),
                                                                                            assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationContext);

                // Assert
                Assert.IsTrue(closeForData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationContext_ReturnsFalse()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculationScenario();
            var calculationToRemove = new GrassCoverErosionInwardsCalculationScenario();

            var calculationContext = new GrassCoverErosionInwardsCalculationScenarioContext(calculationToRemove,
                                                                                            new CalculationGroup(),
                                                                                            new GrassCoverErosionInwardsFailureMechanism(),
                                                                                            assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation
            })
            {
                // Call
                bool closeForData = info.CloseForData(view, calculationContext);

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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                              null,
                                                                                              new GrassCoverErosionInwardsFailureMechanism(),
                                                                                              assessmentSection);
            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                              null,
                                                                                              new GrassCoverErosionInwardsFailureMechanism(),
                                                                                              assessmentSection);
            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                              null,
                                                                                              new GrassCoverErosionInwardsFailureMechanism(),
                                                                                              assessmentSection);
            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            var nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            var calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                              null,
                                                                                              new GrassCoverErosionInwardsFailureMechanism(),
                                                                                              assessmentSection);
            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var context = new GrassCoverErosionInwardsCalculationsContext(failureMechanism, assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var context = new GrassCoverErosionInwardsCalculationsContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var context = new GrassCoverErosionInwardsCalculationsContext(failureMechanism, assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
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

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var context = new GrassCoverErosionInwardsCalculationsContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            using (var view = new GrassCoverErosionInwardsInputView
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
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsInputView
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
            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsInputView
            {
                Data = new GrassCoverErosionInwardsCalculation()
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
            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsInputView
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
            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (var view = new GrassCoverErosionInwardsInputView
            {
                Data = new GrassCoverErosionInwardsCalculation()
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