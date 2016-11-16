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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin.Test
{
    [TestFixture]
    public class StabilityStoneCoverPluginTest
    {
        [Test]
        public void GetPropertiesInfos_ReturnSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(3, propertyInfos.Length);

                PropertyInfo failureMechanismProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverFailureMechanismContext),
                    typeof(StabilityStoneCoverFailureMechanismProperties));
                Assert.IsNull(failureMechanismProperties.AdditionalDataCheck);
                Assert.IsNull(failureMechanismProperties.AfterCreate);
                PropertyInfo waveConditionsOutputProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverWaveConditionsOutput),
                    typeof(StabilityStoneCoverWaveConditionsOutputProperties));
                Assert.IsNull(waveConditionsOutputProperties.AdditionalDataCheck);
                Assert.IsNull(waveConditionsOutputProperties.AfterCreate);
                PropertyInfo waveConditionsInputContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StabilityStoneCoverWaveConditionsInputContext),
                    typeof(StabilityStoneCoverWaveConditionsInputContextProperties));
                Assert.IsNull(waveConditionsInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(waveConditionsInputContextProperties.AfterCreate);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfoClasses()
        {
            // Setup
            using (var plugin = new StabilityStoneCoverPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StabilityStoneCoverFailureMechanismContext),
                    typeof(StabilityStoneCoverFailureMechanismView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>),
                    typeof(IEnumerable<StabilityStoneCoverFailureMechanismSectionResult>),
                    typeof(StabilityStoneCoverResultView));
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
                Assert.AreEqual(7, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyStabilityStoneCoverOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsInputContext)));
            }
        }
    }
}