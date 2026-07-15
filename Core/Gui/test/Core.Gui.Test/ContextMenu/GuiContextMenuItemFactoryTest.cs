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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
    public class ContextMenuItemFactoryTest
    {
        private sealed class MockRepository
        {
            public T StrictMock<T>() where T : class
            {
                return Substitute.For<T>();
            }

            public T Stub<T>() where T : class
            {
                return Substitute.For<T>();
            }

            public void ReplayAll() {}

            public void VerifyAll() {}
        }

        [SetUp]
        public void SetUp() {}

        [Test]
        public void Constructor_WithoutApplicationFeatureCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(null,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith($"Kan geen '{nameof(ApplicationFeatureCommandHandler)}'-afhankelijk element " +
                                    $"in het contextmenu creëren zonder een '{nameof(ApplicationFeatureCommandHandler)}'.",
                                    exception.Message);
            StringAssert.EndsWith("applicationFeatureCommandHandler", exception.Message);
        }

        [Test]
        public void Constructor_WithoutImportCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         null,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith($"Kan geen '{nameof(IImportCommandHandler)}'-afhankelijk element " +
                                    $"in het contextmenu creëren zonder een '{nameof(IImportCommandHandler)}'.",
                                    exception.Message);
            StringAssert.EndsWith("importCommandHandler", exception.Message);
        }

        [Test]
        public void Constructor_WithoutExportCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         null,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith($"Kan geen '{nameof(IExportCommandHandler)}'-afhankelijk element " +
                                    $"in het contextmenu creëren zonder een '{nameof(IExportCommandHandler)}'.",
                                    exception.Message);
            StringAssert.EndsWith("exportCommandHandler", exception.Message);
        }

        [Test]
        public void Constructor_WithoutUpdateCommandHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         null,
                                                         viewCommands,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith($"Kan geen '{nameof(IUpdateCommandHandler)}'-afhankelijk element " +
                                    $"in het contextmenu creëren zonder een '{nameof(IUpdateCommandHandler)}'.",
                                    exception.Message);
            StringAssert.EndsWith("updateCommandHandler", exception.Message);
        }

        [Test]
        public void Constructor_WithoutViewCommandsHandler_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         null,
                                                         new object());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith($"Kan geen '{nameof(IViewCommands)}'-afhankelijk element " +
                                    $"in het contextmenu creëren zonder een '{nameof(IViewCommands)}'.",
                                    exception.Message);
            StringAssert.EndsWith("viewCommandsHandler", exception.Message);
        }

        [Test]
        public void Constructor_WithoutDataObject_ThrowsArgumentNullException()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            StringAssert.StartsWith("Kan geen element in het contextmenu creëren zonder dat de data bekend is.", exception.Message);
            StringAssert.EndsWith("dataObject", exception.Message);
        }

        [Test]
        public void Constructor_ValidInputParameters_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            // Call
            void Call() => new GuiContextMenuItemFactory(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommands,
                                                         new object());

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateOpenItem_Always_ItemWithPropertiesSet(bool canOpenView)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            viewCommands.CanOpenViewFor(nodeData).Returns(canOpenView);
            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateOpenItem();

            // Assert
            Assert.AreEqual("&Openen", item.Text);
            Assert.AreEqual("Open de gegevens in een nieuw documentvenster.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.OpenIcon, item.Image);
            Assert.AreEqual(canOpenView, item.Enabled);
        }

        [Test]
        public void CreateOpenItem_CanOpenView_CausesViewToOpenWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            viewCommands.CanOpenViewFor(nodeData).Returns(true);
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
            viewCommands.Received(1).OpenView(nodeData);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExportItem_Always_ItemWithPropertiesSet(bool hasExportersForNodeData)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            exportCommandHandler.CanExportFrom(nodeData).Returns(hasExportersForNodeData);
            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateExportItem();

            // Assert
            Assert.AreEqual("&Exporteren...", item.Text);
            Assert.AreEqual("Exporteer de gegevens naar een bestand.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExportIcon, item.Image);
            Assert.AreEqual(hasExportersForNodeData, item.Enabled);
        }

        [Test]
        public void CreateExportItem_CanExportFrom_CausesExportToStartWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            exportCommandHandler.CanExportFrom(nodeData).Returns(true);
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
            exportCommandHandler.Received(1).ExportFrom(nodeData);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithoutParameters_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(hasImportersForNodeData
                                                                               ? new[]
                                                                               {
                                                                                   new ImportInfo()
                                                                               }
                                                                               : new ImportInfo[0]);
            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateImportItem();

            // Assert
            Assert.AreEqual("&Importeren...", item.Text);
            Assert.AreEqual("Importeer de gegevens vanuit een bestand.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);
        }

        [Test]
        public void CreateImportItemWithoutParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(importInfos);
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
            importCommandHandler.Received(1).ImportOn(nodeData, importInfos);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateImportItemWithImportInfosParameter_Always_ItemWithPropertiesSet(bool hasImportersForNodeData)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
            Assert.AreEqual("&Importeren...", item.Text);
            Assert.AreEqual("Importeer de gegevens vanuit een bestand.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ImportIcon, item.Image);
            Assert.AreEqual(hasImportersForNodeData, item.Enabled);
        }

        [Test]
        public void CreateImportItemWithImportInfosParameter_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };
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
            importCommandHandler.Received(1)
                                .ImportOn(nodeData,
                                          Arg.Is<ImportInfo[]>(x =>
                                                                   x.Length == importInfos.Length &&
                                                                   x.SequenceEqual(importInfos)));
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

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
        }

        [Test]
        public void CreateImportItemWithTextualParameters_TooltipParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            Image image = Resources.ImportIcon;

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
        }

        [Test]
        public void CreateImportItemWithTextualParameters_ImageParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(hasImportersForNodeData
                                                                               ? new[]
                                                                               {
                                                                                   new ImportInfo()
                                                                               }
                                                                               : new ImportInfo[0]);
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
        }

        [Test]
        public void CreateImportItemWithTextualParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };

            importCommandHandler.GetSupportedImportInfos(nodeData).Returns(importInfos);
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
            importCommandHandler.Received(1).ImportOn(nodeData, importInfos);
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

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
        }

        [Test]
        public void CreateImportItemWithAllParameters_TooltipParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            Image image = Resources.ImportIcon;

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
        }

        [Test]
        public void CreateImportItemWithAllParameters_ImageParameterNull_ThrowArgumentNullException()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
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
        }

        [Test]
        public void CreateImportItemWithAllParameters_SupportedImportInfo_CausesImportToStartWhenClicked()
        {
            // Setup
            const string text = "Import";
            const string toolTip = "Import tooltip";
            Image image = Resources.ImportIcon;

            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            ImportInfo[] importInfos =
            {
                new ImportInfo()
            };
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
            importCommandHandler.Received(1).ImportOn(nodeData, importInfos);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateUpdateItem_Always_ItemWithPropertiesSet(bool canUpdateOn)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            updateCommandHandler.CanUpdateOn(nodeData).Returns(canUpdateOn);
            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);

            // Call
            ToolStripItem item = contextMenuFactory.CreateUpdateItem();

            // Assert
            Assert.AreEqual("&Bijwerken...", item.Text);
            Assert.AreEqual("Werk de geïmporteerde gegevens bij met nieuwe gegevens vanuit een bestand.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.RefreshIcon, item.Image);
            Assert.AreEqual(canUpdateOn, item.Enabled);
        }

        [Test]
        public void CreateUpdateItem_CanUpdateOn_CausesUpdateToStartWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            updateCommandHandler.CanUpdateOn(nodeData).Returns(true);
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
            updateCommandHandler.Received(1).UpdateOn(nodeData);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreatePropertiesItem_Always_ItemWithPropertiesSet(bool hasPropertyInfoForNodeData)
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();
            commandHandler.CanShowPropertiesFor(nodeData).Returns(hasPropertyInfoForNodeData);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);
            // Call
            ToolStripItem item = contextMenuFactory.CreatePropertiesItem();

            // Assert
            Assert.AreEqual("Ei&genschappen", item.Text);
            Assert.AreEqual("Toon de eigenschappen in het Eigenschappenpaneel.", item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.PropertiesHS, item.Image);
            Assert.AreEqual(hasPropertyInfoForNodeData, item.Enabled);
        }

        [Test]
        public void CreatePropertiesItem_CanShowPropertiesFor_CausesPropertiesToBeShownWhenClicked()
        {
            // Setup
            var commandHandler = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var nodeData = new object();

            commandHandler.CanShowPropertiesFor(nodeData).Returns(true);

            var contextMenuFactory = new GuiContextMenuItemFactory(commandHandler,
                                                                   importCommandHandler,
                                                                   exportCommandHandler,
                                                                   updateCommandHandler,
                                                                   viewCommands,
                                                                   nodeData);
            ToolStripItem item = contextMenuFactory.CreatePropertiesItem();

            // Call
            item.PerformClick();

            // Assert
            commandHandler.Received(1).ShowPropertiesForSelection();
        }
    }
}