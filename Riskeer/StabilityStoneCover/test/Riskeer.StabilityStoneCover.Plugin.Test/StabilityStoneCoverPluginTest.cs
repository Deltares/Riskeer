// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Windows.Media;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Util.Extensions;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Views;
using Riskeer.Revetment.Forms.Views;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;
using HydraulicLoadsStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.HydraulicLoadsState.StabilityStoneCoverFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.RegistrationState.StabilityStoneCoverFailureMechanismContext;
using HydraulicLoadsStateFailureMechanismProperties = Riskeer.StabilityStoneCover.Forms.PropertyClasses.HydraulicLoadsState.StabilityStoneCoverFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.StabilityStoneCover.Forms.PropertyClasses.RegistrationState.StabilityStoneCoverFailureMechanismProperties;
using HydraulicLoadsStateFailureMechanismView = Riskeer.StabilityStoneCover.Forms.Views.HydraulicLoadsState.StabilityStoneCoverFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.StabilityStoneCover.Forms.Views.RegistrationState.StabilityStoneCoverFailureMechanismView;

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
                    typeof(HydraulicLoadsStateFailureMechanismContext),
                    typeof(HydraulicLoadsStateFailureMechanismProperties));
                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(RegistrationStateFailureMechanismContext),
                    typeof(RegistrationStateFailureMechanismProperties));
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
            const string symbol = "<symbol>";
            var fontFamily = new FontFamily();

            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            gui.Stub(g => g.ActiveStateInfo).Return(new StateInfo(string.Empty, symbol, fontFamily, p => p));
            mockRepository.ReplayAll();

            using (var plugin = new StabilityStoneCoverPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(4, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(HydraulicLoadsStateFailureMechanismContext),
                    typeof(HydraulicLoadsStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(RegistrationStateFailureMechanismContext),
                    typeof(RegistrationStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverFailureMechanismSectionResultContext),
                    typeof(IObservableEnumerable<NonAdoptableFailureMechanismSectionResult>),
                    typeof(NonAdoptableFailureMechanismResultView<StabilityStoneCoverFailureMechanism>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverWaveConditionsInputContext),
                    typeof(StabilityStoneCoverWaveConditionsCalculation),
                    typeof(WaveConditionsInputView));

                viewInfos.ForEachElementDo(vi =>
                {
                    Assert.AreEqual(symbol, vi.GetSymbol());
                    Assert.AreSame(fontFamily, vi.GetFontFamily());
                });
            }

            mockRepository.VerifyAll();
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
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicLoadsStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RegistrationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyStabilityStoneCoverOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverFailureMechanismSectionResultContext)));
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
                Assert.AreEqual(2, exportInfos.Count(ei => ei.DataType == typeof(StabilityStoneCoverCalculationGroupContext)));
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
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(StabilityStoneCoverCalculationGroupContext)));
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