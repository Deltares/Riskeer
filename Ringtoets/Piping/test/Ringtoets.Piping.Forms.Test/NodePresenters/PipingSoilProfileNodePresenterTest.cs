using System;
using System.Collections.Generic;
using Core.Common.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.NodePresenters;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSoilProfileNodePresenterTest
    {

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSoilProfileNodePresenter();

            // Assert
            Assert.IsInstanceOf<PipingNodePresenterBase<PipingSoilProfile>>(nodePresenter);
            Assert.IsInstanceOf<PipingSoilProfileNodePresenter>(nodePresenter);
        }

        [Test]
        public void UpdateNode_NodeWithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNodeMock = mocks.StrictMock<ITreeNode>();
            var dataNodeMock = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            const string name = "<insert name here>";
            var random = new Random(22);
            double bottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = 1.0
                } 
            };
            var soilProfile = new PipingSoilProfile(name, bottom, layers);

            var nodePresenter = new PipingSoilProfileNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNodeMock, dataNodeMock, soilProfile);

            // Assert
            Assert.AreEqual(name, dataNodeMock.Text);
            Assert.AreEqual(16, dataNodeMock.Image.Height);
            Assert.AreEqual(16, dataNodeMock.Image.Width);
            mocks.VerifyAll();
        }
    }
}