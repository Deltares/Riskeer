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
using NUnit.Framework;
using Ringtoets.Piping.IO.Importer;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class StochasticSoilModelChangeHandlerTest
    {
        [Test]
        public void DefaultConstructor_ImplementedExpectedInterface()
        {
            // Call
            var handler = new StochasticSoilModelChangeHandler();

            // Assert
            Assert.Throws<NotImplementedException>(() => handler.InquireConfirmation());
        }

        [Test]
        public void RequireConfirmation_Always_ReturnFalse()
        {
            // Setup
            var handler = new StochasticSoilModelChangeHandler();

            // Call
            bool requireConfirmation = handler.RequireConfirmation();

            // Assert
            Assert.IsFalse(requireConfirmation);
        }
        [Test]
        public void InquireConfirmation_Always_ThrowsNotImplementedException()
        {
            // Setup
            var handler = new StochasticSoilModelChangeHandler();

            // Call
            TestDelegate testDelegate = () => handler.InquireConfirmation();

            // Assert
            Assert.Throws<NotImplementedException>(testDelegate);
        }
    }
}