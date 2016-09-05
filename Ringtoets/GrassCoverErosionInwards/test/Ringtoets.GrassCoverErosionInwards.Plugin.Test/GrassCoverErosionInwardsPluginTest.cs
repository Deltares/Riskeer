﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // assert
                Assert.IsInstanceOf<PluginBase>(plugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(4, propertyInfos.Length);

                PropertyInfo failureMechanismContextProperties = PluginTestHelper.AssertPropertyInfoDefined
                    <GrassCoverErosionInwardsFailureMechanismContext, GrassCoverErosionInwardsFailureMechanismContextProperties>(propertyInfos);
                Assert.IsNull(failureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(failureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(failureMechanismContextProperties.AfterCreate);

                PropertyInfo dikeProfileProperties = PluginTestHelper.AssertPropertyInfoDefined
                    <DikeProfile, DikeProfileProperties>(propertyInfos);
                Assert.IsNull(dikeProfileProperties.AdditionalDataCheck);
                Assert.IsNull(dikeProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(dikeProfileProperties.AfterCreate);

                PropertyInfo inputContextProperties = PluginTestHelper.AssertPropertyInfoDefined
                    <GrassCoverErosionInwardsInputContext, GrassCoverErosionInwardsInputContextProperties>(propertyInfos);
                Assert.IsNull(inputContextProperties.AdditionalDataCheck);
                Assert.IsNull(inputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(inputContextProperties.AfterCreate);

                PropertyInfo outputProperties = PluginTestHelper.AssertPropertyInfoDefined
                    <GrassCoverErosionInwardsOutput, GrassCoverErosionInwardsOutputProperties>(propertyInfos);
                Assert.IsNull(outputProperties.AdditionalDataCheck);
                Assert.IsNull(outputProperties.GetObjectPropertiesData);
                Assert.IsNull(outputProperties.AfterCreate);
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(9, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyProbabilityAssessmentOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsOutput)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new GrassCoverErosionInwardsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsFailureMechanismResultView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsInputView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsScenariosView)));
            }
        }
    }
}