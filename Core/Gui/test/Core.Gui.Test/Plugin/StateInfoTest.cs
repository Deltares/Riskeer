﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Windows.Media;
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
            const string name = "Name";
            const string symbol = "Symbol";
            var fontFamily = new FontFamily();
            Func<IProject, object> getRootData = o => new object();

            // Call
            var stateInfo = new StateInfo(name, symbol, fontFamily, getRootData);

            // Assert
            Assert.AreEqual(name, stateInfo.Name);
            Assert.AreEqual(symbol, stateInfo.Symbol);
            Assert.AreSame(fontFamily, stateInfo.FontFamily);
            Assert.AreSame(getRootData, stateInfo.GetRootData);
        }

        [Test]
        public void Constructor_FontFamilyNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StateInfo(string.Empty, string.Empty, null, project => project);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("fontFamily", exception.ParamName);
        }

        [Test]
        public void Constructor_GetRootDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StateInfo(string.Empty, string.Empty, new FontFamily(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getRootData", exception.ParamName);
        }
    }
}