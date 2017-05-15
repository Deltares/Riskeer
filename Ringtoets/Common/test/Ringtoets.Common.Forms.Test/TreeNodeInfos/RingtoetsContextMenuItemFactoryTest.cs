// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Map toevoegen", toolStripItem.Text);
            Assert.AreEqual("Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(2, calculationGroup.Children.Count);

            var newGroup = calculationGroup.Children[1] as CalculationGroup;
            Assert.IsNotNull(newGroup);
            Assert.AreEqual("Nieuwe map (1)", newGroup.Name);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => {});

            // Assert
            Assert.AreEqual("Berekening &toevoegen", toolStripItem.Text);
            Assert.AreEqual("Voeg een nieuwe berekening toe aan deze berekeningsmap.", toolStripItem.ToolTipText);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => counter++);

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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.", toolStripItem.ToolTipText);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);

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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen dit toetsspoor.", toolStripItem.ToolTipText);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock);

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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null);

            // Assert
            Assert.AreEqual("I&s relevant", toolStripItem.Text);
            Assert.AreEqual("Geeft aan of dit toetsspoor relevant is of niet.", toolStripItem.ToolTipText);
            Bitmap checkboxIcon = isRelevant ? RingtoetsFormsResources.Checkbox_ticked : RingtoetsFormsResources.Checkbox_empty;
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, context => actionCounter++);

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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van deze berekening.", toolStripItem.ToolTipText);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Deze berekening heeft geen uitvoer om te wissen.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);

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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutputMock);

            // Call
            toolStripItem.PerformClick();

            mocks.VerifyAll();
        }

        #region CreateUpdateForshoreProfileOfCalculationItem

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationItem_WithoutForeshoreProfile_CreatesDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculationMock.Expect(c => c.InputParameters.ForeshoreProfile).Return(null);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationItem(
                calculationMock,
                inquiryHelperMock, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofiel...", toolStripItem.Text);
            Assert.AreEqual("Geselecteerd voorlandprofiel heeft geen wijzingingen om bij te werken.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.UpdateItemIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationItem_WithForeshoreProfile_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Expect(c => c.InputParameters).Return(input);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationItem(
                calculationMock,
                inquiryHelperMock, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofiel...", toolStripItem.Text);
            Assert.AreEqual("Berekening bijwerken waar een voorlandprofiel geselecteerd is.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.UpdateItemIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationItem_WithoutCalculationOutputPerformClick_PerformsAction()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Expect(c => c.InputParameters).Return(input);
            calculationMock.Expect(c => c.HasOutput).Return(false);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationItem(
                calculationMock,
                inquiryHelperMock,
                c => { actionCalculation = c; });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreSame(calculationMock, actionCalculation);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationItem_WithCalculationOutputPerformClickNoContinuation_DoesNotPerformAction()
        {
            // Setup
            string inquireContinuationMessage = "Wanneer het voorlandprofiel wijzigt als gevolg van het bijwerken, " +
                                                "zal het resultaat van deze berekening worden verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}" +
                                                "Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Expect(c => c.InputParameters).Return(input);
            calculationMock.Expect(c => c.HasOutput).Return(true);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            inquiryHelperMock.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(false);
            mocks.ReplayAll();

            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationItem(
                calculationMock,
                inquiryHelperMock,
                c => { actionPerformed = true; });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.IsFalse(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationItem_WithCalculationOutputPerformClickWithContinuation_PerformsAction()
        {
            // Setup
            string inquireContinuationMessage = "Wanneer het voorlandprofiel wijzigt als gevolg van het bijwerken, " +
                                                "zal het resultaat van deze berekening worden verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}" +
                                                "Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Expect(c => c.InputParameters).Return(input);
            calculationMock.Expect(c => c.HasOutput).Return(true);
            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            inquiryHelperMock.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(true);
            mocks.ReplayAll();

            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationItem(
                calculationMock,
                inquiryHelperMock,
                c => { actionPerformed = true; });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        #endregion

        #region CreateUpdateForshoreProfileOfCalculationsItem

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithoutForeshoreProfile_CreatesDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            calculationMock.Expect(c => c.InputParameters.ForeshoreProfile).Return(null);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock
                },
                inquiryHelperMock, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofielen...", toolStripItem.Text);
            Assert.AreEqual("De geselecteerde voorlandprofielen hebben geen wijzigingen om bij te werken.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.UpdateItemIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithForeshoreProfileSynchronized_CreatesDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputMock.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputMock.Stub(ci => ci.IsForeshoreProfileParametersSynchronized).Return(true);
            calculationMock.Stub(c => c.InputParameters).Return(inputMock);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock
                },
                inquiryHelperMock, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofielen...", toolStripItem.Text);
            Assert.AreEqual("De geselecteerde voorlandprofielen hebben geen wijzigingen om bij te werken.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.UpdateItemIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithForeshoreProfile_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Stub(c => c.InputParameters).Return(input);

            var calculationWithoutChangesMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChangesMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputWithoutChangesMock.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputWithoutChangesMock.Stub(ci => ci.IsForeshoreProfileParametersSynchronized).Return(true);
            calculationWithoutChangesMock.Stub(c => c.InputParameters).Return(inputWithoutChangesMock);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock,
                    calculationWithoutChangesMock
                },
                inquiryHelperMock, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofielen...", toolStripItem.Text);
            Assert.AreEqual("Berekeningen bijwerken waar een voorlandprofiel geselecteerd is.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.UpdateItemIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithoutCalculationOutputPerformClick_PerformsAction()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Stub(c => c.InputParameters).Return(input);
            calculationMock.Expect(c => c.HasOutput).Return(false);

            var calculationWithoutChangesMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChangesMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputWithoutChangesMock.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputWithoutChangesMock.Stub(ci => ci.IsForeshoreProfileParametersSynchronized).Return(true);
            calculationWithoutChangesMock.Stub(c => c.InputParameters).Return(inputWithoutChangesMock);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock,
                    calculationWithoutChangesMock
                },
                inquiryHelperMock,
                c => { actionCalculation = c; });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreSame(calculationMock, actionCalculation);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithCalculationOutputPerformClickNoContinuation_DoesNotPerformAction()
        {
            // Setup
            string inquireContinuationMessage = "Wanneer het voorlandprofiel wijzigt als gevolg van het bijwerken, " +
                                                "zal het resultaat van de berekeningen worden verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}" +
                                                "Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputMock.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputMock.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Stub(c => c.InputParameters).Return(inputMock);
            calculationMock.Expect(c => c.HasOutput).Return(true);

            var calculationWithoutChangesMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChangesMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputWithoutChangesMock.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputWithoutChangesMock.Stub(ci => ci.IsForeshoreProfileParametersSynchronized).Return(true);
            calculationWithoutChangesMock.Stub(c => c.InputParameters).Return(inputWithoutChangesMock);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            inquiryHelperMock.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(false);
            mocks.ReplayAll();

            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock,
                    calculationWithoutChangesMock
                },
                inquiryHelperMock,
                c => { actionPerformed = true; });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.IsFalse(actionPerformed);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateForshoreProfileOfCalculationsItem_WithCalculationOutputPerformClickWithContinuation_PerformsAction()
        {
            // Setup
            string inquireContinuationMessage = "Wanneer het voorlandprofiel wijzigt als gevolg van het bijwerken, " +
                                                "zal het resultaat van de berekeningen worden verwijderd." +
                                                $"{Environment.NewLine}{Environment.NewLine}" +
                                                "Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculationMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileParametersSynchronized).Return(false);
            calculationMock.Stub(c => c.InputParameters).Return(input);
            calculationMock.Expect(c => c.HasOutput).Return(true);

            var calculationWithoutChangesMock = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChangesMock = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputWithoutChangesMock.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputWithoutChangesMock.Stub(ci => ci.IsForeshoreProfileParametersSynchronized).Return(true);
            calculationWithoutChangesMock.Stub(c => c.InputParameters).Return(inputWithoutChangesMock);

            var inquiryHelperMock = mocks.StrictMock<IInquiryHelper>();
            inquiryHelperMock.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(true);
            mocks.ReplayAll();

            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForshoreProfileOfCalculationsItem(
                new[]
                {
                    calculationMock,
                    calculationWithoutChangesMock
                },
                inquiryHelperMock,
                c =>
                {
                    Assert.AreSame(calculationMock, c);
                    actionPerformed = true;
                });

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.IsTrue(actionPerformed);
            mocks.VerifyAll();
        }

        #endregion

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
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer deze berekening uit.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, (calc, context) => counter++, context => null);

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
            Assert.AreEqual("&Valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer de invoer voor deze berekening.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => errorMessage);

            // Assert
            Assert.AreEqual("&Valideren", toolStripItem.Text);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, calc => counter++, c => null);

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
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer alle berekeningen binnen deze berekeningsmap uit.", toolStripItem.ToolTipText);
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
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, (group, context) => counter++, context => null);

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
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer alle berekeningen binnen deze berekeningsmap.", toolStripItem.ToolTipText);
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
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(
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
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer alle berekeningen binnen dit toetsspoor uit.", toolStripItem.ToolTipText);
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
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
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
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, fmContext => counter++, context => null);

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
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer alle berekeningen binnen dit toetsspoor.", toolStripItem.ToolTipText);
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
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
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

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
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

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
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

            public CalculationGroup WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}

        #endregion
    }
}