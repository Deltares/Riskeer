﻿using System;
using System.Linq;

using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.Controls;
using Core.Common.Gui;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsGuiPluginTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            using (var ringtoetsGuiPlugin = new RingtoetsGuiPlugin())
            {
                // assert
                Assert.IsInstanceOf<GuiPlugin>(ringtoetsGuiPlugin);
                Assert.IsInstanceOf<RingtoetsRibbon>(ringtoetsGuiPlugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyClasses()
        {
            // setup
            using (var guiPlugin = new RingtoetsGuiPlugin())
            {
                // call
                PropertyInfo[] propertyInfos = guiPlugin.GetPropertyInfos().ToArray();

                // assert
                Assert.AreEqual(1, propertyInfos.Length);

                var assessmentSectionProperties = propertyInfos.Single(pi => pi.ObjectType == typeof(AssessmentSectionBase));
                Assert.AreEqual(typeof(AssessmentSectionBaseProperties), assessmentSectionProperties.PropertyType);
                Assert.IsNull(assessmentSectionProperties.AdditionalDataCheck);
                Assert.IsNull(assessmentSectionProperties.GetObjectPropertiesData);
                Assert.IsNull(assessmentSectionProperties.AfterCreate);
            }
        }

        [Test]
        public void GetProjectTreeViewNodePresenters_ReturnsSupportedNodePresenters()
        {
            // setup
            var mocks = new MockRepository();
            var application = mocks.Stub<IApplication>();
            application.Stub(a => a.ActivityRunner).Return(mocks.Stub<IActivityRunner>());

            var guiStub = mocks.Stub<IGui>();
            guiStub.CommandHandler = mocks.Stub<IGuiCommandHandler>();
            guiStub.Application = application;

            mocks.ReplayAll();

            using (var guiPlugin = new RingtoetsGuiPlugin { Gui = guiStub })
            {
                // call
                ITreeNodePresenter[] nodePresenters = guiPlugin.GetProjectTreeViewNodePresenters().ToArray();

                // assert
                Assert.AreEqual(4, nodePresenters.Length);
                Assert.IsTrue(nodePresenters.Any(np => np is AssessmentSectionBaseNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is PlaceholderWithReadonlyNameNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is FailureMechanismNodePresenter));
                Assert.IsTrue(nodePresenters.Any(np => np is CategoryTreeFolderNodePresenter));
            }
            mocks.VerifyAll();
        }
    }
}