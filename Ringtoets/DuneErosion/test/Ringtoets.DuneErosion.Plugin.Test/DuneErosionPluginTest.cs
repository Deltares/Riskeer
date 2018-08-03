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

using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Plugin.Test
{
    [TestFixture]
    public class DuneErosionPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new DuneErosionPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void Activate_GuiNull_ThrowInvalidOperationException()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(test);
                Assert.AreEqual("Gui cannot be null", exception.Message);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(3, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneErosionFailureMechanismContext),
                    typeof(DuneErosionFailureMechanismProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneLocationCalculationsContext),
                    typeof(DuneLocationCalculationsProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DuneLocationCalculation),
                    typeof(DuneLocationCalculationProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(4, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneErosionFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneLocationCalculationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DuneLocationCalculationsContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<DuneErosionFailureMechanismSectionResult>),
                    typeof(DuneErosionFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DuneErosionFailureMechanismContext),
                    typeof(DuneErosionFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DuneLocationCalculationsContext),
                    typeof(IObservableEnumerable<DuneLocationCalculation>),
                    typeof(DuneLocationCalculationsView));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(DuneLocationCalculationsContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(DuneLocationCalculationsGroupContext)));
            }
        }
        
        [Test]
        public void GetUpdateInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(1, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(ei => ei.DataType == typeof(DuneErosionFailureMechanismSectionsContext)));
            }
        }
    }
}