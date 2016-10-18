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
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Plugin.Test
{
    [TestFixture]
    public class HeightStructuresPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new HeightStructuresPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // setup
            using (var plugin = new HeightStructuresPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(3, propertyInfos.Length);
                PropertyInfo failureMechanismContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(HeightStructuresFailureMechanismContext),
                    typeof(HeightStructuresFailureMechanismContextProperties));
                Assert.IsNull(failureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(failureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(failureMechanismContextProperties.AfterCreate);

                PropertyInfo heightStructuresInputContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(HeightStructuresInputContext),
                    typeof(HeightStructuresInputContextProperties));
                Assert.IsNull(heightStructuresInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(heightStructuresInputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(heightStructuresInputContextProperties.AfterCreate);

                PropertyInfo heightStructureProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(HeightStructure),
                    typeof(HeightStructureProperties));
                Assert.IsNull(heightStructureProperties.AdditionalDataCheck);
                Assert.IsNull(heightStructureProperties.GetObjectPropertiesData);
                Assert.IsNull(heightStructureProperties.AfterCreate);
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new HeightStructuresPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(8, treeNodeInfos.Length);

                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new HeightStructuresPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(HeightStructuresFailureMechanismResultView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(HeightStructuresScenariosView)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetImportInfos_ReturnsExpectedImportInfos()
        {
            // Setup
            using (var plugin = new HeightStructuresPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(HeightStructuresContext)));
            }
        }
    }
}