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

using System;
using System.Collections.ObjectModel;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Theme;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Theme
{
    [TestFixture]
    public class ColorThemeExtensionsTest
    {
        [Test]
        public void Localized_ForEveryItem_ExpectedTranslatedString()
        {
            // Setup
            var nameList = new[]
            {
                Resources.Dark,
                Resources.Light,
                Resources.Metro,
                Resources.Aero,
                Resources.VS2010,
                Resources.Generic
            };
            var translations = new Collection<string>();

            // Call
            foreach (ColorTheme item in Enum.GetValues(typeof(ColorTheme)))
            {
                translations.Add(item.Localized());
            }

            // Assert
            CollectionAssert.AreEqual(nameList, translations);
        } 
    }
}