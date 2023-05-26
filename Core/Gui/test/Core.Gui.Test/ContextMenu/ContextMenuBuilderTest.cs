﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Plugin;
using Core.Gui.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuBuilderTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_NoApplicationFeatureCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(null,
                                                      importCommandHandler,
                                                      exportCommandHandler,
                                                      updateCommandHandler,
                                                      viewCommands,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoImportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      null,
                                                      exportCommandHandler,
                                                      updateCommandHandler,
                                                      viewCommands,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoExportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      importCommandHandler,
                                                      null,
                                                      updateCommandHandler,
                                                      viewCommands,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoUpdateCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      importCommandHandler,
                                                      exportCommandHandler,
                                                      null,
                                                      viewCommands,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoViewCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      importCommandHandler,
                                                      exportCommandHandler,
                                                      updateCommandHandler,
                                                      null,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoDataObject_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      importCommandHandler,
                                                      exportCommandHandler,
                                                      updateCommandHandler,
                                                      viewCommands,
                                                      null,
                                                      treeViewControl);

                // Assert
                var exception = Assert.Throws<ContextMenuBuilderException>(Call);
                Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoTreeViewControl_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            // Call
            void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                  importCommandHandler,
                                                  exportCommandHandler,
                                                  updateCommandHandler,
                                                  viewCommands,
                                                  new object(),
                                                  null);

            // Assert
            var exception = Assert.Throws<ContextMenuBuilderException>(Call);
            Assert.AreEqual("Kan geen instanties maken van de benodigde objecten.", exception.Message);
        }

        [Test]
        public void Constructor_ParamsSet_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                void Call() => new ContextMenuBuilder(applicationFeatureCommands,
                                                      importCommandHandler,
                                                      exportCommandHandler,
                                                      updateCommandHandler,
                                                      viewCommands,
                                                      new object(),
                                                      treeViewControl);

                // Assert
                Assert.DoesNotThrow(Call);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                CollectionAssert.IsEmpty(result.Items);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var dataObject = new object();
            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
                var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
                var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
                var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
                var viewCommands = mocks.StrictMock<IViewCommands>();
                var treeNodeInfo = mocks.StrictMock<TreeNodeInfo<object>>();
                mocks.ReplayAll();

                treeNodeInfo.CanRename = (data, parentData) => data == dataObject;
                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.Data = dataObject;

                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     dataObject,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddRenameItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Hernoemen",
                                                              "Wijzig de naam van dit element.",
                                                              Resources.RenameIcon);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void AddDeleteItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            const string nodeData = "string";
            var parentData = new object();
            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
                var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
                var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
                var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
                var viewCommands = mocks.StrictMock<IViewCommands>();
                var treeNodeInfo = mocks.StrictMock<TreeNodeInfo<string>>();
                var parentTreeNodeInfo = mocks.StrictMock<TreeNodeInfo<object>>();
                mocks.ReplayAll();

                treeNodeInfo.CanRemove = (nd, pnd) => nd == nodeData && pnd == parentData;
                parentTreeNodeInfo.ChildNodeObjects = nd => new object[]
                {
                    nodeData
                };

                treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
                treeViewControl.RegisterTreeNodeInfo(parentTreeNodeInfo);

                treeViewControl.Data = parentData;

                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddDeleteItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "Verwij&deren...",
                                                              "Verwijder dit element uit de boom.",
                                                              Resources.DeleteIcon);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddRemoveAllChildrenItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            treeViewControl.Expect(tvc => tvc.CanRemoveChildNodesOfData(dataObject)).Return(hasChildren).Repeat.AtLeastOnce();

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                 importCommandHandler,
                                                 exportCommandHandler,
                                                 updateCommandHandler,
                                                 viewCommands,
                                                 dataObject,
                                                 treeViewControl);

            // Call
            ContextMenuStrip result = builder.AddDeleteChildrenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);
            string expectedTooltip = hasChildren
                                         ? "Verwijder alle onderliggende elementen van dit element."
                                         : "Er zijn geen onderliggende elementen om te verwijderen.";
            TestHelper.AssertContextMenuStripContainsItem(result,
                                                          0,
                                                          "Ma&p leegmaken...",
                                                          expectedTooltip,
                                                          Resources.DeleteChildrenIcon,
                                                          hasChildren);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExpandAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(dataObject)).Return(hasChildren);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                 importCommandHandler,
                                                 exportCommandHandler,
                                                 updateCommandHandler,
                                                 viewCommands,
                                                 dataObject,
                                                 treeViewControl);

            // Call
            ContextMenuStrip result = builder.AddExpandAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result,
                                                          0,
                                                          "Alles ui&tklappen",
                                                          "Klap dit element en alle onderliggende elementen uit.",
                                                          Resources.ExpandAllIcon,
                                                          hasChildren);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(dataObject)).Return(hasChildren);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                 importCommandHandler,
                                                 exportCommandHandler,
                                                 updateCommandHandler,
                                                 viewCommands,
                                                 dataObject,
                                                 treeViewControl);

            // Call
            ContextMenuStrip result = builder.AddCollapseAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result,
                                                          0,
                                                          "Alles i&nklappen",
                                                          "Klap dit element en alle onderliggende elementen in.",
                                                          Resources.CollapseAllIcon,
                                                          hasChildren);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddOpenItem_WhenBuild_ItemAddedToContextMenu(bool hasViewForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(hasViewForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddOpenItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Openen",
                                                              "Open de gegevens in een nieuw documentvenster.",
                                                              Resources.OpenIcon,
                                                              hasViewForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExportItem_WhenBuild_ItemAddedToContextMenu(bool hasExportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            exportCommandHandler.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddExportItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Exporteren...",
                                                              "Exporteer de gegevens naar een bestand.",
                                                              Resources.ExportIcon,
                                                              hasExportersForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithoutParameters_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            importCommandHandler.Expect(ch => ch.GetSupportedImportInfos(nodeData))
                                .Return(hasImportersForNodeData
                                            ? new[]
                                            {
                                                new ImportInfo()
                                            }
                                            : new ImportInfo[0]);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddImportItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              Resources.ImportIcon,
                                                              hasImportersForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithImportInfosParameter_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            ImportInfo[] importInfos = hasImportersForNodeData
                                           ? new[]
                                           {
                                               new ImportInfo()
                                           }
                                           : new ImportInfo[0];

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddImportItem(importInfos).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Importeren...",
                                                              "Importeer de gegevens vanuit een bestand.",
                                                              Resources.ImportIcon,
                                                              hasImportersForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithTextualParameters_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            const string text = "import";
            const string toolTip = "import tooltip";
            Image image = Resources.ImportIcon;

            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            importCommandHandler.Expect(ch => ch.GetSupportedImportInfos(nodeData))
                                .Return(hasImportersForNodeData
                                            ? new[]
                                            {
                                                new ImportInfo()
                                            }
                                            : new ImportInfo[0]);
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddImportItem(text, toolTip, image).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, text, toolTip, image, hasImportersForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithAllParameters_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            const string text = "import";
            const string toolTip = "import tooltip";
            Image image = Resources.ImportIcon;

            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            ImportInfo[] importInfos = hasImportersForNodeData
                                           ? new[]
                                           {
                                               new ImportInfo()
                                           }
                                           : new ImportInfo[0];

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddImportItem(text, toolTip, image, importInfos).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, text, toolTip, image, hasImportersForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddUpdateItem_WhenBuild_ItemAddedToContextMenu(bool hasUpdatesForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            updateCommandHandler.Expect(ch => ch.CanUpdateOn(nodeData)).Return(hasUpdatesForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddUpdateItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "&Bijwerken...",
                                                              "Werk de geïmporteerde gegevens bij met nieuwe gegevens vanuit een bestand.",
                                                              Resources.RefreshIcon,
                                                              hasUpdatesForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddPropertiesItem_WhenBuild_ItemAddedToContextMenu(bool hasPropertiesForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            applicationFeatureCommands.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertiesForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddPropertiesItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result,
                                                              0,
                                                              "Ei&genschappen",
                                                              "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                              Resources.PropertiesHS,
                                                              hasPropertiesForNodeData);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler, exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);
                var item = new StrictContextMenuItem(null, null, null, null);

                // Call
                ContextMenuStrip result = builder.AddCustomItem(item).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                Assert.AreSame(item, result.Items[0]);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_NoOtherItemsWhenBuild_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddSeparator().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                CollectionAssert.IsEmpty(result.Items);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorAddedAtStart_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);

                var someItem = new StrictContextMenuItem(null, null, null, null);

                // Call
                for (var i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }

                ContextMenuStrip result = builder.AddCustomItem(someItem).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[0]);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedInBetweenItems_OneSeparatorAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);

                var someItem = new StrictContextMenuItem(null, null, null, null);
                var someOtherItem = new StrictContextMenuItem(null, null, null, null);

                builder.AddCustomItem(someItem);

                // Call
                for (var i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }

                ContextMenuStrip result = builder.AddCustomItem(someOtherItem).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(3, result.Items.Count);

                Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);
                Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[2]);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedAtEnd_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     new object(),
                                                     treeViewControl);

                builder.AddCustomItem(new StrictContextMenuItem(null, null, null, null));

                // Call
                for (var i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }

                ContextMenuStrip result = builder.Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
            }

            mocks.VerifyAll();
        }
    }
}