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

using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Plugin.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var grassCoverErosionInwardsGuiPlugin = new GrassCoverErosionInwardsGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(grassCoverErosionInwardsGuiPlugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new GrassCoverErosionInwardsGuiPlugin())
            {
                // call
                var mocks = new MockRepository();
                mocks.ReplayAll();

                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(4, propertyInfos.Length);

                var failureMechanismContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(GrassCoverErosionInwardsFailureMechanismContext));
                Assert.AreEqual(typeof(GrassCoverErosionInwardsFailureMechanismContextProperties), failureMechanismContextProperties.PropertyObjectType);
                Assert.IsNull(failureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(failureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(failureMechanismContextProperties.AfterCreate);

                var dikeProfileProperties = propertyInfos.Single(pi => pi.DataType == typeof(DikeProfile));
                Assert.AreEqual(typeof(DikeProfileProperties), dikeProfileProperties.PropertyObjectType);
                Assert.IsNull(dikeProfileProperties.AdditionalDataCheck);
                Assert.IsNull(dikeProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(dikeProfileProperties.AfterCreate);

                var inputContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(GrassCoverErosionInwardsInputContext));
                Assert.AreEqual(typeof(GrassCoverErosionInwardsInputContextProperties), inputContextProperties.PropertyObjectType);
                Assert.IsNull(inputContextProperties.AdditionalDataCheck);
                Assert.IsNull(inputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(inputContextProperties.AfterCreate);

                var outputProperties = propertyInfos.Single(pi => pi.DataType == typeof(GrassCoverErosionInwardsOutput));
                Assert.AreEqual(typeof(GrassCoverErosionInwardsOutputProperties), outputProperties.PropertyObjectType);
                Assert.IsNull(outputProperties.AdditionalDataCheck);
                Assert.IsNull(outputProperties.GetObjectPropertiesData);
                Assert.IsNull(outputProperties.AfterCreate);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();
            mocks.ReplayAll();

            using (var guiPlugin = new GrassCoverErosionInwardsGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(10, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverErosionInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyProbabilityAssessmentOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverErosionInwardsOutput)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());

            guiStub.Stub(g => g.ApplicationCore).Return(applicationCore);

            mocks.ReplayAll();

            using (var guiPlugin = new GrassCoverErosionInwardsGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = guiPlugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsFailureMechanismResultView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsInputView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(GrassCoverErosionInwardsScenariosView)));
            }
        }
    }
}