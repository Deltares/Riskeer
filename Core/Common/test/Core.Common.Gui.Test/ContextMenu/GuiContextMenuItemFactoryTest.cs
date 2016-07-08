// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
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
        public void Constructor_WithoutCommandHandler_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(null, null,  null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_gui, message);
            StringAssert.EndsWith("applicationFeatureCommandHandler", message);
        }

        [Test]
        public void Constructor_WithoutDataObject_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandler = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandler, viewCommandsMock, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_data, message);
            StringAssert.EndsWith("dataObject", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutExportImportHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, null, viewCommandsMock, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_exportImport_handler, message);
            StringAssert.EndsWith("exportImportCommandHandler", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutViewCommandsHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandlerMock, null, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.GuiContextMenuItemFactory_Can_not_create_gui_context_menu_items_without_view_commands, message);
            StringAssert.EndsWith("viewCommandsHandler", message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithAllInput_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandHandler = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new GuiContextMenuItemFactory(applicationFeatureCommandHandler, exportImportCommandHandlerMock, viewCommandsMock, new object());

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateOpenItem_NoDataAvailableForView_MenuItemIsDisabled()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            viewCommandsMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(false);
            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeData);

            // Call
            var item = contextMenuFactory.CreateOpenItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.IsFalse(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateOpenItem_HasViewForData_MenuItemEnabledAndCausesViewToOpenWhenClicked()
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            viewCommandsMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(true);
            viewCommandsMock.Expect(ch => ch.OpenView(nodeData));
            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeData);

            // Call
            var item = contextMenuFactory.CreateOpenItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Open, item.Text);
            Assert.AreEqual(Resources.Open_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.IsTrue(item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportItem_Always_ItemWithPropertiesSet(bool hasExportersForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            exportImportCommandHandlerMock.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);
            if (hasExportersForNodeData)
            {
                exportImportCommandHandlerMock.Expect(ch => ch.ExportFrom(nodeData));
            }

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeData);

            // Call
            var item = contextMenuFactory.CreateExportItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Export, item.Text);
            Assert.AreEqual(Resources.Export_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.AreEqual(hasExportersForNodeData, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItem_Always_ItemWithPropertiesSet(bool canImportOn)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            exportImportCommandHandlerMock.Expect(ch => ch.CanImportOn(nodeData)).Return(canImportOn);
            if (canImportOn)
            {
                exportImportCommandHandlerMock.Expect(ch => ch.ImportOn(nodeData));
            }

            mocks.ReplayAll();

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeData);

            // Call
            var item = contextMenuFactory.CreateImportItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Import, item.Text);
            Assert.AreEqual(Resources.Import_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(canImportOn, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePropertiesItem_Always_ItemWithPropertiesSet(bool hasPropertyInfoForNodeData)
        {
            // Setup
            var commandHandlerMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportCommandHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var nodeData = new object();
            commandHandlerMock.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertyInfoForNodeData);
            if (hasPropertyInfoForNodeData)
            {
                commandHandlerMock.Expect(ch => ch.ShowPropertiesFor(nodeData));
            }

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandlerMock, exportImportCommandHandlerMock, viewCommandsMock, nodeData);

            mocks.ReplayAll();

            // Call
            var item = contextMenuFactory.CreatePropertiesItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Properties, item.Text);
            Assert.AreEqual(Resources.Properties_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesHS, item.Image);
            Assert.AreEqual(hasPropertyInfoForNodeData, item.Enabled);

            mocks.VerifyAll();
        }
    }
}