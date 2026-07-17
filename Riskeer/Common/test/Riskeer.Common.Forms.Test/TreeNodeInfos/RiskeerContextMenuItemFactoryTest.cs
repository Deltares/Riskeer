// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Gui.ContextMenu;
using Core.Gui.Helpers;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TreeNodeInfos;
using RiskeerFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RiskeerContextMenuItemFactoryTest : NUnitFormTest
    {
        [Test]
        public void CreateAddCalculationGroupItem_Always_CreatesDecoratedItem()
        {
            // Setup
            var calculationGroup = new CalculationGroup();

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

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

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateAddCalculationGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(2, calculationGroup.Children.Count);

            var newGroup = calculationGroup.Children[1] as CalculationGroup;
            Assert.IsNotNull(newGroup);
            Assert.AreEqual("Nieuwe map (1)", newGroup.Name);
        }

        [Test]
        [TestCaseSource(typeof(CalculationTypeTestHelper), nameof(CalculationTypeTestHelper.CalculationTypeWithImageCases))]
        public void CreateAddCalculationItem_Always_CreatesDecoratedItem(CalculationType calculationType, Bitmap expectedImage)
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => {}, calculationType);

            // Assert
            Assert.AreEqual("Berekening &toevoegen", toolStripItem.Text);
            Assert.AreEqual("Voeg een nieuwe berekening toe aan deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(expectedImage, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateAddCalculationItem_PerformClickOnCreatedItem_AddCalculationMethodPerformed()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var calculationType = new Random(21).NextEnumValue<CalculationType>();

            var counter = 0;
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateAddCalculationItem(calculationGroupContext, context => counter++, calculationType);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_GroupWithCalculationOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput
                }
            };

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_GroupWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithoutOutput.HasOutput.Returns(false);
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithoutOutput
                }
            };

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var calculationWithOutputMock1 = Substitute.For<ICalculation>();
            var calculationWithOutputMock2 = Substitute.For<ICalculation>();
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithOutputMock1.HasOutput.Returns(true);
            calculationWithOutputMock2.HasOutput.Returns(true);
            calculationWithoutOutput.HasOutput.Returns(false);

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

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
            calculationWithOutputMock1.Received().ClearOutput();
            calculationWithOutputMock1.Received().NotifyObservers();
            calculationWithOutputMock2.Received().ClearOutput();
            calculationWithOutputMock2.Received().NotifyObservers();
        }

        [Test]
        public void CreateClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var calculationWithOutputMock1 = Substitute.For<ICalculation>();
            var calculationWithOutputMock2 = Substitute.For<ICalculation>();
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithOutputMock1.HasOutput.Returns(true);
            calculationWithOutputMock2.HasOutput.Returns(true);
            calculationWithoutOutput.HasOutput.Returns(false);
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

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInGroupItem(calculationGroup);

            // Call
            toolStripItem.PerformClick();

            // Assert
            calculationWithoutOutput.DidNotReceive().ClearOutput();
            calculationWithOutputMock1.DidNotReceive().ClearOutput();
            calculationWithOutputMock2.DidNotReceive().ClearOutput();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_FailureMechanismWithCalculationOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var calculationWithOutput = Substitute.For<ICalculation>();
            calculationWithOutput.HasOutput.Returns(true);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new[]
            {
                calculationWithOutput
            });
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van alle berekeningen binnen dit faalmechanisme.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_FailureMechanismWithoutCalculationOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var calculationWithoutOutput = Substitute.For<ICalculation>();
            calculationWithoutOutput.HasOutput.Returns(false);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new[]
            {
                calculationWithoutOutput
            });
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Assert
            Assert.AreEqual("&Wis alle uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            // Setup
            var calculationWithOutputMock1 = Substitute.For<ICalculation>();
            var calculationWithOutputMock2 = Substitute.For<ICalculation>();
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithOutputMock1.HasOutput.Returns(true);
            calculationWithOutputMock2.HasOutput.Returns(true);
            calculationWithoutOutput.HasOutput.Returns(false);

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutput
            });
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickOk();
            };

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Call
            toolStripItem.PerformClick();

            // Assert            
            calculationWithOutputMock1.Received().ClearOutput();
            calculationWithOutputMock1.Received().NotifyObservers();
            calculationWithOutputMock2.Received().ClearOutput();
            calculationWithOutputMock2.Received().NotifyObservers();
        }

        [Test]
        public void CreateClearAllCalculationOutputInFailureMechanismItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            // Setup
            var calculationWithOutputMock1 = Substitute.For<ICalculation>();
            var calculationWithOutputMock2 = Substitute.For<ICalculation>();
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithOutputMock1.HasOutput.Returns(true);
            calculationWithOutputMock2.HasOutput.Returns(true);
            calculationWithoutOutput.HasOutput.Returns(false);

            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new[]
            {
                calculationWithOutputMock1,
                calculationWithOutputMock2,
                calculationWithoutOutput
            });
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearAllCalculationOutputInFailureMechanismItem(failureMechanism);

            // Call
            toolStripItem.PerformClick();

            // Assert
            calculationWithOutputMock1.DidNotReceive().ClearOutput();
            calculationWithOutputMock2.DidNotReceive().ClearOutput();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateToggleInAssemblyOfFailureMechanismItem_InAssembly_CreateDecoratedItem(bool inAssembly)
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            failureMechanism.InAssembly.Returns(inAssembly);
            var failureMechanismContext = Substitute.For<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContext.WrappedData.Returns(failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateToggleInAssemblyOfFailureMechanismItem(failureMechanismContext, null);

            // Assert
            Assert.AreEqual("In &assemblage", toolStripItem.Text);
            Assert.AreEqual("Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.", toolStripItem.ToolTipText);
            Bitmap checkboxIcon = inAssembly ? RiskeerFormsResources.Checkbox_ticked : RiskeerFormsResources.Checkbox_empty;
            TestHelper.AssertImagesAreEqual(checkboxIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateToggleInAssemblyOfFailureMechanismItem_PerformClickOnInAssemblyItem_RelevanceChangedAndObserversNotified(bool inAssembly)
        {
            // Setup
            var failureMechanism = Substitute.For<IFailureMechanism>();
            failureMechanism.InAssembly.Returns(inAssembly);

            var failureMechanismContext = Substitute.For<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContext.WrappedData.Returns(failureMechanism);

            var actionCounter = 0;
            StrictContextMenuItem toolStripItem =
                RiskeerContextMenuItemFactory.CreateToggleInAssemblyOfFailureMechanismItem(
                    failureMechanismContext,
                    context => actionCounter++);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, actionCounter);

            failureMechanism.Received().InAssembly = !inAssembly;
            failureMechanism.Received().NotifyObservers();
        }

        [Test]
        public void CreateClearCalculationOutputItem_CalculationWithOutput_CreatesDecoratedAndEnabledItem()
        {
            // Setup
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Wis de uitvoer van deze berekening.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearCalculationOutputItem_CalculationWithoutOutput_CreatesDecoratedAndDisabledItem()
        {
            // Setup
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(false);
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Assert
            Assert.AreEqual("&Wis uitvoer...", toolStripItem.Text);
            Assert.AreEqual("Deze berekening heeft geen uitvoer om te wissen.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateClearCalculationOutputItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
            calculationWithOutput.Received().ClearOutput();
            calculationWithOutput.Received().NotifyObservers();
        }

        [Test]
        public void CreateClearCalculationOutputItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearCalculationOutputItem(calculationWithOutput);

            // Call
            toolStripItem.PerformClick();
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
            var calculationItem = Substitute.For<ICalculationBase>();
            var calculationItemContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();
            calculationItemContext.Parent.Returns(new CalculationGroup());
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            // Assert
            Assert.AreEqual("D&upliceren", toolStripItem.Text);
            Assert.AreEqual("Dupliceer dit element.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CopyHS, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateDuplicateCalculationItem_CalculationItemWithoutParent_ThrowsArgumentException()
        {
            // Setup
            var calculationItem = Substitute.For<ICalculationBase>();
            var calculationItemContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();

            // Call
            void Call() => RiskeerContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            // Assert
            var exception = Assert.Throws<ArgumentException>(Call);
            Assert.AreEqual($"{nameof(calculationItemContext.Parent)} should be set.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(CalculationGroupConfigurations))]
        public void CreateDuplicateCalculationItem_PerformClickOnCreatedItem_DuplicatesCalculationItemWithExpectedNameAndPosition(ICalculationBase calculationItem,
                                                                                                                                  CalculationGroup calculationGroup,
                                                                                                                                  string expectedCalculationItemName)
        {
            // Setup
            var calculationItemContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();

            calculationItemContext.Parent.Returns(calculationGroup);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateDuplicateCalculationItem(calculationItem, calculationItemContext);

            List<ICalculationBase> originalChildren = calculationGroup.Children.ToList();

            // Call
            toolStripItem.PerformClick();

            // Assert
            ICalculationBase duplicatedItem = calculationGroup.Children.Except(originalChildren).SingleOrDefault();
            Assert.IsNotNull(duplicatedItem);
            Assert.AreEqual(expectedCalculationItemName, duplicatedItem.Name);
            Assert.AreEqual(originalChildren.IndexOf(calculationItem) + 1, calculationGroup.Children.IndexOf(duplicatedItem));
        }

        #endregion

        #region CreateUpdateForeshoreProfileOfCalculationItem

        [Test]
        public void CreateUpdateForeshoreProfileOfCalculationItem_NoForeshoreProfile_CreatesExpectedItem()
        {
            // Setup
            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();
            input.ForeshoreProfile.Returns((ForeshoreProfile) null);

            calculation.InputParameters.Returns(input);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
                calculation,
                inquiryHelper, c => {});

            // Assert
            Assert.AreEqual("&Bijwerken voorlandprofiel...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.UpdateItemIcon, toolStripItem.Image);

            Assert.AreEqual("Er moet een voorlandprofiel geselecteerd zijn.", toolStripItem.ToolTipText);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateUpdateForeshoreProfileOfCalculationItem_ForeshoreProfileIsSynchronizedStates_CreatesExpectedItem(bool isSynchronized)
        {
            // Setup
            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();

            input.ForeshoreProfile.Returns(new TestForeshoreProfile());
            input.IsForeshoreProfileInputSynchronized.Returns(isSynchronized);

            calculation.InputParameters.Returns(input);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
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
            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();
            input.ForeshoreProfile.Returns(new TestForeshoreProfile());
            input.IsForeshoreProfileInputSynchronized.Returns(false);
            calculation.InputParameters.Returns(input);
            calculation.HasOutput.Returns(hasOutput);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            if (hasOutput)
            {
                inquiryHelper.InquireContinuation(inquireContinuationMessage).Returns(continuation);
            }

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationItem(
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
            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();
            if (hasForeshoreProfile)
            {
                input.ForeshoreProfile.Returns(new TestForeshoreProfile());
                input.IsForeshoreProfileInputSynchronized.Returns(isSynchronized);
            }
            else
            {
                input.ForeshoreProfile.Returns((ForeshoreProfile) null);
            }

            calculation.InputParameters.Returns(input);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationsItem(
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
            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();
            input.ForeshoreProfile.Returns(new TestForeshoreProfile());
            input.IsForeshoreProfileInputSynchronized.Returns(false);
            calculation.InputParameters.Returns(input);
            calculation.HasOutput.Returns(hasOutput);

            var calculationWithoutChanges = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var inputWithoutChanges = Substitute.For<ICalculationInputWithForeshoreProfile>();
            inputWithoutChanges.ForeshoreProfile.Returns(new TestForeshoreProfile());
            inputWithoutChanges.IsForeshoreProfileInputSynchronized.Returns(true);
            calculationWithoutChanges.InputParameters.Returns(inputWithoutChanges);

            var inquiryHelper = Substitute.For<IInquiryHelper>();
            if (hasOutput)
            {
                inquiryHelper.InquireContinuation(inquireContinuationMessage).Returns(continuation);
            }

            ICalculation<ICalculationInputWithForeshoreProfile> actionCalculation = null;
            var actionPerformed = false;
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateUpdateForeshoreProfileOfCalculationsItem(
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
        }

        #endregion

        #region CreatePerformCalculationItem

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformCalculationItem<TestCalculation, TestCalculationContext>(
                calculationContext, null, context => null);

            // Assert
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer deze berekening uit.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformCalculationItem<TestCalculation, TestCalculationContext>(
                calculationContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Be&rekenen", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            var counter = 0;
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformCalculationItem<TestCalculation, TestCalculationContext>(
                calculationContext, context => counter++, context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        #endregion

        #region CreateValidateCalculationItem

        [Test]
        public void CreateValidateCalculationItem_AdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => null);

            // Assert
            Assert.AreEqual("&Valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer de invoer voor deze berekening.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateCalculationItem_AdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, null, c => errorMessage);

            // Assert
            Assert.AreEqual("&Valideren", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            var counter = 0;
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateCalculationItem(calculationContext, calc => counter++, c => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

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
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

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
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

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
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
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

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInGroupItem(calculationGroupContext, context => counter++, context => null);

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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

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
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

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
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null);

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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

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
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage);

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
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
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

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInGroupItem(
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
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = new TestCalculation();
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Voer alle berekeningen binnen dit faalmechanisme uit.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());

            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = new TestCalculation();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage);

            // Assert
            Assert.AreEqual("Alles be&rekenen", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om uit te voeren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.CalculateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreatePerformAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var calculation = Substitute.For<ICalculation>();
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var counter = 0;
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreatePerformAllCalculationsInFailureMechanismItem(failureMechanismContext, fmContext => counter++, context => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        #endregion

        #region CreateValidateAllCalculationsInFailureMechanismItem

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_CreatesEnabledItem()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = new TestCalculation();
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Valideer alle berekeningen binnen dit faalmechanisme.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsTrue(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_CreatesDisabledItemAndSetMessageInTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = new TestCalculation();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual(errorMessage, toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_CreatesDisabledItemAndSetGeneralValidationMessageTooltip()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            const string errorMessage = "Additional check failed.";

            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                null,
                fm => errorMessage);

            // Assert
            Assert.AreEqual("Alles &valideren", toolStripItem.Text);
            Assert.AreEqual("Er zijn geen berekeningen om te valideren.", toolStripItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ValidateAllIcon, toolStripItem.Image);
            Assert.IsFalse(toolStripItem.Enabled);
        }

        [Test]
        public void CreateValidateAllCalculationsInFailureMechanismItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var calculation = Substitute.For<ICalculation>();
            var counter = 0;
            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateValidateAllCalculationsInFailureMechanismItem(
                failureMechanismContext,
                fm => counter++,
                fm => null);

            // Call
            toolStripItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        #endregion

        #region CreateClearIllustrationPointsOfCalculationsItem

        [Test]
        public void CreateClearIllustrationPointsOfCalculationsItem_Always_CreatesExpectedItem()
        {
            // Setup
            bool isEnabled = new Random(21).NextBoolean();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsItem(() => isEnabled,
                                                                                                                                handler);
            // Assert
            Assert.AreEqual("Wis alle &illustratiepunten...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIllustrationPointsIcon, toolStripItem.Image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateClearIllustrationPointsOfCalculationsItem_EnabledSituation_ReturnsExpectedEnabledStateAndToolTipMessage(bool isEnabled)
        {
            // Setup
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsItem(() => isEnabled,
                                                                                                                                handler);

            // Assert
            Assert.AreEqual(isEnabled, toolStripItem.Enabled);

            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            Assert.AreEqual(expectedToolTipMessage, toolStripItem.ToolTipText);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsItem_WhenClickPerformedAndActionCancelled_ThenNothingHappens()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(false);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsItem(() => true,
                                                                                                                                handler);

            // When
            toolStripItem.PerformClick();

            // Then
            Assert.AreEqual(handler.ReceivedCalls().Count(), 1);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsItem_WhenClickPerformedAndActionContinued_ThenIllustrationPointsClearedAndObserversUpdated()
        {
            // Given
            var observable = Substitute.For<IObservable>();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.ClearIllustrationPoints().Returns(new[]
            {
                observable
            });
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsItem(() => true,
                                                                                                                                handler);

            // When
            toolStripItem.PerformClick();

            // Then
            observable.Received().NotifyObservers();
            handler.Received().ClearIllustrationPoints();
        }

        #endregion

        #region CreateClearIllustrationPointsOfCalculationsInGroupItem

        [Test]
        public void CreateClearIllustrationPointsOfCalculationsInGroupItem_Always_CreatesExpectedItem()
        {
            // Setup
            bool isEnabled = new Random(21).NextBoolean();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInGroupItem(() => isEnabled,
                                                                                                                                       handler);
            // Assert
            Assert.AreEqual("Wis alle &illustratiepunten...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIllustrationPointsIcon, toolStripItem.Image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateClearIllustrationPointsOfCalculationsInGroupItem_EnabledSituation_ReturnsExpectedEnabledStateAndToolTipMessage(bool isEnabled)
        {
            // Setup
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInGroupItem(() => isEnabled,
                                                                                                                                       handler);

            // Assert
            Assert.AreEqual(isEnabled, toolStripItem.Enabled);

            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten binnen deze map met berekeningen."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            Assert.AreEqual(expectedToolTipMessage, toolStripItem.ToolTipText);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsInGroupItem_WhenClickPerformedAndActionCancelled_ThenNothingHappens()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(false);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInGroupItem(() => true,
                                                                                                                                       handler);

            // When
            toolStripItem.PerformClick();

            // Then
            Assert.AreEqual(handler.ReceivedCalls().Count(), 1);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsInGroupItem_WhenClickPerformedAndActionContinued_ThenIllustrationPointsClearedAndObserversUpdated()
        {
            // Given
            var observable = Substitute.For<IObservable>();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.ClearIllustrationPoints().Returns(new[]
            {
                observable
            });
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInGroupItem(() => true,
                                                                                                                                       handler);

            // When
            toolStripItem.PerformClick();

            // Then
            handler.Received().ClearIllustrationPoints();
            observable.Received().NotifyObservers();
        }

        #endregion

        #region CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem

        [Test]
        public void CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem_Always_CreatesExpectedItem()
        {
            // Setup
            bool isEnabled = new Random(21).NextBoolean();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                () => isEnabled, handler);

            // Assert
            Assert.AreEqual("Wis alle &illustratiepunten...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIllustrationPointsIcon, toolStripItem.Image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem_EnabledSituation_ReturnsExpectedEnabledStateAndToolTipMessage(bool isEnabled)
        {
            // Setup
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                () => isEnabled, handler);

            // Assert
            Assert.AreEqual(isEnabled, toolStripItem.Enabled);

            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten binnen dit faalmechanisme."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            Assert.AreEqual(expectedToolTipMessage, toolStripItem.ToolTipText);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsInFailureMechanismItem_WhenClickPerformedAndActionCancelled_ThenNothingHappens()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(false);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                () => true, handler);

            // When
            toolStripItem.PerformClick();

            // Then
            Assert.AreEqual(handler.ReceivedCalls().Count(), 1);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationsInFailureMechanismItem_WhenClickPerformedAndActionContinued_ThenIllustrationPointsClearedAndObserversUpdated()
        {
            // Given
            var observable = Substitute.For<IObservable>();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.ClearIllustrationPoints().Returns(new[]
            {
                observable
            });
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                () => true, handler);

            // When
            toolStripItem.PerformClick();

            // Then
            observable.NotifyObservers();
            handler.Received().ClearIllustrationPoints();
        }

        #endregion

        #region CreateClearIllustrationPointsOfCalculationItem

        [Test]
        public void CreateClearIllustrationPointsOfCalculationItem_Always_CreatesExpectedItem()
        {
            // Setup
            bool isEnabled = new Random(21).NextBoolean();
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationItem(() => isEnabled,
                                                                                                                               handler);
            // Assert
            Assert.AreEqual("Wis illustratiepunten...", toolStripItem.Text);
            TestHelper.AssertImagesAreEqual(RiskeerFormsResources.ClearIllustrationPointsIcon, toolStripItem.Image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateClearIllustrationPointsOfCalculationItem_EnabledSituation_ReturnsExpectedEnabledStateAndToolTipMessage(bool isEnabled)
        {
            // Setup
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            // Call
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationItem(() => isEnabled,
                                                                                                                               handler);

            // Assert
            Assert.AreEqual(isEnabled, toolStripItem.Enabled);

            string expectedToolTipMessage = isEnabled
                                                ? "Wis de berekende illustratiepunten van deze berekening."
                                                : "Deze berekening heeft geen illustratiepunten om te wissen.";
            Assert.AreEqual(expectedToolTipMessage, toolStripItem.ToolTipText);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationItem_WhenClickPerformedAndActionCancelled_ThenNothingHappens()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            handler.InquireConfirmation().Returns(false);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationItem(() => true,
                                                                                                                               handler);
            // When
            toolStripItem.PerformClick();

            // Then
            Assert.AreEqual(handler.ReceivedCalls().Count(), 1);
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationItem_WhenClickPerformedAndActionContinuedAndCalculationAffected_ThenIllustrationPointsClearedAndPostUpdates()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.ClearIllustrationPoints().Returns(true);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationItem(() => true,
                                                                                                                               handler);

            // When
            toolStripItem.PerformClick();

            // Then
            handler.Received().ClearIllustrationPoints();
            handler.Received().DoPostUpdateActions();
        }

        [Test]
        public void GivenEnabledCreateClearIllustrationPointsOfCalculationItem_WhenClickPerformedAndActionContinuedAndCalculationUnaffected_ThenIllustrationPointsClearedAndNoPostUpdates()
        {
            // Given
            var handler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            handler.InquireConfirmation().Returns(true);
            handler.ClearIllustrationPoints().Returns(false);
            StrictContextMenuItem toolStripItem = RiskeerContextMenuItemFactory.CreateClearIllustrationPointsOfCalculationItem(() => true,
                                                                                                                               handler);

            // When
            toolStripItem.PerformClick();

            // Then
            handler.Received().ClearIllustrationPoints();
            handler.DidNotReceive().DoPostUpdateActions();
        }

        #endregion

        #region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<ICalculatableFailureMechanism>
        {
            public TestFailureMechanismContext(ICalculatableFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, CalculationGroup parent, ICalculatableFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public ICalculatableFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, ICalculatableFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, CalculationGroup parent, ICalculatableFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public ICalculatableFailureMechanism FailureMechanism { get; }
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