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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class CalculationInputPropertyChangeHandlerTest
    {
        [Test]
        public void Constructor_Expectedvalues()
        {
            // Call
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Assert
            Assert.IsInstanceOf<ICalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>>(changeHandler);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationInputNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(null, new TestCalculation(), 3, (input, value) => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculationInput", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(new TestCalculationInput(), null, 3, (input, value) => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_ValueNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation<double?>(new TestCalculationInput(),
                                                                                               new TestCalculation(),
                                                                                               null,
                                                                                               (input, value) => { });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("value", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueAfterConfirmation_SetValueNull_ThrowArgumentNullException()
        {
            // Setup
            var changeHandler = new CalculationInputPropertyChangeHandler<ICalculationInput, ICalculation>();

            // Call
            TestDelegate test = () => changeHandler.SetPropertyValueAfterConfirmation(new TestCalculationInput(),
                                                                                      new TestCalculation(),
                                                                                      3,
                                                                                      null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("setValue", exception.ParamName);
        }
    }
}