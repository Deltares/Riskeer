// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.Base.Data;
using Core.Gui.Plugin;
using NUnit.Framework;

namespace Core.Gui.Test.Plugin
{
    [TestFixture]
    public class StateInfoTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string symbol = "Symbol";
            Func<IProject, object> getRootData = o => new object();

            // Call
            var stateInfo = new StateInfo(symbol, getRootData);

            // Assert
            Assert.AreEqual(symbol, stateInfo.Symbol);
            Assert.AreSame(getRootData, stateInfo.GetRootData);
        }

        [Test]
        public void Constructor_GetRootDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StateInfo(string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getRootData", exception.ParamName);
        }
    }
}