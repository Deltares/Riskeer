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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfoClasses()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(1, viewInfos.Length);

                var grassCoverErosionOutwardsFailureMechanismSectionResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>), grassCoverErosionOutwardsFailureMechanismSectionResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsFailureMechanismResultView), grassCoverErosionOutwardsFailureMechanismSectionResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, grassCoverErosionOutwardsFailureMechanismSectionResultViewInfo.Image);
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(5, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundariesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext)));
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(3, propertyInfos.Length);

                PropertyInfo grassCoverErosionOutwardsFailureMechanismProperties = PluginTestHelper.AssertPropertyInfoDefined<
                    GrassCoverErosionOutwardsFailureMechanismContext,
                    GrassCoverErosionOutwardsFailureMechanismProperties>(propertyInfos);
                Assert.IsNull(grassCoverErosionOutwardsFailureMechanismProperties.AdditionalDataCheck);
                Assert.IsNull(grassCoverErosionOutwardsFailureMechanismProperties.AfterCreate);

                var waterLevelHydraulicBoundaryLocationsContextProperties = PluginTestHelper.AssertPropertyInfoDefined<
                    GrassCoverErosionOutwardsDesignWaterLevelLocationsContext,
                    GrassCoverErosionOutwardsDesignWaterLevelLocationsContextProperties>(propertyInfos);
                Assert.IsNull(waterLevelHydraulicBoundaryLocationsContextProperties.AdditionalDataCheck);
                Assert.IsNull(waterLevelHydraulicBoundaryLocationsContextProperties.AfterCreate);

                var waveHeightLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined<
                    GrassCoverErosionOutwardsWaveHeightLocationsContext,
                    GrassCoverErosionOutwardsWaveHeightLocationsContextProperties>(propertyInfos);
                Assert.IsNull(waveHeightLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(waveHeightLocationContextProperties.AfterCreate);
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, exportInfos.Length);
                var hydraulicBoundaryLocationExportInfo = exportInfos.Single(ei => ei.DataType == typeof(HydraulicBoundariesGroupContext));
                Assert.IsNull(hydraulicBoundaryLocationExportInfo.Name);
                Assert.IsNull(hydraulicBoundaryLocationExportInfo.Image);
                Assert.IsNull(hydraulicBoundaryLocationExportInfo.Category);
            }
        }
    }
}