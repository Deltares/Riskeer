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

using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Revetment.Forms.Views;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Forms.Views;

namespace Riskeer.StabilityStoneCover.Plugin.Test
{
    [TestFixture]
    public class StabilityStoneCoverPluginTest
    {
        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(4, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverHydraulicLoadsContext),
                    typeof(StabilityStoneCoverHydraulicLoadsProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverFailurePathContext),
                    typeof(StabilityStoneCoverFailurePathProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverWaveConditionsOutputContext),
                    typeof(StabilityStoneCoverWaveConditionsOutputProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverWaveConditionsInputContext),
                    typeof(StabilityStoneCoverWaveConditionsInputContextProperties));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(4, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverHydraulicLoadsContext),
                    typeof(StabilityStoneCoverFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverFailurePathContext),
                    typeof(StabilityStoneCoverFailurePathView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<StabilityStoneCoverFailureMechanismSectionResult>),
                    typeof(StabilityStoneCoverResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverWaveConditionsInputContext),
                    typeof(StabilityStoneCoverWaveConditionsCalculation),
                    typeof(WaveConditionsInputView));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(8, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverHydraulicLoadsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyStabilityStoneCoverOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsInputContext)));
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

            using (var plugin = new StabilityStoneCoverPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(4, exportInfos.Length);
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext)));
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)));
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext)));
            }
        }

        [Test]
        public void GetUpdateInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(1, updateInfos.Length);
                Assert.AreEqual(1, updateInfos.Count(ei => ei.DataType == typeof(StabilityStoneCoverFailureMechanismSectionsContext)));
            }
        }
    }
}