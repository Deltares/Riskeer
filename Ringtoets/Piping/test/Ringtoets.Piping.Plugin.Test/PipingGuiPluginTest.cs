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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;
using GuiPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test
{
    [TestFixture]
    public class PipingGuiPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component (PipingRibbon)
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var ringtoetsGuiPlugin = new PipingGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(ringtoetsGuiPlugin);
                Assert.IsInstanceOf<PipingRibbon>(ringtoetsGuiPlugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new PipingGuiPlugin())
            {
                // call
                var mocks = new MockRepository();
                mocks.ReplayAll();

                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(8, propertyInfos.Length);

                var pipingFailureMechanismContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingFailureMechanismContext));
                Assert.AreEqual(typeof(PipingFailureMechanismContextProperties), pipingFailureMechanismContextProperties.PropertyObjectType);
                Assert.IsNull(pipingFailureMechanismContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingFailureMechanismContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingFailureMechanismContextProperties.AfterCreate);

                var pipingCalculationContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingCalculationScenarioContext));
                Assert.AreEqual(typeof(PipingCalculationContextProperties), pipingCalculationContextProperties.PropertyObjectType);
                Assert.IsNull(pipingCalculationContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingCalculationContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingCalculationContextProperties.AfterCreate);

                var pipingCalculationGroupProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingCalculationGroupContext));
                Assert.AreEqual(typeof(PipingCalculationGroupContextProperties), pipingCalculationGroupProperties.PropertyObjectType);
                Assert.IsNull(pipingCalculationGroupProperties.AdditionalDataCheck);
                Assert.IsNull(pipingCalculationGroupProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingCalculationGroupProperties.AfterCreate);

                var pipingInputContextProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingInputContext));
                Assert.AreEqual(typeof(PipingInputContextProperties), pipingInputContextProperties.PropertyObjectType);
                Assert.IsNull(pipingInputContextProperties.AdditionalDataCheck);
                Assert.IsNull(pipingInputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingInputContextProperties.AfterCreate);

                var pipingOutputProperties = propertyInfos.Single(pi => pi.DataType == typeof(PipingSemiProbabilisticOutput));
                Assert.AreEqual(typeof(PipingSemiProbabilisticOutputProperties), pipingOutputProperties.PropertyObjectType);
                Assert.IsNull(pipingOutputProperties.AdditionalDataCheck);
                Assert.IsNull(pipingOutputProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingOutputProperties.AfterCreate);

                var pipingSurfaceLineProperties = propertyInfos.Single(pi => pi.DataType == typeof(RingtoetsPipingSurfaceLine));
                Assert.AreEqual(typeof(RingtoetsPipingSurfaceLineProperties), pipingSurfaceLineProperties.PropertyObjectType);
                Assert.IsNull(pipingSurfaceLineProperties.AdditionalDataCheck);
                Assert.IsNull(pipingSurfaceLineProperties.GetObjectPropertiesData);
                Assert.IsNull(pipingSurfaceLineProperties.AfterCreate);

                var stochasticSoilModelProperties = propertyInfos.Single(pi => pi.DataType == typeof(StochasticSoilModel));
                Assert.AreEqual(typeof(StochasticSoilModelProperties), stochasticSoilModelProperties.PropertyObjectType);
                Assert.IsNull(stochasticSoilModelProperties.AdditionalDataCheck);
                Assert.IsNull(stochasticSoilModelProperties.GetObjectPropertiesData);
                Assert.IsNull(stochasticSoilModelProperties.AfterCreate);

                var stochasticSoilProfileProperties = propertyInfos.Single(pi => pi.DataType == typeof(StochasticSoilProfile));
                Assert.AreEqual(typeof(StochasticSoilProfileProperties), stochasticSoilProfileProperties.PropertyObjectType);
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
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());

            Expect.Call(guiStub.ApplicationCore).Return(applicationCore).Repeat.Any();

            mocks.ReplayAll();

            using (var guiPlugin = new PipingGuiPlugin
            {
                Gui = guiStub
            })
            {
                // call
                TreeNodeInfo[] treeNodeInfos = guiPlugin.GetTreeNodeInfos().ToArray();

                // assert
                Assert.AreEqual(13, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLinesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsPipingSurfaceLine)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModelContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilModel)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StochasticSoilProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationScenarioContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingCalculationGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingSemiProbabilisticOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyPipingCalculationReport)));
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfo_ReturnsSupportedViewInfos()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationCore = new ApplicationCore();

            var guiStub = mocks.Stub<IGui>();
            guiStub.Stub(g => g.ApplicationCommands).Return(mocks.Stub<IApplicationFeatureCommands>());

            guiStub.Stub(g => g.ApplicationCore).Return(applicationCore);

            mocks.ReplayAll();

            using (var guiPlugin = new PipingGuiPlugin
            {
                Gui = guiStub
            })
            {
                // Call
                ViewInfo[] viewInfos = guiPlugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(3, viewInfos.Length);

                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingCalculationsView)));
                Assert.IsTrue(viewInfos.Any(vi => vi.ViewType == typeof(PipingFailureMechanismResultView)));
            }
        }
    }
}