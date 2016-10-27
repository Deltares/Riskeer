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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin.Test
{
    [TestFixture]
    public class StabilityPointStructuresPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(2, propertyInfos.Length);

                PropertyInfo failureMechanismContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructuresFailureMechanismContext),
                    typeof(StabilityPointStructuresFailureMechanismProperties));
                Assert.IsNull(failureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNotNull(failureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(failureMechanismContextProperties.AfterCreate);

                PropertyInfo stabilityPointStructureProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructure),
                    typeof(StabilityPointStructureProperties));
                Assert.IsNull(stabilityPointStructureProperties.AdditionalDataCheck);
                Assert.IsNull(stabilityPointStructureProperties.GetObjectPropertiesData);
                Assert.IsNull(stabilityPointStructureProperties.AfterCreate);
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

            using (var plugin = new StabilityPointStructuresPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(8, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresInputContext)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(1, viewInfos.Length);
                ViewInfo stabilityPointStructuresResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult>), stabilityPointStructuresResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(StabilityPointStructuresFailureMechanismResultView), stabilityPointStructuresResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, stabilityPointStructuresResultViewInfo.Image);
            }
        }

        [Test]
        public void GetImportInfos_ReturnsExpectedImportInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(StabilityPointStructuresContext)));
            }
        }
    }
}