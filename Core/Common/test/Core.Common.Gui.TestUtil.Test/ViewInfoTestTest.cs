// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.TestUtil.Test
{
    [TestFixture]
    public class ViewInfoTestTest
    {
        [Test]
        public void SetUp_Always_CreatesViewInfoAndPlugin()
        {
            var mocks = new MockRepository();
            var viewInfoTest = new ObjectViewInfoTest(mocks);
            mocks.ReplayAll();

            viewInfoTest.SetUp();

            // Assert
            Assert.IsInstanceOf<PluginBase>(viewInfoTest.GetPlugin());
            Assert.IsInstanceOf<ViewInfo>(viewInfoTest.GetViewInfo());
        }

        [Test]
        public void TearDown_AfterSetup_DisposesPlugin()
        {
            // Setup
            var mocks = new MockRepository();
            var viewInfoTest = new ObjectViewInfoTest(mocks);
            viewInfoTest.PluginStub.Expect(p => p.Dispose());
            
            mocks.ReplayAll();

            viewInfoTest.SetUp();

            // Call
            viewInfoTest.TearDown();

            // Assert
            mocks.VerifyAll();
        }
    }

    [Ignore]
    public class ObjectViewInfoTest : ViewInfoTest<IView>
    {
        public readonly PluginBase PluginStub;
        public readonly IView ViewStub;

        public ObjectViewInfoTest(MockRepository viewInfoMockRepository)
        {
            PluginStub = viewInfoMockRepository.Stub<PluginBase>();
            PluginStub.Stub(ps => ps.GetViewInfos()).Return(new ViewInfo[] {new ViewInfo<string, IView>()});
            ViewStub = viewInfoMockRepository.Stub<IView>();
        }

        public PluginBase GetPlugin()
        {
            return Plugin;
        }

        public ViewInfo GetViewInfo()
        {
            return Info;
        }


        protected override PluginBase CreatePlugin()
        {
            return PluginStub;
        }

        protected override IView CreateView()
        {
            return ViewStub;
        }
    }
}