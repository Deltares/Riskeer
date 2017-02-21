﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(3, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveImpactAsphaltCoverFailureMechanismContext),
                    typeof(WaveImpactAsphaltCoverFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveImpactAsphaltCoverWaveConditionsOutput),
                    typeof(WaveImpactAsphaltCoverWaveConditionsOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveImpactAsphaltCoverWaveConditionsInputContext),
                    typeof(WaveImpactAsphaltCoverWaveConditionsInputContextProperties));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveImpactAsphaltCoverFailureMechanismContext),
                    typeof(WaveImpactAsphaltCoverFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>),
                    typeof(IEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult>),
                    typeof(WaveImpactAsphaltCoverFailureMechanismResultView));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(7, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveImpactAsphaltCoverFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyWaveImpactAsphaltCoverOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveImpactAsphaltCoverWaveConditionsOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveImpactAsphaltCoverWaveConditionsInputContext)));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new WaveImpactAsphaltCoverPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationContext)));
            }
        }
    }
}