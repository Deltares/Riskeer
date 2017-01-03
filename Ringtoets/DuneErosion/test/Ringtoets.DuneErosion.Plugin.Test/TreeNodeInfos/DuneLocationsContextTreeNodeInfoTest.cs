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

using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 2;

        private MockRepository mocks;
        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            var text = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische randvoorwaarden", text);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void ForeColor_NoLocations_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            Color textColor = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
        }

        [Test]
        public void ForeColor_WithLocations_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var mechanism = new DuneErosionFailureMechanism();
            mechanism.DuneLocations.Add(new TestDuneLocation());
            var context = new DuneLocationsContext(mechanism.DuneLocations, mechanism, assessmentSection);

            // Call
            Color textColor = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var failureMechanism = new DuneErosionFailureMechanism();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSectionMock);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();

                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Done in tearDown
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ContextMenuStrip_Always_AddCustomItem(bool hasDuneLocations)
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
                var failureMechanism = new DuneErosionFailureMechanism();
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSectionMock);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                if (hasDuneLocations)
                {
                    failureMechanism.DuneLocations.Add(new TestDuneLocation());
                }

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(5, menu.Items.Count);

                    var expectedMessage = hasDuneLocations
                                              ? "Voer alle berekeningen binnen dit toetsspoor uit."
                                              : "Er zijn geen locaties om een berekening voor uit te voeren.";

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  expectedMessage,
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  hasDuneLocations);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllLocationsAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    Contribution = 10
                };
                failureMechanism.DuneLocations.AddRange(new[]
                                                        {
                                                            new DuneLocation(1300001, "A", new Point2D(0, 0), 3, 0, 0, 0.000007),
                                                            new DuneLocation(1300002, "B", new Point2D(0, 0), 3, 0, 0, 0.000007)
                                                        });

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath
                };

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
                assessmentSection.Stub(a => a.Id).Return("13-1");
                assessmentSection.Stub(a => a.GetFailureMechanisms()).Return(new[]
                                                                             {
                                                                                 failureMechanism
                                                                             });
                assessmentSection.Stub(a => a.FailureMechanismContribution)
                                 .Return(new FailureMechanismContribution(new[]
                                                                          {
                                                                              failureMechanism
                                                                          }, 1, 1.0/200));
                var context = new DuneLocationsContext(failureMechanism.DuneLocations, failureMechanism, assessmentSection);

                var builder = new CustomItemsOnlyContextMenuBuilder();

                var mainWindowStub = mocks.Stub<IMainWindow>();
                var gui = mocks.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(builder);
                gui.Expect(g => g.MainWindow).Return(mainWindowStub);
                var observerMock = mocks.StrictMock<IObserver>();
                observerMock.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                failureMechanism.DuneLocations.Attach(observerMock);

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig())
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
                                                 {
                                                     var messageList = messages.ToList();

                                                     // Assert
                                                     Assert.AreEqual(10, messageList.Count);
                                                     StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[0]);
                                                     Assert.AreEqual("Duinafslag berekening voor locatie 'A' is niet geconvergeerd.", messageList[1]);
                                                     StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", messageList[2]);
                                                     StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[3]);
                                                     StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[4]);
                                                     Assert.AreEqual("Duinafslag berekening voor locatie 'B' is niet geconvergeerd.", messageList[5]);
                                                     StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", messageList[6]);
                                                     StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[7]);
                                                     Assert.AreEqual("Uitvoeren van 'A' is gelukt.", messageList[8]);
                                                     Assert.AreEqual("Uitvoeren van 'B' is gelukt.", messageList[9]);
                                                 });
                }
            }
        }
    }
}