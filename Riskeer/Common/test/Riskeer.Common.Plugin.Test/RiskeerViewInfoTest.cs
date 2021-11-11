// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Windows.Media;
using Core.Common.Controls.Views;
using Core.Gui;
using Core.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;

namespace Riskeer.Common.Plugin.Test
{
    [TestFixture]
    public class RiskeerViewInfoTest
    {
        [Test]
        public void GenericConstructorWithThreeTypes_GetGuiFuncNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new RiskeerViewInfo<int, string, IView>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getGuiFunc", exception.ParamName);
        }

        [Test]
        public void GenericConstructorWithThreeTypes_ExpectedValues()
        {
            // Call
            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => null);

            // Assert
            Assert.IsInstanceOf<ViewInfo<int, string, IView>>(viewInfo);
            Assert.IsNotNull(viewInfo.GetSymbol);
            Assert.IsNotNull(viewInfo.GetFontFamily);
        }

        [Test]
        public void GenericConstructorWithTwoTypes_ExpectedValues()
        {
            // Call
            var viewInfo = new RiskeerViewInfo<int, IView>(() => null);

            // Assert
            Assert.IsInstanceOf<RiskeerViewInfo<int, int, IView>>(viewInfo);
        }

        [Test]
        public void GetSymbol_GuiNull_ReturnsExpectedValue()
        {
            // Setup
            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => null);

            // Call
            string symbol = viewInfo.GetSymbol();

            // Assert
            Assert.IsNull(symbol);
        }

        [Test]
        public void GetSymbol_ActiveStateInfoNull_ReturnsExpectedValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            mockRepository.ReplayAll();

            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => gui);

            // Call
            string symbol = viewInfo.GetSymbol();

            // Assert
            Assert.IsNull(symbol);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetSymbol_ActiveStateInfoNotNull_ReturnsExpectedValue()
        {
            // Setup
            const string expectedSymbol = "symbol";

            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            gui.Stub(g => g.ActiveStateInfo).Return(new StateInfo(string.Empty, expectedSymbol, new FontFamily(), p => p));
            mockRepository.ReplayAll();

            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => gui);

            // Call
            string actualSymbol = viewInfo.GetSymbol();

            // Assert
            Assert.AreEqual(expectedSymbol, actualSymbol);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetFontFamily_GuiNull_ReturnsExpectedValue()
        {
            // Setup
            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => null);

            // Call
            FontFamily fontFamily = viewInfo.GetFontFamily();

            // Assert
            Assert.IsNull(fontFamily);
        }

        [Test]
        public void GetFontFamily_ActiveStateInfoNull_ReturnsExpectedValue()
        {
            // Setup
            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            mockRepository.ReplayAll();

            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => gui);

            // Call
            FontFamily fontFamily = viewInfo.GetFontFamily();

            // Assert
            Assert.IsNull(fontFamily);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetFontFamily_ActiveStateInfoNotNull_ReturnsExpectedValue()
        {
            // Setup
            var expectedFontFamily = new FontFamily();

            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            gui.Stub(g => g.ActiveStateInfo).Return(new StateInfo(string.Empty, string.Empty, expectedFontFamily, p => p));
            mockRepository.ReplayAll();

            var viewInfo = new RiskeerViewInfo<int, string, IView>(() => gui);

            // Call
            FontFamily actualFontFamily = viewInfo.GetFontFamily();

            // Assert
            Assert.AreEqual(expectedFontFamily, actualFontFamily);
            mockRepository.VerifyAll();
        }
    }
}