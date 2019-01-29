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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Common.Gui.TestUtil.Test.ContextMenu
{
    [TestFixture]
    public class CustomItemsOnlyContextMenuBuilderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Assert
            Assert.IsInstanceOf<IContextMenuBuilder>(builder);
        }

        [Test]
        public void AddRenameItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddRenameItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddDeleteItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddDeleteItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddDeleteChildrenItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddDeleteChildrenItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddExpandAllItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddExpandAllItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddCollapseAllItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddCollapseAllItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddOpenItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddOpenItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddExportItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddExportItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddImportItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddImportItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddUpdateItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddUpdateItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddCustomImportItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddCustomImportItem(null, null, null).Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddPropertiesItem_WhenBuild_StubbedItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddPropertiesItem().Build();

            // Assert
            AssertStubbedContextMenuItem(result);
        }

        [Test]
        public void AddSeparatorItem_WhenBuild_SeparatorItemAddedToContextMenu()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip result = builder.AddSeparator().Build();

            // Assert
            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[0]);
        }

        [Test]
        public void AddCustomItem_WhenBuild_CustomItemAddedToContextMenu()
        {
            // Setup
            const string text = "custom item";
            const string toolTip = "tooltip";
            Bitmap image = CoreCommonGuiResources.Busy_indicator;
            var customItem = new StrictContextMenuItem(text, toolTip, image, (sender, args) => {});

            var builder = new CustomItemsOnlyContextMenuBuilder();

            // Call
            ContextMenuStrip menu = builder.AddCustomItem(customItem).Build();

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 0, text, toolTip, image);
        }

        private void AssertStubbedContextMenuItem(ContextMenuStrip menu)
        {
            TestHelper.AssertContextMenuStripContainsItem(menu, 0, string.Empty, string.Empty, null);
        }
    }
}