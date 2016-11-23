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
using Core.Common.Base;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
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
        [TestCase(true)]
        [TestCase(false)]
        public void CreateToggleRelevancyOfFailureMechanismItem_IsRelevant_CreateDecoratedItem(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>())
            {
                IsRelevant = isRelevant
            };
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip, toolStripItem.ToolTipText);
            var checkboxIcon = isRelevant ? RingtoetsFormsResources.Checkbox_ticked : RingtoetsFormsResources.Checkbox_empty;
            TestHelper.AssertImagesAreEqual(checkboxIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateToggleRelevancyOfFailureMechanismItem_PerformClickOnRelevanceItem_RelevanceChangedAndObserversNotified(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.NotifyObservers());
            failureMechanismMock.Expect(fm => fm.IsRelevant).Return(isRelevant);
            failureMechanismMock.Expect(fm => fm.IsRelevant).SetPropertyWithArgument(!isRelevant);
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanismMock, assessmentSectionMock);
            var actionCounter = 0;
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, context => actionCounter++);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, actionCounter);
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

        #region CreatePerformCalculationItem

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

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
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, (calc, context) => counter++, context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);

            mocks.VerifyAll();
        }

        #endregion

        #region CreateValidateCalculationItem

        [Test]
        public void CreateValidateCalculationItem_AdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Validate_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            var counter = 0;
            var toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, calc => counter++, c => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);

            mocks.VerifyAll();
        }

        #endregion

        #region CreatePerformAllCalculationsInGroupItem

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var nestedGroup = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    nestedGroup
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var counter = 0;
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, (group, context) => counter++, context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        #endregion

        #region CreateValidateAllCalculationsInGroupItem

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_Validate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var nestedGroup = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    nestedGroup
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.ValidateAll_No_calculations_to_validate, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.ValidateAll_No_calculations_to_validate, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var counter = 0;
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(
                calculationGroupContext,
                context => counter++,
                context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        #endregion

        #region CreatePerformAllCalculationInFailureMechanismItem

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

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
            var toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, fmContext => counter++, context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
            mocks.VerifyAll();
        }

        #endregion

        #region CreateValidateAllCalculationsInFailureMechanismItem

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.FailureMechanism_Validate_all_ToolTip, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.ValidateAll_No_calculations_to_validate, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.Validate_all, toolStripItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.ValidateAll_No_calculations_to_validate, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculationMock = mocks.StrictMock<ICalculation>();
            mocks.ReplayAll();

            var counter = 0;
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculationMock
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                fm => counter++,
                fm => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
            mocks.VerifyAll();
        }

        #endregion

        #region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
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

            public Commentable Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }

        #endregion
    }
}