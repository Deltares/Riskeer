// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Forms.Views;

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
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(5, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructuresFailureMechanismContext),
                    typeof(StabilityPointStructuresFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructure),
                    typeof(StabilityPointStructureProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructuresInputContext),
                    typeof(StabilityPointStructuresInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructuresContext),
                    typeof(StructureCollectionProperties<StabilityPointStructure>));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityPointStructuresOutputContext),
                    typeof(StabilityPointStructuresOutputProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(8, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresInputContext)));
            }
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
                Assert.AreEqual(3, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityPointStructuresFailureMechanismContext),
                    typeof(StabilityPointStructuresFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult>),
                    typeof(StabilityPointStructuresFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityPointStructuresScenariosContext),
                    typeof(CalculationGroup),
                    typeof(StabilityPointStructuresScenariosView));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(StabilityPointStructuresContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(StabilityPointStructuresCalculationGroupContext)));
            }
        }

        [Test]
        public void GetUpdateInfos_ReturnSupportedUpdateInfos()
        {
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                UpdateInfo[] updateInfo = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(2, updateInfo.Length);
                Assert.IsTrue(updateInfo.Any(i => i.DataType == typeof(StabilityPointStructuresContext)));
                Assert.IsTrue(updateInfo.Any(i => i.DataType == typeof(StabilityPointStructuresFailureMechanismSectionsContext)));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new StabilityPointStructuresPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(StabilityPointStructuresCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(StabilityPointStructuresCalculationContext)));
            }
        }
    }
}