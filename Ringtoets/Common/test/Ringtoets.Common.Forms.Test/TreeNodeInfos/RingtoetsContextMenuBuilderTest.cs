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

using Core.Common.Gui.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.TreeNodeInfos;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsContextMenuBuilderTest
    {
        [Test]
        public void AddRenameItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddRenameItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddRenameItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddDeleteItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddDeleteItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddDeleteItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExpandAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddExpandAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddExpandAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCollapseAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddCollapseAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddCollapseAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddOpenItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddOpenItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddOpenItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddExportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddExportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddImportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddImportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddImportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddPropertiesItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddPropertiesItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddPropertiesItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddSeparator());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddSeparator();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuItem = mocks.StrictMock<StrictContextMenuItem>();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddCustomItem(contextMenuItem));

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddCustomItem(contextMenuItem);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Build_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.Build());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.Build();

            // Assert
            mocks.VerifyAll();
        }
    }
}
