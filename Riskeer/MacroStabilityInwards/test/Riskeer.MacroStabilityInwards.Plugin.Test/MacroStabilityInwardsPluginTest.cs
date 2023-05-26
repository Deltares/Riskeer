﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Collections.Generic;
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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.Primitives;
using CalculationsStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.CalculationsState.MacroStabilityInwardsFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.RegistrationState.MacroStabilityInwardsFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.MacroStabilityInwards.Forms.PropertyClasses.CalculationsState.MacroStabilityInwardsFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.MacroStabilityInwards.Forms.PropertyClasses.RegistrationState.MacroStabilityInwardsFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.MacroStabilityInwards.Forms.Views.CalculationsState.MacroStabilityInwardsFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.MacroStabilityInwards.Forms.Views.RegistrationState.MacroStabilityInwardsFailureMechanismView;

namespace Riskeer.MacroStabilityInwards.Plugin.Test
{
    [TestFixture]
    public class MacroStabilityInwardsPluginTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(10, propertyInfos.Length);

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
                    typeof(MacroStabilityInwardsInputContext),
                    typeof(MacroStabilityInwardsInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsOutputContext),
                    typeof(MacroStabilityInwardsOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsSurfaceLinesContext),
                    typeof(MacroStabilityInwardsSurfaceLineCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsSurfaceLine),
                    typeof(MacroStabilityInwardsSurfaceLineProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext),
                    typeof(MacroStabilityInwardsStochasticSoilModelCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsStochasticSoilModel),
                    typeof(MacroStabilityInwardsStochasticSoilModelProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsStochasticSoilProfile),
                    typeof(MacroStabilityInwardsStochasticSoilProfileProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsFailureMechanismSectionsContext),
                    typeof(FailureMechanismSectionsProbabilityAssessmentProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(13, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CalculationsStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RegistrationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsSurfaceLinesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilModel)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsFailureMechanismSectionResultContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsScenariosContext)));
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

            using (var plugin = new MacroStabilityInwardsPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(8, viewInfos.Length);

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
                    typeof(MacroStabilityInwardsFailureMechanismSectionResultContext),
                    typeof(IObservableEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult>),
                    typeof(MacroStabilityInwardsFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsCalculationGroupContext),
                    typeof(CalculationGroup),
                    typeof(MacroStabilityInwardsCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsInputContext),
                    typeof(MacroStabilityInwardsCalculationScenario),
                    typeof(MacroStabilityInwardsInputView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsScenariosContext),
                    typeof(CalculationGroup),
                    typeof(MacroStabilityInwardsScenariosView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsOutputContext),
                    typeof(MacroStabilityInwardsCalculationScenario),
                    typeof(MacroStabilityInwardsOutputView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsFailureMechanismSectionsContext),
                    typeof(IEnumerable<FailureMechanismSection>),
                    typeof(FailureMechanismSectionsProbabilityAssessmentView));

                viewInfos.ForEachElementDo(vi =>
                {
                    Assert.AreEqual(symbol, vi.GetSymbol());
                    Assert.AreSame(fontFamily, vi.GetFontFamily());
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetUpdateInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(3, updateInfos.Length);
                Assert.AreEqual(1, updateInfos.Count(updateInfo => updateInfo.DataType == typeof(MacroStabilityInwardsSurfaceLinesContext)));
                Assert.AreEqual(1, updateInfos.Count(updateInfo => updateInfo.DataType == typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext)));
                Assert.AreEqual(1, updateInfos.Count(updateInfo => updateInfo.DataType == typeof(MacroStabilityInwardsFailureMechanismSectionsContext)));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(3, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(MacroStabilityInwardsSurfaceLinesContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(MacroStabilityInwardsCalculationGroupContext)));
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

            using (var plugin = new MacroStabilityInwardsPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(4, exportInfos.Length);
                Assert.AreEqual(2, exportInfos.Count(i => i.DataType == typeof(MacroStabilityInwardsCalculationGroupContext)));
                Assert.AreEqual(2, exportInfos.Count(i => i.DataType == typeof(MacroStabilityInwardsCalculationScenarioContext)));
            }

            mocks.VerifyAll();
        }
    }
}