// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;

namespace Riskeer.Piping.Forms.Test.PresentationObjects.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingSectionSpecificOutputContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario();

            // Call
            var context = new ProbabilisticPipingSectionSpecificOutputContext(calculation);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<ProbabilisticPipingCalculationScenario>>(context);
            Assert.AreSame(calculation, context.WrappedData);
        }
    }
}