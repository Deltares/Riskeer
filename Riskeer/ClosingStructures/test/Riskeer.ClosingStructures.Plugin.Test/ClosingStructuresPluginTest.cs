﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.ClosingStructures.Forms.PropertyClasses;
using Riskeer.ClosingStructures.Forms.Views;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using CalculationsStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.CalculationsState.ClosingStructuresFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.ClosingStructures.Forms.PresentationObjects.RegistrationState.ClosingStructuresFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.ClosingStructures.Forms.PropertyClasses.CalculationsState.ClosingStructuresFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.ClosingStructures.Forms.PropertyClasses.RegistrationState.ClosingStructuresFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.ClosingStructures.Forms.Views.CalculationsState.ClosingStructuresFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.ClosingStructures.Forms.Views.RegistrationState.ClosingStructuresFailureMechanismView;

namespace Riskeer.ClosingStructures.Plugin.Test
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
                    typeof(CalculationsStateFailureMechanismContext),
                    typeof(CalculationsStateFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(RegistrationStateFailureMechanismContext),
                    typeof(RegistrationStateFailureMechanismProperties));

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
                Assert.AreEqual(9, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CalculationsStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RegistrationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismSectionResultContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresScenariosContext)));
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

            using (var plugin = new ClosingStructuresPlugin
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
                    typeof(CalculationsStateFailureMechanismContext),
                    typeof(CalculationsStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(RegistrationStateFailureMechanismContext),
                    typeof(RegistrationStateFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresFailureMechanismSectionResultContext),
                    typeof(IObservableEnumerable<AdoptableFailureMechanismSectionResult>),
                    typeof(StructuresFailureMechanismResultView<ClosingStructuresFailureMechanism, ClosingStructuresInput>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresScenariosContext),
                    typeof(CalculationGroup),
                    typeof(ClosingStructuresScenariosView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresCalculationGroupContext),
                    typeof(CalculationGroup),
                    typeof(ClosingStructuresCalculationsView));

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
            var mocks = new MockRepository();
            var mainWindow = mocks.Stub<IMainWindow>();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.MainWindow).Return(mainWindow);
            mocks.ReplayAll();

            using (var plugin = new ClosingStructuresPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(ClosingStructuresCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(ClosingStructuresCalculationScenarioContext)));
            }

            mocks.VerifyAll();
        }
    }
}