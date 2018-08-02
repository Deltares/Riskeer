// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Views;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test
{
    [TestFixture]
    public class MacroStabilityInwardsPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
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
                Assert.AreEqual(9, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityInwardsFailureMechanismContext),
                    typeof(MacroStabilityInwardsFailureMechanismProperties));

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
                Assert.AreEqual(12, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsSurfaceLinesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilModelCollectionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilModel)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsStochasticSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityInwardsScenariosContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(7, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityInwardsFailureMechanismContext),
                    typeof(MacroStabilityInwardsFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult>),
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
            }
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
            using (var plugin = new MacroStabilityInwardsPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(MacroStabilityInwardsCalculationGroupContext)));
                Assert.IsTrue(exportInfos.Any(tni => tni.DataType == typeof(MacroStabilityInwardsCalculationScenarioContext)));
            }
        }
    }
}