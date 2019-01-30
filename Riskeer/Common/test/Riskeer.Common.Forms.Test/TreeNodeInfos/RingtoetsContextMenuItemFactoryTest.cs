// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using RiskeerFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Test.TreeNodeInfos
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
            Assert.AreEqual("Voeg een nieuwe map toe aan deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.AddFolderIcon, toolStripItem.Image);
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
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => {});

            // Assert
            Assert.AreEqual("Berekening &toevoegen", toolStripItem.Text);
            Assert.AreEqual("Voeg een nieuwe berekening toe aan deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.FailureMechanismIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateAddCalculationItem_PerformClickOnCreatedItem_AddCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var counter = 0;
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);
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
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput
                }
            };

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_GroupWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithoutOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithoutOutput
                }
            };

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
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
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

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
                            calculationWithoutOutput
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
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

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
                            calculationWithoutOutput
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
            var calculationWithOutput = mocks.StrictMock<ICalculation>();
            calculationWithOutput.Expect(c => c.HasOutput).Return(true);
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutput
            });

            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen dit toetsspoor.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_FailureMechanismWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();
            calculationWithoutOutput.Expect(c => c.HasOutput).Return(false);
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithoutOutput
            });
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
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
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

            calculationWithOutputMock1.Expect(c => c.ClearOutput());
            calculationWithOutputMock1.Expect(c => c.NotifyObservers());
            calculationWithOutputMock2.Expect(c => c.ClearOutput());
            calculationWithOutputMock2.Expect(c => c.NotifyObservers());

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutput
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
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutputMock2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutput
            });

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>())
            {
                IsRelevant = isRelevant
            };
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null);

            // Assert
            Assert.AreEqual("I&s relevant", toolStripItem.Text);
            Assert.AreEqual("Geeft aan of dit toetsspoor relevant is of niet.", toolStripItem.ToolTipText);
            Bitmap checkboxIcon = isRelevant ? RiskeerFormsResources.Checkbox_ticked : RiskeerFormsResources.Checkbox_empty;
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
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.NotifyObservers());
            failureMechanism.Expect(fm => fm.IsRelevant).Return(isRelevant);
            failureMechanism.Expect(fm => fm.IsRelevant).SetPropertyWithArgument(!isRelevant);
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
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
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van deze berekening.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_CalculationWithoutOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Deze berekening heeft geen uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateClearCalculationOutputItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var mocks = new MockRepository();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Stub(c => c.HasOutput).Return(true);
            calculationWithOutput.Expect(c => c.ClearOutput());
            calculationWithOutput.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

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
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Stub(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Call
            toolStripItem.PerformClick();

            mocks.VerifyAll();
        }

        #region CreateDuplicateCalculationItem

        private static IEnumerable<TestCaseData> CalculationGroupConfigurations
        {
            get
            {
                var calculationItem = new TestCalculationItem
                {
                    Name = "Element"
                };

                yield return new TestCaseData(calculationItem,
                                              new CalculationGroup
                                              {
                                                  Children =
                                                  {
                                                      calculationItem
                                                  }
                                              },
                                              "Kopie van Element")
                    .SetName("NameOfDefaultCopyUnique");
                yield return new TestCaseData(calculationItem,
                                              new CalculationGroup
                                              {
                                                  Children =
                                                  {
                                                      calculationItem,
                                                      new TestCalculationItem
                                                      {
                                                          Name = "Kopie van Element"
                                                      }
                                                  }
                                              },
                                              "Kopie van Element (1)")
                    .SetName("NameOfDefaultCopySameAsOtherCalculationItem");
                yield return new TestCaseData(calculationItem,
                                              new CalculationGroup
                                              {
                                                  Children =
                                                  {
                                                      new CalculationGroup
                                                      {
                                                          Name = "Kopie van Element"
                                                      },
                                                      calculationItem
                                                  }
                                              },
                                              "Kopie van Element (1)")
                    .SetName("NameOfDefaultCopySameAsOtherCalculationGroup");
            }
        }

        [Test]
        public void CreateDuplicateCalculationItem_CalculationItemWithParent_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItem = mocks.Stub<ICalculationBase>();
            var calculationItemContext = mocks.Stub<ICalculationContext<ICalculationBase, IFailureMechanism>>();
            calculationItemContext.Stub(ic => ic.Parent).Return(new CalculationGroup());
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            // Assert
            Assert.AreEqual("D&upliceren", toolStripItem.Text);
            Assert.AreEqual("Dupliceer dit element.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CopyHS, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateDuplicateCalculationItem_CalculationItemWithoutParent_ThrowsArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItem = mocks.Stub<ICalculationBase>();
            var calculationItemContext = mocks.Stub<ICalculationContext<ICalculationBase, IFailureMechanism>>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => RingtoetsContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual($"{nameof(calculationItemContext.Parent)} should be set.", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(CalculationGroupConfigurations))]
        public void CreateDuplicateCalculationItem_PerformClickOnCreatedItem_DuplicatesCalculationItemWithExpectedNameAndPosition(ICalculationBase calculationItem,
                                                                                                                                  CalculationGroup calculationGroup,
                                                                                                                                  string expectedCalculationItemName)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationItemContext = mocks.Stub<ICalculationContext<ICalculationBase, IFailureMechanism>>();

            calculationItemContext.Stub(c => c.Parent).Return(calculationGroup);

            mocks.ReplayAll();

            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            List<ICalculationBase> originalChildren = calculationGroup.Children.ToList();

            // Call
            toolStripItem.PerformClick();

            // Assert
            ICalculationBase duplicatedItem = calculationGroup.Children.Except(originalChildren).SingleOrDefault();
            Assert.IsNotNull(duplicatedItem);
            Assert.AreEqual(expectedCalculationItemName, duplicatedItem.Name);
            Assert.AreEqual(originalChildren.IndexOf(calculationItem) + 1, calculationGroup.Children.IndexOf(duplicatedItem));

            mocks.VerifyAll();
        }

        #endregion

        #region CreateUpdateForeshoreProfileOfCalculationItem

        [Test]
        public void CreateUpdateForeshoreProfileOfCalculationItem_NoForeshoreProfile_CreatesExpectedItem()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(ci => ci.ForeshoreProfile).Return(null);

            calculation.Stub(c => c.InputParameters).Return(input);
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
                calculation,
                inquiryHelper, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofiel...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.UpdateItemIcon, toolStripItem.Image);

            Assert.AreEqual("Er moet een voorlandprofiel geselecteerd zijn.", toolStripItem.ToolTipText);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateUpdateForeshoreProfileOfCalculationItem_ForeshoreProfileIsSynchronizedStates_CreatesExpectedItem(bool isSynchronized)
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();

            input.Expect(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(isSynchronized);

            calculation.Expect(c => c.InputParameters).Return(input).Repeat.Any();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
                calculation,
                inquiryHelper, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofiel...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.UpdateItemIcon, toolStripItem.Image);

            if (isSynchronized)
            {
                Assert.AreEqual("Er zijn geen wijzigingen om bij te werken.", toolStripItem.ToolTipText);
                Assert.IsFalse(toolStripItem.Enabled);
            }
            else
            {
                Assert.AreEqual("Berekening bijwerken met het voorlandprofiel.", toolStripItem.ToolTipText);
                Assert.IsTrue(toolStripItem.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CreateUpdateForeshoreProfileOfCalculationItem_WithForeshoreProfileAndVariousOutputPerformClick_ExpectedAction(
            [Values(true, false)] bool hasOutput,
            [Values(true, false)] bool continuation)
        {
            // Setup
            string inquireContinuationMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                                $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileInputSynchronized).Return(false);
            calculation.Expect(c => c.InputParameters).Return(input).Repeat.Any();
            calculation.Expect(c => c.HasOutput).Return(hasOutput);
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            if (hasOutput)
            {
                inquiryHelper.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(continuation);
            }

            mocks.ReplayAll();

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
                calculation,
                inquiryHelper,
                c =>
                {
                    actionCalculation = c;
                    actionPerformed = true;
                });

            // Call
            toolStripItem.PerformClick();

            // Assert
            if (hasOutput && !continuation)
            {
                Assert.IsFalse(actionPerformed);
                Assert.IsNull(actionCalculation);
            }
            else
            {
                Assert.IsTrue(actionPerformed);
                Assert.AreSame(calculation, actionCalculation);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region CreateUpdateForeshoreProfileOfCalculationsItem

        [Test]
        [Combinatorial]
        public void CreateUpdateForeshoreProfileOfCalculationsItem_ForeshoreProfileStates_CreatesExpectedItem(
            [Values(true, false)] bool hasForeshoreProfile,
            [Values(true, false)] bool isSynchronized)
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            if (hasForeshoreProfile)
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
                input.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(isSynchronized);
            }
            else
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(null);
            }

            calculation.Stub(c => c.InputParameters).Return(input);

            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationsItem(
                new[]
                {
                    calculation
                },
                inquiryHelper, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofielen...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.UpdateItemIcon, toolStripItem.Image);

            if (hasForeshoreProfile && !isSynchronized)
            {
                Assert.AreEqual("Alle berekeningen met een voorlandprofiel bijwerken.", toolStripItem.ToolTipText);
                Assert.IsTrue(toolStripItem.Enabled);
            }
            else
            {
                Assert.AreEqual("Er zijn geen berekeningen om bij te werken.", toolStripItem.ToolTipText);
                Assert.IsFalse(toolStripItem.Enabled);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CreateUpdateForeshoreProfileOfCalculationsItem_WithVariousOutputPerformClick_ExpectedAction(
            [Values(true, false)] bool hasOutput,
            [Values(true, false)] bool continuation)
        {
            // Setup
            string inquireContinuationMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van alle bij te werken berekeningen " +
                                                $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            input.Expect(i => i.ForeshoreProfile).Return(new TestForeshoreProfile());
            input.Expect(i => i.IsForeshoreProfileInputSynchronized).Return(false);
            calculation.Stub(c => c.InputParameters).Return(input);
            calculation.Expect(c => c.HasOutput).Return(hasOutput);

            var calculationWithoutChanges = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChanges = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            inputWithoutChanges.Stub(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
            inputWithoutChanges.Stub(ci => ci.IsForeshoreProfileInputSynchronized).Return(true);
            calculationWithoutChanges.Stub(c => c.InputParameters).Return(inputWithoutChanges);

            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            if (hasOutput)
            {
                inquiryHelper.Expect(i => i.InquireContinuation(inquireContinuationMessage)).Return(continuation);
            }

            mocks.ReplayAll();

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationsItem(
                new[]
                {
                    calculation,
                    calculationWithoutChanges
                },
                inquiryHelper,
                c =>
                {
                    actionCalculation = c;
                    actionPerformed = true;
                });

            // Call
            toolStripItem.PerformClick();

            // Assert
            if (hasOutput && !continuation)
            {
                Assert.IsFalse(actionPerformed);
                Assert.IsNull(actionCalculation);
            }
            else
            {
                Assert.IsTrue(actionPerformed);
                Assert.AreSame(calculation, actionCalculation);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region CreatePerformCalculationItem

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => null);

            // Assert
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer deze berekening uit.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformCalculationItem(calculation, calculationContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

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
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => null);

            // Assert
            Assert.AreEqual("&Valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer de invoer voor deze berekening.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => errorMessage);

            // Assert
            Assert.AreEqual("&Valideren", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

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

            var parent = new CalculationGroup();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer alle berekeningen binnen deze map met berekeningen uit.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var nestedGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    nestedGroup
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
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

            var parent = new CalculationGroup();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
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
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

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

            var parent = new CalculationGroup();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer alle berekeningen binnen deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var nestedGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    nestedGroup
                }
            };
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
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

            var parent = new CalculationGroup();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
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
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer alle berekeningen binnen dit toetsspoor uit.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.StrictMock<ICalculation>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var counter = 0;
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer alle berekeningen binnen dit toetsspoor.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestCalculation();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RingtoetsContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var calculation = mocks.StrictMock<ICalculation>();
            mocks.ReplayAll();

            var counter = 0;
            var failureMechanism = new TestFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

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
            public TestCalculationGroupContext(CalculationGroup wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationItem : Observable, ICalculationBase
        {
            public string Name { get; set; }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}

        #endregion
    }
}