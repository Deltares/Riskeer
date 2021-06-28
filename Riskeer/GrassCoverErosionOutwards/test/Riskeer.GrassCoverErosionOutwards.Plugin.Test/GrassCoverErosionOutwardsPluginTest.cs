﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionOutwards.Forms.Views;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(5, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsCalculationsContext),
                    typeof(GrassCoverErosionOutwardsFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverErosionOutwardsFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(DesignWaterLevelCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightCalculationsContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(WaveHeightCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionOutwardsWaveConditionsInputContext),
                    typeof(ICalculation<FailureMechanismCategoryWaveConditionsInput>),
                    typeof(WaveConditionsInputView));
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
                Assert.AreEqual(13, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyGrassCoverErosionOutwardsOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext)));
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
                Assert.AreEqual(8, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsCalculationsContext),
                    typeof(GrassCoverErosionOutwardsCalculationsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsFailurePathContext),
                    typeof(GrassCoverErosionOutwardsFailurePathProperties));
                
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext),
                    typeof(DesignWaterLevelCalculationsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightCalculationsContext),
                    typeof(WaveHeightCalculationsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveConditionsOutputContext),
                    typeof(GrassCoverErosionOutwardsWaveConditionsOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveConditionsInputContext),
                    typeof(GrassCoverErosionOutwardsWaveConditionsInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext),
                    typeof(DesignWaterLevelCalculationsGroupProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext),
                    typeof(WaveHeightCalculationsGroupProperties));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new GrassCoverErosionOutwardsPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(6, exportInfos.Length);
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext)));
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext)));
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