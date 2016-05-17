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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using BaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsContextMenuItemFactoryTest : NUnitFormTest
    {
        [Test]
        public void CreateAddCalculationGroupItem_Always_CreatesDecoratedItem()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.AddFolderIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateAddCalculationGroupItem_PerformClickOnCreatedItem_CalculationGroupWithUniqueNameAdded()
        {
            // Setup
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    new CalculationGroup()
                }
            };

            // Precondition
            Assert.AreEqual(1, calculationGroup.Children.Count);

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(2, calculationGroup.Children.Count);

            var newGroup = calculationGroup.Children[1] as CalculationGroup;
            Assert.IsNotNull(newGroup);
            Assert.AreEqual(string.Format("{0} (1)", RingtoetsDataResources.CalculationGroup_DefaultName), newGroup.Name);
        }

        [Test]
        public void CreateAddCalculationItem_Always_CreatesDecoratedItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => { });

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_Add_Calculation, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FailureMechanismIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAddCalculationItem_PerformClickOnCreatedItem_AddCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var counter = 0;
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => counter++);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_GroupWithCalculationOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutputMock
                }
            };

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_ClearOutput_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_GroupWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithoutOutputMock.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithoutOutputMock
                }
            };

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var mocks = new MockRepository();
            var calculationWithOutputMock1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutputMock2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutputMock.Stub(c => c.HasOutput).Return(false);

            calculationWithOutputMock1.Expect(c => c.ClearOutput());
            calculationWithOutputMock1.Expect(c => c.NotifyObservers());
            calculationWithOutputMock2.Expect(c => c.ClearOutput());
            calculationWithOutputMock2.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutputMock1,
                    new CalculationGroup
                    {
                        Children =
                        {
                            calculationWithOutputMock2,
                            calculationWithoutOutputMock
                        }
                    }
                }
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(BaseResources.Confirm, messageBoxTitle);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_ClearOutput_Are_you_sure_clear_all_output, messageBoxText);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var mocks = new MockRepository();
            var calculationWithOutputMock1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutputMock2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutputMock.Stub(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutputMock1,
                    new CalculationGroup
                    {
                        Children =
                        {
                            calculationWithOutputMock2,
                            calculationWithoutOutputMock
                        }
                    }
                }
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_FailureMechanismWithCalculationOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();
            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutputMock
            });

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_FailureMechanismWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();
            calculationWithoutOutputMock.Expect(c => c.HasOutput).Return(false);
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithoutOutputMock
            });
            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutputMock2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutputMock.Stub(c => c.HasOutput).Return(false);

            calculationWithOutputMock1.Expect(c => c.ClearOutput());
            calculationWithOutputMock1.Expect(c => c.NotifyObservers());
            calculationWithOutputMock2.Expect(c => c.ClearOutput());
            calculationWithOutputMock2.Expect(c => c.NotifyObservers());

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutputMock
            });

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickOk();
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Call
            toolStripItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutputMock2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutputMock.Stub(c => c.HasOutput).Return(false);

            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutputMock
            });

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

            // Call
            toolStripItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GroupWithCalculations_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GroupWithoutCalculations_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_IsEnabledFuncTrue_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_IsEnabledFuncFalse_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => false);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var counter = 0;
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationMock
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanisMock);

            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, (group, context) => counter++, context => true);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_FailureMechanismItemWithCalculations_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismMock = new TestFailureMechanism(new[]
            {
                calculationMock
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanismMock, assessmentSectionMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_FailureMechanismItemWithoutCalculations_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismMock = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanismMock, assessmentSectionMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var counter = 0;
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculationMock
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, fmContext => counter++, context => true);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateChangeRelevancyOfFailureMechanismItem_Always_CreateDecoratedItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanismMock, assessmentSectionMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateChangeRelevancyOfFailureMechanismItem(failureMechanismContext, null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.Checkbox_ticked, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateChangeRelevancyOfFailureMechanismItem_PerformClickOnRelevanceItem_RelevanceFalseAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.NotifyObservers());
            failureMechanismMock.Expect(fm => fm.IsRelevant).SetPropertyWithArgument(false);
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanismMock, assessmentSectionMock);
            var counter = 0;
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateChangeRelevancyOfFailureMechanismItem(failureMechanismContext, context => counter++);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePerformCalculationItem_IsEnabledFuncConditional_CreatesDecoratedAndConditionalEnabledItem(bool isEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => isEnabled);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.AreEqual(isEnabled, toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            var counter = 0;
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, (calc, context) => counter++, context => true);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_CalculationWithOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Clear_output_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_CalculationWithoutOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Clear_output, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.ClearOutput_No_output_to_clear, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock.Expect(c => c.ClearOutput());
            calculationWithOutputMock.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(BaseResources.Confirm, messageBoxTitle);
            Assert.AreEqual(RingtoetsFormsResources.Calculation_ContextMenuStrip_Are_you_sure_clear_output, messageBoxText);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var mocks = new MockRepository();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Stub(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Call
            toolStripItem.PerformClick();

            mocks.VerifyAll();
        }

        [Test]
        public void CreateDisabledChangeRelevancyItem_Always_CreatesDecoratedItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismContextMock = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateDisabledChangeRelevancyItem(failureMechanismContextMock);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.Checkbox_empty, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateDisabledChangeRelevancyItem_PerformClickOnCreatedItem_FailureMechanismIsRelevantSetToTrueAndObserversNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var failureMechanismContextMock = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();

            failureMechanismContextMock.Stub(c => c.WrappedData).Return(failureMechanismMock);
            failureMechanismMock.Expect(c => c.IsRelevant).SetPropertyWithArgument(true);
            failureMechanismMock.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateDisabledChangeRelevancyItem(failureMechanismContextMock);

            // Call
            toolStripItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_IsEnabledFuncTrue_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                new TestCalculation()
            });

            var failureMechanismContextMock = mocks.StrictMock<FailureMechanismContext<IFailureMechanism>>(failureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContextMock, null, context => true);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_IsEnabledFuncFalse_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            var failureMechanismContextMock = mocks.StrictMock<FailureMechanismContext<IFailureMechanism>>(failureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContextMock, null, context => false);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        # region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
        }

        private class TestFailureMechanism : FailureMechanismBase<FailureMechanismSectionResult>
        {
            private readonly IEnumerable<ICalculation> calculations;

            public TestFailureMechanism(IEnumerable<ICalculation> calculations)
                : base("Name", "Code")
            {
                this.calculations = calculations;
            }

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    return calculations;
                }
            }

            protected override FailureMechanismSectionResult CreateFailureMechanismSectionResult(FailureMechanismSection section)
            {
                return null;
            }
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public string Comments { get; set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}

            public void ClearHydraulicBoundaryLocation() {}

            public ICalculationInput GetObservableInput()
            {
                return null;
            }

            public ICalculationOutput GetObservableOutput()
            {
                return null;
            }
        }

        # endregion
    }
}