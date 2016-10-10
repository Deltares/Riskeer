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

using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Plugin;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructureTreeNodeInfoTest
    {
        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(StabilityPointStructure), info.TagType);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.ChildNodeObjects);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnDrop);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNotNull(info.Text);
            }
        }

        [Test]
        public void Text_Always_ReturnNameOfStructure()
        {
            // Setup
            const string name = "a nice name";
            StabilityPointStructure structure = CreateSimpleStabilityPointStructure(name);

            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                string text = info.Text(structure);

                // Assert
                Assert.AreEqual(name, text);
            }
        }

        [Test]
        public void Image_Always_ReturnStructureIcon()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.StructuresIcon, image);
            }
        }

        private static TreeNodeInfo GetInfo(StabilityPointStructuresPlugin gui)
        {
            return gui.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructure));
        }

        private static StabilityPointStructure CreateSimpleStabilityPointStructure(string name)
        {
            return new StabilityPointStructure(name, "1", new Point2D(1, 2),
                                               123.456,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               456.789, 0.456,
                                               567.890, 0.567,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               123.546, 0.123,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               555.555,
                                               456.789, 0.456,
                                               555.55,
                                               0.55,
                                               567.890, 0.567,
                                               7777777.777, 0.777,
                                               567.890, 0.567,
                                               42,
                                               0.55,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               StabilityPointStructureType.FloodedCulvert
                );
        }
    }
}