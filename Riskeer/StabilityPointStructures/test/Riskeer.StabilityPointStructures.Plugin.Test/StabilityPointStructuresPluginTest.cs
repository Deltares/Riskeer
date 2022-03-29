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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PropertyClasses;
using Riskeer.StabilityPointStructures.Forms.Views;
using CalculationStateFailureMechanismContext = Riskeer.StabilityPointStructures.Forms.PresentationObjects.CalculationsState.StabilityPointStructuresFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.StabilityPointStructures.Forms.PresentationObjects.RegistrationState.StabilityPointStructuresFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.StabilityPointStructures.Forms.PropertyClasses.CalculationsState.StabilityPointStructuresFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.StabilityPointStructures.Forms.PropertyClasses.RegistrationState.StabilityPointStructuresFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.StabilityPointStructures.Forms.Views.CalculationsState.StabilityPointStructuresFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.StabilityPointStructures.Forms.Views.RegistrationState.StabilityPointStructuresFailureMechanismView;

namespace Riskeer.StabilityPointStructures.Plugin.Test
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
                    typeof(CalculationStateFailureMechanismContext),
                    typeof(CalculationsStateFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(RegistrationStateFailureMechanismProperties),
                    typeof(RegistrationStateFailureMechanismProperties));

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
                Assert.AreEqual(9, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CalculationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RegistrationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresFailureMechanismSectionResultContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityPointStructuresInputContext)));
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

            using (var plugin = new StabilityPointStructuresPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(5, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(CalculationStateFailureMechanismContext),
                    typeof(CalculationsStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(RegistrationStateFailureMechanismContext),
                    typeof(RegistrationStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityPointStructuresFailureMechanismSectionResultContext),
                    typeof(IObservableEnumerable<AdoptableFailureMechanismSectionResult>),
                    typeof(StructuresFailureMechanismResultView<StabilityPointStructuresFailureMechanism, StabilityPointStructuresInput>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityPointStructuresScenariosContext),
                    typeof(CalculationGroup),
                    typeof(StabilityPointStructuresScenariosView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityPointStructuresCalculationGroupContext),
                    typeof(CalculationGroup),
                    typeof(StabilityPointStructuresCalculationsView));

                viewInfos.ForEachElementDo(vi =>
                {
                    Assert.AreEqual(symbol, vi.GetSymbol());
                    Assert.AreSame(fontFamily, vi.GetFontFamily());
                });
            }

            mockRepository.VerifyAll();
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
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new StabilityPointStructuresPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(StabilityPointStructuresCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(StabilityPointStructuresCalculationScenarioContext)));
            }

            mocks.VerifyAll();
        }
    }
}