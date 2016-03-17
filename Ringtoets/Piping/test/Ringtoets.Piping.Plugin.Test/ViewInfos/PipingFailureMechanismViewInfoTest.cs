﻿using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class PipingFailureMechanismViewInfoTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetViewInfos().First(tni => tni.ViewType == typeof(PipingFailureMechanismView));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.DataType);
            Assert.AreEqual(typeof(PipingFailureMechanismContext), info.ViewDataType);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, info.Image);
        }

        [Test]
        public void GetViewName_Always_ReturnsTextFromResources()
        {
            // Setup
            var viewMock = mocks.StrictMock<PipingFailureMechanismView>();

            // Call & Assert
            Assert.AreEqual(PipingDataResources.PipingFailureMechanism_DisplayName, info.GetViewName(viewMock, null));
        }
    }
}
