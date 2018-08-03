// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.ClosingStructures.Plugin.Test
{
    [TestFixture]
    public class ClosingStructuresPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(5, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresFailureMechanismContext),
                    typeof(ClosingStructuresFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructure),
                    typeof(ClosingStructureProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresContext),
                    typeof(StructureCollectionProperties<ClosingStructure>));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresInputContext),
                    typeof(ClosingStructuresInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresOutputContext),
                    typeof(ClosingStructuresOutputProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(8, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresScenariosContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresFailureMechanismContext),
                    typeof(ClosingStructuresFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<ClosingStructuresFailureMechanismSectionResult>),
                    typeof(ClosingStructuresFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresScenariosContext),
                    typeof(CalculationGroup),
                    typeof(ClosingStructuresScenariosView));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ClosingStructuresContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ClosingStructuresCalculationGroupContext)));
            }
        }

        [Test]
        public void GetUpdateInfos_ReturnSupportedUpdateInfos()
        {
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                UpdateInfo[] updateInfo = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(2, updateInfo.Length);
                Assert.IsTrue(updateInfo.Any(i => i.DataType == typeof(ClosingStructuresContext)));
                Assert.IsTrue(updateInfo.Any(i => i.DataType == typeof(ClosingStructuresFailureMechanismSectionsContext)));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(ClosingStructuresCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(ClosingStructuresCalculationContext)));
            }
        }
    }
}