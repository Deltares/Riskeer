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

using System;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    public class WaveHeightContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(WaveHeightContext), info.TagType);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
            }
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(null);

                // Assert
                const string expectedName = "Golfhoogtes";
                Assert.AreEqual(expectedName, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(Resources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new WaveHeightContext(assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }
            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemCalculateDisabled()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new WaveHeightContext(assessmentSectionMock);

            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                    // Assert
                    const string expectedItemText = "&Berekenen";
                    const string expectedItemTooltip = "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de golfhoogtes te berekenen.";
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, Resources.FailureMechanismIcon, false);
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemCalculateEnabled()
        {
            // Setup
            var guiMock = mockRepository.StrictMock<IGui>();
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var nodeData = new WaveHeightContext(assessmentSectionMock)
            {
                WrappedData =
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                }
            };
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                    // Assert
                    const string expectedItemText = "&Berekenen";
                    const string expectedItemTooltip = "Bereken de golfhoogtes.";

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, Resources.FailureMechanismIcon);
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ForeColor_ContextHasNoHydraulicBoundaryDatabase_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var waterLevelContext = new WaveHeightContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(waterLevelContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoCalculations_ReturnControlColor()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mockRepository.ReplayAll();

            var waterLevelContext = new WaveHeightContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(waterLevelContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasCalculations_ReturnControlColor()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var location = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2)
            {
                WaveHeight = 1.0
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(location);
            mockRepository.ReplayAll();

            var waterLevelContext = new WaveHeightContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(waterLevelContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var gui = mockRepository.DynamicMock<IGui>();

            var contextMenuRunAssessmentLevelCalculationsIndex = 0;

            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(100001, "", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(100002, "", 3.3, 4.4)
            {
                WaveHeight = 4.2
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                },
                FilePath = "D:/nonExistingDirectory/nonExistingFile",
                Version = "random"
            };

            var assessmentSectionMock = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };
            var waveHeightContext = new WaveHeightContext(assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Expect(cmp => cmp.Get(waveHeightContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mockRepository.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
                    plugin.Gui = gui;

                    var contextMenuAdapter = info.ContextMenuStrip(waveHeightContext, null, treeViewControl);

                    // When
                    Action action = () => { contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick(); };

                    // Then
                    string message = string.Format("Berekeningen konden niet worden gestart. Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                   hydraulicBoundaryDatabase.FilePath);
                    TestHelper.AssertLogMessageWithLevelIsGenerated(action, new Tuple<string, LogLevelConstant>(message, LogLevelConstant.Error));

                    Assert.IsNaN(hydraulicBoundaryLocation1.WaveHeight); // No result set
                    Assert.AreEqual(4.2, hydraulicBoundaryLocation2.WaveHeight); // Previous result not cleared
                }
            }
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaveHeightContext));
        }
    }
}