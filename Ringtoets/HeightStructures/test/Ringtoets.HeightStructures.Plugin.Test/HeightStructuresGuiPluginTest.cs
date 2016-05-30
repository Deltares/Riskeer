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
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Plugin.Test
{
    [TestFixture]
    public class HeightStructuresGuiPluginTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var heightStructuresGuiPlugin = new HeightStructuresGuiPlugin())
            {
                // Assert
                Assert.IsInstanceOf<GuiPlugin>(heightStructuresGuiPlugin);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new HeightStructuresGuiPlugin())
            {
                // call
                var mocks = new MockRepository();
                mocks.ReplayAll();

                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(2, propertyInfos.Length);
                var failureMechanismContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(HeightStructuresFailureMechanismContext));
                Assert.AreEqual(typeof(HeightStructuresFailureMechanismContextProperties), failureMechanismContextProperties.PropertyObjectType);
                Assert.IsNull(failureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(failureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(failureMechanismContextProperties.AfterCreate);

                var heightStructuresInputContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(HeightStructuresInputContext));
                Assert.AreEqual(typeof(HeightStructuresInputContextProperties), heightStructuresInputContextProperties.PropertyObjectType);
                Assert.IsNull(heightStructuresInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(heightStructuresInputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(heightStructuresInputContextProperties.AfterCreate);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();
            var guiStub = mocks.Stub<IGui>();

            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new HeightStructuresGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(7, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresCalculationContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HeightStructuresInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyProbabilityAssessmentOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>)));
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

            using (var guiPlugin = new HeightStructuresGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = guiPlugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(1, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(HeightStructuresFailureMechanismResultView)));
            }
        }
    }
}