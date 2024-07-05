﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Windows.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms.Views;
using NUnit.Framework;

namespace Core.Components.Gis.Forms.Test.Views
{
    [TestFixture]
    public class BackgroundMapDataSelectionControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string name = "Random display name";

            // Call
            var backgroundMapDataSelectionControl = new TestBackgroundMapDataSelectionControl(name);

            // Assert
            Assert.IsInstanceOf<UserControl>(backgroundMapDataSelectionControl);
            Assert.AreEqual(name, backgroundMapDataSelectionControl.DisplayName);
            Assert.IsNull(backgroundMapDataSelectionControl.SelectedMapData);
        }

        private class TestBackgroundMapDataSelectionControl : BackgroundMapDataSelectionControl
        {
            public override event EventHandler<EventArgs> SelectedMapDataChanged
            {
                add {}
                remove {}
            }

            public TestBackgroundMapDataSelectionControl(string displayName)
                : base(displayName) {}

            public override ImageBasedMapData SelectedMapData { get; }
        }
    }
}