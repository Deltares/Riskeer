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

using System;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using PipingPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component (PipingRibbon)
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var plugin = new PipingPlugin())
            {
                // assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsInstanceOf<PipingRibbon>(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // setup
            using (var plugin = new PipingPlugin())
            {
                // call
                var mocks = new MockRepository();
                mocks.ReplayAll();

                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(6, propertyInfos.Length);

                PropertyInfo pipingFailureMechanismContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingFailureMechanismContext),
                    typeof(PipingFailureMechanismContextProperties));
                Assert.IsNull(pipingFailureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingFailureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingFailureMechanismContextProperties.AfterCreate);

                PropertyInfo pipingInputContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingInputContext),
                    typeof(PipingInputContextProperties));
                Assert.IsNull(pipingInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingInputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingInputContextProperties.AfterCreate);

                PropertyInfo pipingOutputProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingSemiProbabilisticOutput),
                    typeof(PipingSemiProbabilisticOutputProperties));
                Assert.IsNull(pipingOutputProperties.AdditionalDataCheck);
                Assert.IsNull(pipingOutputProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingOutputProperties.AfterCreate);

                PropertyInfo pipingSurfaceLineProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(RingtoetsPipingSurfaceLine),
                    typeof(RingtoetsPipingSurfaceLineProperties));
                Assert.IsNull(pipingSurfaceLineProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSurfaceLineProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSurfaceLineProperties.AfterCreate);

                PropertyInfo stochasticSoilModelProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StochasticSoilModel),
                    typeof(StochasticSoilModelProperties));
                Assert.IsNull(stochasticSoilModelProperties.AdditionalDataCheck);
                Assert.IsNull(stochasticSoilModelProperties.GetObjectPropertiesData);
                Assert.IsNull(stochasticSoilModelProperties.AfterCreate);

                PropertyInfo stochasticSoilProfileProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StochasticSoilProfile),
                    typeof(StochasticSoilProfileProperties));
                Assert.IsNull(stochasticSoilProfileProperties.AdditionalDataCheck);
                Assert.IsNull(stochasticSoilProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(stochasticSoilProfileProperties.AfterCreate);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(13, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLinesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModelsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModel)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingSemiProbabilisticOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingScenariosContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingOutput)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(5, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingCalculationsView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismResultView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingInputView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingScenariosView)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetFileInfos_Always_ReturnsExpectedFileInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());
            mocks.ReplayAll();

            using (var plugin = new PipingPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, importInfos.Length);
                Assert.AreEqual(1, importInfos.Count(i => i.DataType == typeof(RingtoetsPipingSurfaceLinesContext)));
                Assert.AreEqual(1, importInfos.Count(i => i.DataType == typeof(StochasticSoilModelsContext)));
            }
            mocks.VerifyAll();
        }
    }
}