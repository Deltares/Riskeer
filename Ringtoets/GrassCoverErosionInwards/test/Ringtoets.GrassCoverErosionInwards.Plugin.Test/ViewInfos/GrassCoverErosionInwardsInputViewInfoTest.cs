// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsInputViewInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
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
            Assert.AreEqual(typeof(GrassCoverErosionInwardsInput), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsInputResourceName()
        {
            // Setup
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView())
            {
                GrassCoverErosionInwardsInput input = new GrassCoverErosionInwardsInput();

                // Call
                string viewName = info.GetViewName(view, input);

                // Assert
                Assert.AreEqual(Resources.GrassCoverErosionInwardsInputContext_NodeDisplayName, viewName);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsWrappedInput()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsInput input = new GrassCoverErosionInwardsInput();
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsInputContext context = new GrassCoverErosionInwardsInputContext(input,
                                                                                                    calculation,
                                                                                                    new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                    assessmentSection);
            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreEqual(input, viewData);
            mocks.VerifyAll();
        }

        [Test]
        public void CloseForData_ViewCorrespondingToRemovedCalculationScenarioContext_ReturnsTrue()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculationContext calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation,
                                                                                                                     new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                                     assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_ViewNotCorrespondingToRemovedCalculationScenarioContext_ReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsCalculation calculationToRemove = new GrassCoverErosionInwardsCalculation();

            GrassCoverErosionInwardsCalculationContext calculationContext = new GrassCoverErosionInwardsCalculationContext(calculationToRemove,
                                                                                                                     new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                                     assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                                      new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                      assessmentSection);
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                                      new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                      assessmentSection);
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            CalculationGroup nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                                                      new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                      assessmentSection);
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            CalculationGroup nestedGroup = new CalculationGroup();
            nestedGroup.Children.Add(calculation);
            calculationGroup.Children.Add(nestedGroup);

            GrassCoverErosionInwardsCalculationGroupContext calculationGroupContext = new GrassCoverErosionInwardsCalculationGroupContext(new CalculationGroup(),
                                                                                                      new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                      assessmentSection);
            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_ViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_ViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_NestedViewCorrespondingToRemovedFailureMechanism_ReturnsTrue()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_NestedViewNotCorrespondingToRemovedFailureMechanism_ReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            GrassCoverErosionInwardsFailureMechanismContext failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
        public void CloseForData_ViewCorrespondingToRemovedAssessmentSection_ReturnsTrue()
        {
            // Setup
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = new GrassCoverErosionInwardsCalculation().InputParameters
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
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
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
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            CalculationGroup calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            GrassCoverErosionInwardsFailureMechanism failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(section => section.GetFailureMechanisms()).Return(new[]
            {
                failureMechanism
            });

            mocks.ReplayAll();

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
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
        public void AfterCreate_Always_SetsCalculationOnView()
        {
            // Setup
            IAssessmentSection assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation();
            GrassCoverErosionInwardsInputContext context = new GrassCoverErosionInwardsInputContext(calculation.InputParameters, calculation,
                                                                new GrassCoverErosionInwardsFailureMechanism(), 
                                                                assessmentSection);

            using (GrassCoverErosionInwardsInputView view = new GrassCoverErosionInwardsInputView
            {
                Data = calculation.InputParameters
            })
            {
                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.AreSame(calculation, view.Calculation);
                mocks.VerifyAll();
            }
        }
    }
}
