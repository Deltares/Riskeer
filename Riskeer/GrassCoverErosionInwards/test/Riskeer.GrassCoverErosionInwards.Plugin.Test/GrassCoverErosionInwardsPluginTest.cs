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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionInwards.Forms.Views;
using CalculationsStateFailureMechanismContext = Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.CalculationsState.GrassCoverErosionInwardsFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.RegistrationState.GrassCoverErosionInwardsFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses.CalculationsState.GrassCoverErosionInwardsFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses.RegistrationState.GrassCoverErosionInwardsFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.GrassCoverErosionInwards.Forms.Views.CalculationsState.GrassCoverErosionInwardsFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.GrassCoverErosionInwards.Forms.Views.RegistrationState.GrassCoverErosionInwardsFailureMechanismView;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(9, propertyInfos.Length);

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
                    typeof(DikeProfile),
                    typeof(DikeProfileProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionInwardsInputContext),
                    typeof(GrassCoverErosionInwardsInputContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(GrassCoverErosionInwardsOutputContext),
                    typeof(GrassCoverErosionInwardsOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DikeProfilesContext),
                    typeof(DikeProfileCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(OvertoppingOutputContext),
                    typeof(OvertoppingOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DikeHeightOutputContext),
                    typeof(DikeHeightOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(OvertoppingRateOutputContext),
                    typeof(OvertoppingRateOutputProperties));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(12, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CalculationsStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RegistrationStateFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsFailureMechanismSectionResultContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(OvertoppingOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeHeightOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(OvertoppingRateOutputContext)));
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

            using (var plugin = new GrassCoverErosionInwardsPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(9, viewInfos.Length);

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
                    typeof(GrassCoverErosionInwardsScenariosContext),
                    typeof(CalculationGroup),
                    typeof(GrassCoverErosionInwardsScenariosView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionInwardsFailureMechanismSectionResultContext),
                    typeof(IObservableEnumerable<AdoptableWithProfileProbabilityFailureMechanismSectionResult>),
                    typeof(GrassCoverErosionInwardsFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionInwardsInputContext),
                    typeof(GrassCoverErosionInwardsCalculation),
                    typeof(GrassCoverErosionInwardsInputView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DikeHeightOutputContext),
                    typeof(GrassCoverErosionInwardsCalculation),
                    typeof(DikeHeightOutputGeneralResultFaultTreeIllustrationPointView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(OvertoppingOutputContext),
                    typeof(GrassCoverErosionInwardsCalculation),
                    typeof(OvertoppingOutputGeneralResultFaultTreeIllustrationPointView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(OvertoppingRateOutputContext),
                    typeof(GrassCoverErosionInwardsCalculation),
                    typeof(OvertoppingRateOutputGeneralResultFaultTreeIllustrationPointView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverErosionInwardsCalculationGroupContext),
                    typeof(CalculationGroup),
                    typeof(GrassCoverErosionInwardsCalculationsView));

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
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(2, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(DikeProfilesContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(GrassCoverErosionInwardsFailureMechanismSectionsContext)));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, importInfos.Length);
                Assert.IsTrue(importInfos.Any(tni => tni.DataType == typeof(GrassCoverErosionInwardsCalculationGroupContext)));
                Assert.IsTrue(importInfos.Any(tni => tni.DataType == typeof(DikeProfilesContext)));
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

            using (var plugin = new GrassCoverErosionInwardsPlugin
            {
                Gui = gui
            })
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(GrassCoverErosionInwardsCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(GrassCoverErosionInwardsCalculationScenarioContext)));
            }

            mocks.VerifyAll();
        }
    }
}