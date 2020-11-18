// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuItemFactoryTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutApplicationFeatureCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(null,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, exception.Message);
            StringAssert.EndsWith("applicationFeatureCommandHandler", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutImportCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         null,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_import_handler, exception.Message);
            StringAssert.EndsWith("importCommandHandler", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutExportCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         null,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_export_handler, exception.Message);
            StringAssert.EndsWith("exportCommandHandler", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutUpdateCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         null,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_update_handler, exception.Message);
            StringAssert.EndsWith("updateCommandHandler", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutViewCommandsHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         null,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_view_commands, exception.Message);
            StringAssert.EndsWith("viewCommandsHandler", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutDataObject_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_data, exception.Message);
            StringAssert.EndsWith("dataObject", exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidInputParameters_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            Assert.DoesNotThrow(Call);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateOpenItem_Always_ItemWithPropertiesSet(bool canOpenView)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            viewCommands.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(canOpenView);

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateOpenItem();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.AreEqual(canOpenView, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateOpenItem_CanOpenView_CausesViewToOpenWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            viewCommands.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(true);
            viewCommands.Expect(ch => ch.OpenView(nodeData));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateOpenItem();

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportItem_Always_ItemWithPropertiesSet(bool hasExportersForNodeData)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            exportCommandHandler.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateExportItem();

            // Assert
            Assert.AreEqual(Resources.Export, item.Text);
            Assert.AreEqual(Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.AreEqual(hasExportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateExportItem_CanExportFrom_CausesExportToStartWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            exportCommandHandler.Expect(ch => ch.CanExportFrom(nodeData)).Return(true);
            exportCommandHandler.Expect(ch => ch.ExportFrom(nodeData));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateExportItem();

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithoutParameters_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            importCommandHandler.Expect(ich => ich.GetSupportedImportInfos(nodeData)).Return(hasImportersForNodeData
                                                                                                 ? new[]
                                                                                                 {
                                                                                                     new ImportInfo()
                                                                                                 }
                                                                                                 : new ImportInfo[0]);

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateImportItem();

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithoutParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.Expect(ich => ich.GetSupportedImportInfos(nodeData)).Return(importInfos);
            importCommandHandler.Expect(ich => ich.ImportOn(nodeData, importInfos));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateImportItem();

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithImportInfosParameter_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            ImportInfo[] importInfos = hasImportersForNodeData
                                           ? new[]
                                           {
                                               new ImportInfo()
                                           }
                                           : new ImportInfo[0];

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateImportItem(importInfos);

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithImportInfosParameter_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.Expect(ich => ich.ImportOn(nodeData, importInfos));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateImportItem(importInfos);

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CreateImportItemWithTextualParameters_InvalidTextParameter_ThrowArgumentException(string text)
        {
            // Setup
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, toolTip, image);

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "Text should be set.");
            Assert.AreEqual("text", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithTextualParameters_TooltipParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, null, image);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("toolTip", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithTextualParameters_ImageParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, toolTip, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("image", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithTextualParameters_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            importCommandHandler.Expect(ich => ich.GetSupportedImportInfos(nodeData)).Return(hasImportersForNodeData
                                                                                                 ? new[]
                                                                                                 {
                                                                                                     new ImportInfo()
                                                                                                 }
                                                                                                 : new ImportInfo[0]);

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateImportItem(text, toolTip, image);

            // Assert
            Assert.AreEqual(text, item.Text);
            Assert.AreEqual(toolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(image, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithTextualParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.Expect(ich => ich.GetSupportedImportInfos(nodeData)).Return(importInfos);
            importCommandHandler.Expect(ich => ich.ImportOn(nodeData, importInfos));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateImportItem(text, toolTip, image);

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CreateImportItemWithAllParameters_InvalidTextParameter_ThrowArgumentException(string text)
        {
            // Setup
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, toolTip, image, Enumerable.Empty<ImportInfo>());

            // Assert
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, "Text should be set.");
            Assert.AreEqual("text", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithAllParameters_TooltipParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, null, image, Enumerable.Empty<ImportInfo>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("toolTip", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithAllParameters_ImageParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            void Call() => contextMenuFactory.CreateImportItem(text, toolTip, null, Enumerable.Empty<ImportInfo>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("image", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithAllParameters_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            mocks.ReplayAll();

            ImportInfo[] importInfos = hasImportersForNodeData
                                           ? new[]
                                           {
                                               new ImportInfo()
                                           }
                                           : new ImportInfo[0];

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateImportItem(text, toolTip, image, importInfos);

            // Assert
            Assert.AreEqual(text, item.Text);
            Assert.AreEqual(toolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(image, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateImportItemWithAllParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.Expect(ich => ich.ImportOn(nodeData, importInfos));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateImportItem(text, toolTip, image, importInfos);

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateUpdateItem_Always_ItemWithPropertiesSet(bool canUpdateOn)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            updateCommandHandler.Expect(ch => ch.CanUpdateOn(nodeData)).Return(canUpdateOn);

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateUpdateItem();

            // Assert
            Assert.AreEqual(Resources.Update, item.Text);
            Assert.AreEqual(Resources.Update_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.RefreshIcon, item.Image);
            Assert.AreEqual(canUpdateOn, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateUpdateItem_CanUpdateOn_CausesUpdateToStartWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            updateCommandHandler.Expect(ch => ch.CanUpdateOn(nodeData)).Return(true);
            updateCommandHandler.Expect(ch => ch.UpdateOn(nodeData));

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            ToolStripItem item = contextMenuFactory.CreateUpdateItem();

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePropertiesItem_Always_ItemWithPropertiesSet(bool hasPropertyInfoForNodeData)
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            commandHandler.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertyInfoForNodeData);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            mocks.ReplayAll();

            // Call
            ToolStripItem item = contextMenuFactory.CreatePropertiesItem();

            // Assert
            Assert.AreEqual(Resources.Properties, item.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesHS, item.Image);
            Assert.AreEqual(hasPropertyInfoForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePropertiesItem_CanShowPropertiesFor_CausesPropertiesToBeShownWhenClicked()
        {
            // Setup
            var commandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();

            commandHandler.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(true);
            commandHandler.Expect(ch => ch.ShowPropertiesForSelection());

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            mocks.ReplayAll();

            ToolStripItem item = contextMenuFactory.CreatePropertiesItem();

            // Call
            item.PerformClick();

            // Assert
            mocks.VerifyAll();
        }
    }
}