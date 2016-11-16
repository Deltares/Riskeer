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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.HydraRing.Data;
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
                Assert.AreEqual(4, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsFailureMechanismContext),
                    typeof(GrassCoverErosionOutwardsFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverErosionOutwardsFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocation>),
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocation>),
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationsView));
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
                Assert.AreEqual(10, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundariesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyGrassCoverErosionOutwardsOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsInputContext)));
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
                Assert.AreEqual(7, propertyInfos.Length);

                PropertyInfo grassCoverErosionOutwardsFailureMechanismProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsFailureMechanismContext),
                    typeof(GrassCoverErosionOutwardsFailureMechanismProperties));
                Assert.IsNull(grassCoverErosionOutwardsFailureMechanismProperties.AdditionalDataCheck);
                Assert.IsNull(grassCoverErosionOutwardsFailureMechanismProperties.AfterCreate);

                var waterLevelHydraulicBoundaryLocationsContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext),
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContextProperties));
                Assert.IsNull(waterLevelHydraulicBoundaryLocationsContextProperties.AdditionalDataCheck);
                Assert.IsNull(waterLevelHydraulicBoundaryLocationsContextProperties.AfterCreate);

                var waveHeightLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext),
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationsContextProperties));
                Assert.IsNull(waveHeightLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(waveHeightLocationContextProperties.AfterCreate);

                var grassCoverErosionWaveConditionsOutputProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveConditionsOutput),
                    typeof(GrassCoverErosionOutwardsWaveConditionsOutputProperties));
                Assert.IsNull(grassCoverErosionWaveConditionsOutputProperties.AdditionalDataCheck);
                Assert.IsNull(grassCoverErosionWaveConditionsOutputProperties.AfterCreate);

                var grassCoverErosionWaveConditionsInputContextroperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveConditionsInputContext),
                    typeof(GrassCoverErosionOutwardsWaveConditionsInputContextProperties));
                Assert.IsNull(grassCoverErosionWaveConditionsInputContextroperties.AdditionalDataCheck);
                Assert.IsNull(grassCoverErosionWaveConditionsInputContextroperties.AfterCreate);

                var grassCoverErosionDesignWaterLevelLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationContext),
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties));
                Assert.IsNull(grassCoverErosionDesignWaterLevelLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(grassCoverErosionDesignWaterLevelLocationContextProperties.AfterCreate);

                var grasCoverErosionWaveHeightWaterLevelLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationContext),
                    typeof(GrassCoverErosionOutwardsWaveHeightLocationContextProperties));
                Assert.IsNull(grasCoverErosionWaveHeightWaterLevelLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(grasCoverErosionWaveHeightWaterLevelLocationContextProperties.AfterCreate);
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
                Assert.AreEqual(4, exportInfos.Length);
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(HydraulicBoundariesGroupContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext)));
            }
        }

        [Test]
        public void Activate_GuiNull_ThrowInvalidOperationException()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                var exception = Assert.Throws<InvalidOperationException>(test);
                Assert.AreEqual("Gui cannot be null", exception.Message);
            }
        }
    }
}