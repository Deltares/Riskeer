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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin.Test
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
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(3, propertyInfos.Length);

                var failureMechanism = new ClosingStructuresFailureMechanism();
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSection);
                PropertyInfo closingStructuresFailureMechanismContextPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresFailureMechanismContext),
                    typeof(ClosingStructureFailureMechanismProperties));
                Assert.IsNull(closingStructuresFailureMechanismContextPropertyInfo.AfterCreate);

                PropertyInfo closingStructurePropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructure),
                    typeof(ClosingStructureProperties));
                Assert.IsNull(closingStructurePropertyInfo.AfterCreate);

                PropertyInfo closingStructuresInputContextPropertyInfo = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ClosingStructuresInputContext),
                    typeof(ClosingStructuresInputContextProperties));
                Assert.IsNull(closingStructuresInputContextPropertyInfo.AfterCreate);
            }
            mocks.VerifyAll();
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
                Assert.AreEqual(8, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructure)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ClosingStructuresScenariosContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresFailureMechanismContext),
                    typeof(ClosingStructuresFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>),
                    typeof(IEnumerable<ClosingStructuresFailureMechanismSectionResult>),
                    typeof(ClosingStructuresFailureMechanismResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ClosingStructuresScenariosContext),
                    typeof(CalculationGroup),
                    typeof(ClosingStructuresScenariosView));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsExpectedImportInfos()
        {
            // Setup
            using (var plugin = new ClosingStructuresPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(1, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ClosingStructuresContext)));
            }
        }
    }
}