// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using NSubstitute;
using NUnit.Framework;

namespace Core.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuBuilderTest
    {
        [Test]
        public void Constructor_NoApplicationFeatureCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Constructor_NoImportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Constructor_NoExportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Constructor_NoUpdateCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Constructor_NoViewCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
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
        }

        [Test]
        public void Constructor_NoDataObject_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Constructor_NoTreeViewControl_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

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
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void AddRenameItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var dataObject = new object();
            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
                var importCommandHandler = Substitute.For<IImportCommandHandler>();
                var exportCommandHandler = Substitute.For<IExportCommandHandler>();
                var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var treeNodeInfo = Substitute.For<TreeNodeInfo<object>>();
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
                var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
                var importCommandHandler = Substitute.For<IImportCommandHandler>();
                var exportCommandHandler = Substitute.For<IExportCommandHandler>();
                var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var treeNodeInfo = Substitute.For<TreeNodeInfo<string>>();
                var parentTreeNodeInfo = Substitute.For<TreeNodeInfo<object>>();
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
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddRemoveAllChildrenItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var treeViewControl = Substitute.For<ITreeViewControl>();
            treeViewControl.CanRemoveChildNodesOfData(dataObject).Returns(hasChildren);
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
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var treeViewControl = Substitute.For<ITreeViewControl>();

            treeViewControl.CanExpandOrCollapseForData(dataObject).Returns(hasChildren);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var treeViewControl = Substitute.For<ITreeViewControl>();

            treeViewControl.CanExpandOrCollapseForData(dataObject).Returns(hasChildren);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddOpenItem_WhenBuild_ItemAddedToContextMenu(bool hasViewForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            viewCommands.CanOpenViewFor(nodeData).Returns(hasViewForNodeData);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExportItem_WhenBuild_ItemAddedToContextMenu(bool hasExportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            exportCommandHandler.CanExportFrom(nodeData).Returns(hasExportersForNodeData);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithoutParameters_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(hasImportersForNodeData
                                                                               ? new[]
                                                                               {
                                                                                   new ImportInfo()
                                                                               }
                                                                               : new ImportInfo[0]);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItemWithImportInfosParameter_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(hasImportersForNodeData
                                                                               ? new[]
                                                                               {
                                                                                   new ImportInfo()
                                                                               }
                                                                               : new ImportInfo[0]);
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

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddUpdateItem_WhenBuild_ItemAddedToContextMenu(bool hasUpdatesForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            updateCommandHandler.CanUpdateOn(nodeData).Returns(hasUpdatesForNodeData);
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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddPropertiesItem_WhenBuild_ItemAddedToContextMenu(bool hasPropertiesForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            applicationFeatureCommands.CanShowPropertiesFor(nodeData).Returns(hasPropertiesForNodeData);
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
        }

        [Test]
        public void AddCustomItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        public void AddSeparator_NoOtherItemsWhenBuild_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorAddedAtStart_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedInBetweenItems_OneSeparatorAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedAtEnd_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
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
        }
    }
}